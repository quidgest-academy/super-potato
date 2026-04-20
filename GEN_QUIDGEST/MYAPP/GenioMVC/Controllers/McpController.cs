using CSGenio.framework;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using CSGenio.core.ai;
using GenioServer.security;
using GenioMVC;
using CSGenio.persistence;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using GenioMVC.Models.Navigation;

[Controller]
public class McpController(IToolRepo _repo, IConfiguration _configuration) : Controller
{
    [HttpPost]
    [HttpGet]
    public async Task<IActionResult> HandleMcpAsync()
    {
        //According to MCP protocol, we must either return a text/event-stream if we accept streaming or 405 on GET requests
        if (HttpMethods.IsGet(Request.Method))
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);

        }
        else if (HttpMethods.IsPost(Request.Method))
        {
            // Validate user as early as possible
            User user;
            try
            {
                user = BuildUser();
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error($"Authentication failed: {ex.Message}");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            catch (Exception ex)
            {
                Log.Error($"Error during authentication: {ex.Message}");
                var errorResponse = CreateErrorResponse(-32603, "Authentication error");
                var jsonOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
                return Content(JsonSerializer.Serialize(errorResponse, jsonOptions), "application/json", System.Text.Encoding.UTF8);
            }

            using var reader = new StreamReader(Request.Body);
            string requestBody = await reader.ReadToEndAsync();

            var response = ProcessMcpMessage(requestBody, user);
            return Content(response, "application/json", System.Text.Encoding.UTF8);
        }
        else
        {
            return BadRequest();
        }

    }


    private string ProcessMcpMessage(string requestBody, User user)
    {
        var message = JsonSerializer.Deserialize<JsonElement>(requestBody);
        if (!message.TryGetProperty("method", out var methodProperty))
        {
            return JsonSerializer.Serialize(CreateErrorResponse(-32600, "Invalid Request - missing method"));
        }
        string method = methodProperty.GetString();
        var id = message.TryGetProperty("id", out var idProperty) ?
            (idProperty.ValueKind == JsonValueKind.Number ? (object)idProperty.GetInt32() : idProperty.GetString()) :
            null;
        Log.Debug($"Received MCP request with method: {method}");

        var response = method switch
        {
            "initialize" => HandleInitialize(message, id),
            "tools/list" => HandleToolsList(message, id, user),
            "tools/call" => HandleToolCall(message, id, user),
            "notification/initialized" => EmptyReply(),
            _ => CreateErrorResponse(-32601, "Method not found", id)
        };

        var jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        return JsonSerializer.Serialize(response, jsonOptions);
    }

    private object EmptyReply()
    {
        return new
        {
            jsonrpc = "2.0"
        };
    }

    private object CreateErrorResponse(int code, string message, object id = null)
    {
        return new
        {
            jsonrpc = "2.0",
            error = new { code = code, message = message },
            id = id
        };
    }


    // Define supported protocol versions (latest first)
    private readonly List<string> _supportedVersions = new List<string>
    {
        "2025-06-18",
        "2025-03-26",
        "2024-10-07",        
    };
    /// <summary>
    /// From MCP:
    /// In the initialize request, the client MUST send a protocol version it supports.
    /// This SHOULD be the latest version supported by the client.
    /// If the server supports the requested protocol version, it MUST respond with the same version.
    /// Otherwise, the server MUST respond with another protocol version it supports. 
    /// This SHOULD be the latest version supported by the server.
    /// If the client does not support the version in the server’s response, it SHOULD disconnect.
    /// </summary>
    /// <param name="clientRequestedVersion">The client requested version</param>    
    /// <returns>The negotiated version</returns>
    private string NegotiateVersion(string clientRequestedVersion)
    {
        if (string.IsNullOrWhiteSpace(clientRequestedVersion))
        {
            throw new ArgumentException("Client must specify a protocol version", nameof(clientRequestedVersion));
        }

        // If server supports the client's requested version, use it
        if (_supportedVersions.Contains(clientRequestedVersion))
        {
            return clientRequestedVersion;
        }

        return _supportedVersions.First();
    }



    private object HandleInitialize(JsonElement message, object id)
    {
        // Extract the client's protocol version to match it
        string clientProtocolVersion;
        if (message.TryGetProperty("params", out var @params) &&
            @params.TryGetProperty("protocolVersion", out var protocolProp))
        {
            clientProtocolVersion = NegotiateVersion(protocolProp.GetString());
        }
        else
        {
            return CreateErrorResponse(-32601, "Invalid protocol version", id);
        }

        return new
        {
            jsonrpc = "2.0",
            id = id,
            result = new
            {
                protocolVersion = clientProtocolVersion, // Match the client's version
                capabilities = new
                {
                    tools = new { },
                },
                serverInfo = new
                {
                    name = "genio-mcp-server",
                    version = "1.0.0"
                }
            }
        };
    }

    private object HandleToolsList(JsonElement message, object id, User user)
    {
        var allTools = _repo.GetToolsForUser(user);
        return new
        {
            jsonrpc = "2.0",
            id = id,
            result = new {
                tools = allTools
            }
        };
    }

    private object HandleToolCall(JsonElement message, object id, User user)
    {
        // Extract tool name and arguments from the request
        if (!message.TryGetProperty("params", out var parameters))
        {
            return CreateErrorResponse(-32602, "Invalid params - params object required", id);
        }

        if (!parameters.TryGetProperty("name", out var nameProperty))
        {
            return CreateErrorResponse(-32602, "Invalid params - tool name required", id);
        }

        string toolName = nameProperty.GetString();

        if (string.IsNullOrWhiteSpace(toolName))
        {
            return CreateErrorResponse(-32602, "Invalid params - tool name cannot be empty", id);
        }

        // Extract arguments (optional, default to empty object if not provided)
        var arguments = parameters.TryGetProperty("arguments", out var argsProperty)
            ? argsProperty
            : JsonDocument.Parse("{}").RootElement;

        // Get the tool from the repository
        var tool = _repo.GetTool(toolName);

        if (tool == null)
        {
            return CreateErrorResponse(-32602, $"Tool '{toolName}' not found", id);
        }

        // Check if user has access to this tool
        if (!tool.HasAccess(user))
        {
            return CreateErrorResponse(-32603, $"Access denied to tool '{toolName}'", id);
        }

        // For now, we don't consider the user current module to validate permission. We assume the call is always in the tool module.
        user.CurrentModule = tool.AppModule;        
        // Execute the tool
        var sp = PersistentSupport.getPersistentSupport(user.Year);
        sp.openTransaction();

        try
        {
            var toolResult = tool.Execute(sp, user, arguments);
            sp.closeTransaction(); 
            return new
            {
                jsonrpc = "2.0",
                id = id,
                result = toolResult
            };
        }
        catch (Exception ex)
        {
            Log.Error($"Error executing tool '{toolName}': {ex.Message}");
            sp.rollbackTransaction();
            return new
            {
                jsonrpc = "2.0",
                id = id,
                result = new
                {
                    content = new[]
                    {
                        new
                        {
                            type = "text",
                            text = $"Error executing tool '{toolName}': {ex.Message}"
                        }
                    },
                    isError = true,
                }
            };
        }
      
    }

    private User BuildUser()
    {
        if(Configuration.AiConfig.MCPSecurityMode == CSGenio.MCPSecurityMode.None)
        {
            //Impersonate a default MCP user with admin roles in all modules
            var user = new User("mcp", "", Configuration.DefaultYear);
            var adminUser = SecurityFactory.ElevateUserToAdmin(user);
            return adminUser;
        }
        else if (Configuration.AiConfig.MCPSecurityMode == CSGenio.MCPSecurityMode.JWT)
        {
            // Validate JWT encryption key is configured
            if (string.IsNullOrEmpty(Configuration.AiConfig.JWTEncryptionKey))
            {
                throw new InvalidOperationException("JWT encryption key is not configured. Please set Configuration.AiConfig.JWTEncryptionKey.");
            }

            // Extract JWT token from Authorization header
            var authHeader = Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Missing or invalid Authorization header. Bearer token required.");
            }
            string token = authHeader.Substring("Bearer ".Length).Trim();

            // Validate and decode the JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Configuration.AiConfig.JWTEncryptionKeyDecode);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Extract claims from the token
                var username = principal.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    throw new UnauthorizedAccessException("JWT token is missing required claims (username).");
                }

                // Reuse the UserContext to build the User object
                var claimsIdentity = new ClaimsIdentity(principal.Claims, "jwt");                
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                HttpContext.User = claimsPrincipal;

                HttpContext.Session.SetString("user.identity", username);
                UserContext userContext = new UserContext(HttpContext, _configuration);

                var user = userContext.User;

                var module = principal.FindFirst("module")?.Value;
                if (!string.IsNullOrEmpty(module))
                    user.CurrentModule = module;

                var system = principal.FindFirst("system")?.Value;
                if (!string.IsNullOrEmpty(system))
                    user.Year = system;

                return user;
            }
            catch (SecurityTokenException ex)
            {
                Log.Error($"JWT token validation failed: {ex.Message}");
                throw new UnauthorizedAccessException("Invalid or expired JWT token.", ex);
            }
            catch (Exception ex)
            {
                Log.Error($"Error processing JWT token: {ex.Message}");
                throw new UnauthorizedAccessException("Failed to authenticate user from JWT token.", ex);
            }
        }
        else
        {
             throw new NotImplementedException("Unknown MCP security mode.");
        }
    }

}   
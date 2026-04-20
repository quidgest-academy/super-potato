using CSGenio.core.di;
using CSGenio.framework;
using CSGenio.business;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Headers;
using MimeKit;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Linq;
using System.Text.Json.Nodes;


namespace CSGenio.core.ai
{
    // Core service for interacting with the Chatbot API. Ensures endpoints are correctly formatted and requests are securely sent.
    public class ChatbotService : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _chatbotEndpointUrl;

        public ChatbotService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _chatbotEndpointUrl = EnsureApiUrl(Configuration.AiConfig?.APIEndpoint);
        }

        public ChatbotService()
        {
            _httpClient = GenioDI.HttpFactory.CreateClient();
            _chatbotEndpointUrl = EnsureApiUrl(Configuration.AiConfig?.APIEndpoint);
        }

        // Normalizes the API backend URL by ensuring it ends with '/api'.
        public static string EnsureApiUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            var clean = url.Trim().TrimEnd('/');
            if (clean.EndsWith("/api", StringComparison.OrdinalIgnoreCase)) return clean;
            return $"{clean}/api";
        }

        // Constructs the full endpoint URL by appending the specified path.
        private string BuildEndpoint(string path)
        {
            if (string.IsNullOrEmpty(_chatbotEndpointUrl))
            {
                throw new InvalidOperationException("ChatbotEndpointUrl is not configured.");
            }

            string targetUrl = _chatbotEndpointUrl;
            if (!string.IsNullOrEmpty(path))
            {
                path = path.TrimStart('/');
                targetUrl = $"{_chatbotEndpointUrl}/{path}";
            }
            return targetUrl;
        }

        /// <summary>
        /// Generates a JWT token for the given user to authenticate MCP tool calls
        /// </summary>
        /// <param name="user">The user to generate the token for</param>
        /// <returns>JWT token string</returns>
        private static string GenerateJwtToken(User user)
        {
            if (user == null)
                return null;

            if (string.IsNullOrEmpty(Configuration.AiConfig.JWTEncryptionKey))
            {
                throw new InvalidOperationException("JWT encryption key is not configured. Please set Configuration.AiConfig.JWTEncryptionKey.");
            }

            var key = Encoding.UTF8.GetBytes(Configuration.AiConfig.JWTEncryptionKeyDecode);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            if (!string.IsNullOrEmpty(user.CurrentModule))
                claims.Add(new Claim("module", user.CurrentModule));

            if (!string.IsNullOrEmpty(user.Year))
                claims.Add(new Claim("system", user.Year));

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static void SetAuthorizationHeaders(HttpRequestMessage request, User user)
        {
            // Add JWT token to Authorization header if JWT security mode is enabled
            if (Configuration.AiConfig.MCPSecurityMode == MCPSecurityMode.JWT)
            {
                try
                {
                    Log.Debug("Creating jwt token");
                    string jwtToken = GenerateJwtToken(user);
                    if (!string.IsNullOrEmpty(jwtToken))
                    {
                        request.Headers.Add("X-End-User-Token", jwtToken);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error generating JWT token for Chatbot API request. {ex.Message}");                    
                }
            }
        }


        private static string EnrichJsonWithServerData(string content, User user)
        {
            var json = JsonNode.Parse(content);

            json["project"] = Configuration.Application.Name;
            json["user"] = user.Name;
            json["appVersion"] = VersionInfo.GenAssemblyVersion;
            json["mcpUrl"] = Configuration.AiConfig.AppMCPEndpoint;

            return json.ToJsonString();
        }


        /// <summary>
        /// Sends a JSON-based HTTP request to the Chatbot API with user context.
        /// </summary>
        /// <param name="path">The API endpoint path</param>
        /// <param name="method">The HTTP method to use</param>
        /// <param name="content">The request content as a stream</param>
        /// <param name="user">The user context for authentication</param>
        /// <returns>The response content as a string</returns>
        public async Task<string> SendChatbotRequestAsync(string path, HttpMethod method, Stream content, User user)
        {
            // Read the content stream
            string jsonContent;
            using (StreamReader reader = new StreamReader(content))
                jsonContent = await reader.ReadToEndAsync();

            jsonContent = EnrichJsonWithServerData(jsonContent, user);
            // Build the HTTP request
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(BuildEndpoint(path)),
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            SetAuthorizationHeaders(request, user);

            // Send the request and return the response
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpRequestMessage> BuildRequest(string path, HttpMethod method, Stream content) {
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(BuildEndpoint(path))
            };

            string jsonContent;

            using (StreamReader reader = new StreamReader(content))
                jsonContent = await reader.ReadToEndAsync();

            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            return request;

        }

        private MultipartFormDataContent BuildHttpRequestData(AgentRequestData requestData)
        {
            var form = new MultipartFormDataContent();

            // Add all fields from flattened data
            var flattenedData = requestData.FlattenData();
            foreach (var kvp in flattenedData)
            {
                var value = kvp.Value;
                // Serialize complex objects (like JsonSchema) to JSON
                var stringValue = value is string str ? str : System.Text.Json.JsonSerializer.Serialize(value);
                form.Add(new StringContent(stringValue), kvp.Key);
            }

            // Add files separately as byte arrays
            if (requestData.Files != null && requestData.Files.Count > 0)
            {
                foreach (DBFile file in requestData.Files)
                {
                    var fileContent = new ByteArrayContent(file.File);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(MimeTypes.GetMimeType(file.Name));
                    form.Add(fileContent, "filesToProcess", file.Name);
                }
            }

            return form;
        }
        
        /// <summary>
        /// Sends a prompt to the Chatbot API and retrieves the response as a stream.
        /// </summary>
        /// <param name="requestData">The request data as a stream</param>
        /// <returns>The response stream from the API</returns>
        public async Task<Stream> GetChatbotStreamAsync(Stream requestData, User user)
        {
            string jsonContent;
            using (StreamReader reader = new StreamReader(requestData))
                jsonContent = await reader.ReadToEndAsync();

            jsonContent = EnrichJsonWithServerData(jsonContent, user);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, BuildEndpoint("get-job-result"))
            {
                Content = content
            };
            SetAuthorizationHeaders(request, user);

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// Sends a prompt to the Chatbot API and retrieves the response as a stream(formData Handling).
        /// </summary>
        /// <param name="fields">Formdata fields</param>
        /// <param name="files">Formdata files</param>
        /// <returns>The response stream from the API</returns>
        public async Task<Stream> GetChatbotStreamAsync(
            IEnumerable<KeyValuePair<string, string>> fields,
            IEnumerable<(string FileName, string ContentType, Stream Content)> files,
            User user)
        {
            var boundary = Guid.NewGuid().ToString();
            var multipartContent = new MultipartFormDataContent(boundary);

            //Add server side information to form fields, never trust client to send these values
            var enrichedFields = fields.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            enrichedFields["project"] = Configuration.Application.Name;
            enrichedFields["user"] = user.Name;
            enrichedFields["appVersion"] = VersionInfo.GenAssemblyVersion;
            enrichedFields["mcpUrl"] = Configuration.AiConfig.AppMCPEndpoint ?? string.Empty;

            //Add fields
            foreach (var field in enrichedFields)
            {
                var stringContent = new StringContent(field.Value);
                multipartContent.Add(stringContent, $"\"{field.Key}\"");
            }
            //Add files
            foreach (var file in files)
            {
                var fileContent = new StreamContent(file.Content);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(file.ContentType);
                multipartContent.Add(fileContent, file.FileName);
            }
            // Set the Content-Type header with the boundary
            multipartContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data")
            {
                Parameters = { new NameValueHeaderValue("boundary", boundary) }
            };
            var request = new HttpRequestMessage(HttpMethod.Post, BuildEndpoint("prompt/submit"))
            {
                Content = multipartContent
            };
            SetAuthorizationHeaders(request, user);
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// Calls a specific API endpoint on the Chatbot service and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response into.</typeparam>
        /// <param name="requestData">The request data to send.</param>
        /// <param name="endpointPath">The specific API path to call (e.g., "/function/json").</param>
        /// <returns>The deserialized response data.</returns>
        private async Task<T> CallChatbotApiAsync<T>(object requestData, string endpointPath)
        {
            if (string.IsNullOrWhiteSpace(_chatbotEndpointUrl))
            {
                throw new InvalidOperationException("Chatbot endpoint URL is not configured. Please check the client configuration.");
            }

            HttpContent content;
            if (requestData is HttpContent httpContent)
            {
                content = httpContent;
            }
            else
            {
                content = new StringContent(
                    JsonConvert.SerializeObject(requestData),
                    Encoding.UTF8,
                    "application/json");
            }

            var response = await _httpClient.PostAsync($"{_chatbotEndpointUrl}/{endpointPath}", content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                return apiResponse.Data;
            }
            catch (Newtonsoft.Json.JsonException jsonEx)
            {
                throw new InvalidOperationException("Failed to deserialize the response from the chatbot.", jsonEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An unexpected error occurred while calling the chatbot function.", ex);
            }
        }

        /// <summary>
        /// Calls a specific function on the Chatbot API and deserializes the response.
        /// If files are provided, sends them as multipart/form-data; otherwise, sends JSON.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response into</typeparam>
        /// <param name="requestData">The request data to send</param>
        /// <returns>The deserialized response data</returns>
        public Task<T> CallChatbotFunctionAsync<T>(AgentRequestData requestData)
        {
            HttpContent httpRequestData = BuildHttpRequestData(requestData);
            return CallChatbotApiAsync<T>(httpRequestData, "function/json");
        }

        /// <summary>
        /// Calls a specific function on the Chatbot API and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response into</typeparam>
        /// <param name="requestData">The request data to send</param>
        /// <returns>The deserialized response data</returns>
        [Obsolete("Use CallChatbotFunctionAsync<T>(AgentRequestData requestData) instead.")]
        public Task<T> CallChatbotFunctionAsync<T>(object requestData)
        {
            return CallChatbotApiAsync<T>(requestData, "function/json");
        }


        /// <summary>
        /// Calls an agent prompt on the chatbot API.
        /// </summary>
        public Task<T> CallChabotAgentPromptAsync<T>(AgentRequestData requestData)
        {
            // If the AgentContextData.UserPrompt comes filled it's because a message was sent via the input while on the Agent chat
            string path = string.IsNullOrEmpty(requestData.AgentContextData.UserPrompt) ? "prompt/structured" : "prompt/direct-agent-chat";
            var data = requestData.FlattenData();

            return CallChatbotApiAsync<T>(data, path);
        }

        /// <summary>
        /// Sends an HTTP request to retrieve a file from the chatbot service and returns the response stream.
        /// </summary>
        /// <param name="fileName">The name of the file to retrieve.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <returns>
        /// A <see cref="Stream"/> containing the file content from the chatbot service.
        /// </returns>
        public async Task<Stream> GetChatbotFileAsync(string fileName, HttpMethod method)
        {
            var filePath = $"temp/{fileName}";
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(BuildEndpoint(filePath))
            };
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
        
        // Represents the standard API response structure used by the Chatbot service.
        public class ApiResponse<T>
        {
            public bool Success { get; set;}
            public string Message { get; set;}
            public T Data { get; set;}
        }
    }
}

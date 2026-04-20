using CSGenio.framework;
using CSGenio.persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSGenio.core.ai;

public abstract class McpTool
{
    [JsonPropertyName("name")]
    public abstract string Name { get; }

    [JsonPropertyName("title")]
    public abstract string Title { get; }

    [JsonPropertyName("description")]
    public abstract string Description { get; }

    [JsonPropertyName("inputSchema")]
    public abstract McpSchemaBase InputSchema { get; }

    [JsonPropertyName("outputSchema")]
    public abstract McpSchemaBase OutputSchema { get; }

    [JsonPropertyName("annotations")]
    public virtual McpToolAnnotations Annotations { get; } = null;

    /// <summary>
    /// Specifies the minimum role level required to access and execute this tool.
    /// </summary>
    public abstract Role MininumRole { get;}

    /// <summary>
    /// The module where this tool can be executed.
    /// </summary>
    public abstract string AppModule { get; }

    public abstract object Execute(PersistentSupport support, User user, JsonElement arguments);

    /// <summary>
    /// Determines whether the specified user has the required access level for the application module.
    /// </summary>
    /// <param name="user">The user whose access is being evaluated</param>
    public bool HasAccess(User user)
    {
        var roles = user.GetModuleRoles(AppModule);
        return roles.Any(userRole => MininumRole.HasRole(userRole));
    }
}

public class McpSchemaBase
{
    [JsonPropertyName("properties")]
    public Dictionary<string, McpProperty> Properties { get; set; } = new();

    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";
    
    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new();
}

public class McpProperty
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "string";
    [JsonPropertyName("format")]
    public string Format { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("items")]
    public McpProperty Items { get; set; }
    [JsonPropertyName("properties")]
    public Dictionary<string, McpProperty> Properties { get; set; }
}

/// <summary>
/// Optional metadata that provides hints to clients about tool behavior.
/// According to MCP specification.
/// </summary>
public class McpToolAnnotations
{
    /// <summary>
    /// Indicates if the tool performs destructive operations.
    /// </summary>
    [JsonPropertyName("destructiveHint")]
    public bool? DestructiveHint { get; set; }

    /// <summary>
    /// Indicates if the tool is idempotent (can be called multiple times with the same result).
    /// </summary>
    [JsonPropertyName("idempotentHint")]
    public bool? IdempotentHint { get; set; }

    /// <summary>
    /// Indicates if the tool accesses external/open world data.
    /// </summary>
    [JsonPropertyName("openWorldHint")]
    public bool? OpenWorldHint { get; set; }

    /// <summary>
    /// Indicates if the tool only reads data without modifying it.
    /// </summary>
    [JsonPropertyName("readOnlyHint")]
    public bool? ReadOnlyHint { get; set; }

}
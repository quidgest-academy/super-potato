using CSGenio.business;
using System.Collections.Generic;
using System.Linq;

namespace CSGenio.core.ai;

public class AgentRequestData
{
    public object JsonSchema { get; set; }
    public string Prompt { get; set; }
    public string SystemPrompt { get; set; }
    public string Project { get; set; }
    public List<DBFile> Files { get; set; }
    public string McpUrl { get; set; }

    public AgentContextData AgentContextData { get; set; }

    public AgentRequestData(
        object jsonSchema,
        string prompt,
        string systemPrompt,
        string project,
        List<DBFile> files,
        AgentContextData agentContextData,
        string mcpUrl
    )
    {
        JsonSchema = jsonSchema;
        Prompt = prompt;
        SystemPrompt = systemPrompt;
        Project = project;
        Files = files;
        AgentContextData = agentContextData;
        McpUrl = mcpUrl;
    }

    /// <summary>
    /// Flattens the request and context data into a dictionary,
    /// preparing it for serialization and transmission to the chatbot backend.
    /// Excludes Files as they should be handled separately
    /// Null values are excluded from the result.
    /// </summary>
    public Dictionary<string, object> FlattenData()
    {
        var dict = new Dictionary<string, object>
        {
            ["JsonSchema"] = JsonSchema,
            ["Prompt"] = Prompt,
            ["SystemPrompt"] = SystemPrompt,
            ["Project"] = Project,
            ["Username"] = AgentContextData?.Username,
            ["AgentId"] = AgentContextData?.AgentId,
            ["FormId"] = AgentContextData?.FormId,
            ["UserPrompt"] = AgentContextData?.UserPrompt,
            ["CurrentRecordId"] = AgentContextData?.CurrentRecordId,
            ["Module"] = AgentContextData?.Module,
            ["Subsystem"] = AgentContextData?.Subsystem
        };

        // Remove nulls
        return dict
            .Where(kvp => kvp.Value is not null)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value!);
    }
}

public class AgentContextData
{
    public string Username { get; set; }
    public string AgentId { get; set; }
    public string FormId { get; set; }
    public string CurrentRecordId { get; set; }
    public string UserPrompt { get; set; }
    public string Module { get; set; }
    public string Subsystem { get; set; }
}
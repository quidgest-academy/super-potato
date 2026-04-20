using System;
using System.Collections.Generic;
using System.Linq;
using CSGenio.framework;

namespace CSGenio.core.ai;

public class McpToolRepo : IToolRepo
{
    private readonly List<McpTool> _tools;

    public McpToolRepo()
    {
        _tools = new List<McpTool>();
    }

    /// <summary>
    /// Registers an MCP tool in the repository
    /// </summary>
    /// <param name="tool">The MCP tool to register</param>
    public void RegisterTool(McpTool tool)
    {
        if (tool == null)
            throw new ArgumentNullException(nameof(tool));

        if (_tools.Any(t => t.Name == tool.Name))
            throw new InvalidOperationException($"Tool with name '{tool.Name}' is already registered");

        _tools.Add(tool);
    }

    /// <summary>
    /// Gets all tools that a user has access to based on their role and current module
    /// </summary>
    /// <param name="user">The user object containing role and module information</param>
    /// <returns>List of tools the user can access</returns>
    public List<McpTool> GetToolsForUser(User user)
    {
        if (user == null)
            return new List<McpTool>();

        return _tools.Where(tool=> tool.HasAccess(user)).ToList();
    }

    /// <summary>
    /// Gets a tool by name
    /// </summary>
    /// <param name="name">The tool name</param>
    /// <returns>The tool if found, null otherwise</returns>
    public McpTool GetTool(string name)
    {
        return _tools.FirstOrDefault(t => t.Name == name);
    }

}


using System.Collections.Generic;
using CSGenio.framework;

namespace CSGenio.core.ai;

public interface IToolRepo
{
    /// <summary>
    /// Registers an MCP tool in the repository
    /// </summary>
    /// <param name="tool">The MCP tool to register</param>
    void RegisterTool(McpTool tool);

    /// <summary>
    /// Gets all tools that a user has access to based on their role and current module
    /// </summary>
    /// <param name="user">The user object containing role and module information</param>
    /// <returns>List of tools the user can access</returns>
    List<McpTool> GetToolsForUser(User user);

    /// <summary>
    /// Gets a tool by name
    /// </summary>
    /// <param name="name">The tool name</param>
    /// <returns>The tool if found, null otherwise</returns>
    McpTool GetTool(string name);
}
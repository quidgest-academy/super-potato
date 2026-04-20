using System.Collections.Generic;

namespace quidgest.uitests.core;

public class MenuTree
{
    public Dictionary<string, Dictionary<string, MenuTreeNode>> Modules { get; set; } = new();

    public MenuTreeNode FindMenu(string module, string menuId) => Modules[module][menuId];

    public void AddModule(string module) => Modules.Add(module, new Dictionary<string, MenuTreeNode>());

    public void AddMenu(string module, string menuId, string parentId)
    {
        var mod = Modules[module];
        var menu = new MenuTreeNode()
        {
            Id = menuId,
            Parent = parentId != null ? mod[parentId] : null,
        };
        mod.Add(menuId, menu);
    }
}

public class MenuTreeNode
{
    public string Id { get; set; }
    public MenuTreeNode Parent { get; set; }
}


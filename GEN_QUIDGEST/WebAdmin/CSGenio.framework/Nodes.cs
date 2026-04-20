using System;
using System.Data;
using System.Configuration;
using System.Collections;

/// <summary>
/// Summary description for nodos
/// </summary>
namespace CSGenio.framework
{
    public class Nodes
    {
        private string name;

        private string codigo;

        private int level;
                
        private Hashtable ramos;

        public Nodes(string name, string codigo, int level)
        {
            this.name = name;
            this.codigo = codigo;
            this.level = level;
            this.ramos = new Hashtable();

        }

        
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Code
        {
            get { return codigo; }
            set { codigo = value; }
        }

        public int QLevel
        {
            get { return level; }
            set { level = value; }
        }
        public int TotalBranches
        {
            get { return ramos.Count; }
        }

        public void AddBranch(int num, Nodes node_child)
        {
            node_child.QLevel = this.QLevel + 1;
            ramos.Add(num, node_child);
        }

        public Nodes searchBranch(int num)
        {
            return (Nodes)ramos[num];
        }

        public bool HasBranches()
        {
            if (ramos.Count != 0)
                return true;
            else
                return false;

        }

        public string SearchNameLevelBranch(int num)
        {
            if (ramos.ContainsKey(num))
            {
                Nodes node = (Nodes)ramos[num];
                return node.Name+"["+node.QLevel;
            }
            else
            {
                return null;
            }

        }

        public string SearchCodeNameLevelBranch(int num)
        {
            if (ramos.ContainsKey(num))
            {
                Nodes node = (Nodes)ramos[num];
                return node.Code + "[" + node.Name + "[" + node.QLevel;
            }
            else
            {
                return null;
            }

        }

        

    }
}

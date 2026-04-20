using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.Helpers
{
	public class TreeBranchInfo<A> where A : CSGenio.business.Area
	{
		/// <summary>
		/// Branch Id (Loading on demand)
		/// </summary>
		public int BranchLevel { get; set; } = -1;

		/// <summary>
		/// Campo para obter a key primaria do registo que pertence a TreeNode
		/// </summary>
		public FieldRef KeySelector { get; set; }

		/// <summary>
		/// Campo para obter identifier do TreeNode
		/// </summary>
		public FieldRef Selector { get; set; }

		/// <summary>
		/// Campo para obter identifier do parent TreeNode
		/// </summary>
		public FieldRef ParentSelector { get; set; }

		/// <summary>
		/// Identifica se os dados do Branch atual estão com estrutura em arvore
		/// </summary>
		public bool IsTree { get; set; }

		public bool IsTreeTable { get; set; }

		public string Area { get; set;  }

		public string Form { get; set; }

		public Func<object, CriteriaSet> Limit { get; set; }

		public List<ColumnSort> Sorts { get; set; }

		public FieldRef[] SelectFields { get; set; }

		public TreeNode CreateNode(A element)
		{
			var node = new TreeNode()
			{
				// Branch Id
				BId = BranchLevel,
				// Branch area
				Area = Area,
				// Support form
				Form = Form
			};

			if (Selector != null)
				node.Identifier = element.returnValueField(Selector).ToString();

			if (ParentSelector != null)
				node.ParentIdentifier = element.returnValueField(ParentSelector).ToString();

			if (KeySelector != null)
				node.Key = element.returnValueField(KeySelector);

			// Fields
			node.Fields = new Dictionary<string, object>();
			foreach (CSGenio.framework.RequestedField fld in element.Fields.Values)
				node.Fields[fld.FullName] = fld.Value;

			return node;
		}

		public IEnumerable<TreeNode> BuildBranch(IEnumerable<A> items, bool orderBySelector)
		{
			if (items == null)
				return Enumerable.Empty<TreeNode>();

			//Determines which lookup method to use for this branch
			//true - by parent; false - by ID length
			bool lookupByParent = IsTreeTable || ParentSelector != null;

			var nodes = items.Select(item => CreateNode(item));

			// Order the entity list by selector (Identifier)
			if (orderBySelector)
				nodes = nodes.OrderBy(x => x.Identifier);

			// Evalute Linq Expresion
			nodes = nodes.ToList();

			if (IsTree)
			{
				ILookup<string, TreeNode> childNodesByParentId = null;
				ILookup<int, TreeNode> childNodesByIdLength = null;

				if (lookupByParent)
					childNodesByParentId = nodes.ToLookup(n => n.ParentIdentifier);
				else
					childNodesByIdLength = nodes.ToLookup(n => n.Identifier.Length);

				// Calculate the children of each node
				foreach (TreeNode node in nodes)
				{
					IEnumerable<TreeNode> childs = null;
					if (lookupByParent)
					{
						if (childNodesByParentId.Contains(node.Identifier))
							childs = childNodesByParentId[node.Identifier];
					}
					else
					{
						int levelLength = 1,
							currentLevelLength = node.Identifier.Length + levelLength;

						if (childNodesByIdLength.Contains(currentLevelLength))
						{
							var _childs = childNodesByIdLength[currentLevelLength];
							childs = _childs.Where(n => n.Identifier.StartsWith(node.Identifier));
						}
					}

					if (childs != null)
					{
						node.Children = new List<TreeNode>(childs);
						node.Children.ForEach(c => c.HasParent = true);
					}
				}
			}

			// Return just top level
			return nodes.Where(node => node.HasParent == false);
		}

		public IEnumerable<TreeNode> BuildBranch(UserContext userContext, CriteriaSet filters = null, string selectedKey = null, string identifier = null)
		{
			var condition = CriteriaSet.And();

			if (Limit != null)
				condition.SubSet(Limit(selectedKey));
			condition.SubSet(filters);

			var items = Models.ModelBase.Where<A>(userContext, false, condition, SelectFields, 0, -1, Sorts, identifier);

			return BuildBranch(items.Rows, !(Sorts?.Any() ?? false));
		}
	}
}

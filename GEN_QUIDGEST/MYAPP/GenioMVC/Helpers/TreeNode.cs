using System.Text.Json.Serialization;

namespace GenioMVC.Helpers
{
	/// <summary>
	/// Simple implementation of a generic tree node
	/// </summary>
	public class TreeNode
	{
		/// <summary>
		/// Branch Id (Loading on demand)
		/// </summary>
		public int BId { get; set; } = -1;

		/// <summary>
		/// Has Parent?
		/// </summary>
		[JsonIgnore]
		public bool HasParent { get; set; }

		/// <summary>
		/// List containing all the children of this node
		/// </summary>
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public List<TreeNode> Children { get; set; }

		/// <summary>
		/// Identifier of TreeNode
		/// </summary>
		[JsonIgnore]
		public string Identifier { get; set; }

		/// <summary>
		/// Identifier of Parent TreeNode
		/// </summary>
		[JsonIgnore]
		public string ParentIdentifier { get; set; }

		/// <summary>
		/// Area
		/// </summary>
		public string Area { get; set; }

		/// <summary>
		/// Form name
		/// </summary>
		public string Form { get; set; }

		/// <summary>
		/// Key value
		/// </summary>
		public object Key { get; set; }

		/// <summary>
		/// Node Fields
		/// </summary>
		public Dictionary<string, object> Fields { get; set; }
	}
}

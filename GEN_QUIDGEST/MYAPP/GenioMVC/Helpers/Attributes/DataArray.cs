using System.Reflection;

namespace GenioMVC.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
	public class DataArray : Attribute
	{
		private readonly string arrayName;
		private readonly ArrayType arrayType;
		private readonly bool customDynamicArray;

		public DataArray(string arrayName, ArrayType arrayType, bool customDynamicArray = false)
		{
			this.arrayName = arrayName;
			this.arrayType = arrayType;
			this.customDynamicArray = customDynamicArray;
		}

		public Dictionary<string, string> GetDictionary()
		{
			string typeNamespace = this.customDynamicArray ? "GenioServer" : "CSGenio.core";
			Type type = Type.GetType("CSGenio.business.Array" + arrayName + ", " + typeNamespace);
			MethodInfo m = type.GetMethod("GetDictionary");
			switch (arrayType)
			{
				case ArrayType.Numeric:
					return MapNumericArray(m);
				case ArrayType.Character:
					return MapCharArray(m);
				case ArrayType.Logical:
					return MapLogicalArray(m);
				default:
					// Not implemented
					return new Dictionary<string, string>();
			}
		}

		public Dictionary<string, string> MapCharArray(MethodInfo m)
		{
			Dictionary<string, string> dic = m.Invoke(null, null) as Dictionary<string, string>;
			return dic.ToDictionary(p => p.Key, p => Helpers.GetTextFromResources(p.Value));
		}

		public Dictionary<string, string> MapNumericArray(MethodInfo m)
		{
			Dictionary<decimal, string> dic = m.Invoke(null, null) as Dictionary<decimal, string>;
			return dic.ToDictionary(p => p.Key.ToString(), p => Helpers.GetTextFromResources(p.Value));
		}

		public Dictionary<string, string> MapLogicalArray(MethodInfo m)
		{
			Dictionary<int, string> dic = m.Invoke(null, null) as Dictionary<int, string>;
			return dic.ToDictionary(p => p.Key.ToString().ToLower(), p => Helpers.GetTextFromResources(p.Value));
		}
	}

	public enum ArrayType
	{
		Numeric, Character, Logical
	}
}

using System.Collections.Generic;

namespace CSGenio.business
{
	/// <summary>
	/// Array typology (Building typology)
	/// </summary>
	public class ArrayTypology : Array<decimal>
	{
		/// <summary>
		/// The instance
		/// </summary>
		private static readonly ArrayTypology _instance = new ArrayTypology();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static ArrayTypology Instance { get => _instance; }

		/// <summary>
		/// Array code type
		/// </summary>
		public static ArrayType Type { get { return ArrayType.NUMERIC; } }

		/// <summary>
		/// T0
		/// </summary>
		public const decimal E_0_1 = 0M;
		/// <summary>
		/// T1
		/// </summary>
		public const decimal E_1_2 = 1M;
		/// <summary>
		/// T2
		/// </summary>
		public const decimal E_2_3 = 2M;
		/// <summary>
		/// T3 or more
		/// </summary>
		public const decimal E_3_4 = 3M;

		/// <summary>
		/// Prevents a default instance of the <see cref="ArrayTypology"/> class from being created.
		/// </summary>
		private ArrayTypology() : base() {}

		/// <summary>
        /// Loads the dictionary.
        /// </summary>
        /// <returns></returns>
		protected override Dictionary<decimal, ArrayElement> LoadDictionary()
		{
			return new Dictionary<decimal, ArrayElement>()
			{
				{ E_0_1, new ArrayElement() { ResourceId = "T036607", HelpId = "", Group = "" } },
				{ E_1_2, new ArrayElement() { ResourceId = "T133664", HelpId = "", Group = "" } },
				{ E_2_3, new ArrayElement() { ResourceId = "T233813", HelpId = "", Group = "" } },
				{ E_3_4, new ArrayElement() { ResourceId = "T3_OR_MORE43214", HelpId = "", Group = "" } },
			};
		}

		/// <summary>
		/// Gets the element's description.
		/// </summary>
		/// <param name="cod">The cod.</param>
		/// <returns></returns>
		public static string CodToDescricao(decimal cod)
		{
			return Instance.CodToDescricaoImpl(cod);
		}

		/// <summary>
		/// Gets the elements.
		/// </summary>
		/// <returns></returns>
		public static List<decimal> GetElements()
		{
			return Instance.GetElementsImpl();
		}

		/// <summary>
		/// Gets the element.
		/// </summary>
		/// <param name="cod">The cod.</param>
		/// <returns></returns>
		public static ArrayElement GetElement(string cod)
		{
            return Instance.GetElementImpl(decimal.Parse(cod));
        }

		/// <summary>
		/// Gets the dictionary.
		/// </summary>
		/// <returns></returns>
		public static IDictionary<decimal, string> GetDictionary()
		{
			return Instance.GetDictionaryImpl();
		}

		/// <summary>
		/// Gets the help identifier.
		/// </summary>
		/// <param name="cod">The cod.</param>
		/// <returns></returns>
		public static string GetHelpId(string cod)
		{
			return Instance.GetHelpIdImpl(decimal.Parse(cod));
		}
	}
}

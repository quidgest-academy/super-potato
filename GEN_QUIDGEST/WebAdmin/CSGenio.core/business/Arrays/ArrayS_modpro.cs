using System.Collections.Generic;

namespace CSGenio.business
{
	/// <summary>
	/// Array s_modpro (Modo de processamento)
	/// </summary>
	public class ArrayS_modpro : Array<string>
	{
		/// <summary>
		/// The instance
		/// </summary>
		private static readonly ArrayS_modpro _instance = new ArrayS_modpro();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static ArrayS_modpro Instance { get => _instance; }

		/// <summary>
		/// Array code type
		/// </summary>
		public static ArrayType Type { get { return ArrayType.STRING; } }

		/// <summary>
		/// Individual
		/// </summary>
		public const string E_INDIV_1 = "INDIV";
		/// <summary>
		/// Global
		/// </summary>
		public const string E_GLOBAL_2 = "global";
		/// <summary>
		/// Unidade orgânica
		/// </summary>
		public const string E_UNIDADE_3 = "unidade";
		/// <summary>
		/// Horário
		/// </summary>
		public const string E_HORARIO_4 = "horario";

		/// <summary>
		/// Prevents a default instance of the <see cref="ArrayS_modpro"/> class from being created.
		/// </summary>
		private ArrayS_modpro() : base() {}

		/// <summary>
        /// Loads the dictionary.
        /// </summary>
        /// <returns></returns>
		protected override Dictionary<string, ArrayElement> LoadDictionary()
		{
			return new Dictionary<string, ArrayElement>()
			{
				{ E_INDIV_1, new ArrayElement() { ResourceId = "INDIVIDUAL42893", HelpId = "", Group = "" } },
				{ E_GLOBAL_2, new ArrayElement() { ResourceId = "GLOBAL58588", HelpId = "", Group = "" } },
				{ E_UNIDADE_3, new ArrayElement() { ResourceId = "UNIDADE_ORGANICA38383", HelpId = "", Group = "" } },
				{ E_HORARIO_4, new ArrayElement() { ResourceId = "HORARIO56549", HelpId = "", Group = "" } },
			};
		}

		/// <summary>
		/// Gets the element's description.
		/// </summary>
		/// <param name="cod">The cod.</param>
		/// <returns></returns>
		public static string CodToDescricao(string cod)
		{
			return Instance.CodToDescricaoImpl(cod);
		}

		/// <summary>
		/// Gets the elements.
		/// </summary>
		/// <returns></returns>
		public static List<string> GetElements()
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
            return Instance.GetElementImpl(cod);
        }

		/// <summary>
		/// Gets the dictionary.
		/// </summary>
		/// <returns></returns>
		public static IDictionary<string, string> GetDictionary()
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
			return Instance.GetHelpIdImpl(cod);
		}
	}
}

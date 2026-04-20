using System.Collections.Generic;

namespace CSGenio.business
{
	/// <summary>
	/// Array s_resul (Resultado)
	/// </summary>
	public class ArrayS_resul : Array<string>
	{
		/// <summary>
		/// The instance
		/// </summary>
		private static readonly ArrayS_resul _instance = new ArrayS_resul();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static ArrayS_resul Instance { get => _instance; }

		/// <summary>
		/// Array code type
		/// </summary>
		public static ArrayType Type { get { return ArrayType.STRING; } }

		/// <summary>
		/// Sucesso
		/// </summary>
		public const string E_OK_1 = "ok";
		/// <summary>
		/// Erro
		/// </summary>
		public const string E_ER_2 = "er";
		/// <summary>
		/// Aviso
		/// </summary>
		public const string E_WA_3 = "wa";
		/// <summary>
		/// Cancelado
		/// </summary>
		public const string E_C_4 = "c";

		/// <summary>
		/// Prevents a default instance of the <see cref="ArrayS_resul"/> class from being created.
		/// </summary>
		private ArrayS_resul() : base() {}

		/// <summary>
        /// Loads the dictionary.
        /// </summary>
        /// <returns></returns>
		protected override Dictionary<string, ArrayElement> LoadDictionary()
		{
			return new Dictionary<string, ArrayElement>()
			{
				{ E_OK_1, new ArrayElement() { ResourceId = "SUCESSO65230", HelpId = "", Group = "" } },
				{ E_ER_2, new ArrayElement() { ResourceId = "ERRO38355", HelpId = "", Group = "" } },
				{ E_WA_3, new ArrayElement() { ResourceId = "AVISO03237", HelpId = "", Group = "" } },
				{ E_C_4, new ArrayElement() { ResourceId = "CANCELADO05982", HelpId = "", Group = "" } },
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

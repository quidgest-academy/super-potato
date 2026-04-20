using System.Collections.Generic;

namespace CSGenio.business
{
	/// <summary>
	/// Array s_prstat (Estado do processo)
	/// </summary>
	public class ArrayS_prstat : Array<string>
	{
		/// <summary>
		/// The instance
		/// </summary>
		private static readonly ArrayS_prstat _instance = new ArrayS_prstat();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static ArrayS_prstat Instance { get => _instance; }

		/// <summary>
		/// Array code type
		/// </summary>
		public static ArrayType Type { get { return ArrayType.STRING; } }

		/// <summary>
		/// Em execução
		/// </summary>
		public const string E_EE_1 = "EE";
		/// <summary>
		/// Em fila de espera
		/// </summary>
		public const string E_FE_2 = "FE";
		/// <summary>
		/// Agendado para execução
		/// </summary>
		public const string E_AG_3 = "AG";
		/// <summary>
		/// Terminado
		/// </summary>
		public const string E_T_4 = "T";
		/// <summary>
		/// Cancelado
		/// </summary>
		public const string E_C_5 = "C";
		/// <summary>
		/// Não responde
		/// </summary>
		public const string E_NR_6 = "NR";
		/// <summary>
		/// Abortado
		/// </summary>
		public const string E_AB_7 = "AB";
		/// <summary>
		/// A cancelar
		/// </summary>
		public const string E_AC_8 = "AC";

		/// <summary>
		/// Prevents a default instance of the <see cref="ArrayS_prstat"/> class from being created.
		/// </summary>
		private ArrayS_prstat() : base() {}

		/// <summary>
        /// Loads the dictionary.
        /// </summary>
        /// <returns></returns>
		protected override Dictionary<string, ArrayElement> LoadDictionary()
		{
			return new Dictionary<string, ArrayElement>()
			{
				{ E_EE_1, new ArrayElement() { ResourceId = "EM_EXECUCAO53706", HelpId = "", Group = "" } },
				{ E_FE_2, new ArrayElement() { ResourceId = "EM_FILA_DE_ESPERA21822", HelpId = "", Group = "" } },
				{ E_AG_3, new ArrayElement() { ResourceId = "AGENDADO_PARA_EXECUC11223", HelpId = "", Group = "" } },
				{ E_T_4, new ArrayElement() { ResourceId = "TERMINADO46276", HelpId = "", Group = "" } },
				{ E_C_5, new ArrayElement() { ResourceId = "CANCELADO05982", HelpId = "", Group = "" } },
				{ E_NR_6, new ArrayElement() { ResourceId = "NAO_RESPONDE33275", HelpId = "", Group = "" } },
				{ E_AB_7, new ArrayElement() { ResourceId = "ABORTADO52378", HelpId = "", Group = "" } },
				{ E_AC_8, new ArrayElement() { ResourceId = "A_CANCELAR43988", HelpId = "", Group = "" } },
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

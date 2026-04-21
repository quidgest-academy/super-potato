using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;

using CSGenio.framework;
using CSGenio.persistence;
using CSGenio.core.persistence;
using GenioServer.security;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL FOR IMPORTS]/
// USE /[MANUAL FOR IMPORTS GlobalFunctions]/

namespace CSGenio.business
{
	/// <summary>
	/// Summary description for GlobalFunctions.
	/// </summary>
	public sealed partial class GlobalFunctions
	{
		/// <summary>
		/// Initializes all the manual functions.
		/// </summary>
		private static void initTodasFuncoes()
		{
			todasFuncoes = new Hashtable(10, (float)0.5);
			todasFuncoes.Add("password_alterar", 0);
			todasFuncoes.Add("password_verificaAntiga", 1);
			todasFuncoes.Add("validarAssinatura", 2);
			todasFuncoes.Add("devolverCamposAssinatura", 3);
			todasFuncoes.Add("escreverAssinatura", 4);
			todasFuncoes.Add("password_gerar", 5);
			todasFuncoes.Add("CriarDocumQweb", 6);
			todasFuncoes.Add("GetUserProfile", 7);
			//funcoes Csharp
			todasFuncoes.Add("Age", 8);
			todasFuncoes.Add("Average", 9);
			// Cargas
		}

		#region Funções

		/// <summary>
		/// Calculate age
		/// </summary>
		/// <param name="birthdate">Date of birth to calculate the age</param>
		public decimal Age(DateTime? birthdate)
		{
//BEGIN_FUNCTION:a0349c0a-2449-49aa-9b25-2d308f802456
			DateTime today = DateTime.Now;
			int age = 0;
				         		

            if (birthdate != null) {

                age = today.Year - birthdate.Value.Year;
			}
            if (birthdate.Value.Month > today.Month || ((birthdate.Value.Month == today.Month) && (birthdate.Value.Day > today.Day)))
            {
                age--;

            }

            return age;
//END_FUNCTION
		}

		/// <summary>
		/// Calculate the average price of properties
		/// </summary>
		public decimal Average()
		{
//BEGIN_FUNCTION:4c7b4f61-37bc-48d5-a2df-bc378752f723
var average = new SelectQuery()
	.Select(SqlFunctions.Average(CSGenioAprope.FldPrice), "average")
	.From(Area.AreaPROPE);

return DBConversion.ToNumeric(sp.ExecuteScalar(average));
//END_FUNCTION
		}

		#endregion

		#region MANCS


		#endregion

		private static readonly List<string> m_allManualFuntionsNames = new List<string>()
		{
			"Age",
			"Average"
		};

		public static List<string> AllManualFuntionsNames
		{
			get
			{
				return m_allManualFuntionsNames;
			}
		}

		/// <summary>
		/// Check if function can be executed from the outside (from the client-side)
		/// </summary>
		/// <param name="functionName"></param>
		/// <returns></returns>
		public static bool CheckAllowedFunctions(string functionName)
		{
			return m_allManualFuntionsNames.Contains(functionName);
		}
	}
}

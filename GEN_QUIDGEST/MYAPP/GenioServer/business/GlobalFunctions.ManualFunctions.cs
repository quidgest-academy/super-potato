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
			todasFuncoes = new Hashtable(14, (float)0.5);
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
			todasFuncoes.Add("getCityTax", 10);
			todasFuncoes.Add("averagePriceAgent", 11);
			todasFuncoes.Add("propsNotBooking", 12);
			todasFuncoes.Add("propsWithoutContacts", 13);
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
	.From(Area.AreaPROPE)
	.Where(CriteriaSet.And().Equal(CSGenioAprope.FldZzstate,0));

return DBConversion.ToNumeric(sp.ExecuteScalar(average));
//END_FUNCTION
		}

		/// <summary>
		/// Get the last added tax
		/// </summary>
		/// <param name="codProperty"></param>
		public decimal getCityTax(string codProperty)
		{
			try
			{
				object paramcodProperty = QueryUtils.ToValidDbValue(codProperty, FieldType.KEY_VARCHAR);

				SelectQuery query = new SelectQuery()
					.Select(new SqlFunction(SqlFunctionType.Custom,
						"getCityTax"
						, paramcodProperty
						), "x");

				var result = sp.ExecuteScalar(query);
				return DBConversion.ToNumeric(result);
			}
			catch (Exception e)
			{
				throw new BusinessException(null, "GlobalFunctions.decimal getCityTax", "Error on execution: " + e.Message, e);
			}
		}

		/// <summary>
		/// Calculates average price of properties sold by one agent
		/// </summary>
		/// <param name="codagent"></param>
		public decimal averagePriceAgent(string codagent)
		{
//BEGIN_FUNCTION:3188a6b9-8ca5-47e4-a321-7d0597fec81d
            var average = new SelectQuery()
                .Select(SqlFunctions.Average(CSGenioAprope.FldPrice), "averagePerAgent")
                .From(Area.AreaPROPE)
                .Where(CriteriaSet.And()
				.Equal(CSGenioAprope.FldZzstate, 0)
				.Equal(CSGenioAprope.FldCodagent, codagent));

            return DBConversion.ToNumeric(sp.ExecuteScalar(average));
//END_FUNCTION
		}

		/// <summary>
		/// Return properties not booked in specific day
		/// </summary>
		/// <param name="visitDate"></param>
		public System.Collections.Generic.List<object> propsNotBooking(DateTime? visitDate)
		{
//BEGIN_FUNCTION:083f7b70-5add-4e77-9082-ae33d7097a18
			var props = new List<object>(); 
			SelectQuery query = new SelectQuery()
				.Select(CSGenioAprope.FldCodprope)
				.From(Area.AreaPROPE)
				.Join(Area.AreaCONTA, TableJoinType.Left).On(CriteriaSet.And().Equal(CSGenioAconta.FldCodprope, CSGenioAprope.FldCodprope).Equal(CSGenioAconta.FldVisit_date, visitDate?.Date))
				.Where(CriteriaSet.And().Equal(CSGenioAconta.FldVisit_date, null));	

			var data = sp.Execute(query);
			for (int i = 0; i < data.NumRows; i++) {
				props.Add(data.GetString(i, 0));
			
			}

			return props;
//END_FUNCTION
		}

		/// <summary>
		/// Only shows properties without contacts
		/// </summary>
		public System.Collections.Generic.List<object> propsWithoutContacts()
		{
//BEGIN_FUNCTION:075df84f-aa1f-43d4-87aa-a68d14568d2d
			var props = new List<object>();
			SelectQuery query = new SelectQuery()
				.Select(CSGenioAprope.FldTitle)
				.From(Area.AreaPROPE)
				.Join(Area.AreaCONTA, TableJoinType.Left)
					.On(CriteriaSet.And().Equal(CSGenioAprope.FldCodprope, CSGenioAconta.FldCodprope))
				.Where(CriteriaSet.And().Equal(CSGenioAconta.FldVisit_date, null));

            var data = sp.Execute(query);
            for (int i = 0; i < data.NumRows; i++)
            {
                props.Add(data.GetString(i, 0));

            }

            return props;
//END_FUNCTION
		}

		#endregion

		#region MANCS


		#endregion

		private static readonly List<string> m_allManualFuntionsNames = new List<string>()
		{
			"Age",
			"Average",
			"getCityTax",
			"averagePriceAgent",
			"propsNotBooking",
			"propsWithoutContacts"
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

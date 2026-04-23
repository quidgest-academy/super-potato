using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Helpers;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

using SelectList = Microsoft.AspNetCore.Mvc.Rendering.SelectList;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace GenioMVC.Models
{
	public class Conta : ModelBase
	{
		[JsonIgnore]
		public CSGenioAconta klass { get { return baseklass as CSGenioAconta; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("Conta.ValCodconta")]
		public string ValCodconta { get { return klass.ValCodconta; } set { klass.ValCodconta = value; } }

		[DisplayName("Date")]
		/// <summary>Field : "Date" Tipo: "D" Formula:  ""</summary>
		[ShouldSerialize("Conta.ValDate")]
		[DataType(DataType.Date)]
		[DateAttribute("D")]
		public DateTime? ValDate { get { return klass.ValDate; } set { klass.ValDate = value ?? DateTime.MinValue; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("Conta.ValCodprope")]
		public string ValCodprope { get { return klass.ValCodprope; } set { klass.ValCodprope = value; } }

		private Prope _prope;
		[DisplayName("Prope")]
		[ShouldSerialize("Prope")]
		public virtual Prope Prope
		{
			get
			{
				if (!isEmptyModel && (_prope == null || (!string.IsNullOrEmpty(ValCodprope) && (_prope.isEmptyModel || _prope.klass.QPrimaryKey != ValCodprope))))
					_prope = Models.Prope.Find(ValCodprope, m_userContext, Identifier, _fieldsToSerialize);
				_prope ??= new Models.Prope(m_userContext, true, _fieldsToSerialize);
				return _prope;
			}
			set { _prope = value; }
		}

		[DisplayName("Client name")]
		/// <summary>Field : "Client name" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Conta.ValClient")]
		public string ValClient { get { return klass.ValClient; } set { klass.ValClient = value; } }

		[DisplayName("Email do cliente")]
		/// <summary>Field : "Email do cliente" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Conta.ValEmail")]
		public string ValEmail { get { return klass.ValEmail; } set { klass.ValEmail = value; } }

		[DisplayName("Phone number")]
		/// <summary>Field : "Phone number" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Conta.ValPhone")]
		public string ValPhone { get { return klass.ValPhone; } set { klass.ValPhone = value; } }

		[DisplayName("Description")]
		/// <summary>Field : "Description" Tipo: "MO" Formula:  ""</summary>
		[ShouldSerialize("Conta.ValDescript")]
		[DataType(DataType.MultilineText)]
		public string ValDescript { get { return klass.ValDescript; } set { klass.ValDescript = value; } }

		[DisplayName("Preço")]
		/// <summary>Field : "Preço" Tipo: "$" Formula: ++ "[PROPE->PRICE]"</summary>
		[ShouldSerialize("Conta.ValPricepro")]
		[CurrencyAttribute("EUR", 4)]
		public decimal? ValPricepro { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValPricepro, 4)); } set { klass.ValPricepro = Convert.ToDecimal(value); } }

		[DisplayName("Visit Date")]
		/// <summary>Field : "Visit Date" Tipo: "D" Formula:  ""</summary>
		[ShouldSerialize("Conta.ValVisit_date")]
		[DataType(DataType.Date)]
		[DateAttribute("D")]
		public DateTime? ValVisit_date { get { return klass.ValVisit_date; } set { klass.ValVisit_date = value ?? DateTime.MinValue; } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("Conta.ValZzstate")]
		/// <summary>Field: "ZZSTATE", Type: "INT", Formula: ""</summary>
		public virtual int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public Conta(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAconta(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public Conta(UserContext userContext, CSGenioAconta val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}

		public void FillRelatedAreas(CSGenioAconta csgenioa)
		{
			if (csgenioa == null)
				return;

			foreach (RequestedField Qfield in csgenioa.Fields.Values)
			{
				switch (Qfield.Area)
				{
					case "prope":
						_prope ??= new Prope(m_userContext, true, _fieldsToSerialize);
						_prope.klass.insertNameValueField(Qfield.FullName, Qfield.Value);
						break;
					default:
						break;
				}
			}
		}

		/// <summary>
		/// Search the row by key.
		/// </summary>
		/// <param name="id">The primary key.</param>
		/// <param name="userCtx">The user context.</param>
		/// <param name="identifier">The identifier.</param>
		/// <param name="fieldsToSerialize">The fields to serialize.</param>
		/// <param name="fieldsToQuery">The fields to query.</param>
		/// <returns>Model or NULL</returns>
		public static Conta Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAconta>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new Conta(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<Conta> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAconta>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<Conta>((r) => new Conta(userCtx, r));
		}

// USE /[MANUAL FOR MODEL CONTA]/
	}
}

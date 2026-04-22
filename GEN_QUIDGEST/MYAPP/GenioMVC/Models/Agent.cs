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
	public class Agent : ModelBase
	{
		[JsonIgnore]
		public CSGenioAagent klass { get { return baseklass as CSGenioAagent; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValCodagent")]
		public string ValCodagent { get { return klass.ValCodagent; } set { klass.ValCodagent = value; } }

		[DisplayName("Photography")]
		/// <summary>Field : "Photography" Tipo: "IJ" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValPhotography")]
		[ImageThumbnailJsonConverter(75, 75)]
		public ImageModel ValPhotography { get { return new ImageModel(klass.ValPhotography) { Ticket = ValPhotographyQTicket }; } set { klass.ValPhotography = value; } }
		[JsonIgnore]
		public string ValPhotographyQTicket = null;

		[DisplayName("Agent's name")]
		/// <summary>Field : "Agent's name" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValName")]
		public string ValName { get { return klass.ValName; } set { klass.ValName = value; } }

		[DisplayName("Birthdate")]
		/// <summary>Field : "Birthdate" Tipo: "D" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValBirthdat")]
		[DataType(DataType.Date)]
		[DateAttribute("D")]
		public DateTime? ValBirthdat { get { return klass.ValBirthdat; } set { klass.ValBirthdat = value ?? DateTime.MinValue; } }

		[DisplayName("E-mail")]
		/// <summary>Field : "E-mail" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValEmail")]
		public string ValEmail { get { return klass.ValEmail; } set { klass.ValEmail = value; } }

		[DisplayName("Telephone")]
		/// <summary>Field : "Telephone" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValTelephon")]
		public string ValTelephon { get { return klass.ValTelephon; } set { klass.ValTelephon = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValCborn")]
		public string ValCborn { get { return klass.ValCborn; } set { klass.ValCborn = value; } }

		private Cborn _cborn;
		[DisplayName("Cborn")]
		[ShouldSerialize("Cborn")]
		public virtual Cborn Cborn
		{
			get
			{
				if (!isEmptyModel && (_cborn == null || (!string.IsNullOrEmpty(ValCborn) && (_cborn.isEmptyModel || _cborn.klass.QPrimaryKey != ValCborn))))
					_cborn = Models.Cborn.Find(ValCborn, m_userContext, Identifier, _fieldsToSerialize);
				_cborn ??= new Models.Cborn(m_userContext, true, _fieldsToSerialize);
				return _cborn;
			}
			set { _cborn = value; }
		}

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValCodcaddr")]
		public string ValCodcaddr { get { return klass.ValCodcaddr; } set { klass.ValCodcaddr = value; } }

		private Caddr _caddr;
		[DisplayName("Caddr")]
		[ShouldSerialize("Caddr")]
		public virtual Caddr Caddr
		{
			get
			{
				if (!isEmptyModel && (_caddr == null || (!string.IsNullOrEmpty(ValCodcaddr) && (_caddr.isEmptyModel || _caddr.klass.QPrimaryKey != ValCodcaddr))))
					_caddr = Models.Caddr.Find(ValCodcaddr, m_userContext, Identifier, _fieldsToSerialize);
				_caddr ??= new Models.Caddr(m_userContext, true, _fieldsToSerialize);
				return _caddr;
			}
			set { _caddr = value; }
		}

		[DisplayName("Number of properties")]
		/// <summary>Field : "Number of properties" Tipo: "N" Formula: SR "[PROPE->1]"</summary>
		[ShouldSerialize("Agent.ValNrprops")]
		[NumericAttribute(0)]
		public decimal? ValNrprops { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValNrprops, 0)); } set { klass.ValNrprops = Convert.ToDecimal(value); } }

		[DisplayName("Profit")]
		/// <summary>Field : "Profit" Tipo: "$" Formula: SR "[PROPE->PROFIT]"</summary>
		[ShouldSerialize("Agent.ValProfit")]
		[CurrencyAttribute("EUR", 2)]
		public decimal? ValProfit { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValProfit, 2)); } set { klass.ValProfit = Convert.ToDecimal(value); } }

		[DisplayName("Last property sold (price)")]
		/// <summary>Field : "Last property sold (price)" Tipo: "$" Formula: U1 "PROPE[PROPE->DTSOLD][PROPE->PRICE]"</summary>
		[ShouldSerialize("Agent.ValLastprop")]
		[CurrencyAttribute("EUR", 2)]
		public decimal? ValLastprop { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValLastprop, 2)); } set { klass.ValLastprop = Convert.ToDecimal(value); } }

		[DisplayName("Age")]
		/// <summary>Field : "Age" Tipo: "N" Formula: + "Age([AGENT->BIRTHDAT])"</summary>
		[ShouldSerialize("Agent.ValAge")]
		[NumericAttribute(0)]
		public decimal? ValAge { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValAge, 0)); } set { klass.ValAge = Convert.ToDecimal(value); } }

		[DisplayName("AveragePrice")]
		/// <summary>Field : "AveragePrice" Tipo: "$" Formula: + "averagePriceAgent([AGENT->CODAGENT])"</summary>
		[ShouldSerialize("Agent.ValAverage_price")]
		[CurrencyAttribute("EUR", 2)]
		public decimal? ValAverage_price { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValAverage_price, 2)); } set { klass.ValAverage_price = Convert.ToDecimal(value); } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("Agent.ValZzstate")]
		/// <summary>Field: "ZZSTATE", Type: "INT", Formula: ""</summary>
		public virtual int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public Agent(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAagent(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public Agent(UserContext userContext, CSGenioAagent val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}

		public void FillRelatedAreas(CSGenioAagent csgenioa)
		{
			if (csgenioa == null)
				return;

			foreach (RequestedField Qfield in csgenioa.Fields.Values)
			{
				switch (Qfield.Area)
				{
					case "cborn":
						_cborn ??= new Cborn(m_userContext, true, _fieldsToSerialize);
						_cborn.klass.insertNameValueField(Qfield.FullName, Qfield.Value);
						break;
					case "caddr":
						_caddr ??= new Caddr(m_userContext, true, _fieldsToSerialize);
						_caddr.klass.insertNameValueField(Qfield.FullName, Qfield.Value);
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
		public static Agent Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAagent>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new Agent(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<Agent> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAagent>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<Agent>((r) => new Agent(userCtx, r));
		}

// USE /[MANUAL FOR MODEL AGENT]/
	}
}

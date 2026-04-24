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
	public class Stats : ModelBase
	{
		[JsonIgnore]
		public CSGenioAstats klass { get { return baseklass as CSGenioAstats; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("Stats.ValCodstats")]
		public string ValCodstats { get { return klass.ValCodstats; } set { klass.ValCodstats = value; } }

		[DisplayName("Country")]
		/// <summary>Field : "Country" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Stats.ValCountry")]
		public string ValCountry { get { return klass.ValCountry; } set { klass.ValCountry = value; } }

		[DisplayName("Properties Not Sold")]
		/// <summary>Field : "Properties Not Sold" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("Stats.ValNotsold")]
		[NumericAttribute(0)]
		public decimal? ValNotsold { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValNotsold, 0)); } set { klass.ValNotsold = Convert.ToDecimal(value); } }

		[DisplayName("Profit")]
		/// <summary>Field : "Profit" Tipo: "$" Formula:  ""</summary>
		[ShouldSerialize("Stats.ValProfit")]
		[CurrencyAttribute("EUR", 2)]
		public decimal? ValProfit { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValProfit, 2)); } set { klass.ValProfit = Convert.ToDecimal(value); } }

		[DisplayName("Properties Sold")]
		/// <summary>Field : "Properties Sold" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("Stats.ValSold")]
		[NumericAttribute(0)]
		public decimal? ValSold { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValSold, 0)); } set { klass.ValSold = Convert.ToDecimal(value); } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("Stats.ValZzstate")]
		/// <summary>Field: "ZZSTATE", Type: "INT", Formula: ""</summary>
		public virtual int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public Stats(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAstats(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public Stats(UserContext userContext, CSGenioAstats val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}

		public void FillRelatedAreas(CSGenioAstats csgenioa)
		{
			if (csgenioa == null)
				return;

			foreach (RequestedField Qfield in csgenioa.Fields.Values)
			{
				switch (Qfield.Area)
				{
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
		public static Stats Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAstats>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new Stats(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<Stats> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAstats>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<Stats>((r) => new Stats(userCtx, r));
		}

// USE /[MANUAL FOR MODEL STATS]/
	}
}

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
	public class Prope : ModelBase
	{
		[JsonIgnore]
		public CSGenioAprope klass { get { return baseklass as CSGenioAprope; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValCodprope")]
		public string ValCodprope { get { return klass.ValCodprope; } set { klass.ValCodprope = value; } }

		[DisplayName("Main photo")]
		/// <summary>Field : "Main photo" Tipo: "IJ" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValPhoto")]
		[ImageThumbnailJsonConverter(75, 75)]
		public ImageModel ValPhoto { get { return new ImageModel(klass.ValPhoto) { Ticket = ValPhotoQTicket }; } set { klass.ValPhoto = value; } }
		[JsonIgnore]
		public string ValPhotoQTicket = null;

		[DisplayName("Title")]
		/// <summary>Field : "Title" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValTitle")]
		public string ValTitle { get { return klass.ValTitle; } set { klass.ValTitle = value; } }

		[DisplayName("Price")]
		/// <summary>Field : "Price" Tipo: "$" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValPrice")]
		[CurrencyAttribute("EUR", 2)]
		public decimal? ValPrice { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValPrice, 2)); } set { klass.ValPrice = Convert.ToDecimal(value); } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValCodagent")]
		public string ValCodagent { get { return klass.ValCodagent; } set { klass.ValCodagent = value; } }

		private Agent _agent;
		[DisplayName("Agent")]
		[ShouldSerialize("Agent")]
		public virtual Agent Agent
		{
			get
			{
				if (!isEmptyModel && (_agent == null || (!string.IsNullOrEmpty(ValCodagent) && (_agent.isEmptyModel || _agent.klass.QPrimaryKey != ValCodagent))))
					_agent = Models.Agent.Find(ValCodagent, m_userContext, Identifier, _fieldsToSerialize);
				_agent ??= new Models.Agent(m_userContext, true, _fieldsToSerialize);
				return _agent;
			}
			set { _agent = value; }
		}

		[DisplayName("Description")]
		/// <summary>Field : "Description" Tipo: "MO" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValDescript")]
		[DataType(DataType.MultilineText)]
		public string ValDescript { get { return klass.ValDescript; } set { klass.ValDescript = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValCodcity")]
		public string ValCodcity { get { return klass.ValCodcity; } set { klass.ValCodcity = value; } }

		private City _city;
		[DisplayName("City")]
		[ShouldSerialize("City")]
		public virtual City City
		{
			get
			{
				if (!isEmptyModel && (_city == null || (!string.IsNullOrEmpty(ValCodcity) && (_city.isEmptyModel || _city.klass.QPrimaryKey != ValCodcity))))
					_city = Models.City.Find(ValCodcity, m_userContext, Identifier, _fieldsToSerialize);
				_city ??= new Models.City(m_userContext, true, _fieldsToSerialize);
				return _city;
			}
			set { _city = value; }
		}

		[DisplayName("Size (m2)")]
		/// <summary>Field : "Size (m2)" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValSize")]
		[NumericAttribute(0)]
		public decimal? ValSize { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValSize, 0)); } set { klass.ValSize = Convert.ToDecimal(value); } }

		[DisplayName("Bathrooms number")]
		/// <summary>Field : "Bathrooms number" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValBathnr")]
		[NumericAttribute(0)]
		public decimal? ValBathnr { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValBathnr, 0)); } set { klass.ValBathnr = Convert.ToDecimal(value); } }

		[DisplayName("Construction date")]
		/// <summary>Field : "Construction date" Tipo: "D" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValDtconst")]
		[DataType(DataType.Date)]
		[DateAttribute("D")]
		public DateTime? ValDtconst { get { return klass.ValDtconst; } set { klass.ValDtconst = value ?? DateTime.MinValue; } }

		[DisplayName("Building type")]
		/// <summary>Field : "Building type" Tipo: "AC" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValBuildtyp")]
		[DataArray("Buildtyp", GenioMVC.Helpers.ArrayType.Character)]
		public string ValBuildtyp { get { return klass.ValBuildtyp; } set { klass.ValBuildtyp = value; } }
		[JsonIgnore]
		public SelectList ArrayValbuildtyp { get { return new SelectList(CSGenio.business.ArrayBuildtyp.GetDictionary(), "Key", "Value", ValBuildtyp); } set { ValBuildtyp = value.SelectedValue as string; } }

		[DisplayName("Building typology")]
		/// <summary>Field : "Building typology" Tipo: "AN" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValTypology")]
		[DataArray("Typology", GenioMVC.Helpers.ArrayType.Numeric)]
		public decimal ValTypology { get { return klass.ValTypology; } set { klass.ValTypology = value; } }
		[JsonIgnore]
		public SelectList ArrayValtypology { get { return new SelectList(CSGenio.business.ArrayTypology.GetDictionary(), "Key", "Value", ValTypology); } set { ValTypology = Convert.ToDecimal(value.SelectedValue); } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValId")]
		[NumericAttribute(0)]
		public decimal? ValId { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValId, 0)); } set { klass.ValId = Convert.ToDecimal(value); } }

		[DisplayName("Building age")]
		/// <summary>Field : "Building age" Tipo: "N" Formula: + "Year([Today])- Year([PROPE->DTCONST])"</summary>
		[ShouldSerialize("Prope.ValBuildage")]
		[NumericAttribute(0)]
		public decimal? ValBuildage { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValBuildage, 0)); } set { klass.ValBuildage = Convert.ToDecimal(value); } }

		[DisplayName("Ground size")]
		/// <summary>Field : "Ground size" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValGrdsize")]
		[NumericAttribute(0)]
		public decimal? ValGrdsize { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValGrdsize, 0)); } set { klass.ValGrdsize = Convert.ToDecimal(value); } }

		[DisplayName("Floor")]
		/// <summary>Field : "Floor" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValFloornr")]
		[NumericAttribute(0)]
		public decimal? ValFloornr { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValFloornr, 0)); } set { klass.ValFloornr = Convert.ToDecimal(value); } }

		[DisplayName("Sold")]
		/// <summary>Field : "Sold" Tipo: "L" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValSold")]
		public bool ValSold { get { return Convert.ToBoolean(klass.ValSold); } set { klass.ValSold = Convert.ToInt32(value); } }

		[DisplayName("Profit")]
		/// <summary>Field : "Profit" Tipo: "$" Formula: + "iif([PROPE->SOLD]==1,[PROPE->PRICE],0)"</summary>
		[ShouldSerialize("Prope.ValProfit")]
		[CurrencyAttribute("EUR", 2)]
		public decimal? ValProfit { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValProfit, 2)); } set { klass.ValProfit = Convert.ToDecimal(value); } }

		[DisplayName("Sold date")]
		/// <summary>Field : "Sold date" Tipo: "D" Formula:  ""</summary>
		[ShouldSerialize("Prope.ValDtsold")]
		[DataType(DataType.Date)]
		[DateAttribute("D")]
		public DateTime? ValDtsold { get { return klass.ValDtsold; } set { klass.ValDtsold = value ?? DateTime.MinValue; } }

		[DisplayName("AveragePrice")]
		/// <summary>Field : "AveragePrice" Tipo: "N" Formula: + "Average()"</summary>
		[ShouldSerialize("Prope.ValAverage")]
		[NumericAttribute(0)]
		public decimal? ValAverage { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValAverage, 0)); } set { klass.ValAverage = Convert.ToDecimal(value); } }

		[DisplayName("Tax")]
		/// <summary>Field : "Tax" Tipo: "N" Formula: + "getCityTax([PROPE->CODPROPE])"</summary>
		[ShouldSerialize("Prope.ValTax")]
		[NumericAttribute(2)]
		public decimal? ValTax { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValTax, 2)); } set { klass.ValTax = Convert.ToDecimal(value); } }

		[DisplayName("Total Contacts")]
		/// <summary>Field : "Total Contacts" Tipo: "N" Formula: SR "[CONTA->1]"</summary>
		[ShouldSerialize("Prope.ValNumbercontacts")]
		[NumericAttribute(0)]
		public decimal? ValNumbercontacts { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValNumbercontacts, 0)); } set { klass.ValNumbercontacts = Convert.ToDecimal(value); } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("Prope.ValZzstate")]
		/// <summary>Field: "ZZSTATE", Type: "INT", Formula: ""</summary>
		public virtual int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public Prope(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAprope(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public Prope(UserContext userContext, CSGenioAprope val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}

		public void FillRelatedAreas(CSGenioAprope csgenioa)
		{
			if (csgenioa == null)
				return;

			foreach (RequestedField Qfield in csgenioa.Fields.Values)
			{
				switch (Qfield.Area)
				{
					case "agent":
						_agent ??= new Agent(m_userContext, true, _fieldsToSerialize);
						_agent.klass.insertNameValueField(Qfield.FullName, Qfield.Value);
						break;
					case "city":
						_city ??= new City(m_userContext, true, _fieldsToSerialize);
						_city.klass.insertNameValueField(Qfield.FullName, Qfield.Value);
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
		public static Prope Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAprope>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new Prope(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<Prope> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAprope>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<Prope>((r) => new Prope(userCtx, r));
		}

// USE /[MANUAL FOR MODEL PROPE]/
	}
}

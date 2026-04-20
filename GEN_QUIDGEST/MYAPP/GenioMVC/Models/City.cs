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
	public class City : ModelBase
	{
		[JsonIgnore]
		public CSGenioAcity klass { get { return baseklass as CSGenioAcity; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("City.ValCodcity")]
		public string ValCodcity { get { return klass.ValCodcity; } set { klass.ValCodcity = value; } }

		[DisplayName("City")]
		/// <summary>Field : "City" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("City.ValCity")]
		public string ValCity { get { return klass.ValCity; } set { klass.ValCity = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("City.ValCodcount")]
		public string ValCodcount { get { return klass.ValCodcount; } set { klass.ValCodcount = value; } }

		private Count _count;
		[DisplayName("Count")]
		[ShouldSerialize("Count")]
		public virtual Count Count
		{
			get
			{
				if (!isEmptyModel && (_count == null || (!string.IsNullOrEmpty(ValCodcount) && (_count.isEmptyModel || _count.klass.QPrimaryKey != ValCodcount))))
					_count = Models.Count.Find(ValCodcount, m_userContext, Identifier, _fieldsToSerialize);
				_count ??= new Models.Count(m_userContext, true, _fieldsToSerialize);
				return _count;
			}
			set { _count = value; }
		}

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("City.ValZzstate")]
		/// <summary>Field: "ZZSTATE", Type: "INT", Formula: ""</summary>
		public virtual int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public City(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAcity(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public City(UserContext userContext, CSGenioAcity val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}

		public void FillRelatedAreas(CSGenioAcity csgenioa)
		{
			if (csgenioa == null)
				return;

			foreach (RequestedField Qfield in csgenioa.Fields.Values)
			{
				switch (Qfield.Area)
				{
					case "count":
						_count ??= new Count(m_userContext, true, _fieldsToSerialize);
						_count.klass.insertNameValueField(Qfield.FullName, Qfield.Value);
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
		public static City Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAcity>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new City(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<City> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAcity>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<City>((r) => new City(userCtx, r));
		}

// USE /[MANUAL FOR MODEL CITY]/
	}
}

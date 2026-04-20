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
	public class S_pax : ModelBase
	{
		[JsonIgnore]
		public CSGenioAs_pax klass { get { return baseklass as CSGenioAs_pax; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("S_pax.ValCodpranx")]
		public string ValCodpranx { get { return klass.ValCodpranx; } set { klass.ValCodpranx = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("S_pax.ValCods_apr")]
		public string ValCods_apr { get { return klass.ValCods_apr; } set { klass.ValCods_apr = value; } }

		private S_apr _s_apr;
		[DisplayName("S_apr")]
		[ShouldSerialize("S_apr")]
		public virtual S_apr S_apr
		{
			get
			{
				if (!isEmptyModel && (_s_apr == null || (!string.IsNullOrEmpty(ValCods_apr) && (_s_apr.isEmptyModel || _s_apr.klass.QPrimaryKey != ValCods_apr))))
					_s_apr = Models.S_apr.Find(ValCods_apr, m_userContext, Identifier, _fieldsToSerialize);
				_s_apr ??= new Models.S_apr(m_userContext, true, _fieldsToSerialize);
				return _s_apr;
			}
			set { _s_apr = value; }
		}

		[DisplayName("Document")]
		/// <summary>Field : "Document" Tipo: "IB" Formula:  ""</summary>
		[ShouldSerialize("S_pax.ValDocument")]
		[Document("ValDocument", true, false, false)]
		public string ValDocument { get { return klass.ValDocument; } set { klass.ValDocument = value; } }
		public string ValDocumentfk { get { return klass.ValDocumentfk; } set { klass.ValDocumentfk = value; } }

		[DisplayName("Created by")]
		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		[ShouldSerialize("S_pax.ValOpercria")]
		public string ValOpercria { get { return klass.ValOpercria; } set { klass.ValOpercria = value; } }

		[DisplayName("Created on")]
		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		[ShouldSerialize("S_pax.ValDatacria")]
		[DataType(DataType.Date)]
		[DateAttribute("OD")]
		public DateTime? ValDatacria { get { return klass.ValDatacria; } set { klass.ValDatacria = value ?? DateTime.Now;  } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("S_pax.ValZzstate")]
		/// <summary>Field: "ZZSTATE", Type: "INT", Formula: ""</summary>
		public virtual int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public S_pax(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAs_pax(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public S_pax(UserContext userContext, CSGenioAs_pax val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}

		public void FillRelatedAreas(CSGenioAs_pax csgenioa)
		{
			if (csgenioa == null)
				return;

			foreach (RequestedField Qfield in csgenioa.Fields.Values)
			{
				switch (Qfield.Area)
				{
					case "s_apr":
						_s_apr ??= new S_apr(m_userContext, true, _fieldsToSerialize);
						_s_apr.klass.insertNameValueField(Qfield.FullName, Qfield.Value);
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
		public static S_pax Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAs_pax>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new S_pax(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<S_pax> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAs_pax>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<S_pax>((r) => new S_pax(userCtx, r));
		}

// USE /[MANUAL FOR MODEL S_PAX]/
	}
}

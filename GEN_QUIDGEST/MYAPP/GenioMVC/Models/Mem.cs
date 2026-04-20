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
	public class Mem : ModelBase
	{
		[JsonIgnore]
		public CSGenioAmem klass { get { return baseklass as CSGenioAmem; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("Mem.ValCodmem")]
		public string ValCodmem { get { return klass.ValCodmem; } set { klass.ValCodmem = value; } }

		[DisplayName("Name")]
		/// <summary>Field : "Name" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Mem.ValLogin")]
		public string ValLogin { get { return klass.ValLogin; } set { klass.ValLogin = value; } }

		[DisplayName("Routine")]
		/// <summary>Field : "Routine" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Mem.ValRotina")]
		public string ValRotina { get { return klass.ValRotina; } set { klass.ValRotina = value; } }

		[DisplayName("Date")]
		/// <summary>Field : "Date" Tipo: "D" Formula:  ""</summary>
		[ShouldSerialize("Mem.ValAltura")]
		[DataType(DataType.Date)]
		[DateAttribute("D")]
		public DateTime? ValAltura { get { return klass.ValAltura; } set { klass.ValAltura = value ?? DateTime.MinValue; } }

		[DisplayName("Obs")]
		/// <summary>Field : "Obs" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Mem.ValObs")]
		public string ValObs { get { return klass.ValObs; } set { klass.ValObs = value; } }

		[DisplayName("Host")]
		/// <summary>Field : "Host" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Mem.ValHostid")]
		public string ValHostid { get { return klass.ValHostid; } set { klass.ValHostid = value; } }

		[DisplayName("Client ip address")]
		/// <summary>Field : "Client ip address" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Mem.ValClientid")]
		public string ValClientid { get { return klass.ValClientid; } set { klass.ValClientid = value; } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("Mem.ValZzstate")]
		/// <summary>Field: "ZZSTATE", Type: "INT", Formula: ""</summary>
		public virtual int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public Mem(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAmem(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public Mem(UserContext userContext, CSGenioAmem val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}

		public void FillRelatedAreas(CSGenioAmem csgenioa)
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
		public static Mem Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAmem>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new Mem(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<Mem> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAmem>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<Mem>((r) => new Mem(userCtx, r));
		}

// USE /[MANUAL FOR MODEL MEM]/
	}
}

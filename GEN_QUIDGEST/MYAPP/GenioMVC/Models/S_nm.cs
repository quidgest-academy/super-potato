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
	public class S_nm : ModelBase
	{
		[JsonIgnore]
		public CSGenioAs_nm klass { get { return baseklass as CSGenioAs_nm; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValCodmesgs")]
		public string ValCodmesgs { get { return klass.ValCodmesgs; } set { klass.ValCodmesgs = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValCodsigna")]
		public string ValCodsigna { get { return klass.ValCodsigna; } set { klass.ValCodsigna = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValCodpmail")]
		public string ValCodpmail { get { return klass.ValCodpmail; } set { klass.ValCodpmail = value; } }

		[DisplayName("Sender")]
		/// <summary>Field : "Sender" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValFrom")]
		public string ValFrom { get { return klass.ValFrom; } set { klass.ValFrom = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValCodtpnot")]
		public string ValCodtpnot { get { return klass.ValCodtpnot; } set { klass.ValCodtpnot = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValCoddestn")]
		public string ValCoddestn { get { return klass.ValCoddestn; } set { klass.ValCoddestn = value; } }

		[DisplayName("To")]
		/// <summary>Field : "To" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValTo")]
		public string ValTo { get { return klass.ValTo; } set { klass.ValTo = value; } }

		[DisplayName("Manual destination")]
		/// <summary>Field : "Manual destination" Tipo: "L" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValDestnman")]
		public bool ValDestnman { get { return Convert.ToBoolean(klass.ValDestnman); } set { klass.ValDestnman = Convert.ToInt32(value); } }

		[DisplayName("Manual destination")]
		/// <summary>Field : "Manual destination" Tipo: "MO" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValTomanual")]
		[DataType(DataType.MultilineText)]
		public string ValTomanual { get { return klass.ValTomanual; } set { klass.ValTomanual = value; } }

		[DisplayName("Cc")]
		/// <summary>Field : "Cc" Tipo: "MO" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValCc")]
		[DataType(DataType.MultilineText)]
		public string ValCc { get { return klass.ValCc; } set { klass.ValCc = value; } }

		[DisplayName("Bcc")]
		/// <summary>Field : "Bcc" Tipo: "MO" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValBcc")]
		[DataType(DataType.MultilineText)]
		public string ValBcc { get { return klass.ValBcc; } set { klass.ValBcc = value; } }

		[DisplayName("Notification ID")]
		/// <summary>Field : "Notification ID" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValIdnotif")]
		public string ValIdnotif { get { return klass.ValIdnotif; } set { klass.ValIdnotif = value; } }

		[DisplayName("Create a website alert")]
		/// <summary>Field : "Create a website alert" Tipo: "L" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValNotifica")]
		public bool ValNotifica { get { return Convert.ToBoolean(klass.ValNotifica); } set { klass.ValNotifica = Convert.ToInt32(value); } }

		[DisplayName("Sends email?")]
		/// <summary>Field : "Sends email?" Tipo: "L" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValEmail")]
		public bool ValEmail { get { return Convert.ToBoolean(klass.ValEmail); } set { klass.ValEmail = Convert.ToInt32(value); } }

		[DisplayName("Subject")]
		/// <summary>Field : "Subject" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValAssunto")]
		public string ValAssunto { get { return klass.ValAssunto; } set { klass.ValAssunto = value; } }

		[DisplayName("Aggregate")]
		/// <summary>Field : "Aggregate" Tipo: "L" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValAgregado")]
		public bool ValAgregado { get { return Convert.ToBoolean(klass.ValAgregado); } set { klass.ValAgregado = Convert.ToInt32(value); } }

		[DisplayName("Sends attachment?")]
		/// <summary>Field : "Sends attachment?" Tipo: "L" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValAnexo")]
		public bool ValAnexo { get { return Convert.ToBoolean(klass.ValAnexo); } set { klass.ValAnexo = Convert.ToInt32(value); } }

		[DisplayName("HTML format?")]
		/// <summary>Field : "HTML format?" Tipo: "L" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValHtml")]
		public bool ValHtml { get { return Convert.ToBoolean(klass.ValHtml); } set { klass.ValHtml = Convert.ToInt32(value); } }

		[DisplayName("Enabled?")]
		/// <summary>Field : "Enabled?" Tipo: "L" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValAtivo")]
		public bool ValAtivo { get { return Convert.ToBoolean(klass.ValAtivo); } set { klass.ValAtivo = Convert.ToInt32(value); } }

		[DisplayName("Name")]
		/// <summary>Field : "Name" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValDesignac")]
		public string ValDesignac { get { return klass.ValDesignac; } set { klass.ValDesignac = value; } }

		[DisplayName("Message")]
		/// <summary>Field : "Message" Tipo: "MO" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValMensagem")]
		[DataType(DataType.MultilineText)]
		public string ValMensagem { get { return klass.ValMensagem; } set { klass.ValMensagem = value; } }

		[DisplayName("Saves on DB?")]
		/// <summary>Field : "Saves on DB?" Tipo: "L" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValGravabd")]
		public bool ValGravabd { get { return Convert.ToBoolean(klass.ValGravabd); } set { klass.ValGravabd = Convert.ToInt32(value); } }

		[DisplayName("Created by")]
		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValOpercria")]
		public string ValOpercria { get { return klass.ValOpercria; } set { klass.ValOpercria = value; } }

		[DisplayName("Created on")]
		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValDatacria")]
		[DataType(DataType.Date)]
		[DateAttribute("OD")]
		public DateTime? ValDatacria { get { return klass.ValDatacria; } set { klass.ValDatacria = value ?? DateTime.Now;  } }

		[DisplayName("Changed by")]
		/// <summary>Field : "Changed by" Tipo: "EN" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValOpermuda")]
		public string ValOpermuda { get { return klass.ValOpermuda; } set { klass.ValOpermuda = value; } }

		[DisplayName("Changed on")]
		/// <summary>Field : "Changed on" Tipo: "ED" Formula:  ""</summary>
		[ShouldSerialize("S_nm.ValDatamuda")]
		[DataType(DataType.Date)]
		[DateAttribute("ED")]
		public DateTime? ValDatamuda { get { return klass.ValDatamuda; } set { klass.ValDatamuda = value ?? DateTime.MinValue;  } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("S_nm.ValZzstate")]
		/// <summary>Field: "ZZSTATE", Type: "INT", Formula: ""</summary>
		public virtual int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public S_nm(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAs_nm(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public S_nm(UserContext userContext, CSGenioAs_nm val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}

		public void FillRelatedAreas(CSGenioAs_nm csgenioa)
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
		public static S_nm Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAs_nm>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new S_nm(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<S_nm> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAs_nm>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<S_nm>((r) => new S_nm(userCtx, r));
		}

// USE /[MANUAL FOR MODEL S_NM]/
	}
}

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
	public class Psw : ModelBase
	{
		[JsonIgnore]
		public CSGenioApsw klass { get { return baseklass as CSGenioApsw; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValCodpsw")]
		public string ValCodpsw { get { return klass.ValCodpsw; } set { klass.ValCodpsw = value; } }

		[DisplayName("Name")]
		/// <summary>Field : "Name" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValNome")]
		public string ValNome { get { return klass.ValNome; } set { klass.ValNome = value; } }

		[DisplayName("Password")]
		/// <summary>Field : "Password" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("NeverEncryptedField")]
		[DataType(DataType.Password), JsonIgnore]
		public string ValPassword { get { return klass.ValPassword; } set { klass.ValPassword = value; } }
		[DataType(DataType.Password), JsonIgnore]
		public string ValPasswordDecrypted { get { return klass.ValPasswordDecrypted; } set { klass.ValPasswordDecrypted = value; } }

		[DisplayName("Certified Series Number")]
		/// <summary>Field : "Certified Series Number" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValCertsn")]
		public string ValCertsn { get { return klass.ValCertsn; } set { klass.ValCertsn = value; } }

		[DisplayName("Email")]
		/// <summary>Field : "Email" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValEmail")]
		public string ValEmail { get { return klass.ValEmail; } set { klass.ValEmail = value; } }

		[DisplayName("Password type")]
		/// <summary>Field : "Password type" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValPswtype")]
		public string ValPswtype { get { return klass.ValPswtype; } set { klass.ValPswtype = value; } }

		[DisplayName("Salt")]
		/// <summary>Field : "Salt" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValSalt")]
		public string ValSalt { get { return klass.ValSalt; } set { klass.ValSalt = value; } }

		[DisplayName("Password date")]
		/// <summary>Field : "Password date" Tipo: "D" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValDatapsw")]
		[DataType(DataType.Date)]
		[DateAttribute("D")]
		public DateTime? ValDatapsw { get { return klass.ValDatapsw; } set { klass.ValDatapsw = value ?? DateTime.MinValue; } }

		[DisplayName("User ID")]
		/// <summary>Field : "User ID" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValUserid")]
		public string ValUserid { get { return klass.ValUserid; } set { klass.ValUserid = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValPsw2favl")]
		public string ValPsw2favl { get { return klass.ValPsw2favl; } set { klass.ValPsw2favl = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValPsw2fatp")]
		public string ValPsw2fatp { get { return klass.ValPsw2fatp; } set { klass.ValPsw2fatp = value; } }

		[DisplayName("Expiration date")]
		/// <summary>Field : "Expiration date" Tipo: "D" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValDatexp")]
		[DataType(DataType.Date)]
		[DateAttribute("D")]
		public DateTime? ValDatexp { get { return klass.ValDatexp; } set { klass.ValDatexp = value ?? DateTime.MinValue; } }

		[DisplayName("Login attempts")]
		/// <summary>Field : "Login attempts" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValAttempts")]
		[NumericAttribute(0)]
		public decimal? ValAttempts { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValAttempts, 0)); } set { klass.ValAttempts = Convert.ToDecimal(value); } }

		[DisplayName("Phone number")]
		/// <summary>Field : "Phone number" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValPhone")]
		public string ValPhone { get { return klass.ValPhone; } set { klass.ValPhone = value; } }

		[DisplayName("Status")]
		/// <summary>Field : "Status" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValStatus")]
		[NumericAttribute(0)]
		public decimal? ValStatus { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValStatus, 0)); } set { klass.ValStatus = Convert.ToDecimal(value); } }

		[DisplayName("Has login?")]
		/// <summary>Field : "Has login?" Tipo: "L" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValAssocia")]
		public bool ValAssocia { get { return Convert.ToBoolean(klass.ValAssocia); } set { klass.ValAssocia = Convert.ToInt32(value); } }

		[DisplayName("Created by")]
		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValOpercria")]
		public string ValOpercria { get { return klass.ValOpercria; } set { klass.ValOpercria = value; } }

		[DisplayName("Created on")]
		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValDatacria")]
		[DataType(DataType.Date)]
		[DateAttribute("OD")]
		public DateTime? ValDatacria { get { return klass.ValDatacria; } set { klass.ValDatacria = value ?? DateTime.Now;  } }

		[DisplayName("Changed by")]
		/// <summary>Field : "Changed by" Tipo: "EN" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValOpermuda")]
		public string ValOpermuda { get { return klass.ValOpermuda; } set { klass.ValOpermuda = value; } }

		[DisplayName("Changed on")]
		/// <summary>Field : "Changed on" Tipo: "ED" Formula:  ""</summary>
		[ShouldSerialize("Psw.ValDatamuda")]
		[DataType(DataType.Date)]
		[DateAttribute("ED")]
		public DateTime? ValDatamuda { get { return klass.ValDatamuda; } set { klass.ValDatamuda = value ?? DateTime.MinValue;  } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("Psw.ValZzstate")]
		/// <summary>Field: "ZZSTATE", Type: "INT", Formula: ""</summary>
		public virtual int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public Psw(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioApsw(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public Psw(UserContext userContext, CSGenioApsw val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}

		public void FillRelatedAreas(CSGenioApsw csgenioa)
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
		public static Psw Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioApsw>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new Psw(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<Psw> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioApsw>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<Psw>((r) => new Psw(userCtx, r));
		}

// USE /[MANUAL FOR MODEL PSW]/
	}
}

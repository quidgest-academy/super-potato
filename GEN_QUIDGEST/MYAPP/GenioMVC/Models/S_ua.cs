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
	public class S_ua : ModelBase
	{
		[JsonIgnore]
		public CSGenioAs_ua klass { get { return baseklass as CSGenioAs_ua; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("S_ua.ValCodua")]
		public string ValCodua { get { return klass.ValCodua; } set { klass.ValCodua = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("S_ua.ValCodpsw")]
		public string ValCodpsw { get { return klass.ValCodpsw; } set { klass.ValCodpsw = value; } }

		private Psw _psw;
		[DisplayName("Psw")]
		[ShouldSerialize("Psw")]
		public virtual Psw Psw
		{
			get
			{
				if (!isEmptyModel && (_psw == null || (!string.IsNullOrEmpty(ValCodpsw) && (_psw.isEmptyModel || _psw.klass.QPrimaryKey != ValCodpsw))))
					_psw = Models.Psw.Find(ValCodpsw, m_userContext, Identifier, _fieldsToSerialize);
				_psw ??= new Models.Psw(m_userContext, true, _fieldsToSerialize);
				return _psw;
			}
			set { _psw = value; }
		}

		[DisplayName("System")]
		/// <summary>Field : "System" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_ua.ValSistema")]
		public string ValSistema { get { return klass.ValSistema; } set { klass.ValSistema = value; } }

		[DisplayName("Module")]
		/// <summary>Field : "Module" Tipo: "AC" Formula:  ""</summary>
		[ShouldSerialize("S_ua.ValModulo")]
		[DataArray("S_module", GenioMVC.Helpers.ArrayType.Character)]
		public string ValModulo { get { return klass.ValModulo; } set { klass.ValModulo = value; } }
		[JsonIgnore]
		public SelectList ArrayValmodulo { get { return new SelectList(CSGenio.business.ArrayS_module.GetDictionary(), "Key", "Value", ValModulo); } set { ValModulo = value.SelectedValue as string; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "C" Formula: + "KeyToString([S_UA->CODPSW]) + [S_UA->MODULO]"</summary>
		[ShouldSerialize("S_ua.ValNaodupli")]
		public string ValNaodupli { get { return klass.ValNaodupli; } set { klass.ValNaodupli = value; } }

		[DisplayName("Role")]
		/// <summary>Field : "Role" Tipo: "AC" Formula:  ""</summary>
		[ShouldSerialize("S_ua.ValRole")]
		[DataArray("S_roles", GenioMVC.Helpers.ArrayType.Character)]
		public string ValRole { get { return klass.ValRole; } set { klass.ValRole = value; } }
		[JsonIgnore]
		public SelectList ArrayValrole { get { return new SelectList(CSGenio.business.ArrayS_roles.GetDictionary(), "Key", "Value", ValRole); } set { ValRole = value.SelectedValue as string; } }

		[DisplayName("Level")]
		/// <summary>Field : "Level" Tipo: "N" Formula: + "GetLevelFromRole([S_UA->NIVEL], [S_UA->ROLE])"</summary>
		[ShouldSerialize("S_ua.ValNivel")]
		[NumericAttribute(0)]
		public decimal? ValNivel { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValNivel, 0)); } set { klass.ValNivel = Convert.ToDecimal(value); } }

		[DisplayName("Created by")]
		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		[ShouldSerialize("S_ua.ValOpercria")]
		public string ValOpercria { get { return klass.ValOpercria; } set { klass.ValOpercria = value; } }

		[DisplayName("Created on")]
		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		[ShouldSerialize("S_ua.ValDatacria")]
		[DataType(DataType.Date)]
		[DateAttribute("OD")]
		public DateTime? ValDatacria { get { return klass.ValDatacria; } set { klass.ValDatacria = value ?? DateTime.Now;  } }

		[DisplayName("Changed by")]
		/// <summary>Field : "Changed by" Tipo: "EN" Formula:  ""</summary>
		[ShouldSerialize("S_ua.ValOpermuda")]
		public string ValOpermuda { get { return klass.ValOpermuda; } set { klass.ValOpermuda = value; } }

		[DisplayName("Changed on")]
		/// <summary>Field : "Changed on" Tipo: "ED" Formula:  ""</summary>
		[ShouldSerialize("S_ua.ValDatamuda")]
		[DataType(DataType.Date)]
		[DateAttribute("ED")]
		public DateTime? ValDatamuda { get { return klass.ValDatamuda; } set { klass.ValDatamuda = value ?? DateTime.MinValue;  } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("S_ua.ValZzstate")]
		/// <summary>Field: "ZZSTATE", Type: "INT", Formula: ""</summary>
		public virtual int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public S_ua(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAs_ua(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public S_ua(UserContext userContext, CSGenioAs_ua val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}

		public void FillRelatedAreas(CSGenioAs_ua csgenioa)
		{
			if (csgenioa == null)
				return;

			foreach (RequestedField Qfield in csgenioa.Fields.Values)
			{
				switch (Qfield.Area)
				{
					case "psw":
						_psw ??= new Psw(m_userContext, true, _fieldsToSerialize);
						_psw.klass.insertNameValueField(Qfield.FullName, Qfield.Value);
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
		public static S_ua Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAs_ua>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new S_ua(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<S_ua> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAs_ua>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<S_ua>((r) => new S_ua(userCtx, r));
		}

// USE /[MANUAL FOR MODEL S_UA]/
	}
}

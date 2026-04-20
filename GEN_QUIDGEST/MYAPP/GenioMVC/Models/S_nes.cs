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
	public class S_nes : ModelBase
	{
		[JsonIgnore]
		public CSGenioAs_nes klass { get { return baseklass as CSGenioAs_nes; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("S_nes.ValCodsigna")]
		public string ValCodsigna { get { return klass.ValCodsigna; } set { klass.ValCodsigna = value; } }

		[DisplayName("Name")]
		/// <summary>Field : "Name" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_nes.ValName")]
		public string ValName { get { return klass.ValName; } set { klass.ValName = value; } }

		[DisplayName("Image")]
		/// <summary>Field : "Image" Tipo: "IJ" Formula:  ""</summary>
		[ShouldSerialize("S_nes.ValImage")]
		[ImageThumbnailJsonConverter(75, 75)]
		public ImageModel ValImage { get { return new ImageModel(klass.ValImage) { Ticket = ValImageQTicket }; } set { klass.ValImage = value; } }
		[JsonIgnore]
		public string ValImageQTicket = null;

		[DisplayName("Text after signature")]
		/// <summary>Field : "Text after signature" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_nes.ValTextass")]
		public string ValTextass { get { return klass.ValTextass; } set { klass.ValTextass = value; } }

		[DisplayName("Username")]
		/// <summary>Field : "Username" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_nes.ValUsername")]
		public string ValUsername { get { return klass.ValUsername; } set { klass.ValUsername = value; } }

		[DisplayName("Password")]
		/// <summary>Field : "Password" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_nes.ValPassword")]
		public string ValPassword { get { return klass.ValPassword; } set { klass.ValPassword = value; } }

		[DisplayName("Created by")]
		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		[ShouldSerialize("S_nes.ValOpercria")]
		public string ValOpercria { get { return klass.ValOpercria; } set { klass.ValOpercria = value; } }

		[DisplayName("Created on")]
		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		[ShouldSerialize("S_nes.ValDatacria")]
		[DataType(DataType.Date)]
		[DateAttribute("OD")]
		public DateTime? ValDatacria { get { return klass.ValDatacria; } set { klass.ValDatacria = value ?? DateTime.Now;  } }

		[DisplayName("Changed by")]
		/// <summary>Field : "Changed by" Tipo: "EN" Formula:  ""</summary>
		[ShouldSerialize("S_nes.ValOpermuda")]
		public string ValOpermuda { get { return klass.ValOpermuda; } set { klass.ValOpermuda = value; } }

		[DisplayName("Changed on")]
		/// <summary>Field : "Changed on" Tipo: "ED" Formula:  ""</summary>
		[ShouldSerialize("S_nes.ValDatamuda")]
		[DataType(DataType.Date)]
		[DateAttribute("ED")]
		public DateTime? ValDatamuda { get { return klass.ValDatamuda; } set { klass.ValDatamuda = value ?? DateTime.MinValue;  } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("S_nes.ValZzstate")]
		/// <summary>Field: "ZZSTATE", Type: "INT", Formula: ""</summary>
		public virtual int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public S_nes(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAs_nes(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public S_nes(UserContext userContext, CSGenioAs_nes val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}

		public void FillRelatedAreas(CSGenioAs_nes csgenioa)
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
		public static S_nes Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAs_nes>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new S_nes(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<S_nes> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAs_nes>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<S_nes>((r) => new S_nes(userCtx, r));
		}

// USE /[MANUAL FOR MODEL S_NES]/
	}
}

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
	public class S_apr : ModelBase
	{
		[JsonIgnore]
		public CSGenioAs_apr klass { get { return baseklass as CSGenioAs_apr; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValCodascpr")]
		public string ValCodascpr { get { return klass.ValCodascpr; } set { klass.ValCodascpr = value; } }

		[DisplayName("Process type")]
		/// <summary>Field : "Process type" Tipo: "AC" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValType")]
		[DataArray("S_tpproc", GenioMVC.Helpers.ArrayType.Character)]
		public string ValType { get { return klass.ValType; } set { klass.ValType = value; } }
		[JsonIgnore]
		public SelectList ArrayValtype { get { return new SelectList(CSGenio.business.ArrayS_tpproc.GetDictionary(), "Key", "Value", ValType); } set { ValType = value.SelectedValue as string; } }

		[DisplayName("Request date")]
		/// <summary>Field : "Request date" Tipo: "D" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValDaterequ")]
		[DataType(DataType.Date)]
		[DateAttribute("D")]
		public DateTime? ValDaterequ { get { return klass.ValDaterequ; } set { klass.ValDaterequ = value ?? DateTime.MinValue; } }

		[DisplayName("Start time")]
		/// <summary>Field : "Start time" Tipo: "DT" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValInitprc")]
		[DataType(DataType.Date)]
		[DateAttribute("DT")]
		public DateTime? ValInitprc { get { return klass.ValInitprc; } set { klass.ValInitprc = value ?? DateTime.MinValue; } }

		[DisplayName("End time")]
		/// <summary>Field : "End time" Tipo: "DT" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValEndprc")]
		[DataType(DataType.Date)]
		[DateAttribute("DT")]
		public DateTime? ValEndprc { get { return klass.ValEndprc; } set { klass.ValEndprc = value ?? DateTime.MinValue; } }

		[DisplayName("Duration")]
		/// <summary>Field : "Duration" Tipo: "T" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValDuration")]
		[DateAttribute("T")]
		public string ValDuration { get { return klass.ValDuration; } set { klass.ValDuration = value; } }

		[DisplayName("Status")]
		/// <summary>Field : "Status" Tipo: "AC" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValStatus")]
		[DataArray("S_prstat", GenioMVC.Helpers.ArrayType.Character)]
		public string ValStatus { get { return klass.ValStatus; } set { klass.ValStatus = value; } }
		[JsonIgnore]
		public SelectList ArrayValstatus { get { return new SelectList(CSGenio.business.ArrayS_prstat.GetDictionary(), "Key", "Value", ValStatus); } set { ValStatus = value.SelectedValue as string; } }

		[DisplayName("Result message")]
		/// <summary>Field : "Result message" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValRsltmsg")]
		public string ValRsltmsg { get { return klass.ValRsltmsg; } set { klass.ValRsltmsg = value; } }

		[DisplayName("Finished")]
		/// <summary>Field : "Finished" Tipo: "L" Formula: + "iif([S_APR->STATUS]=="T" || [S_APR->STATUS]=="AB" || [S_APR->STATUS]=="C" ,1, 0)"</summary>
		[ShouldSerialize("S_apr.ValFinished")]
		public bool ValFinished { get { return Convert.ToBoolean(klass.ValFinished); } set { klass.ValFinished = Convert.ToInt32(value); } }

		[DisplayName("Last update")]
		/// <summary>Field : "Last update" Tipo: "DS" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValLastupdt")]
		[DataType(DataType.Date)]
		[DateAttribute("DS")]
		public DateTime? ValLastupdt { get { return klass.ValLastupdt; } set { klass.ValLastupdt = value ?? DateTime.MinValue; } }

		[DisplayName("Result")]
		/// <summary>Field : "Result" Tipo: "AC" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValResult")]
		[DataArray("S_resul", GenioMVC.Helpers.ArrayType.Character)]
		public string ValResult { get { return klass.ValResult; } set { klass.ValResult = value; } }
		[JsonIgnore]
		public SelectList ArrayValresult { get { return new SelectList(CSGenio.business.ArrayS_resul.GetDictionary(), "Key", "Value", ValResult); } set { ValResult = value.SelectedValue as string; } }

		[DisplayName("Process info")]
		/// <summary>Field : "Process info" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValInfo")]
		public string ValInfo { get { return klass.ValInfo; } set { klass.ValInfo = value; } }

		[DisplayName("Percentage")]
		/// <summary>Field : "Percentage" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValPercenta")]
		[NumericAttribute(0)]
		public decimal? ValPercenta { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValPercenta, 0)); } set { klass.ValPercenta = Convert.ToDecimal(value); } }

		[DisplayName("Process mode")]
		/// <summary>Field : "Process mode" Tipo: "AC" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValModoproc")]
		[DataArray("S_modpro", GenioMVC.Helpers.ArrayType.Character)]
		public string ValModoproc { get { return klass.ValModoproc; } set { klass.ValModoproc = value; } }
		[JsonIgnore]
		public SelectList ArrayValmodoproc { get { return new SelectList(CSGenio.business.ArrayS_modpro.GetDictionary(), "Key", "Value", ValModoproc); } set { ValModoproc = value.SelectedValue as string; } }

		[DisplayName("Executed by external app")]
		/// <summary>Field : "Executed by external app" Tipo: "L" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValExternal")]
		public bool ValExternal { get { return Convert.ToBoolean(klass.ValExternal); } set { klass.ValExternal = Convert.ToInt32(value); } }

		[DisplayName("Process ID")]
		/// <summary>Field : "Process ID" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValId")]
		[NumericAttribute(0)]
		public decimal? ValId { get { return Convert.ToDecimal(GenFunctions.RoundQG(klass.ValId, 0)); } set { klass.ValId = Convert.ToDecimal(value); } }

		[DisplayName("Entid key")]
		/// <summary>Field : "Entid key" Tipo: "CF" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValCodentit")]
		public string ValCodentit { get { return klass.ValCodentit; } set { klass.ValCodentit = value; } }

		[DisplayName("Motive")]
		/// <summary>Field : "Motive" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValMotivo")]
		public string ValMotivo { get { return klass.ValMotivo; } set { klass.ValMotivo = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CF" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValCodpsw")]
		public string ValCodpsw { get { return klass.ValCodpsw; } set { klass.ValCodpsw = value; } }

		[DisplayName("Canceled by")]
		/// <summary>Field : "Canceled by" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValOpershut")]
		public string ValOpershut { get { return klass.ValOpershut; } set { klass.ValOpershut = value; } }

		[DisplayName("Real time status")]
		/// <summary>Field : "Real time status" Tipo: "AC" Formula: + "iif([S_APR->STATUS] == "EE" || [S_APR->STATUS] == "D" || [S_APR->STATUS] == "AC" || [S_APR->STATUS] == "AG", iif((Diferenca_entre_Datas([S_APR->LASTUPDT], [Now], "S") > 10 && [S_APR->STATUS] != "AG") || (Diferenca_entre_Datas([S_APR->LASTUPDT], [Now], "S") > 45 && [S_APR->STATUS] == "AG"), "NR", [S_APR->STATUS]), [S_APR->STATUS])"</summary>
		[ShouldSerialize("S_apr.ValRtstatus")]
		[DataArray("S_prstat", GenioMVC.Helpers.ArrayType.Character)]
		public string ValRtstatus { get { return klass.ValRtstatus; } set { klass.ValRtstatus = value; } }
		[JsonIgnore]
		public SelectList ArrayValrtstatus { get { return new SelectList(CSGenio.business.ArrayS_prstat.GetDictionary(), "Key", "Value", ValRtstatus); } set { ValRtstatus = value.SelectedValue as string; } }

		[DisplayName("Created by")]
		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValOpercria")]
		public string ValOpercria { get { return klass.ValOpercria; } set { klass.ValOpercria = value; } }

		[DisplayName("Created on")]
		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValDatacria")]
		[DataType(DataType.Date)]
		[DateAttribute("OD")]
		public DateTime? ValDatacria { get { return klass.ValDatacria; } set { klass.ValDatacria = value ?? DateTime.Now;  } }

		[DisplayName("Changed by")]
		/// <summary>Field : "Changed by" Tipo: "EN" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValOpermuda")]
		public string ValOpermuda { get { return klass.ValOpermuda; } set { klass.ValOpermuda = value; } }

		[DisplayName("Changed on")]
		/// <summary>Field : "Changed on" Tipo: "ED" Formula:  ""</summary>
		[ShouldSerialize("S_apr.ValDatamuda")]
		[DataType(DataType.Date)]
		[DateAttribute("ED")]
		public DateTime? ValDatamuda { get { return klass.ValDatamuda; } set { klass.ValDatamuda = value ?? DateTime.MinValue;  } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("S_apr.ValZzstate")]
		/// <summary>Field: "ZZSTATE", Type: "INT", Formula: ""</summary>
		public virtual int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public S_apr(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAs_apr(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public S_apr(UserContext userContext, CSGenioAs_apr val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}

		public void FillRelatedAreas(CSGenioAs_apr csgenioa)
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
		public static S_apr Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAs_apr>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new S_apr(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<S_apr> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAs_apr>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<S_apr>((r) => new S_apr(userCtx, r));
		}

// USE /[MANUAL FOR MODEL S_APR]/
	}
}

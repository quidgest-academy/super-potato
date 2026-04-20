using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml.Serialization;

using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Models.Cav;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.Helpers.Cav
{
	/// <summary>
	/// Singleton class responsible to provide all CSV (XML) metadata information
	/// </summary>
	public sealed class XmlCavService
	{
		private static XmlCavService instance = null;
		private static readonly object padlock = new object();

		private readonly List<FieldType> ExcludedType = new List<FieldType>
		{
			FieldType.KEY_VARCHAR,
			FieldType.KEY_INT,
			FieldType.KEY_GUID,
			FieldType.MEMO,
			FieldType.MEMO_COMP_RTF,
			FieldType.BINARY,
			FieldType.BINARY,
			FieldType.DATETIMESECONDS,
			FieldType.IMAGE,
			FieldType.DOCUMENT,
			FieldType.BINARY,
			FieldType.GEOGRAPHY_POINT,
			FieldType.GEOGRAPHY_SHAPE,
			FieldType.GEOMETRY_SHAPE,
			FieldType.ENCRYPTED
		};

		private readonly Dictionary<FieldType, string> fieldTypeMap = GetFieldTypeMap();
		private static Dictionary<FieldType, string> GetFieldTypeMap()
		{
			var fieldMapList = new Dictionary<FieldType, string>();
			fieldMapList.Add(FieldType.TEXT, "A");
			fieldMapList.Add(FieldType.KEY_GUID, "A");
			fieldMapList.Add(FieldType.KEY_INT, "A");
			fieldMapList.Add(FieldType.KEY_VARCHAR, "A");
			fieldMapList.Add(FieldType.DATE, "D");
			fieldMapList.Add(FieldType.DATETIME, "H");
			fieldMapList.Add(FieldType.DATETIMESECONDS, "H");
			fieldMapList.Add(FieldType.TIME_HOURS, "A");
			fieldMapList.Add(FieldType.NUMERIC, "N");
			fieldMapList.Add(FieldType.CURRENCY, "$");
			fieldMapList.Add(FieldType.LOGIC, "B");
			fieldMapList.Add(FieldType.MEMO, "A");
			fieldMapList.Add(FieldType.BINARY, "A");
			fieldMapList.Add(FieldType.MEMO_COMP_RTF, "A");
			fieldMapList.Add(FieldType.IMAGE, "A");
			fieldMapList.Add(FieldType.DOCUMENT, "A");
			fieldMapList.Add(FieldType.ARRAY_TEXT, "A");
			fieldMapList.Add(FieldType.ARRAY_NUMERIC, "A");
			fieldMapList.Add(FieldType.ARRAY_LOGIC, "A");
			fieldMapList.Add(FieldType.GEOGRAPHY_POINT, "A");
			fieldMapList.Add(FieldType.GEOGRAPHY_SHAPE, "A");
			fieldMapList.Add(FieldType.GEOMETRY_SHAPE, "A");
			return fieldMapList;
		}

		XmlCavService() { }

		/// <summary>
		/// GET the instance of XmlCavService object (Thread safe)
		/// </summary>
		public static XmlCavService Instance
		{
			get
			{
				lock (padlock)
				{
					if (instance == null)
					{
						instance = new XmlCavService();
					}
					return instance;
				}
			}
		}

		/// <summary>
		/// Get the table by area with all info (fileds incleded).
		/// </summary>
		/// <returns></returns>
		public CAVTable GetTable(string area, string lang)
		{
			try
			{
				AreaInfo areaInfo = Area.GetInfoArea(area.ToLower());
				if (!areaInfo.VisivelCav.Equals(CavVisibilityType.Nunca))
				{
					CAVTable table = new CAVTable { Id = areaInfo.Alias.ToUpper(), Description = areaInfo.DescriptionCav };
					FillFieldFromTable(table, lang);
					return table;
				}
				else
					return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Fill the table fields informations
		/// </summary>
		/// <param name="table"></param>
		public void FillFieldFromTable(CAVTable table, string lang)
		{
			List<Models.Cav.Field> fields = new List<Models.Cav.Field>();
			AreaInfo area = Area.GetInfoArea(table.Id.ToLower());

			foreach (var field in area.DBFieldsList)
			{
				//Retirar o zzstate da lista de campos
				if (ExcludedType.Contains(field.FieldType) || field.VisivelCav.Equals(CavVisibilityType.Nunca) || field.Name.Equals("zzstate"))
					continue;

				Models.Cav.Field newField = new Models.Cav.Field();
				newField.Id = table.Id.ToUpper() + "." + field.Name.ToUpper();
				newField.TableId = table.Id;
				string type = "";

				if (fieldTypeMap.TryGetValue(field.FieldType, out type))
					newField.Type = type;

				if (string.IsNullOrEmpty(field.CavDesignation))
					newField.Description = CSGenio.framework.GenFunctions.CapitalizeInitials(field.Name);
				else
					newField.Description = field.CavDesignation;

				//the field is a enumeration type (Array)
				if (!string.IsNullOrEmpty(field.ArrayName))
					newField.ArrayElements = GetListArrayElement(field.ArrayClassName, lang);

				fields.Add(newField);
			}

			fields.Sort(delegate (Models.Cav.Field f1, Models.Cav.Field f2) { return String.Compare(f1.Description, f2.Description, StringComparison.InvariantCultureIgnoreCase); });
			table.Fields = fields;
		}

		public List<FieldArray> GetListArrayElement(string arrayId, string lang)
		{
			//string lang = Models.Navigation.UserContext.Current.User.Language;
			List<FieldArray> res = new List<FieldArray>();
			try
			{
				var array = new ArrayInfo(arrayId);
				foreach (var elem in array.Elements)
					res.Add(new FieldArray { ArrayId = arrayId, Description = array.GetDescription(elem, lang), Id = elem });
			}
			catch (Exception)
			{
				return null;
			}

			return res;
		}

		public List<string> GetTableInfo(string tableID, List<string> columns)
		{
			List<string> res = new List<string>();
			var area = Area.GetInfoArea(tableID.ToLower());

			foreach (var column in columns)
				res.Add(TableMetaAcessor(column, area));

			return res;
		}

		private string TableMetaAcessor(string column, AreaInfo obj)
		{
			switch (column)
			{
				case "Table.Id":
					return obj.Alias.ToUpper();
				case "Table.Description":
					return obj.AreaDesignation;
				case "Table.DBname":
					return obj.TableName.ToUpper();
				case "Table.PrimaryKeyDBName":
					return obj.PrimaryKeyName.ToUpper();
				default:
					throw new Exception("Metadata field " + column + " is unknown in TableList");
			}
		}

		public List<string> GetFieldInfo(string tableId, string id, List<string> columns)
		{
			var area = Area.GetInfoArea(tableId.ToLower());
			if (!area.DBFields.ContainsKey(id.ToLower()))
				return null;

			var obj = area.DBFields[id.ToLower()];
			if (obj == null)
				return null;

			List<string> res = new List<string>();
			foreach (var column in columns)
				res.Add(FieldMetaAcessor(column, obj));
			return res;
		}

		private string FieldMetaAcessor(string column, CSGenio.framework.Field obj)
		{
			switch (column)
			{
				case "Field.Id":
					return obj.Alias.ToUpper() + "." + obj.Name.ToUpper();
				case "Field.TableId":
					return obj.Alias.ToUpper();
				case "Field.Description":
					return obj.CavDesignation;
				case "Field.Type":
					{
						string tipo = "";
						fieldTypeMap.TryGetValue(obj.FieldType, out tipo);
						return tipo;
					}
				case "Field.InternalType":
					return obj.FieldFormat.ToString();
				case "Field.Width":
					return obj.FieldSize.ToString();
				case "Field.Decimal":
					return obj.Decimals.ToString();
				case "Field.BDname":
					return obj.Name.ToUpper();
				case "Field.Array":
					return obj.ArrayName;
				case "Field.ArrayClassName":
					return obj.ArrayClassName;
				default:
					throw new Exception("Metadata field " + obj + " is unknown in FieldList");
			}
		}

		public List<string> GetRelationListBySource(string srcTable)
		{
			List<string> res = new List<string>();
			if (string.IsNullOrEmpty(srcTable))
				return null;

			var area = Area.GetInfoArea(srcTable.ToLower());

			foreach (var rel in area.ParentTables)
			{
				//var areaRel = m_metadata.Tables.Find(x => x.Nome == rel.TabelaDestino);
				var areaRel = Area.GetInfoArea(rel.Value.AliasTargetTab);

				if (areaRel.VisivelCav == CSGenio.business.CavVisibilityType.Propria)
					continue;

				if (areaRel.VisivelCav == CSGenio.business.CavVisibilityType.Nunca)
					continue;

				res.Add(rel.Key.ToUpper());
			}
			return res;
		}

		public List<string> GetRelationListByDestiny(string dstTable)
		{
			List<string> res = new List<string>();

			if (string.IsNullOrEmpty(dstTable))
				return null;

			var area = Area.GetInfoArea(dstTable.ToLower());

			foreach (var rel in area.ChildTable ?? Enumerable.Empty<ChildRelation>())
			{
				var areaRel = Area.GetInfoArea(rel.ChildArea.ToLower());

				if (areaRel.VisivelCav == CSGenio.business.CavVisibilityType.Propria || areaRel.VisivelCav == CSGenio.business.CavVisibilityType.Nunca)
					continue;

				res.Add(rel.ChildArea.ToUpper());
			}

			return res;
		}

		public List<string> GetRelationInfo(string srcid, string dstid, List<string> columns)
		{
			List<string> res = new List<string>();
			var area = Area.GetInfoArea(srcid.ToLower());
			if (area == null)
				return null;

			Relation rel = null;
			if (area.ParentTables.ContainsKey(dstid.ToLower()))
				rel = area.ParentTables[dstid.ToLower()];

			if (rel == null)
				return null;

			var areaDst = Area.GetInfoArea(dstid.ToLower());

			foreach (var column in columns)
				res.Add(RelMetaAcessor(column, srcid, rel, area, areaDst));

			return res;
		}

		private string RelMetaAcessor(string column, string src, Relation obj, AreaInfo tabSrc, AreaInfo tabDst)
		{
			switch (column)
			{
				case "Relation.SrcTable":
					return src;
				case "Relation.SrcFieldId":
					return obj.SourceRelField;
				case "Relation.DstTable":
					return obj.TargetTable;
				case "Relation.DstFieldId":
					return obj.TargetRelField;
				case "TableSrc.Id":
					return tabSrc.Alias;
				case "TableSrc.Description":
					return tabSrc.AreaDesignation;
				case "TableSrc.DBname":
					return tabSrc.TableName;
				case "TableDst.Id":
					return tabDst.Alias;
				case "TableDst.Description":
					return tabDst.AreaDesignation;
				case "TableDst.DBname":
					return tabDst.TableName;
				default:
					throw new Exception("Metadata field " + column + " is unknown in RelationList");
			}
		}

		public List<List<string>> GetRelationListMetadata(List<string> conditions, List<string> columns)
		{
			List<List<string>> res = new List<List<string>>();

			if (conditions != null && conditions.Count > 0)
			{
				var cond = conditions[0].Split('=');

				//select * from Relations where SrcTable = @x
				if (cond[0] == "Relation.SrcTable")
				{
					foreach (var rel in GetRelationListBySource(cond[1]))
					{
						var relation = GetRelationInfo(cond[1], rel, columns);
						if (relation != null)
							res.Add(relation);
					}
				}
				else if (cond[0] == "Relation.DstTable")
				{
					foreach (var rel in GetRelationListByDestiny(cond[1]))
					{
						var relation = GetRelationInfo(rel, cond[1], columns);
						if (relation != null)
							res.Add(relation);
					}
				}
				else
				{
					foreach (var rel in GetRelationListBySource(cond[1]))
					{
						var relation = GetRelationInfo(cond[1], rel, columns);
						if (relation != null)
							res.Add(relation);
					}

					foreach (var rel in GetRelationListByDestiny(cond[1]))
					{
						var relation = GetRelationInfo(cond[1], rel, columns);
						if (relation != null)
							res.Add(relation);
					}
				}
			}

			return res;
		}

		public List<List<string>> GetRelationList(string table, bool uptables)
		{
			var conditions = uptables ? new List<string>() { "Relation.SrcTable=" + table } : new List<string>() { "Relation.DstTable=" + table };
			var columns = new List<string>() { "TableSrc.Id", "TableSrc.Description", "TableDst.Id", "TableDst.Description" };

			return GetRelationListMetadata(conditions, columns);
		}

		// For now, we will not allow the use of the fields in the tables below.
		// It will first be implemented in Vue, which is why there is 'allowTableBelow', to allow disabling in MVC.
		public bool ExistRelationship(string baseTable, string relatedTable, bool allowTableBelow = false)
		{
			List<List<string>> upTables = GetRelationList(baseTable, true);
			foreach (var item in upTables)
			{
				if (item.Contains(relatedTable, StringComparer.OrdinalIgnoreCase))
					return true;
			}

			if (allowTableBelow)
			{
				List<List<string>> downTables = GetRelationList(baseTable, false);
				foreach (var item in downTables)
				{
					if (item.Contains(relatedTable, StringComparer.OrdinalIgnoreCase))
						return true;
				}
			}

			return false;
		}

		public object ConstructRelationList(string table)
		{
			if (table == null)
				return new { result = "E", message = "Não foi definida uma tabela base" };

			// Obtém uptables
			List<List<string>> relations_uptables = GetRelationList(table, true);

			// Obtém downtables
			List<List<string>> relations_downtables = GetRelationList(table, false);

			relations_uptables.Sort((x, y) => String.Compare(x[3], y[3], StringComparison.InvariantCultureIgnoreCase));

			List<object> uptables = new List<object>();
			for (int i = 0; i < relations_uptables.Count; i++)
			{
				var son = new
				{
					id = relations_uptables[i][2],
					name = relations_uptables[i][3],
				};
				uptables.Add(son);
			}

			relations_downtables.Sort((x, y) => String.Compare(x[1], y[1], StringComparison.InvariantCultureIgnoreCase));

			List<object> downtables = new List<object>();
			for (int i = 0; i < relations_downtables.Count; i++)
			{
				var parent = new
				{
					id = relations_downtables[i][0],
					name = relations_downtables[i][1],
				};
				downtables.Add(parent);
			}

			// Obter nome humano da tabela base
			string table_name = table;
			if ((relations_uptables != null && relations_uptables.Count > 0) || (relations_downtables != null && relations_downtables.Count > 0))
				table_name = relations_uptables.Count > 0 ? relations_uptables[0][1] : relations_downtables[0][3];

			var result = new
			{
				result = "OK",
				id = table,
				name = table_name,
				uptables = uptables,
				downtables = downtables
			};

			return result;
		}

		public List<CAVTable> GetTableUpList(string origin, string lang)
		{
			List<CAVTable> res = new List<CAVTable>();
			AreaInfo area = Area.GetInfoArea(origin.ToLower());

			//verificar se a tabela origin pertence a lista das tabelas disponiveis para CAV
			//Acrescentar na geração essa informação na AreaInfo
			res.Add(new CAVTable { Id = area.Alias.ToUpper(), Description = area.DescriptionCav });

			if (area.Pathways != null)
				foreach (var dst in area.Pathways.Keys)
				{
					AreaInfo areaRel = Area.GetInfoArea(dst);

					if (!(areaRel.VisivelCav == CavVisibilityType.Relacionada
					   || areaRel.VisivelCav == CavVisibilityType.Sempre))
						continue;

					//verificar se a tabela origin pertence a lista das tabelas disponiveis para CAV
					//Acrescentar na geração essa informação na AreaInfo
					res.Add(new CAVTable { Id = areaRel.Alias.ToUpper(), Description = areaRel.DescriptionCav });
				}

			foreach (var item in res)
				FillFieldFromTable(item, lang);

			return res;
		}

		public List<string> GetTableUpInfo(string id, string origin, List<string> columns)
		{
			AreaInfo obj = Area.GetInfoArea(id);
			List<string> res = new List<string>();
			foreach (var column in columns)
				res.Add(TableUpMetaAcessor(column, obj, origin));
			return res;
		}

		private string TableUpMetaAcessor(string meta, AreaInfo obj, string origin)
		{
			switch (meta)
			{
				case "TableUp.Id":
					return origin;
				case "TableUp.TableId":
					return obj.Alias;
				case "TableUp.Description":
					return obj.AreaDesignation;
				case "TableUp.DBname":
					return obj.TableName;
				default:
					throw new Exception("Metadata field " + meta + " is unknown in TableList");
			}
		}

		// Get the list of tables below the base table.
		public List<CAVTable> GetTableBelowList(string baseTable, string lang)
		{
			List<CAVTable> res = new List<CAVTable>();
			var childTables = GetRelationListByDestiny(baseTable);
			foreach (var childTable in childTables)
			{
				AreaInfo areaRel = Area.GetInfoArea(childTable.ToLower());
				res.Add(new CAVTable { Id = areaRel.Alias.ToUpper(), Description = areaRel.DescriptionCav, Down = true });
			}

			foreach (var item in res)
				FillFieldFromTable(item, lang);

			return res;
		}

		// Get the list of tables related to the base table.
		public List<CAVTable> GetTablesList(string baseTable, string lang, bool includeTablesBelow = false)
		{
			List<CAVTable> result = GetTableUpList(baseTable, lang);

			if (includeTablesBelow)
			{
				// TODO: It needs to be filtered to only be the configured relationships.
				List<CAVTable> belowTables = GetTableBelowList(baseTable, lang);
				result.AddRange(belowTables);
			}

			return result;
		}

		/// <summary>
		/// Faz parse a cada uma das condições da mensagem para um mapa
		/// </summary>
		/// <param name="list">A lista de condições a fazer parse</param>
		/// <returns>O mapa de condições e valores</returns>
		private Dictionary<string, string> ParseConditions(List<string> list)
		{
			Dictionary<string, string> res = new Dictionary<string, string>();
			if (list == null)
				return res;
			foreach (var c in list)
			{
				var split = c.Split('=');
				if (split.Length < 2)
					throw new Exception("Invalid condition sintax");
				res.Add(split[0], split[1]);
			}
			return res;
		}

		List<List<string>> GetTableUpListMetadata(List<string> condition, List<string> columns, string lang)
		{
			List<List<string>> res = new List<List<string>>();
			Dictionary<string, string> conditions = ParseConditions(condition);
			if (!conditions.ContainsKey("TableUp.Id"))
				throw new NotImplementedException();

			string id = conditions["TableUp.Id"];
			res.Add(GetTableUpInfo(id, id, columns));

			foreach (var rel in GetTableUpList(id, lang))
				res.Add(GetTableUpInfo(rel.Id, id, columns));

			return res;
		}

		public List<Relation> GetCaminho(string src, string dst)
		{
			if (src == null)
				throw new ArgumentNullException("src");
			if (dst == null)
				throw new ArgumentNullException("dst");

			var info = Area.GetInfoArea(src.ToLower());

			if (!info.Pathways.ContainsKey(dst.ToLower()))
				return null;

			List<Relation> res = new List<Relation>();
			res = info.GetRelations(dst.ToLower());

			IEnumerable<Relation> resConverted = res.Select(p => new Relation(null, p.SourceTable.ToUpper(), p.AliasSourceTab.ToUpper(), p.SourceIntKey.ToUpper(), p.SourceRelField.ToUpper(), null, p.TargetTable.ToUpper(), p.AliasTargetTab.ToUpper(), p.TargetIntKey.ToUpper(), p.TargetRelField.ToUpper()));
			return resConverted.ToList();
		}

		/// <summary>
		/// Save the query to database
		/// </summary>
		/// <param name="query">query info</param>
		/// <param name="usr">user</param>
		/// <param name="id">Query primary key</param>
		/// <returns></returns>
		public bool SaveQuery(ReportDefinition query, User usr, string id)
		{
			if (query == null || usr == null)
				return false;

			PersistentSupport sp = PersistentSupport.getPersistentSupport(usr.Year);

			try
			{
				sp.openConnection();

				CSGenioAcavreport report = null;

				if (id != null)
					report = CSGenioAcavreport.search(sp, id, usr);
				else
					report = new CSGenioAcavreport(usr);


				report.ValTitle = query.Title;
				report.ValAcesso = query.Acesso;

				XmlSerializer serializer = new XmlSerializer(typeof(ReportDefinition));
				using (StringWriter writer = new StringWriter())
				{
					serializer.Serialize(writer, query);
					report.ValDataxml = writer.ToString();
				}

				// Update database record
				// Should not be done in maintenance mode
				if (!Maintenance.Current.IsActive)
				{
					if (id == null)
						report.insert(sp);
					else
						report.update(sp);
				}

				sp.closeConnection();

				return true;
			}
			catch (Exception)
			{
				sp.closeConnection();
				return false;
			}
		}

		/// <summary>
		/// Save the query to database
		/// </summary>
		/// <param name="usr">user</param>
		/// <returns></returns>
		public List<ReportModel> LoadQueryList(User usr)
		{
			List<ReportModel> result = new List<ReportModel>();

			if (usr == null)
				return result;

			PersistentSupport sp = PersistentSupport.getPersistentSupport(usr.Year);

			try
			{
				sp.openConnection();
				//return all public query and non public query that the current user owned
				CriteriaSet queryFilter = CriteriaSet.Or();
				queryFilter.Equal(CSGenioAcavreport.FldAcesso, "PUB");
				queryFilter.SubSet(CriteriaSet.And().NotEqual(CSGenioAcavreport.FldAcesso, "PUB").Equal(CSGenioAcavreport.FldOpercria, usr.Name));

				List<CSGenioAcavreport> reports = CSGenioAcavreport.searchList(sp, usr, queryFilter);
				foreach (var item in reports)
					result.Add(new ReportModel { ID = item.ValCodreport, Acess = item.ValAcesso, Title = item.ValTitle, Opercria = item.ValOpercria });

				sp.closeConnection();
				return result;
			}
			catch (Exception)
			{
				sp.closeConnection();
				return result;
			}
		}

		/// <summary>
		/// Load a query from database
		/// </summary>
		/// <param name="queryID">Query primary key</param>
		/// <param name="user">Usr</param>
		/// <returns>Query report definition</returns>
		public ReportDefinition LoadQuery(string queryID, User usr)
		{
			if (usr == null || String.IsNullOrEmpty(queryID))
				return null;

			PersistentSupport sp = PersistentSupport.getPersistentSupport(usr.Year);

			try
			{
				sp.openConnection();
				CSGenioAcavreport report = CSGenioAcavreport.search(sp, queryID, usr);
				sp.closeConnection();

				if (report != null)
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ReportDefinition));

					ReportDefinition reportDef = null;
					using (TextReader reader = new StringReader(report.ValDataxml))
					{
						reportDef = (ReportDefinition)serializer.Deserialize(reader);
					}
					return reportDef;
				}
			}
			catch (Exception)
			{
				sp.closeConnection();
			}
			return null;
		}
	}

	[XmlRoot("METADATA")]
	public class XmlCavMetadata
	{
		[XmlElement("SYSTEM")]
		public string System { get; set; }

		[XmlElement("MODULE")]
		public string Module { get; set; }

		[XmlArray("TABLES")]
		[XmlArrayItem("AREA")]
		public List<XmlCavMetaTable> Tables { get; set; }

		[XmlArray("ARRAYS")]
		[XmlArrayItem("ARRAY")]
		public List<XmlCavMetaArray> Arrays { get; set; }

		// limites que podem ser gerados a partir das eph's
		// ou adicionados ad-hoc via rotina manual
		[XmlArray("LIMITS")]
		[XmlArrayItem("LIMIT")]
		public List<XmlCavLimit> Limits { get; set; }
	}

	public class XmlCavLimit
	{
		[XmlAttribute("nome")]
		public string Nome { get; set; }

		[XmlAttribute("queryvalues")]
		public string QueryValues { get; set; }

		[XmlArray("LEVELS")]
		[XmlArrayItem("LEVEL")]
		public List<string> Levels { get; set; }

		[XmlArray("APPLY")]
		[XmlArrayItem("TO")]
		public List<XmlCavLimitFilter> Filters { get; set; }
	}

	public class XmlCavLimitFilter
	{
		[XmlAttribute("area")]
		public string Area { get; set; }

		[XmlAttribute("campo")]
		public string Campo { get; set; }

		[XmlAttribute("operator")]
		public string Operator { get; set; }

		[XmlAttribute("criteria")]
		public string Criteria { get; set; }

		// a propriedade Repeat é a que distingue se a criteria vai ser repetida ou não para os valores
		[XmlAttribute("repeat")]
		public bool Repeat { get; set; }

		[XmlArray("WHERE")]
		[XmlArrayItem("AREA")]
		public List<string> LimitedAreas { get; set; }
	}

	public class XmlCavMetaTable
	{
		[XmlArray("FIELDS")]
		[XmlArrayItem("FIELD")]
		public List<XmlCavMetaField> Fields { get; set; }

		[XmlArray("RELATIONS")]
		[XmlArrayItem("REL")]
		public List<XmlCavMetaRelation> Relations { get; set; }

		[XmlArray("PATHS")]
		[XmlArrayItem("PATH")]
		public List<XmlCavMetaPath> Paths { get; set; }

		[XmlAttribute("nome")]
		public string Nome { get; set; }

		[XmlAttribute("nomeCompleto")]
		public string NomeCompleto { get; set; }

		[XmlAttribute("descricao")]
		public string Descricao { get; set; }

		[XmlAttribute("visible")]
		public CSGenio.business.CavVisibilityType Visible { get; set; }

		[XmlAttribute("access")]
		public string Level { get; set; }
	}

	public class XmlCavMetaField
	{
		[XmlIgnore]
		public string TableId { get; set; }

		[XmlAttribute("nome")]
		public string Nome { get; set; }

		[XmlAttribute("nomeCompleto")]
		public string NomeCompleto { get; set; }

		[XmlAttribute("descricao")]
		public string Descricao { get; set; }

		[XmlAttribute("tipo")]
		public string TipoInterno { get; set; }

		[XmlAttribute("formato")]
		public string Formato { get; set; }

		[XmlAttribute("width")]
		public string Width { get; set; }

		[XmlAttribute("decimal")]
		public string Decimal { get; set; }

		[XmlAttribute("array")]
		public string Array { get; set; }

		[XmlAttribute("visible")]
		public CSGenio.business.CavVisibilityType Visible { get; set; }

		[XmlAttribute("access")]
		public string Level { get; set; }
	}

	public class XmlCavMetaRelation
	{
		[XmlAttribute("campoOrigem")]
		public string CampoOrigem { get; set; }

		[XmlAttribute("tabelaDestino")]
		public string TabelaDestino { get; set; }

		[XmlAttribute("campoDestino")]
		public string CampoDestino { get; set; }
	}

	public class XmlCavMetaPath
	{
		[XmlAttribute("dst")]
		public string Destiny { get; set; }

		[XmlAttribute("step")]
		public string Step { get; set; }
	}

	public class XmlCavMetaArray
	{
		[XmlArray("ELEMS")]
		[XmlArrayItem("ELEM")]
		public List<XmlCavMetaArrayElement> Elements { get; set; }

		[XmlAttribute("nome")]
		public string Nome { get; set; }

		[XmlAttribute("descricao")]
		public string Descricao { get; set; }
	}

	public class XmlCavMetaArrayElement
	{
		[XmlAttribute("codigo")]
		public string Codigo { get; set; }

		[XmlAttribute("descricao")]
		public string Descricao { get; set; }
	}

	public enum LineType
	{
		YearSeparator,
		Header,
		DetailLine,
		GroupHeader,
		TotalHeader,
		TotalLine
	}


	public static class ResultsHelpers
	{
		public static string[][] totaTypeLabels = new string[][] {
			new string[] { "SUM", Resources.Resources.SOMATORIO37638 },
			new string[] { "AVG", Resources.Resources.MEDIA55090 },
			new string[] { "COUNT", Resources.Resources.TOTAL_DE_ELEMENTOS21962 },
			new string[] { "MAX", Resources.Resources.MAXIMO52072 },
			new string[] { "MIN", Resources.Resources.MINIMO33485 }
		};

		public static List<SpecialList> CreateResultsTableFlat(ReportReplyGroup group, ReportDefinition query)
		{
			List<SpecialList> result = new List<SpecialList>();

			List<ReportField> allFields = query.GetReportFields();

			// TODO: este caso pode passar para dentro da função? como se faz isto?
			// pode-se acrescentar um parametro extra que vem de cima e que diz se tem pagebreak ou não
			// cujo valor por omissão quando se invoca a função aqui é true, o que obriga a escrever sempre os cabeçalhos
			// testar para verificar se isto funciona bem (aproveitar a oportunidade quando se implementar os pagebreaks)

			// define-se o cabeçalho

			// acrescenta os títulos das colunas
			SpecialList header = new SpecialList() { "" };
			header.Type = LineType.Header;

			foreach (ReportField f in allFields)
				header.Add(f.GetTitle());

			result.Add(header);

			ResultsTableFlat(result, group, query, 0, true, allFields);

			return result;
		}

		public static void ResultsTableFlat(List<SpecialList> result, ReportReplyGroup group, ReportDefinition query, int nivel, bool first, List<ReportField> allFields)
		{
			// se tem filhos, então estamos num grupo
			if (!group.IsLeaf())
			{
				// tem pageBreak ?
				bool pageBreak = false;
				if (nivel <= query.Groups.Count && nivel > 0)
					pageBreak = query.Groups[nivel - 1].PageBreak;

				// o limite é o número de campos agrupados, os restantes valores são valores totalizadores
				// aqui temos de descobrir quantos campos são agregados e subtrair esse valor ao número total de campos
				// caso contrário a opção seria saber quantos campos de groupBy existem a este nível
				// o que requer precorrer todos os grupos até nivel -1 e somar os campos de groupBy que não são totalizadores
				int limite = (nivel > 0 && nivel <= query.Groups.Count) ? group.Values.Count - query.Groups[nivel - 1].Fields.Count(f => !string.IsNullOrEmpty(f.TotalType)) : 0;

				// chamada recursiva para escrever os "filhos" do grupo actual
				for (int i = 0; i < group.Groups.Count; i++)
					ResultsTableFlat(result, group.Groups[i], query, nivel + 1, i == 0, allFields);

				// os totalizadores passam a ser exibidos no final de cada grupo da seguinte forma
				//           campo1   campo2   ...   campoN   ...   campoX   campoY   campoZ       (linha do cabeçalho da tabela)
				// ....                                                                            (linhas de detalhes e sub-grupos)
				// Grupo N   valor1   valor2   ...   valorN                                        (linha dos valores do grupo)
				// SUM                                                  11       13       17       (linhas dos valores dos totalizadores para cada campo)
				// MAX                                                   3        5        7
				// MIN                                                   -        -        2       (podem não ter sido pedidos totalizadores para todas as colunas, nestes casos deixa um espaço em branco)

				bool escreveNivel = false;

				// se existem totalizadores
				if (group.Values.Count > limite)
				{
					// primeiro descobrimos qual a lista de totalizadores no nível actual onde temos de procurar
					List<ReportField> listaTotalizadores = null;
					escreveNivel = false;

					if (nivel > 0 && nivel <= query.Groups.Count)
						listaTotalizadores = query.Groups[nivel - 1].Fields.FindAll(x => !string.IsNullOrEmpty(x.TotalType));
					else
						listaTotalizadores = query.DetailsGroup.Fields.FindAll(x => !string.IsNullOrEmpty(x.TotalType));

					foreach (string[] totalType in totaTypeLabels)
					{
						// totalType[0] - id da função
						// totalType[1] - label com a descrição da função

						// lista de totalizadores para o tipo de função actual
						List<ReportField> totalizadores = listaTotalizadores.FindAll(x => x.TotalType.Equals(totalType[0]));

						// se existem totalizadores deste tipo
						if (totalizadores.Count > 0)
						{
							// se ainda não escreveu o cabeçalho para este conjunto de totalizadores, então escreve-o antes dos valores
							if (!escreveNivel)
							{
								escreveNivel = true;

								SpecialList totalHeader = new SpecialList();
								totalHeader.Type = LineType.TotalHeader;

								// escreve o nível do grupo e os valores pelos quais está agrupado nas respectivas colunas
								if (nivel == 0)
									totalHeader.Add(Resources.Resources.TOTAIS03749);
								else
									totalHeader.Add(string.Format(Resources.Resources.GRUPO22663 + " {0}", nivel));

								for (int i = 0; i < allFields.Count; i++)
								{
									if (i < nivel) // penso que isto esteja mal (verificar!!! - presumo que seja limite em vez de nivel)
										totalHeader.Add(string.IsNullOrEmpty(group.Values[i]) ? "-" : group.Values[i]);
									else
										totalHeader.Add("");
								}
								result.Add(totalHeader);
							}

							SpecialList totalValues = new SpecialList() { totalType[1] };
							totalValues.Type = LineType.TotalLine;

							for (int i = 0; i < allFields.Count; i++)
								totalValues.Add("");

							// coloca os valores nas respectivas colunas
							foreach (ReportField f in totalizadores)
							{
								// isto tem de devolver sempre um valor válido, se não algo correu mal noutro sitio!
								int pos = allFields.FindIndex(x => x.FieldId == f.FieldId);
								int posTotalGroup = listaTotalizadores.FindIndex(x => x.FieldId == f.FieldId && x.TotalType == f.TotalType);
								totalValues[pos + 1] = group.Values[posTotalGroup + limite];
							}

							result.Add(totalValues);
						}
					}
				}
			}
			else
			{
				// se não tem filhos estamos numa linha

				SpecialList current = new SpecialList { "" };
				current.Type = LineType.DetailLine;

				// escreve os valores da linha
				// começa na segunda coluna, porque a primeira é para ter os títulos dos totalizadores
				foreach (string value in group.Values)
					current.Add(value);

				result.Add(current);
			}
		}
	}

	public class ReportModel
	{
		public string ID { get; set; }

		public string Title { get; set; }

		public string Acess { get; set; }

		public string Opercria { get; set; }
	}
}

using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;

using CSGenio.business;
using CSGenio.framework;
using GenioMVC;
using GenioMVC.Helpers.Cav;
using GenioMVC.Models.Cav;
using GenioMVC.Models.Navigation;
using GenioMVC.ViewModels.Cav;
using Microsoft.AspNetCore.Mvc;

namespace GenioMVC.Controllers.Cav
{
	public class CavController : ControllerExtension
	{
		XmlCavService cavServices = XmlCavService.Instance;

		public CavController(UserContextService userContext) : base(userContext) { }

		//
		// GET: /Cav/
		[HttpGet]
		public ActionResult Index(string area)
		{
			if (!string.IsNullOrEmpty(area) && Area.ListaAreas.Contains(area.ToLower()))
			{
				List<CAVTable> tables = new List<CAVTable>();
				string? reportEncoded = HttpContext.Session.GetString("Query");
				ReportDefinition? query = reportEncoded == null ? null : JsonConvert.DeserializeObject<ReportDefinition>(reportEncoded);

				//if report does not exist it will be created a new one
				if (query == null)
				{
					query = CreateEmptyQuery(area);
					HttpContext.Session.SetString("Query", JsonConvert.SerializeObject(query));
					HttpContext.Session.SetString("QueryId", "new");
					ViewBag.QueryId = "new";
				}
				else
					tables = cavServices.GetTablesList(query.BaseTable, UserContext.Current.User.Language);

				GlobalViewModel model = new GlobalViewModel(tables, query);
				return JsonOK(model);
			}
			else
				return JsonOK(new GlobalViewModel(null, null)); // Empty model
		}

		public class RequestCavNewQueryModel
		{
			public string Area { get; set; }
		}

		[HttpPost]
		public ActionResult NewQuery([FromBody] RequestCavNewQueryModel requestModel)
		{
			string area = requestModel.Area;
			if (cavServices != null && !string.IsNullOrEmpty(area))
			{
				ReportDefinition query = CreateEmptyQuery(area);
				HttpContext.Session.SetString("Query", JsonConvert.SerializeObject(query));
				HttpContext.Session.SetString("QueryId", "new");

				List<CAVTable> tables = cavServices.GetTablesList(query.BaseTable, UserContext.Current.User.Language);
				GlobalViewModel model = new GlobalViewModel(tables, query);
				return JsonOK(new { Success = true, Model = model });
			}

			Log.Error($"CAV - Could not create new report. {area};");
			return Json(new { Success = false, Message = "Could not create new report!" });
		}

		private ReportDefinition CreateEmptyQuery(string area)
		{
			if (cavServices != null && string.IsNullOrEmpty(area))
				return null;

			ReportDefinition query = new ReportDefinition();
			query.DetailsGroup = new ReportGroup();
			query.DetailsGroup.Fields = new List<ReportField>();
			query.BaseTable = area.ToUpper();

			if (!string.IsNullOrEmpty(query.BaseTable))
			{
				List<CAVTable> tables = cavServices.GetTablesList(query.BaseTable, UserContext.Current.User.Language);
				CAVTable table = tables.FirstOrDefault(p => p.Id.Equals(query.BaseTable, System.StringComparison.OrdinalIgnoreCase));
				if (table != null)
					query.BaseTableDescription = table.Description;
			}

			return query;
		}

		[HttpGet]
		public ActionResult Details()
		{
			if (cavServices != null)
			{
				string reportEncoded = HttpContext.Session.GetString("Query");
				ReportDefinition query = JsonConvert.DeserializeObject<ReportDefinition>(reportEncoded);
				List<CAVTable> tables = new List<CAVTable>();

				if (!string.IsNullOrEmpty(query.BaseTable))
					tables = cavServices.GetTablesList(query.BaseTable, UserContext.Current.User.Language);

				GlobalViewModel model = new GlobalViewModel(tables, query);

				return JsonOK(model);
			}

			return JsonOK();
		}

		[HttpGet]
		public ActionResult GetTabContentByType(string type)
		{
			if (cavServices != null)
			{
				string reportEncoded = HttpContext.Session.GetString("Query");
				ReportDefinition query = JsonConvert.DeserializeObject<ReportDefinition>(reportEncoded);
				List<CAVTable> tables = new List<CAVTable>();

				if (!string.IsNullOrEmpty(query.BaseTable))
					tables = cavServices.GetTablesList(query.BaseTable, UserContext.Current.User.Language);

				GlobalViewModel model = new GlobalViewModel(tables, query);
				ViewBag.Table = query.BaseTable;

				switch (type)
				{
					case "C":
					case "G":
					case "O":
					case "T":
					case "E":
						return JsonOK(model);
					default:
						return JsonOK();
				}
			}

			return JsonOK();
		}

		public class RequestCavAddFieldModel
		{
			public string TableId { get; set; }
			public string FieldId { get; set; }
		}

		[HttpPost]
		public ActionResult AddField([FromBody] RequestCavAddFieldModel requestModel)
		{
			var tableId = requestModel.TableId;
			var fieldId = requestModel.FieldId;

			string? reportEncoded = HttpContext.Session.GetString("Query");
			ReportDefinition? query = reportEncoded == null ? null : JsonConvert.DeserializeObject<ReportDefinition>(reportEncoded);
			if (query != null && cavServices != null)
			{
				List<CAVTable> tables = cavServices.GetTablesList(query.BaseTable, UserContext.Current.User.Language);
				CAVTable table = cavServices.GetTable(tableId, UserContext.Current.User.Language);

				if (table != null)
				{
					if (table.Id.Equals(query.BaseTable, System.StringComparison.OrdinalIgnoreCase) || cavServices.ExistRelationship(query.BaseTable, tableId))
					{
						var fieldToAdd = tableId + "." + fieldId;

						GenioMVC.Models.Cav.Field field = table.Fields.FirstOrDefault(p => p.Id.Equals(fieldToAdd, System.StringComparison.OrdinalIgnoreCase));
						if (field != null)
						{
							ReportField rptField = new ReportField { FieldId = field.Id, Title = field.Description, TableId = field.TableId };
							if (query.DetailsGroup.Fields == null)
								query.DetailsGroup.Fields = new List<ReportField>();

							query.DetailsGroup.Fields.Add(rptField);

							// Check if it's an below table (Prototype for implementation testing)
							var _table = tables.FirstOrDefault(tbl => tbl.Id == tableId);
							if (_table != null && _table.Down)
							{
								if (query.ExtraPaths == null)
									query.ExtraPaths = new List<ReportLink>();

								if (!query.ExtraPaths.Any(link => link.DestTable == tableId))
									query.ExtraPaths.Add(new ReportLink()
									{
										SourceTable = query.BaseTable,
										DestTable = tableId,
										Down = true
									});
							}

							HttpContext.Session.SetString("Query", JsonConvert.SerializeObject(query));

							GlobalViewModel model = new GlobalViewModel(tables, query);
							return JsonOK(new { Success = true, Model = model });
						}
					}
				}
			}

			Log.Error($"CAV - Error adding field. Unrelated field. table: {tableId}; field: {fieldId};");
			return Json(new { Success = false, Message = "Error adding field. Unrelated field!" });
		}


		public class RequestCavUpdateModel
		{
			public List<ReportField> Fields { get; set; }
			public List<ReportOrdering> Orderby { get; set; }
			public ReportCondition Conditions { get; set; }
			public List<ReportGroup> Groupby { get; set; }
			public string Area { get; set; }
			public List<List<ReportField>> Totals { get; set; }
		}

		[HttpPost]
		public ActionResult UpdateQuery([FromBody] RequestCavUpdateModel requestModel)
		{
			var fields = requestModel.Fields;
			var orderby = requestModel.Orderby;
			var conditions = requestModel.Conditions;
			var groupby = requestModel.Groupby;
			var area = requestModel.Area;
			var totals = requestModel.Totals;

			// utilizar este método para atravessar o limbo
			// ou seja, antes de qualquer operação que exija
			// que a query esteja actualizada do lado do servidor
			// deve-se invocar este método
			string reportEncoded = HttpContext.Session.GetString("Query");
			ReportDefinition query = JsonConvert.DeserializeObject<ReportDefinition>(reportEncoded);

			List<ReportField> _fields = fields;
			List<ReportGroup> _groupby = groupby?.Where(group => group.Fields?.Any() == true).ToList();
			if (totals?.Count > 0)
			{
				_fields = fields.Concat(totals[0]).ToList();
				if (groupby?.Count == totals.Count - 1)
				{
					_groupby = new List<ReportGroup>();
					for (var groupIndex = 1; groupIndex < totals.Count; groupIndex++)
					{
						var _group = totals[groupIndex];
						_groupby.Add(new ReportGroup()
						{
							Fields = groupby[groupIndex - 1]?.Fields?.Where(f => string.IsNullOrEmpty(f.TotalType)).Concat(_group).ToList(),
							PageBreak = groupby[groupIndex - 1]?.PageBreak ?? false
						});
					}
				}
			}

			try
			{
				query.DetailsGroup = new ReportGroup() { Fields = _fields };
				query.Condition = conditions;
				query.Groups = _groupby;
				query.Orderings = orderby;

				// check if there any link for removed field (Prototype for implementation testing)
				if (query.ExtraPaths != null)
					query.ExtraPaths.RemoveAll(link => !_fields.Any(fld => fld.TableId == link.DestTable));
			}
			catch (Exception e)
			{
				Log.Error($"CAV - Error updating the query. {e.Message}");
				// se falhou a des-serializar a query é porque não tem campos escolhidos, logo não se grava nada
				// TODO: melhorar isto (CreateReportDefinition)
			}

			return Index(area);
		}

		public class RequestCavSaveModel
		{
			public string Id { get; set; }
			public bool QueryOverride { get; set; }
			public string Title { get; set; }
			public string AccessType { get; set; }
		}

		[HttpPost]
		public ActionResult SaveQueryData([FromBody] RequestCavSaveModel requestModel)
		{
			string id = requestModel.Id;
			bool queryOverride = requestModel.QueryOverride;
			string title = requestModel.Title;
			string accessType = requestModel.AccessType;

			try
			{
				if (cavServices == null)
				{
					Log.Error("CAV - Could not save the query. The 'XMLCavService' needs to be initialized");
					return JsonERROR("Could not save the query!");
				}

				if (!queryOverride || (!string.IsNullOrEmpty(id) && id.Equals("new", StringComparison.InvariantCultureIgnoreCase)))
					id = null;

				string reportEncoded = HttpContext.Session.GetString("Query");
				ReportDefinition query = JsonConvert.DeserializeObject<ReportDefinition>(reportEncoded);

				if (query != null)
				{
					query.Title = title;
					query.Acesso = accessType;
				}

				if (cavServices.SaveQuery(query, UserContext.Current.User, id))
					return JsonOK("Query saved successfully!");

				Log.Error($"CAV - Error saving the query! id: {id}");
				return JsonERROR("Could not save the query!");
			}
			catch (Exception e)
			{
				Log.Error($"CAV - Error saving the query! id: {id}; {e.Message}");
				return JsonERROR("Error saving the query!");
			}
		}

		public ActionResult ExecuteQuery(string table, string data, int? page, string queryid = null)
		{
			try
			{
				Dictionary<string, string> collection = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

				ReportDefinition query = null;
				// Construir pedido
				if (queryid == null && cavServices != null)
					query = CreateReportDefinition(table, collection);

				return ExecuteQuery2(new RequestCavQuery2Model() {
					Query = query,
					Page = page,
					Queryid = queryid
				});
			}
			catch (Exception e)
			{
				return onErrorExecuteQuery(e);
			}
		}


		public class RequestCavQuery2Model
		{
			public ReportDefinition Query { get; set; }
			public int? Page { get; set; }
			public string Queryid { get; set; }
		}


		public ActionResult ExecuteQuery2([FromBody]RequestCavQuery2Model requestModel)
		{
			var query = requestModel.Query;
			var page = requestModel.Page;
			var queryid = requestModel.Queryid;

			try
			{
				ResultModel model_result = null;
				if (queryid == null && cavServices != null)
				{
					ReportReply response = new ReportReply();
					try
					{
						CavEngine cavEng = new CavEngine(cavServices);
						response = cavEng.ExecuteQuery(UserContext.Current.User, query);
					}
					catch (Exception e)
					{
						// Não foi possível concluir o pedido
						throw new FrameworkException("Error processing Advanced Query", "ExecuteQueryNew", e.Message, e);
					}

					// Construir o modelo de dados para a vista
					model_result = new ResultModel(response, query);
				}

				if (model_result == null)
					throw new FrameworkException("Some problem with the service or the query is not recognized.", "ExecuteQuery2", "Either queryid isn't null or cavServices is null", new ArgumentNullException());

				TempData.SetObject("CavModelResult", model_result.Result);
				TempData.SetObject("CavModelQuery", model_result.Query);
				TempData.SetObject("CavQuerySQL", model_result.Result.QuerySQL);


				List<SpecialList> results = ResultsHelpers.CreateResultsTableFlat(model_result.Result.MainGroup, model_result.Query);

				// Paginação de resultados

				var pageNumber = page ?? 1;
				// Para já está com 50 rows, mas podem ser mais...
				const int pageSize = 50;
				//IPagedList<SpecialList> paged_list = results.ToPagedList(pageNumber, 50);
				//List<SpecialList> final_list = paged_list.ToList();
				List<SpecialList> final_list = results.Skip((pageNumber-1)*pageSize).Take(pageSize).ToList();

				// isto não pode ser feito assim para multi-ano, por agora leva esta martelada só para resolver
				if (pageNumber != 1 && !(model_result.Query.Years != null && model_result.Query.Years.Count > 0 && model_result.Query.MultiYearMode == "PAGE"))
				{
					SpecialList header = results.First(); // Vai se reter a tabela header para aparecer em todas as páginas
					final_list.Insert(0, header);
				}

				return JsonOK(new
				{
					Success = true,
					querySQL = model_result.Result.QuerySQL,
					record_count = model_result.Result.ResultCount.ToString(),
					total_pages = results.Count / pageSize, //paged_list.PageCount,
					current_page = pageNumber, //paged_list.PageNumber,
					results = final_list
				});
			}
			catch (Exception e)
			{
				return onErrorExecuteQuery(e);
			}
		}

		private ActionResult onErrorExecuteQuery(Exception error)
		{
			var errorMessage = Resources.Resources.OCORREU_UM_ERRO_AO_P53091;
			if (error is GenioException genioError)
				errorMessage += " - " + genioError.UserMessage;

			Log.Error($"CAV - onErrorExecuteQuery: {error.Message}");

			TempData.SetObject("CavQuerySQL", errorMessage);
			return JsonERROR(errorMessage);
		}

		[HttpPost]
		public ActionResult GetTableRelations(string table)
		{
			if (cavServices != null)
			{
				object result = cavServices.ConstructRelationList(table);
				return Json(result);
			}
			else
			{
				Log.Error("CAV - The 'XMLCavService' needs to be initialized");
				return Json(new { result = "E", message = "CAV service is unavailable" });
			}
		}

		private static ReportDefinition CreateReportDefinition(string table, Dictionary<string, string> collection)
		{
			ReportDefinition result = new ReportDefinition()
			{
				BaseTable = table,
				DetailsGroup = new ReportGroup() { Fields = JsonConvert.DeserializeObject<List<ReportField>>(collection["fields"]) },
				Condition = JsonConvert.DeserializeObject<ReportCondition>(collection["conditions"]),
				Orderings = JsonConvert.DeserializeObject<List<ReportOrdering>>(collection["orderby"]),
				Groups = JsonConvert.DeserializeObject<List<ReportGroup>>(collection["groupby"]),
				ExtraPaths = JsonConvert.DeserializeObject<List<ReportLink>>(collection["relations"])
			};

			return result;
		}

		public ActionResult ObtainSQLQuery()
		{
			// Versão antiga do HomeController
			string result = TempData.GetObject<string>("CavQuerySQL");
			return new ContentResult { Content = result, ContentType = "text/html" };
		}

		public ActionResult LoadQueryList()
		{
			var reportList = cavServices.LoadQueryList(UserContext.Current.User);
			return JsonOK(reportList);
		}

		public ActionResult LoadQuery(string queryid)
		{
			ReportDefinition query = cavServices.LoadQuery(queryid, UserContext.Current.User);

			if (query != null)
			{
				HttpContext.Session.SetString("Query", JsonConvert.SerializeObject(query));
				HttpContext.Session.SetString("QueryId", "new");

				List<CAVTable> tables = cavServices.GetTablesList(query.BaseTable, UserContext.Current.User.Language);
				GlobalViewModel model = new GlobalViewModel(tables, query);
				return JsonOK(model);
			}

			Log.Error($"CAV - Error getting query. {queryid}");
			return JsonOK();
		}

		public FileResult GenerateExcel()
		{
			// Versão antiga do HomeController
			// Model received from the ExecuteQuery request
			var reportReplay = TempData.GetObject<ReportReply>("CavModelResult");
			var reportQuery = TempData.GetObject<ReportDefinition>("CavModelQuery");
			ResultModel model = new ResultModel(reportReplay, reportQuery);

			// Report Excel instance
			ReportExcel report = new ReportExcel(model);

			// Generate Excel file
			byte[] exelfile = report.GenerateExcelBytes();

			TempData.SetObject("CavModelResult", model.Result);
			TempData.SetObject("CavModelQuery", model.Query);

			// Write it back to the client
			//string fileName = "Consulta_" + model.Query.BaseTable + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
			string fileName = model.Query.BaseTable + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
			Response.Headers["content-disposition"] = "attachment;  filename=" + fileName + ".xlsx";
			return File(exelfile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
		}
	}
}

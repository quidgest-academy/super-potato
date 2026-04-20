using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.IO;
using CSGenio;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Models;
using Quidgest.Persistence.GenericQuery;
using Microsoft.AspNetCore.Mvc;

namespace GenioMVC.Controllers
{
	public class AuditViewerController : ControllerBase
	{
		public AuditViewerController(UserContextService userContext) : base(userContext) { }

		public string CurrentYear
		{
			get
			{
				return UserContext.Current.User.Year;
			}
		}

		//
		// GET: /AuditViewer/Index
		[HttpGet]
		public ActionResult Index(string logTable = "", string logRow ="")
		{
			var model = new AuditModel(UserContext.Current)
			{
				Result = new AuditResult()
			};

			try
			{
				var dataSystem = Configuration.ResolveDataSystem(CurrentYear,Configuration.DbTypes.NORMAL); // Default == null

				model.DataSystem = dataSystem;
				model.Year = CurrentYear;

				// Default database is the system database
				model.LogDatabaseSelected = false;

				if (logTable != "")
				{
					logTable = logTable.ToUpper();
					AuditModel.LogTables logTableEnum;

					if (!Enum.TryParse(logTable, out logTableEnum))
					{
						model.ResultMsg = Resources.Resources.REFERENCIA_A_TABELA_53231;
						return JsonOK(model);
					}

					model.LogTable = logTableEnum;
					model.LockTable = true;
				}
				else
					model.LogTable = AuditModel.LogTables.logFORall;

				model.FillColumnNames();

				if (logRow != "")
				{
					model.LockRow = true;
					model.SelectedRow = logRow;
					PersistentSupport sp = PersistentSupport.getPersistentSupport(model.Year);
					model.FillSelectedRowValues(sp);
				}

				// Check if log database exists
				model.LogDatabaseExists = dataSystem.DataSystemLog != null && dataSystem.DataSystemLog.Schemas.Count != 0 ? true : false;

				// Set max log row days
				model.MaxLogRowDays = Configuration.MaxLogRowDays;
			}
			catch (Exception e)
			{
				model.ResultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
			}

			return JsonOK(model);
		}

		/// <summary>
		/// For view change / row selection
		/// </summary>
		/// <param name="model">Audit model</param>
		/// <returns>View with selected table / row</returns>
		public ActionResult Refresh(AuditModel model)
		{
			try
			{
				if (Configuration.Years.Count == 0)
				{
					model.ResultMsg = Resources.Resources.FICHEIRO_DE_CONFIGUR13972;
					return JsonOK(model);
				}

				// Set max log row days
				model.MaxLogRowDays = Configuration.MaxLogRowDays;

				// Fill column values
				model.Result = new AuditResult();
				model.FillColumnNames();

				// Fill current row values, if row is selected
				if (!string.IsNullOrEmpty(model.SelectedRow))
				{
					PersistentSupport sp = PersistentSupport.getPersistentSupport(model.Year);
					model.FillSelectedRowValues(sp);
				}
			}
			catch (Exception e)
			{
				model.ResultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
			}

			return JsonOK(model);
		}

		/// <summary>
		/// For table reloading
		/// </summary>
		/// <param name="param">Datatable specific parameters</param>
		/// <param name="logTable">Selected view table</param>
		/// <param name="rowSelected">Selected row ID</param>
		/// <param name="logDatabase">True if the log database is selected</param>
		/// <param name="year">Selected year</param>
		/// <param name="export">True for table exporting</param>
		/// <param name="exportType">Type of table exporting (pdf, excel, odt or csv)</param>
		/// <returns>
		/// Json result with data values for datatable if not exporting;
		/// Json result with url to download exported table when exporting
		/// </returns>
		public ActionResult Reload(JQueryDataTableParamModel param, string logTable, string rowSelected, bool logDatabase, string year, bool export, string exportType)
		{
			var model = new AuditModel(UserContext.Current)
			{
				SelectedRow = rowSelected,
				LogDatabaseSelected = logDatabase
			};

			AuditModel.LogTables logTableEnum;
			if (!Enum.TryParse(logTable, out logTableEnum))
			{
				model.ResultMsg = Resources.Resources.REFERENCIA_A_TABELA_53231;
				return JsonOK(model);
			}

			model.Year = year;
			model.LogTable = logTableEnum;

			// Set max log row days
			model.MaxLogRowDays = Configuration.MaxLogRowDays;

			try
			{
				// Get PersistentSupport reference
				PersistentSupport sp = logDatabase ? PersistentSupport.getPersistentSupportLog(model.Year, UserContext.Current.User.Name) : PersistentSupport.getPersistentSupport(model.Year);

				// Process simple search filters
				CriteriaSet searchFilters = null;
				if (param.search != null && !string.IsNullOrEmpty(param.search.value))
				{
					string viewName;
					if (model.LogTable == AuditModel.LogTables.logFORall)
						viewName = model.LogTable.ToString();
					else
						viewName = AuditModel.LogTables.logFORall.ToString() + "_" + model.LogTable.ToString() + "_VIEW";

					string searchValue = param.search.value;

					searchFilters = CriteriaSet.Or();
					searchFilters.Like(viewName, "who", searchValue);
					searchFilters.Like(viewName, "cod", searchValue);

					if (model.LogTable == AuditModel.LogTables.logFORall)
					{
						searchFilters.Like(viewName, "logtable", searchValue);
						searchFilters.Like(viewName, "logfield", searchValue);
						searchFilters.Like(viewName, "val", searchValue);
					}
					else
					{
						AreaInfo table = CSGenio.business.Area.GetInfoArea(model.LogTable.ToString().ToLower());

						if (table != null)
							foreach (var field in table.DBFieldsList.Where(field => field.CriaLog))
								searchFilters.Like(viewName, string.Format("{0}.{1}", field.Alias, field.Name), searchValue);
					}
				}

				// Process column sort
				List<ColumnSort> sortColumns = null;
				if (param.order != null)
				{
					sortColumns = new List<ColumnSort>();

					foreach (DTOrder orderColumn in param.order)
					{
						int columnIndex = orderColumn.column + 1;
						SortOrder columnSort = orderColumn.dir == "asc" ? SortOrder.Ascending : SortOrder.Descending;
						sortColumns.Add(new ColumnSort(columnIndex, columnSort));
					}
				}

				// Get result
				model.Where(sp, param.start, param.length, searchFilters, sortColumns, export);

				if (export)
				{
					string file = "LogFOR_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + "." + exportType;
					var user = new User("LogExport", new Guid().ToString(), Configuration.DefaultYear);
					var exportar = new Exports(user);

					string title = Resources.Resources.AUDITORIA_DO_SISTEMA08460;
					if (model.LogTable == AuditModel.LogTables.logFORall)
						title += " - " + DateTime.Now.ToString(CultureInfo.CurrentCulture);
					else
						title += ": " +
								 ((DisplayAttribute)
									(typeof (AuditModel.LogTables).GetField(model.LogTable.ToString())
										.GetCustomAttributes(typeof (DisplayAttribute), false)
										.First())).GetName() + " - " +
								 DateTime.Now.ToString(CultureInfo.CurrentCulture);

					List<Exports.QColumn> columns = model.getColumnDefs();

					var fileBytes = exportar.ExportList(model.Result.RawData, columns, exportType, file, title);
					QCache.Instance.ExportFiles.Put(file, fileBytes);
					return Json(new { Url = Url.Action("DownloadExportFile", "AuditViewer", new { id = file, type = exportType }) });
				}

				// Turn table data into a string matrix
				var result = model.Result.Values.Select(element => element.ColumnValues).ToList();

				// Send result to dataTable
				return Json(new
				{
					draw = param.draw,
					recordsTotal = model.TotalRows,
					recordsFiltered = model.TotalFilteredRows,
					data = result
				});
			}
			catch (Exception e)
			{
				model.ResultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
			}

			// Send error result to dataTable
			return Json(new
			{
				draw = param.draw,
				recordsTotal = model.TotalRows,
				recordsFiltered = model.TotalFilteredRows,
				error = model.ResultMsg
			});
		}
	}
}

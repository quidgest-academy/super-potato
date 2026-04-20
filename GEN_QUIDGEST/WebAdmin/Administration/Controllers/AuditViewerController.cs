using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace Administration.Controllers
{
	public class AuditViewerController : ControllerBase
	{
		/// <summary>
        /// The transfer operation jobs
        /// </summary>
        private static readonly Dictionary<string, ExecuteQueryCore.TransferLogOperation> transferOperationJobs 
            = new Dictionary<string, ExecuteQueryCore.TransferLogOperation>();

		/// <summary>
		/// For view change / row selection
		/// </summary>
		/// <param name="model">Audit model</param>
		/// <returns>View with selected table / row</returns>
		
		[HttpPost]
		public IActionResult Refresh([FromBody] Models.AuditModel model)
		{
			try
			{
				model.ResultMsg = string.Empty;
				if (CSGenio.framework.Configuration.Years.Count == 0)
				{
					model.ResultMsg = Resources.Resources.FICHEIRO_DE_CONFIGUR13972;
					return Json(model);
				}

				// Set max log row days
				model.MaxLogRowDays = CSGenio.framework.Configuration.MaxLogRowDays;

				// Fill column values
				model.Result = new Models.AuditResult();
				model.FillColumnNames();

				// Fill current row values, if row is selected
				if (!string.IsNullOrEmpty(model.SelectedRow))
				{
					PersistentSupport sp = PersistentSupport.getPersistentSupport(CurrentYear);
					model.FillSelectedRowValues(sp);
				}

			}
			catch (Exception e)
			{
				model.ResultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
			}

			return Json(model);
		}

		public struct AuditViewerReloadParameters
		{
			/// <summary>
			/// Datatable specific parameters
			/// </summary>
			public Models.JQueryDataTableParamModel param { get; set; }
			/// <summary>
			/// Selected view table
			/// </summary>
			public string logTable { get; set; }
			/// <summary>
			/// Selected row ID
			/// </summary>
			public string rowSelected { get; set; }
			/// <summary>
			/// True if the log database is selected
			/// </summary>
			public bool logDatabase { get; set; }
			/// <summary>
			/// True for table exporting
			/// </summary>
			public bool export { get; set; }
			/// <summary>
			/// Type of table exporting (pdf, excel, odt or csv)
			/// </summary>
			public string exportType { get; set; }
		}

		/// <summary>
		/// For table reloading
		/// </summary>
		/// <param name="data">Data table parameters</param>
		/// <returns>
		/// Json result with data values for datatable if not exporting; 
		/// Json result with url to download exported table when exporting
		/// </returns>
		
		[HttpPost]
		public IActionResult Reload([FromBody] AuditViewerReloadParameters data)
		{
			var model = new Models.AuditModel
			{
				SelectedRow = data.rowSelected,
				LogDatabaseSelected = data.logDatabase
			};

			var dataSystem = CSGenio.framework.Configuration.ResolveDataSystem(CurrentYear, CSGenio.framework.Configuration.DbTypes.NORMAL); // Default == null
			if (dataSystem == null)
			{
				model.ResultMsg = Resources.Resources.FICHEIRO_DE_CONFIGUR13972;
				return Json(model);
			}

			if (!Enum.TryParse(data.logTable, out Models.AuditModel.LogTables logTableEnum))
			{
				model.ResultMsg = Resources.Resources.REFERENCIA_A_TABELA_53231;
				return Json(model);
			}

			model.LogTable = logTableEnum;

			// Set max log row days
			model.MaxLogRowDays = CSGenio.framework.Configuration.MaxLogRowDays;

			// Check if log database exists
			model.LogDatabaseExists = dataSystem.DataSystemLog != null && dataSystem.DataSystemLog.Schemas.Count != 0;

			try
			{
				// Get PersistentSupport reference
				PersistentSupport sp = data.logDatabase ? PersistentSupport.getPersistentSupportLog(CurrentYear, "") : PersistentSupport.getPersistentSupport(CurrentYear);

				// Process simple search filters
				CriteriaSet searchFilters = GetSearchFilters(data.param.global_search, model);

				// Process column sort
				List<ColumnSort> sortColumns = GetSortColumns(data.param.sort);

				// Get result
				model.Where(sp, data.param.page-1, data.param.per_page, searchFilters, sortColumns, data.export, CurrentYear);
				model.FillColumnNames();

				if (data.export)
				{
					return ExportLogData(data.exportType, model);
				}
			}
			catch (Exception e)
			{
				var msg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper()); ;
				if(data.export)
					return Json(new { Success = false, Message = msg });

				model.ResultMsg = msg;
			}

			return Json(model);
		}
		
		/// <summary>
		/// Retrieve the search filters for the log data
		/// </summary>
		/// <param name="textFilter">Text filters</param>
		/// <param name="model">Audit model</param>
		/// <returns>
		/// CriteriaSet containing search filters
		/// </returns>
		private CriteriaSet GetSearchFilters(string textFilter, Models.AuditModel model)
		{
			CriteriaSet searchFilters = null;
			if (!string.IsNullOrEmpty(textFilter))
			{
				string viewName;
				if (model.LogTable == Models.AuditModel.LogTables.logFORall)
				{
					viewName = model.LogTable.ToString();
				}
				else
				{
					viewName = Models.AuditModel.LogTables.logFORall.ToString() + "_" + model.LogTable.ToString() + "_VIEW";
				}

				string searchValue = "%" + textFilter + "%"; 

				searchFilters = CriteriaSet.Or();
				searchFilters.Like(viewName, "who", searchValue);
				searchFilters.Like(viewName, "cod", searchValue);

				if (model.LogTable == Models.AuditModel.LogTables.logFORall)
				{
					searchFilters.Like(viewName, "logtable", searchValue);
					searchFilters.Like(viewName, "logfield", searchValue);
					searchFilters.Like(viewName, "val", searchValue);
					
				}
				else
				{
					AreaInfo table = CSGenio.business.Area.GetInfoArea(model.LogTable.ToString().ToLower());
					if (table != null)
					{
						foreach (var field in table.DBFieldsList.Where(field => field.CriaLog))
						{
							searchFilters.Like(viewName, string.Format("{0}.{1}", field.Alias, field.Name), searchValue);
						}
					}
				}
			}
			return searchFilters; 
		}

		/// <summary>
		/// Retrieve the sort columns for the log data
		/// </summary>
		/// <param name="columns">Sort columns</param>
		/// <returns>
		/// List containing the sort columns
		/// </returns>
		private List<ColumnSort> GetSortColumns(IEnumerable<Models.DTOrder> columns)
		{
			List<ColumnSort> sortColumns = null;
			if (columns != null)
			{
				sortColumns = new List<ColumnSort>();

				foreach (Models.DTOrder orderColumn in columns)
				{
					int.TryParse(orderColumn.name, out int columnIndex);
					SortOrder columnSort = orderColumn.order == "asc" ? SortOrder.Ascending : SortOrder.Descending;
					sortColumns.Add(new ColumnSort(columnIndex + 1, columnSort));
				}
			}
			return sortColumns;
		}


		/// <summary>
		/// Exports the log data
		/// </summary>
		/// <param name="exportType">Type of Export</param>
		/// <param name="model">Audit Model</param>
		/// <returns>
		/// Json object with the identifier of the file that contains the results
		/// </returns>
		private IActionResult ExportLogData(string exportType, Models.AuditModel model)
		{
			string file = "LogFOR_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + "." + exportType;
			var user = new User("LogExport", new Guid().ToString(), CSGenio.framework.Configuration.DefaultYear);
			var exportar = new Exports(user);

			string title = Resources.Resources.AUDITORIA_DO_SISTEMA08460;
			if (model.LogTable == Models.AuditModel.LogTables.logFORall)
				title += " - " + DateTime.Now.ToString(CultureInfo.CurrentCulture);
			else
				title += ": " +
							((System.ComponentModel.DataAnnotations.DisplayAttribute)
								(typeof (Models.AuditModel.LogTables).GetField(model.LogTable.ToString())
									.GetCustomAttributes(typeof (System.ComponentModel.DataAnnotations.DisplayAttribute), false)
									.First())).GetName() + " - " +
							DateTime.Now.ToString(CultureInfo.CurrentCulture);

			List<Exports.QColumn> columns = model.getColumnDefs();

			var fileBytes = exportar.ExportList(model.Result.RawData, columns, exportType, file, title);
			QCache.Instance.ExportFiles.Put("webadmin_exportfile_" + file, fileBytes);
			return Json(new { Success = true, fileId = file });
		}

		/// <summary>
		/// Transfer log rows from the system database to the system log database.
		/// </summary>
		/// <param name="transferAll">if set to <c>true</c> [transfer all].</param>
		/// <returns></returns>
		
		[HttpPost]
		public IActionResult TransferLog(bool transferAll)
		{
			if (CSGenio.framework.Configuration.Years.Count == 0)
			{
				return Json(new { Success = false, Message = Resources.Resources.FICHEIRO_DE_CONFIGUR13972 });
			}

			try 
			{
				// Call log transfer from the destination database
				CSGenio.persistence.PersistentSupport logSp 
					= CSGenio.persistence.PersistentSupport.getPersistentSupportLog(CurrentYear, "");

				string jobId = Guid.NewGuid().ToString();
				var job = new ExecuteQueryCore.TransferLogOperation();
				transferOperationJobs.Add(jobId, job);

				System.Threading.Tasks.Task.Factory.StartNew(() => logSp.transferLog(transferAll, job));
				
				return Json(new { Success = true, RequestId = jobId });
			}
			catch(PersistenceException e)
			{
				return Json(new { Success = false, Message = Translations.Get(e.UserMessage, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper()) + " : " + Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper()) });
			}
			catch (Exception e)
			{
				return Json(new { Success = false, Message = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper()) });
			}
		}

		/// <summary>
		/// Gets the progress of the transfer log operation.
		/// </summary>
		/// <returns></returns>
		
		[HttpGet]
		public IActionResult Progress(string requestId)
		{
			if (!transferOperationJobs.ContainsKey(requestId))
                return Json(new { Success = false, Progress = 0, Completed = true, Message = "There is no ongoing transfer operation associated with the provided id" });

            var job = transferOperationJobs[requestId];

			if (job.Total == 0)
				return Json(new { Success = true, Progress = 0, Completed = false, Message = Resources.Resources.A_INICIAR_O_PROCESSO04003 });

			string message;
			var progress = job.Copied * 100 / job.Total;
			bool success = !job.Completed || string.IsNullOrEmpty(job.ErrorMessage);

			// Not completed => show progress
			if (!job.Completed)
				message = string.Format(Resources.Resources.A_TRANSFERIR_OS_DADO21520, job.CurrentTable);
			// Completed with errors => show error messages
			else if (!success)
				message = job.ErrorMessage;
			// Transfer completed
			else
				message = Resources.Resources.DADOS_TRANSFERIDOS_C46285;

			return Json(new { Success = success, Progress = progress, Completed = job.Completed, Message = message });
		}
	}
}

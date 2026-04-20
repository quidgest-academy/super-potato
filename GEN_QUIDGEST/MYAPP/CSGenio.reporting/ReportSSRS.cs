using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;


#if NETFRAMEWORK
using Microsoft.Reporting.WebForms;
using System.Security;
using System.Security.Permissions;
#else
using Microsoft.Reporting.NETCore;
#endif
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CSGenio.reporting
{
    /// <summary>
    /// Reporting Services render result
    /// </summary>
    public class ReportSSRS_Result
    {
        public string MimeType;
        public string Encoding;
        public string FileName;
        public string FileNameExtension;
        public string[] Streams;
        public Warning[] Warnings;
        public byte[] File;

        public ReportSSRS_Result() { }
    }

    public class ReportSSRS : IDisposable
    {
        #region Private fields
        private string downloadFileName;
        private bool isServerReport;
        private string fullReportPhysicalPath;
        private LocalReport localReportInstance;
        private ServerReport serverReportInstance;
        private List<ReportParameter> reportParameters;
        private PersistentSupport sp;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportSSRS"/> class.
        /// </summary>
        /// <param name="reportPath">The report path</param>
        /// <param name="downloadFileName">Name of the download file</param>
        /// <param name="fullReportPath">The full path for .rdl file (Local report)</param>
        /// <param name="isServerReport">This is the Server report?</param>
        /// <param name="sp">Persistent Support (Local report)</param>
        public ReportSSRS(string reportPath, string downloadFileName, string fullReportPath = null, bool isServerReport = true, PersistentSupport sp = null)
        {
            this.isServerReport = isServerReport;
            this.sp = sp;
            reportsContructor(reportPath, downloadFileName, fullReportPath);
        }

        public ReportSSRS(ServerReport report, string reportPath, string downloadFileName)
        {
            this.isServerReport = true;
            reportsContructor(reportPath, downloadFileName, null, report);
        }

        public ReportSSRS(LocalReport report, string downloadFileName, string fullReportPath = null, PersistentSupport sp = null)
        {
            this.isServerReport = false;
            this.sp = sp;
            reportsContructor(fullReportPath, downloadFileName, fullReportPath, null, report);
        }

        private void reportsContructor(string reportPath, string downloadFileName, string fullReportPath, ServerReport srvReport = null, LocalReport lclReport = null)
        {
            //Added to support RS over HTTPS
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

            reportParameters = new List<ReportParameter>();
            this.downloadFileName = downloadFileName;
            if (!this.isServerReport)
            {
                fullReportPhysicalPath = fullReportPath;
                using (FileStream reportFile = new FileStream(fullReportPhysicalPath, FileMode.Open, FileAccess.Read))
                {
                    localReportInstance = lclReport != null ? lclReport : new LocalReport();
                    localReportInstance.DisplayName = this.downloadFileName;
                    localReportInstance.LoadReportDefinition(reportFile);
                    localReportInstance.ReportPath = reportPath;
                }
            }
            else
            {
                serverReportInstance = srvReport != null ? srvReport : new ServerReport();
                serverReportInstance.DisplayName = this.downloadFileName;
                serverReportInstance.ReportServerUrl = new Uri(Configuration.SSRSServer.url);
                serverReportInstance.ReportPath = reportPath;
                serverReportInstance.Timeout = 6000000; //Default 600000ms = 10minutes. Increased to 100minutes
            }
        }

        /// <summary>
        /// Get the report export type, based on a given file extension.
        /// </summary>
        /// <param name="fileType">PDF | XLSX | DOC | ...</param>
        /// <returns>The SRSS export type corresponding to the given file extension.</returns>
        public static string GetExportType(string fileType)
        {
            switch (fileType)
            {
                case "XLSX":
                    return "EXCELOPENXML";
                case "DOC":
                    return "WORDOPENXML";
                default:
                    return "PDF";
            }
        }

        private Dictionary<string, string> getParametersAsDictionary()
        {
            var parameters = new Dictionary<string, string>();

            foreach (var param in reportParameters)
                parameters.Add(param.Name, param.Values[0]);// TODO: Multiple values ???

            return parameters;
        }

        private void ProcessDataSources()
        {
            // 1. Clear Report Data
            localReportInstance.DataSources.Clear();

            // 2. Get the data for the report
            // Look-up the DB query in the "DataSets" 
            // element of the report file (.rdl/.rdlc which contains XML)
            var reportDef = CSGenio.reporting.serialization.Report.GetReportFromFile(fullReportPhysicalPath);

            var webParameters = getParametersAsDictionary();
            // Run each query (usually, there is only one) and attach it to the report
            foreach (var ds in reportDef.DataSets)
            {
                //copy the parameters from the QueryString into the ReportParameters definitions (objects)
                ds.AssignParameters(getParametersAsDictionary());

                //attach the data/table to the Report's dataset(s), by name
                ReportDataSource rds = new ReportDataSource();
                rds.Name = ds.Name; //This refers to the dataset name in the RDLC file
                rds.Value = sp.getDataSourceForLocalSSRS(ds);

                localReportInstance.DataSources.Add(rds);
            }
            localReportInstance.Refresh();
        }
		
		private void ProcessSubReportDataSources(object sender, SubreportProcessingEventArgs e)
        {
            var subReportDef = CSGenio.reporting.serialization.Report.GetReportFromFile(Configuration.PathReports+ "\\" + e.ReportPath+".rdlc");

            var subReportParameters = e.Parameters.ToDictionary(p => p.Name, p => p.Values[0]);

            foreach (var ds in subReportDef.DataSets)
            {
                ds.AssignParameters(subReportParameters);

                ReportDataSource rds = new ReportDataSource();
                rds.Name = ds.Name;
                rds.Value = sp.getDataSourceForLocalSSRS(ds);

                e.DataSources.Add(rds);
                
            }


        }


        public void ConstructReport(User user,
            string areaBase,
            string[] historicFieldNames,
            string[] historicFieldValues,
            string[] globFields,
            string[] areaReports,
            ReportLimitParameter[] limitSelectionFields = null,
            string[] specialFormulasFields = null,
            List<ReportParameter> listReportParameters = null)
        {
            List<ReportParameterInfo> loadedReportParams = this.isServerReport ? this.ServerReportInstance.GetParameters().ToList() : this.localReportInstance.GetParameters().ToList();
            string[] reportNames = null;
            if (loadedReportParams != null && loadedReportParams.Count != 0)
                reportNames = loadedReportParams.Select(x => x.Name).ToArray();

            string module = user.CurrentModule;

            //parametro database, caso o relat�rio fa�a uso de datasources com connection string din�micas
            if (loadedReportParams.Any(prm => prm.Name == "Database"))
            {
                var ds = Configuration.ResolveDataSystem(user.Year, Configuration.DbTypes.NORMAL);
                reportParameters.Add(new ReportParameter("Database", new string[] { ds.Schemas[0].Schema }));
            }

            if (globFields != null)
                reportParameters.AddRange(getGlobFields(user, module, globFields).Where(x => loadedReportParams.Any(y => y.Name == x.Name)));

            if (specialFormulasFields != null)
                reportParameters.AddRange(getSpecialFormulas(reportNames, specialFormulasFields, user, module).Where(x => loadedReportParams.Any(y => y.Name == x.Name)));

            if (historicFieldValues != null)
                reportParameters.AddRange(getHistoricFields(historicFieldNames, historicFieldValues, areaReports, user, module, areaBase).Where(x => loadedReportParams.Any(y => y.Name == x.Name)));


            if(listReportParameters != null)
                reportParameters.AddRange(listReportParameters);

            if(areaBase != null)
                reportParameters.AddRange(getEphParameters(user, module, areaBase).Where(x => loadedReportParams.Any(y => y.Name == x.Name)));

            reportParameters.AddRange(getFillLimits(reportNames, limitSelectionFields).Where(x => loadedReportParams.Any(y => y.Name == x.Name)));

            if (!this.isServerReport)// Local report
            {
                //Using the method presented in the article: https://www.codeproject.com/Articles/607382/Running-a-RDL-RDLC-SQL-Report-in-ASP-NET-without-S
                ProcessDataSources();
				localReportInstance.SubreportProcessing += new SubreportProcessingEventHandler(ProcessSubReportDataSources);
            }

            if (this.isServerReport)
                this.ServerReportInstance.SetParameters(reportParameters);
            else
                this.LocalReportInstance.SetParameters(reportParameters);
        }


        public void ConstructReport(User user, string[] historicFieldNames, string[] historicFieldValues)
        {
            List<ReportParameter> result = new List<ReportParameter>();

            if (historicFieldValues.Length > 0)
            {
               
                for (int i = 0; historicFieldNames.Length > i; i++)
                    result.Add(new ReportParameter(historicFieldNames[i].ToString(), historicFieldValues[i].ToString()));
            }


            ConstructReport(user, null, null, null, null, null, null, null, result);
        }

        #region QWeb Functions Call
        public string GetReportInvokeUrl()
        {
            StringBuilder ReportUrl = new StringBuilder();
            ReportUrl.Append(GetReportServerUrl() + "?" + GetReportNamePath());
            ReportUrl.Append("&rc:Toolbar=True&rs:Command=Render&rc:Zoom=100&rc:Parameters=False"); //parametriza��es sobre o viewer 
            ReportUrl.Append(GetParamsUrl());
            return ReportUrl.ToString();
        }

        //retorna o endere�o do report server to o report viewer
        public string GetReportServerUrl()
        {
            return Configuration.SSRSServer.url;
        }

        //routine to devolver os parametros em forma de string to ser adicionada ao URL caso se prentenda chamar o report por URL.
        public string GetParamsUrl()
        {
            StringBuilder url = new StringBuilder();
            foreach (var param in reportParameters)
                url.Append("&" + param.Name + "=" + param.Values);
            return url.ToString();
        }

        //devolve o name do relatorio concatenado com a pasta em que se insere no reportserver caso esteja definida
        public string GetReportNamePath()
        {
            return this.isServerReport ? serverReportInstance.ReportPath : "/" + downloadFileName;
        }

        public Dictionary<string, List<string>> GetParamValues()
        {
            Dictionary<string, List<string>> valuesConv = null;
            try
            {
                valuesConv = reportParameters.ToDictionary(x => x.Name, y => y.Values.Cast<string>().ToList());
            }
            catch
            {
                valuesConv = new Dictionary<string, List<string>>();
            }

            return valuesConv;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the local report instance.
        /// </summary>
        /// <value>The report instance.</value>
        public LocalReport LocalReportInstance
        {
            get
            {
                return localReportInstance;
            }
        }
        /// <summary>
        /// Gets the server report instance.
        /// </summary>
        /// <value>The report instance.</value>
        public ServerReport ServerReportInstance
        {
            get
            {
                return serverReportInstance;
            }
        }
        #endregion

        /// <summary>
        /// Render the SSRS report
        /// </summary>
        /// <param name="exportType">PDF | EXCEL | WORDOPENXML | EXCELOPENXML | ...</param>
        /// <returns></returns>
        public ReportSSRS_Result Render(string exportType)
        {
            var result = new ReportSSRS_Result();
            result.FileName = downloadFileName;
            if (!this.isServerReport)
            {
#if NETFRAMEWORK
                PermissionSet permissionSet = new PermissionSet(PermissionState.None);
                permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
                /*
                * Starting from log4net version 2.0.10, where the 'Improper Restriction of XML External Entity Reference (XXE) (CWE ID 611)' vulnerability was fixed, 
                * local reports that initialize an AppDomain for sandboxing started to throw security errors because they internally use Serialization.
                */
                permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.SerializationFormatter));

                /*
                * https://learn.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2010/ee344968(v=vs.100)
                * The default base permission is Execution.
                * If any additional error appears, we can evaluate whether we can use: AppDomain.CurrentDomain.PermissionSet.Copy()
                */
                localReportInstance.SetBasePermissionsForSandboxAppDomain(permissionSet);
#endif
                result.File = localReportInstance.Render(exportType, null, out result.MimeType, out result.Encoding, out result.FileNameExtension, out result.Streams, out result.Warnings);
            }
            else
                result.File = serverReportInstance.Render(exportType, null, out result.MimeType, out result.Encoding, out result.FileNameExtension, out result.Streams, out result.Warnings);
            return result;
        }

        /// <summary>
        /// M�todo to preencher as formulas especiais
        /// </summary>
        /// <param name="paramReportNames">Nome dos reports</param>
        /// <param name="specialFormulasFields">Formulas especiais</param>
        /// <param name="utilizador">user em sess�o</param>
        /// <param name="modulo">module</param>
        /// <returns></returns>
        private IEnumerable<ReportParameter> getSpecialFormulas(string[] paramReportNames, string[] specialFormulasFields, User user, string module)
        {
            List<ReportParameter> result = new List<ReportParameter>();

            try
            {
                if (paramReportNames != null && specialFormulasFields != null)
                {
                    foreach (string specialFormula in specialFormulasFields)
                    {
                        //MF - 2014-04-02 Reformulei o tratamento das formulas. Estava uma salganhada com ifs
                        string key = string.Concat("f_", specialFormula), val = string.Empty;  // Os nomes dos parametros das formulas devem come�ar com "f_"

                        if (paramReportNames.Any(prm => prm == key)) // Validate se o report utiliza o parametro
                        {
                            switch (specialFormula.ToLower())
                            {
                                case "ano": val = user.Year.ToString(); break;
                                case "sigla": val = Configuration.Acronym; break;
                                case "original": val = "ORIGINAL"; break;
                                case "duplicado": val = "DUPLICADO"; break;
                                case "triplicado": val = "TRIPLICADO"; break;
                                case "quadruplicado": val = "QUADRUPLICADO"; break;
                                case "user": val = user.Name; break;
                                case "moeda": val = Configuration.Currency; break;
                                //MF 02-04-2014 - Em reporting services, passa a ser obrigat�rio definir a formula "pastabd" 
                                //na propriedade "Field" do menu Qlisting no genio
                                case "pastabd":
                                    string tipoSGBD;
                                    var ds = Configuration.ResolveDataSystem(user.Year, Configuration.DbTypes.NORMAL);
                                    if (ds.GetDatabaseType() == DatabaseType.ORACLE) tipoSGBD = "O-";
                                    else tipoSGBD = "S-";
                                    val = tipoSGBD + module + "-" + ds.Schemas[0].Schema + "-" + Configuration.Version.ToString().Replace(',', '.');
                                    break;
                                //RMR(2019-12-10) - Option to include the current language selected by the user, into the report
                                case "language":
                                    val = user.Language;
                                    break;
                                case "module":
                                    val = user.CurrentModule;
                                    break;
                                default: key = null; break;
                            }

                            //so adiciono o par�metro, se for uma formula conhecida/implementada
                            if (!string.IsNullOrEmpty(key))
                                result.Add(new ReportParameter(key, val));
                        }
                    }
                }
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "ReportSSRS.getSpecialFormulas", "Error filling special formulas: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "ReportSSRS.getSpecialFormulas", "Error filling special formulas: " + ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// M�todo to preencher as limita��es entre datas
        /// </summary>
        /// <param name="utilizador">user em sess�o</param>
        /// <param name="modulo">module</param>
        private IEnumerable<ReportParameter> getFillLimits(string[] paramReportNames, ReportLimitParameter[] limitSelectionFields)
        {
            List<ReportParameter> result = new List<ReportParameter>();

            if (limitSelectionFields == null)
                return result;

            foreach (var genLimit in limitSelectionFields)
            {
                switch (genLimit.Source)
                {
                    case ReportLimitParameter.LimitSource.SE:
                        {
                            var limit = genLimit as ReportLimitParameter_SE;
                            switch (limit.FieldType)
                            {
                                case "D":
                                    SetDateLimit(string.Concat(limit.FullFieldName.Replace(".", "_"), "MIN"), limit.MinFieldValue, paramReportNames, ref result);
                                    SetDateLimit(string.Concat(limit.FullFieldName.Replace(".", "_"), "MAX"), limit.MaxFieldValue, paramReportNames, ref result);
                                    break;
                                default:
                                    SetLimitValue(string.Concat(limit.FullFieldName.Replace(".", "_"), "MIN"), Convert.ToString(limit.MinFieldValue), paramReportNames, ref result);
                                    SetLimitValue(string.Concat(limit.FullFieldName.Replace(".", "_"), "MAX"), Convert.ToString(limit.MaxFieldValue), paramReportNames, ref result);
                                    break;
                            }
                        }
                        break;
                    case ReportLimitParameter.LimitSource.SU:
                        {
                            var limit = genLimit as ReportLimitParameter_SU;
                            switch (limit.FieldType)
                            {
                                case "D":
                                    SetDateLimit(limit.FullFieldName, limit.FieldValue, paramReportNames, ref result);
                                    break;
                                default:
                                    SetLimitValue(limit.FullFieldName, Convert.ToString(limit.FieldValue), paramReportNames, ref result);
                                    break;
                            }
                        }
                        break;
                    case ReportLimitParameter.LimitSource.DB:
                        {
                            // DBEdit com limita��o em arvore
                            var limit = genLimit as ReportLimitParameter_DB;
                            if (string.IsNullOrEmpty(limit.FieldValue))
                                throw new BusinessException(null, "ReportSSRS.getFillLimits", "Null or Empty tree seelction limit value");
                            SetLimitValue(limit.FullFieldName, Convert.ToString(limit.FieldValue), paramReportNames, ref result);
                        }
                        break;

                    case ReportLimitParameter.LimitSource.DM:
                        {
                            //Multiple selections
                            var limit = genLimit as ReportLimitParameter_DM;
                            SetLimitValues(limit.FullFieldName, limit.FieldValue, paramReportNames, ref result);
                        }
                        break;
                    case ReportLimitParameter.LimitSource.AC:
                        {
                            //AC - Array choice Conditions
                            var limit = genLimit as ReportLimitParameter_AC;
                            if (string.IsNullOrEmpty(limit.FieldValue))
                                throw new BusinessException(null, "ReportSSRS.getFillLimits", "Null or Empty area condition limit value");
                            SetLimitValue(limit.FullFieldName, Convert.ToString(limit.FieldValue), paramReportNames, ref result);
                        }
                        break;
                }
            }

            return result;
        }

        #region Auxiliar Methods to the getFillLimits functions
        private void SetLimitValue(string FieldName, string FieldValue, string[] paramReportNames, ref List<ReportParameter> output)
        {
            if (string.IsNullOrEmpty(FieldName) || string.IsNullOrEmpty(FieldValue))
                throw new BusinessException(null, "ReportSSRS.SetLimitValue", "Null or Empty argument value");
            var finalFullFieldName = FieldName.Replace('.', '_');
            if (paramReportNames.Any(x => x == finalFullFieldName))
            {
                output.Add(new ReportParameter(finalFullFieldName, FieldValue));
            }
        }

        private ReportParameter BuildMultipleValueParameter(string parameterName, IEnumerable<string> values) {
            //The values are concatenated so that it's easier to use in the report with STRING_SPLIT
            var valueStr = string.Join(",", values);
            return new ReportParameter(parameterName, valueStr);
        }

        private void SetLimitValues(string FieldName, string[] FieldValue, string[] paramReportNames, ref List<ReportParameter> output)
        {
			if (FieldValue == null)
				return;
			
            if (string.IsNullOrEmpty(FieldName) || FieldValue.Length == 0)
                throw new BusinessException(null, "ReportSSRS.SetLimitValue", "Null or Empty argument value");
            var finalFullFieldName = FieldName.Replace('.', '_');
            if (paramReportNames.Any(x => x == finalFullFieldName))
            {
                var reportParam = BuildMultipleValueParameter(finalFullFieldName, FieldValue);
                output.Add(reportParam);
            }
        }

        private void SetDateLimit(string FieldName, object FieldValue, string[] paramReportNames, ref List<ReportParameter> output)
        {
            if (string.IsNullOrEmpty(FieldName) || FieldValue == null)
                throw new BusinessException(null, "ReportSSRS.SetDateLimit", "Null or Empty argument value");
            if (FieldValue is string)
            {
                DateTime tempDate;
                if (!DateTime.TryParse(FieldValue as string, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out tempDate))
                    throw new BusinessException(null, "ReportSSRS.SetDateLimit", "Error parsing date limit: " + Convert.ToString(FieldValue));
                FieldValue = tempDate;
            }
            string dateValue = (FieldValue as DateTime?).GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
            SetLimitValue(FieldName, dateValue, paramReportNames, ref output);
        }
        #endregion

        /// <summary>
        /// Gets EPH parameters for report generation based on user permissions and area configuration.
        /// Groups EPHs by parameter key and consolidates unique values to avoid duplicates.
        /// </summary>
        /// <param name="user">User object containing EPH permissions</param>
        /// <param name="module">Module identifier</param>
        /// <param name="area">Area identifier</param>
        /// <returns>Collection of report parameters with unique values</returns>
        private IEnumerable<ReportParameter> getEphParameters(User user, string module, string area)
        {
            List<ReportParameter> result = new List<ReportParameter>();

            // Get EPHs linked to the user
            Area areaBase = Area.createArea(area, user, module);
            List<EPHOfArea> ephsDaArea = areaBase.CalculateAreaEphs(user.Ephs, null, false);

            // Group by parameter key and consolidate unique values
            var groupedEphs = ephsDaArea
                .GroupBy(v => string.Join("_", new string[] { v.Eph.Name, v.Eph.Table, v.Eph.Field }))
                .Select(group => new {
                    ParamKey = group.Key,
                    UniqueValues = group.SelectMany(v => v.ValuesList).Distinct()
                });

            // Create report parameters for each group, with possibly multiple values
            return groupedEphs.Select(item => BuildMultipleValueParameter(item.ParamKey, item.UniqueValues));
        }

        /// <summary>
        /// M�todo to preencher as condi��es de historial
        /// </summary>
        private IEnumerable<ReportParameter> getHistoricFields(string[] historicFieldNames, string[] historicFieldValues, string[] areasReport, User user, string module, string area)
        {
            List<ReportParameter> result = new List<ReportParameter>();

            AreaInfo areaBase = Area.GetInfoArea(area);

            for (int i = 0; i < historicFieldNames.Length; i++)
            {
                //FS(2008-11-03) Ignorar Qvalues de historial que venham a null (em principio n�o deveria acontecer, caso entre aqui investigar raz�es)
                if (historicFieldValues[i] == null)
                    continue;

                string ParamName = "";

                //podemos incializar o tpField com um tipo qualquer pois vamos sempre actualizar a vari�vel quando encontrarmos o Qfield
                FieldType tpField = FieldType.KEY_VARCHAR;
                //to cada name tem que se verificar se vem s� o name da area ou se vem o name completo
                //se vier s� o name da area tem que se descobrir a rela��o com a area base
                if (!historicFieldNames[i].Contains("."))//s� vem o name da area pelo que assumimos que � uma key prim�ria
                {
                    if (area == historicFieldNames[i])//AV(2011/03/29) o hist�rico � da �rea base do rpt
                    {
                        tpField = areaBase.DBFields[areaBase.PrimaryKeyName].FieldType;
                        ParamName = areaBase.Alias + "_" + areaBase.PrimaryKeyName;
                    }
                    else
                    {
                        //AV(2011/03/29) o hist�rico � de uma table directamente acima da �rea base do rpt
                        if (areaBase.ParentTables.ContainsKey(historicFieldNames[i]))
                        {
                            Relation relacao = (Relation)areaBase.ParentTables[historicFieldNames[i]];
                            tpField = areaBase.DBFields[relacao.SourceRelField].FieldType;
                            ParamName = relacao.AliasSourceTab + "_" + relacao.SourceRelField;
                        }
                        else
                        {
                            foreach (string areaReport in areasReport)
                            {
                                AreaInfo areaRpt = Area.GetInfoArea(areaReport);
                                //AV(2011/03/29) o hist�rico � de uma das �reas do rpt identificada nas defini��es
                                if (areaReport == historicFieldNames[i])
                                {
                                    tpField = areaRpt.DBFields[areaRpt.PrimaryKeyName].FieldType;
                                    ParamName = areaRpt.Alias + "_" + areaRpt.PrimaryKeyName;
                                    break;
                                }
                                else
                                {
                                    //AV(2011/03/29) o hist�rico � de uma table directamente acima das �reas do rpt identificada nas defini��es
                                    if (areaRpt.ParentTables.ContainsKey(historicFieldNames[i]))
                                    {
                                        Relation relacao = (Relation)areaRpt.ParentTables[historicFieldNames[i]];
                                        tpField = areaRpt.DBFields[relacao.SourceRelField].FieldType;
                                        ParamName = relacao.AliasSourceTab + "_" + relacao.SourceRelField;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //vem o name da area e do Qfield (table.Qfield)
                    string[] campoCompleto = historicFieldNames[i].Split('.');
                    //AV(2011/03/29) o hist�rico � dum Qfield da �rea base do rpt
                    if (area == campoCompleto[0])
                    {
                        tpField = areaBase.DBFields[campoCompleto[1]].FieldType;
                        ParamName = historicFieldNames[i].Replace(".", "_");
                    }
                    else
                    {
                        foreach (string areaReport in areasReport)
                        {
                            AreaInfo areaRpt = Area.GetInfoArea(areaReport);
                            //AV(2011/03/29) o hist�rico � dum Qfield de uma das �reas do rpt identificada nas defini��es
                            if (areaReport == campoCompleto[0])
                            {
                                tpField = areaRpt.DBFields[campoCompleto[1]].FieldType;
                                ParamName = historicFieldNames[i].Replace(".", "_");  //em reporting services, os par�metros nao podem conter . no name
                                break;
                            }
                        }
                    }
                }
                //preenche o Qvalue
                if (!string.IsNullOrEmpty(ParamName))    //to os casos em que o Qvalue de historial pertence a uma �rea diferente da �rea base, e que o user se "esqueceu" de incluir nas areas do report
                    result.Add(new ReportParameter(ParamName, historicFieldValues[i].ToString()));
            }

            return result;
        }

        /// <summary>
        /// M�todo to preencher os fields da table glob
        /// </summary>
        /// <param name="utilizador">user em sess�o</param>
        /// <param name="modulo">module</param>
        private IEnumerable<ReportParameter> getGlobFields(User user, string module, string[] camposGlob)
        {
            List<ReportParameter> result = new List<ReportParameter>();

            PersistentSupport sp = null;
            try
            {
                sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
                sp.openConnection();

                //string fieldValue = "";
                foreach (string campoGlob in camposGlob)
                {
                    //SO 2008.08.25 verificar se o Qfield exists na table GLOB
                    Area glob = Area.createArea("glob", user, module);
                    string nomeCampoSemGlob = campoGlob.Split('.')[1];
                    if (!glob.DBFields.ContainsKey(nomeCampoSemGlob))
                        throw new BusinessException(null, "ReportSSRS.getGlobFields", "The field " + campoGlob + " is not in glob table.");
                    Field campoBD = glob.DBFields[nomeCampoSemGlob];

                    SelectQuery qs = new SelectQuery()
                        .Select("glob", nomeCampoSemGlob)
                        .From(Configuration.Program + "glob", "glob");

                    result.Add(new ReportParameter("g_" + nomeCampoSemGlob, new List<string>() { Convert.ToString(sp.ExecuteScalar(qs)) }.ToArray()));
                }
            }
            catch (GenioException ex)
            {
                if (ex.ExceptionSite == "Reports.getGlobFields")
                    throw;
                throw new BusinessException(ex.UserMessage, "ReportSSRS.getGlobFields", "Error filling glob fields: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "ReportSSRS.getGlobFields", "Error filling glob fields: " + ex.Message, ex);
            }
            finally
            {
                if (sp != null)
                    sp.closeConnection();
            }

            return result;
        }

        /// <summary>
        /// Modifies the credentials of the SSRS report according to our configurations
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="domain">Domain</param>
        public void SetServerCredentials(string username, string password, string domain)
        {
#if NETFRAMEWORK
            ServerReportInstance.ReportServerCredentials = new ReportServerCredentials(username, password, domain);
#else
            ServerReportInstance.ReportServerCredentials.NetworkCredentials = new System.Net.NetworkCredential(username, password, domain);
#endif
        }


        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.localReportInstance != null) this.localReportInstance.Dispose();
        }



        #endregion
    }
}

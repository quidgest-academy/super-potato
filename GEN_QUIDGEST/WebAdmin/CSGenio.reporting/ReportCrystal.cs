#if NETFRAMEWORK
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Globalization;
using System.Linq;
using Area = CSGenio.business.Area;

namespace CSGenio.reporting
{
    /// <summary>
    /// ReportCrystal
    /// </summary>
    public class ReportCrystal
    {
        private string nomeReport;//nomeReport
        private string area; //area do report
        private string tipo; // tipo de report
        private Dictionary<string, string> listaCamposArray;//lista de fields que sao array
        private string[] camposGlob; //lista de camposGlob
        private string[] formulasEspeciais;//fields que são formulas especiais
        private string campoEntreDatas;//Qfield entre datas  
        private string[] limites;//respectivosLimites
        /// <summary>
        /// Limitations of the Report
        /// </summary>
        private List<ReportLimitParameter> limitations;
        private string[] nomesCamposHistorial;//nomes fields de historial
        private object[] valoresCamposHistorial;//Qvalues fields de historial
        private string recordSelectionFormula;//formula record selection
        private ReportDocument reportDocument;//report do crystal
        private bool temSubReports;//se tem subreports
        private List<string> camposDataDefinition;//camposDataDefinition
        private List<string>[] camposDataDefinitionSubreports;//fields data definition no subreport
        private string[] areasReport;
		private bool subReportRecord; //coloca a recordSelectionFormula dos subreports igual a do report
        private Dictionary<string, string> listaCamposEspeciais; //Lista de fields especiais f_xxxxx

        /// Construtor da classe, to reports com fields especiais
        /// </summary>
        /// <param name="nomeReport">name do report</param>
        /// <param name="area">area base do report</param>
        /// <param name="tipo">tipo de formatação do report</param>
        /// <param name="campoEntreDatas">Qfield entre datas</param>
        /// <param name="limites">limites</param>
        /// <param name="nomesCamposHistorial">nomes dos fields que sao limitações de historial</param>
        /// <param name="valoresCamposHistorial">Qvalues dos fields que sao limitacoes de historial</param>
        /// <param name="listaCamposArray">lista de fields que sao arrays</param>
        /// <param name="camposGlob">fields da table glob</param>
        /// <param name="formulasEspeciais">fields que sao formulas especiais</param>
        /// <param name="recordSelectionFormula">record selection formula</param>
        /// <param name="listaCamposEspeciais">fields especiais existentes no report</param>

        public ReportCrystal(string nomeReport, string area, string tipo, string campoEntreDatas, string[] limites, string[] nomesCamposHistorial, object[] valoresCamposHistorial, Dictionary<string, string> listaCamposArray, string[] camposGlob, string[] formulasEspeciais, string recordSelectionFormula, string[] areasReport, bool subReportRecord, Dictionary<string, string> listaCamposEspeciais)
        {
            this.area = area;
            this.tipo = tipo;
            this.listaCamposArray = listaCamposArray;
            this.camposGlob = camposGlob;
            this.formulasEspeciais = formulasEspeciais;
            this.recordSelectionFormula = recordSelectionFormula;
            this.campoEntreDatas = campoEntreDatas;
            this.limites = limites;
            this.nomesCamposHistorial = nomesCamposHistorial;
            this.valoresCamposHistorial = valoresCamposHistorial;
            this.nomeReport = nomeReport;
            this.areasReport = areasReport;
            reportDocument = new ReportDocument();
            camposDataDefinition = new List<string>();
            this.subReportRecord = subReportRecord;
            this.listaCamposEspeciais = listaCamposEspeciais;
        }

        /// <summary>
        /// Construtor da classe
        /// </summary>
        /// <param name="nomeReport">name do report</param>
        /// <param name="area">area base do report</param>
        /// <param name="tipo">tipo de formatação do report</param>
        /// <param name="campoEntreDatas">Qfield entre datas</param>
        /// <param name="limites">limites</param>
        /// <param name="nomesCamposHistorial">nomes dos fields que sao limitações de historial</param>
        /// <param name="valoresCamposHistorial">Qvalues dos fields que sao limitacoes de historial</param>
        /// <param name="listaCamposArray">lista de fields que sao arrays</param>
        /// <param name="camposGlob">fields da table glob</param>
        /// <param name="formulasEspeciais">fields que sao formulas especiais</param>
        /// <param name="recordSelectionFormula">record selection formula</param>
        public ReportCrystal(string nomeReport, string area, string tipo, string campoEntreDatas, string[] limites, string[] nomesCamposHistorial, object[] valoresCamposHistorial, Dictionary<string, string> listaCamposArray, string[] camposGlob, string[] formulasEspeciais, string recordSelectionFormula, string[] areasReport, bool subReportRecord)
        {
            this.area = area;
            this.tipo = tipo;
            this.listaCamposArray = listaCamposArray;
            this.camposGlob = camposGlob;
            this.formulasEspeciais = formulasEspeciais;
            this.recordSelectionFormula = recordSelectionFormula;
            this.campoEntreDatas = campoEntreDatas;
            this.limites = limites;
            this.nomesCamposHistorial = nomesCamposHistorial;
            this.valoresCamposHistorial = valoresCamposHistorial;
            this.nomeReport = nomeReport;
            this.areasReport = areasReport;
            reportDocument = new ReportDocument();
            camposDataDefinition = new List<string>();
			this.subReportRecord = subReportRecord;
        }

        /// <summary>
        /// Construtor da classe
        /// </summary>
        /// <param name="nomeReport">name do report</param>
        /// <param name="area">area base do report</param>
        /// <param name="tipo">tipo de formatação do report</param>
        public ReportCrystal(string nomeReport, string area, string tipo)
        {
            this.area = area;
            this.tipo = tipo;
            this.nomeReport = nomeReport;
        }

        /// <summary>
        /// Add a new array formula to the crystal report.
        /// </summary>
        /// <param name="field">The field of which to add a formula of</param>
        /// <example>
        /// report.AddArrayField(CSGenioAfactu.FldTipo);
        /// </example>
        public void AddArrayField(Quidgest.Persistence.FieldRef fieldRef)
        {
            //we need the complete field metainfo to fetch the associated array
            Field field = Area.GetFieldInfo(fieldRef);

            // Convert array name
            string arrayPrefix = "";
            if (field.FieldType == FieldType.ARRAY_NUMERIC)
                arrayPrefix = "dbo.GetValArrayN";
            else if (field.FieldType == FieldType.ARRAY_TEXT)
                arrayPrefix = "dbo.GetValArrayC";
            else if (field.FieldType == FieldType.ARRAY_LOGIC)
                arrayPrefix = "dbo.GetValArrayL";
            else
                return; //TODO: should we throw exception here?

            string arrayName = field.ArrayName.Replace(arrayPrefix, "");

            if (listaCamposArray == null)
                listaCamposArray = new Dictionary<string, string>();

            listaCamposArray.Add(fieldRef.FullName, arrayName);
        }

        /// <summary>
        /// Adds a new custom formula to the report.
        /// The value will be formatted correctly according to its type
        /// </summary>
        /// <param name="formula">The formula name to add</param>
        /// <param name="value">The value to add</param>
        /// <example>
        /// report.AddValueFormula("g_minister", glob.ValMinister);
        /// </example>
        public void AddValueFormula(string formula, object value)
        {
            if (listaCamposEspeciais == null)
                listaCamposEspeciais = new Dictionary<string, string>();

            FieldFormatting format = FieldFormatting.CARACTERES;
            if (value is DateTime)
                format = FieldFormatting.DATA;
            else if (value is double || value is decimal)
                format = FieldFormatting.FLOAT;
            else if (value is int)
                format = FieldFormatting.INTEIRO;
            else if (value is Guid)
                format = FieldFormatting.GUID;

            string crystalVal = CrystalConversion.FromInternal(value, format);

            listaCamposEspeciais.Add(formula, crystalVal);
        }

		
        /// <summary>
        /// Método que faz a configuração do report a nível da base de dados - to todas as tables.   
        /// </summary>
        /// <param name="connectionInfo">Objecto que possui as credenciais to ligar a base de dados</param>
        /// <param name="reportDocument">Objecto que representa o report a ser configurado</param>
        private void SetDBLogonForReport(ConnectionInfo connectionInfo, ReportDocument reportDocument)
        {
            Tables tables = reportDocument.Database.Tables;
            foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = connectionInfo;
                table.ApplyLogOnInfo(tableLogonInfo);
            }
        }

        /// <summary>
        /// Método to obter a connection info necessária to o report abrir
        /// </summary>
        /// <param name="utilizador">user em sessão</param>
        /// <returns>a connection info necessária to abrir o report</returns>
        private ConnectionInfo getConnectionInfo(User user)
        {
            //informação da connection
            ConnectionInfo connectionInfo = new ConnectionInfo();
            var ds = Configuration.ResolveDataSystem(user.Year, Configuration.DbTypes.NORMAL);

            if (ds.GetDatabaseType() == DatabaseType.ORACLE)
                connectionInfo.ServerName = ds.TnsName;
            else
            {
                //AV(2010/10/14) quando o porto é especificado tem que ser enviado to o rpt
                if (!string.IsNullOrEmpty(ds.Port))
                    connectionInfo.ServerName = ds.Server + "," + ds.Port;
                else
                    connectionInfo.ServerName = ds.Server;
            }

            // JMT - 05-03-2012 - em Oracle o DatabaseName não pode ser preenchido, porque o report pede login e não vai ser executado
            if (ds.GetDatabaseType() != DatabaseType.ORACLE)
                connectionInfo.DatabaseName = ds.Schemas[0].Schema;
            connectionInfo.UserID = ds.LoginDecode();
            connectionInfo.Password = ds.PasswordDecode();
            return connectionInfo;
        }

        /// <summary>
        /// Método que constroi o report
        /// </summary>
        /// <param name="utilizador">user em sessão</param>
        /// <param name="modulo">module</param>
        /// <param name="sp">PersistentSupport</param>
        public void buildReportDocument(User user, string module, PersistentSupport sp) //To ser possível a passagem do sp a utilizar.
        {
            ConnectionInfo connectionInfo = getConnectionInfo(user);
            string reportPath = Configuration.PathReports + "\\" + nomeReport;

            reportDocument.Load(reportPath, OpenReportMethod.OpenReportByTempCopy);

            DelimitarRecordSelection();

            //carregar os fields data definition do report
            IEnumerator camposDataDefinitionEnum = reportDocument.DataDefinition.FormulaFields.GetEnumerator();
            while (camposDataDefinitionEnum.MoveNext())
                camposDataDefinition.Add(((FormulaFieldDefinition)camposDataDefinitionEnum.Current).Name.ToLower());

            //se tem subreports
            if (reportDocument.Subreports != null)
            {
                int nrSubreports = reportDocument.Subreports.Count;
                this.temSubReports = true;
                camposDataDefinitionSubreports = new List<string>[reportDocument.Subreports.Count];
                for (int r = 0; r < nrSubreports; r++)
                {
                    camposDataDefinitionSubreports[r] = new List<string>();
                    //carregar os fields data definition dos subreports
                    IEnumerator subReportDataDefinitionEnum = reportDocument.Subreports[r].DataDefinition.FormulaFields.GetEnumerator();
                    while (subReportDataDefinitionEnum.MoveNext())
                        camposDataDefinitionSubreports[r].Add(((FormulaFieldDefinition)subReportDataDefinitionEnum.Current).Name.ToLower());
                }
            }
            else
                this.temSubReports = true;
            preencherCamposArray(user, module, sp);
            preencherCamposGlob(user, module, sp);
            preencherFormulasEspeciais(user, module, sp);
            preencherCamposHistorial(user, module);
            preencherEPHs(user, module, sp);
            preencherEntreDatas(user, module);
            PreencherLimites();
            adicionaCondicaoRecordSelectionFormula(recordSelectionFormula);
            preencherCamposEspeciais(user, module);
            
            SetDBLogonForReport(connectionInfo, reportDocument);
        }

        /// <summary>
        /// Método que constroi o report
        /// </summary>
        /// <param name="utilizador">user em sessão</param>
        /// <param name="modulo">module</param>
        public void buildReportDocument(User user, string module) 
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
            sp.openConnection();
            buildReportDocument(user, module, sp);
            sp.closeConnection();
        }

		/// <summary>
        /// Método to preencher os fields especiais que o report contenha.
        /// </summary>
        /// <param name="utilizador">user em sessão</param>
        /// <param name="modulo">module</param>
        ///CHN - 13-02-2017 - Possível com esta função passar qualquer Qfield que o report precise (ex. f_colunaXX, to o preenchimento de uma coluna dinâmicamente)
		private void preencherCamposEspeciais(User user, string module)
        {
            try
            {
				if(listaCamposEspeciais != null)
                foreach (KeyValuePair<string, string> campoEspecial in listaCamposEspeciais)
                {
                    if (camposDataDefinition.Contains((campoEspecial.Key).ToLower()))
                        replaceFormulaValues(campoEspecial.Key, campoEspecial.Value);
                    if (temSubReports)
                    {
                        for (int r = 0; r < reportDocument.Subreports.Count; r++)
                        {
                            if (camposDataDefinitionSubreports[r].Contains((campoEspecial.Key).ToLower()))
                                replaceFormulaValues(campoEspecial.Key, campoEspecial.Value);
                        }
                    }
                }
            }
            catch (GenioException ex)
			{
				throw new BusinessException(ex.UserMessage, "ReportCrystal.preencherCamposEspeciais", "Error filling special fields: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new BusinessException(null, "ReportCrystal.preencherCamposEspeciais", "Error filling special fields: " + ex.Message, ex);
            }
        }
		
        /// <summary>
        /// Método to preencher os fields da table glob
        /// </summary>
        /// <param name="utilizador">user em sessão</param>
        /// <param name="modulo">module</param>
        private void preencherCamposGlob(User user, string module, PersistentSupport sp)
        {
            try
            {
				//sp.openConnection(); 
                string fieldValue = "";
                foreach (string campoGlob in camposGlob)
                {
                    //SO 2008.08.25 verificar se o Qfield exists na table GLOB
                    Area glob = Area.createArea("glob", user, module);
                    string nomeCampoSemGlob = campoGlob.Split('.')[1];
                    if (!glob.DBFields.ContainsKey(nomeCampoSemGlob))
						throw new BusinessException(null, "ReportCrystal.preencherCamposGlob", "The field " + campoGlob + " is not in glob table.");
                    Field campoBD = glob.DBFields[nomeCampoSemGlob];

                    SelectQuery qs = new SelectQuery()
                        .Select("glob", nomeCampoSemGlob)
                        .From(Configuration.Program + "glob", "glob");

                    object valorCampoObj = sp.ExecuteScalar(qs);
                    fieldValue = CrystalConversion.FromInternal(valorCampoObj, campoBD.FieldFormat);

                    if (camposDataDefinition.Contains(("g_" + nomeCampoSemGlob).ToLower()))
                        reportDocument.DataDefinition.FormulaFields["g_" + nomeCampoSemGlob].Text = "'" + fieldValue + "'";
                    if (temSubReports)
                    {
                        for (int r = 0; r < reportDocument.Subreports.Count; r++)
                        {
                            if (camposDataDefinitionSubreports[r].Contains(("g_" + nomeCampoSemGlob).ToLower()))
                                reportDocument.Subreports[r].DataDefinition.FormulaFields["g_" + nomeCampoSemGlob].Text = "'" + fieldValue + "'";
                        }
                    }
                }
            }
            catch (GenioException ex)
			{
				if (ex.ExceptionSite == "ReportCrystal.preencherCamposGlob")
					throw;
				throw new BusinessException(ex.UserMessage, "ReportCrystal.preencherCamposGlob", "Error filling glob fields: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
				throw new BusinessException(null, "ReportCrystal.preencherCamposGlob", "Error filling glob fields: " + ex.Message, ex);
            }
            //finally
            //{
            //    if (sp != null)
            //        sp.closeConnection();
            //}
        }

        // faz o replace de uma formula pelo Qvalue no report e nos sub-reports
        private void replaceFormulaValues(string key, string value)
        {
            // TODO
            // Avaliar se vale a pena utilizar o método sugerido neste link to fazer os replaces
            // http://devtoolshed.com/content/crystal-reports-and-aspnet
			// assim podia-se utilizar esta função em todas as funções desta classe de forma genérica
			// não alterei ainda em todos os sítios, porque é feito ToLower dos identificadores e ainda
			// pode estragar alguma fórmula
            if (camposDataDefinition.Contains(key))
                reportDocument.DataDefinition.FormulaFields[key].Text = value;
            if (temSubReports)
            {
                for (int r = 0; r < reportDocument.Subreports.Count; r++)
                    if (camposDataDefinitionSubreports[r].Contains(key))
                        reportDocument.Subreports[r].DataDefinition.FormulaFields[key].Text = value;
            }
        }

        /// <summary>
        /// Método to preencher as formulas especiais
        /// </summary>
        /// <param name="utilizador">user em sessão</param>
        /// <param name="modulo">module</param>
        private void preencherFormulasEspeciais(User user, string module, PersistentSupport sp)
        {
            try
            {
                //sp.openConnection();
                foreach (string formulaEspecial in formulasEspeciais)
                {
                    //adicionar o f_ano
                    if (formulaEspecial == "ano" && camposDataDefinition.Contains("f_ano"))
                        replaceFormulaValues("f_ano", user.Year.ToString());
                    else if (formulaEspecial == "sigla" && camposDataDefinition.Contains("f_sigla"))
                        replaceFormulaValues("f_sigla", "\"" + Configuration.Acronym + "\"");
                    else if (formulaEspecial == "original" && camposDataDefinition.Contains("f_original"))
                        replaceFormulaValues("f_original", "\"ORIGINAL\""); 
                    else if (formulaEspecial == "duplicado" && camposDataDefinition.Contains("f_original"))
                        replaceFormulaValues("f_original", "\"DUPLICADO\"");
                    else if (formulaEspecial == "triplicado" && camposDataDefinition.Contains("f_original"))
                        replaceFormulaValues("f_original", "\"TRIPLICADO\"");
                    else if (formulaEspecial == "quadruplicado" && camposDataDefinition.Contains("f_original"))
                        replaceFormulaValues("f_original", "\"QUADRUPLICADO\"");						
                    else if (formulaEspecial == "user" && camposDataDefinition.Contains("f_user"))
                        replaceFormulaValues("f_user", "\"" + user.Name + "\"");
                    else if (formulaEspecial == "moeda" && camposDataDefinition.Contains("f_moeda"))
                        replaceFormulaValues("f_moeda", "\"" + Configuration.Currency + "\"");
                }
                //JG-2009-04-16
                if (camposDataDefinition.Contains("f_pastabd"))
                {
                    string tipoSGBD = "";
                    var ds = Configuration.ResolveDataSystem(user.Year, Configuration.DbTypes.NORMAL);
                    
                    if (ds.GetDatabaseType() == DatabaseType.ORACLE)
                        tipoSGBD = "O-";
                    else
                        tipoSGBD = "S-";

                    string f_pastabd = "\"" + tipoSGBD + module + "-" + ds.Schemas[0].Schema + "-" + Configuration.Version.ToString().Replace(',', '.') + "\"";
                    replaceFormulaValues("f_pastabd", f_pastabd);
                }
            }
            catch (GenioException ex)
			{
				throw new BusinessException(ex.UserMessage, "ReportCrystal.preencherFormulasEspeciais", "Error filling special formulas: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
				throw new BusinessException(null, "ReportCrystal.preencherFormulasEspeciais", "Error filling special formulas: " + ex.Message, ex);
            }
            //finally
            //{
            //    if (sp != null)
            //        sp.closeConnection();
            //}
        }

        public void SetReportLimits(List<ReportLimitParameter> limits)
        {
            this.limitations = limits;
        }

        /// <summary>
        /// Método to preencher as limitações
        /// </summary>
        private void PreencherLimites()
        {
            if (this.limitations == null)
                return;
            foreach (var genLimit in this.limitations)
            {
                switch (genLimit.Source) {
                    case ReportLimitParameter.LimitSource.SE:
                        {
                            var limit = genLimit as ReportLimitParameter_SE;
                            switch (limit.FieldType)
                            {
                                case "D":
                                    SetDateLimit(limit.MinFieldName, limit.MinFieldValue);
                                    SetDateLimit(limit.MaxFieldName, limit.MaxFieldValue);
                                    break;
                                case "C":
                                    SetStringLimit(limit.MinFieldName, limit.MinFieldValue);
                                    SetStringLimit(limit.MaxFieldName, limit.MaxFieldValue);
                                    break;
                                default:
                                    SetLimitValue(limit.MinFieldName, Convert.ToString(limit.MinFieldValue));
                                    SetLimitValue(limit.MaxFieldName, Convert.ToString(limit.MaxFieldValue));
                                    break;
                            }
                            var tempFieldName = Configuration.Program + limit.FullFieldName;
                            adicionaCondicaoRecordSelectionFormula("{" + tempFieldName + "} >= {@" + limit.MinFieldName + "} AND {" + tempFieldName + "} <= {@" + limit.MaxFieldName + "}");
                        }
                        break;
                    case ReportLimitParameter.LimitSource.SU:
                        {
                            var limit = genLimit as ReportLimitParameter_SU;
                            switch (limit.FieldType)
                            {
                                case "D":
                                    SetDateLimit(limit.FieldName, limit.FieldValue);
                                    break;
                                case "C":
                                    SetStringLimit(limit.FieldName, limit.FieldValue);
                                    break;
                                default:
                                    SetLimitValue(limit.FieldName, Convert.ToString(limit.FieldValue));
                                    break;
                            }

                            // Limite type
                            var limitOperator = string.Empty;
                            switch (limit.LimitType)
                            {
                                case "EQUAL": limitOperator = "="; break;
                                case "LESS": limitOperator = "<"; break;
								case "LESSEQUAL": limitOperator = "<="; break;
                                case "GREAT": limitOperator = ">"; break;
                                case "GREATEQUAL": limitOperator = ">="; break;
                                case "DIFF": limitOperator = "<>"; break;
                                default:
                                    throw new BusinessException(null, "ReportCrystal.PreencherLimites", "Not yet implemented SU limite type: " + limit.LimitType ?? "NULL");
                            }
                            var tempFieldName = Configuration.Program + limit.FullFieldName;
                            adicionaCondicaoRecordSelectionFormula("{" + tempFieldName + "} " + limitOperator + " {@" + limit.FieldName + "}");
                        }
                        break;
                    case ReportLimitParameter.LimitSource.DB: 
                        {
                            // DBEdit com limitação em arvore
                            var limit = genLimit as ReportLimitParameter_DB;
                            if (string.IsNullOrEmpty(limit.FieldValue))
                                throw new BusinessException(null, "ReportCrystal.PreencherLimites", "Null or Empty tree seelction limit value");
                            var tempFieldName = Configuration.Program + limit.FullFieldName;
                            adicionaCondicaoRecordSelectionFormula(string.Format("Left({{{0}}}, {1}) = '{2}'", tempFieldName, limit.FieldValue.Length, limit.FieldValue));
                        }
                        break;
                    case ReportLimitParameter.LimitSource.DM:
                        {
                            // DBEdit com seleção multipla
                            var limit = genLimit as ReportLimitParameter_DM;
                            if (limit.FieldValue == null)
                                throw new BusinessException(null, "ReportCrystal.PreencherLimites", "Null or Empty multi seelction limit value");

                            AreaInfo areaBase = Area.GetInfoArea(area);
                            var tempFieldName = Configuration.Program + limit.FullFieldName;

                            //RMR(2020-04-22) - Support for multi-selected integer keys
                            List<string> keys = new List<string>();
                            if (areaBase.KeyType == FieldType.KEY_INT)
                                keys = limit.FieldValue.Select(val => $"{{{tempFieldName}}} = {val}").ToList();
                            else
                                keys =limit.FieldValue.Select(val => $"{{{tempFieldName}}} = '{{{val.TrimStart('{').TrimEnd('}')}}}'").ToList();

                            adicionaCondicaoRecordSelectionFormula("(" + string.Join(" OR ", keys) + ")");
                        }
                        break;
                }
            }
        }

        private void SetLimitValue(string FieldName, string FieldValue)
        {
            if(string.IsNullOrEmpty(FieldName) || string.IsNullOrEmpty(FieldValue))
                throw new BusinessException(null, "ReportCrystal.SetLimitValue", "Null or Empty argument value");
            if (camposDataDefinition.Contains(FieldName))
            {
                reportDocument.DataDefinition.FormulaFields[FieldName].Text = FieldValue;
                if (temSubReports)
                {
                    for (int r = 0; r < reportDocument.Subreports.Count; r++)
                    {
                        if (camposDataDefinitionSubreports[r].Contains(FieldName))
                        {
                            reportDocument.Subreports[r].DataDefinition.FormulaFields[FieldName].Text = FieldValue;
                        }
                    }
                }
            }
        }

        private void SetDateLimit(string FieldName, object FieldValue)
        {
            if (string.IsNullOrEmpty(FieldName) || FieldValue == null)
                throw new BusinessException(null, "ReportCrystal.SetDateLimit", "Null or Empty argument value");
            if (FieldValue is string)
            {
                DateTime tempDate;
                if (!DateTime.TryParse(FieldValue as string, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out tempDate))
                    throw new BusinessException(null, "ReportCrystal.SetDateLimit", "Error parsing date limit: " + Convert.ToString(FieldValue));
                FieldValue = tempDate;
            }
            string dateValue = "DateTime(" + (FieldValue as DateTime?).GetValueOrDefault().ToString("yyyy,MM,dd,HH,mm,ss") + ")";
            SetLimitValue(FieldName, dateValue);
        }

        private void SetStringLimit(string FieldName, object FieldValue)
        {
            if (string.IsNullOrEmpty(FieldName) || FieldValue == null)
                throw new BusinessException(null, "ReportCrystal.SetStringLimit", "Null or Empty argument value");
            SetLimitValue(FieldName, string.Format("'{0}'", FieldValue));
        }

        /// <summary>
        /// Método to preencher as limitações entre datas
        /// </summary>
        /// <param name="utilizador">user em sessão</param>
        /// <param name="modulo">module</param>
        private void preencherEntreDatas(User user, string module)
        {
            //data no format "yyyy , MM , dd , HH,mm,ss" (Qyear, mês, day, hora24, minutes, segundos)
            string[] aux = new string[2];
            for (int i = 0; i < limites.Length; i++)
            {
                DateTime date = new DateTime();
                if (!DateTime.TryParse(limites[i].ToString(), out date))
                    throw new BusinessException(null, "ReportCrystal.preencherEntreDatas", "Error parsing date limit: " + limites[i].ToString());
                aux[i] = date.ToString("yyyy,MM,dd,HH,mm,ss");
            }
            if (!campoEntreDatas.Equals(""))
            {
                //adicionar f_data01
                if (camposDataDefinition.Contains("f_data01"))
                {
                    reportDocument.DataDefinition.FormulaFields["f_data01"].Text = "DateTime(" + aux[0] + ")";
                    if (temSubReports)
                    {
                        for (int r = 0; r < reportDocument.Subreports.Count; r++)
                        {
                            if (camposDataDefinitionSubreports[r].Contains("f_data01"))
                            {
                                reportDocument.Subreports[r].DataDefinition.FormulaFields["f_data01"].Text = "DateTime(" + aux[0] + ")";
                            }
                        }
                    }
                }
                //adicionar f_data11
                if (camposDataDefinition.Contains("f_data11"))
                {
                    reportDocument.DataDefinition.FormulaFields["f_data11"].Text = "DateTime(" + aux[1] + ")";
                    if (temSubReports)
                    {
                        for (int r = 0; r < reportDocument.Subreports.Count; r++)
                            if (camposDataDefinitionSubreports[r].Contains("f_data11"))
                                reportDocument.Subreports[r].DataDefinition.FormulaFields["f_data11"].Text = "DateTime(" + aux[1] + ")";
                    }
                }
                adicionaCondicaoRecordSelectionFormula("{" + Configuration.Program + campoEntreDatas + "} >= {@f_data01} AND {" + Configuration.Program + campoEntreDatas + "} <= {@f_data11}");
            }
        }

        /// <summary>
        /// Método to preencher os fields que são arrays
        /// </summary>
        /// <param name="utilizador">user em sessão</param>
        /// <param name="modulo">module</param>
        private void preencherCamposArray(User user, string module, PersistentSupport sp)
        {
            try
            {
                //sp.openConnection();
                IEnumerator arrays = listaCamposArray.Keys.GetEnumerator();
                while (arrays.MoveNext())
                {
                    string tableField = arrays.Current.ToString();

                    string arrayName = listaCamposArray[tableField];
                    string tabelaCampoSemPonto = tableField.Replace(".", "");
                    string stringArray = ArraysCrystalReports.returnArrayCrystal(arrayName, Configuration.Program + tableField, user.Year);
                    try
                    {
                        if (camposDataDefinition.Contains(("f_" + tabelaCampoSemPonto).ToLower()))
                            reportDocument.DataDefinition.FormulaFields["f_" + tabelaCampoSemPonto].Text = stringArray;
                        if (temSubReports)
                        {
                            for (int r = 0; r < reportDocument.Subreports.Count; r++)
                                if (camposDataDefinitionSubreports[r].Contains(("f_" + tabelaCampoSemPonto).ToLower()))
                                    reportDocument.Subreports[r].DataDefinition.FormulaFields["f_" + tabelaCampoSemPonto].Text = stringArray;
                        }
                    }
                    catch (System.Runtime.InteropServices.COMException ex)
                    {
                        //o Qfield não está definido no report, não se faz nada
						Log.Error(ex.ToString());
                    }
                }

            }
            catch (GenioException ex)
			{
				throw new BusinessException(ex.UserMessage, "ReportCrystal.preencherCamposArray", "Error filling array fields: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new BusinessException(null, "ReportCrystal.preencherCamposArray", "Error filling array fields: " + ex.Message, ex);
            }
            //finally
            //{
            //    if (sp != null)
            //        sp.closeConnection();
            //}
        }


        private string AuxAdicionaCondicaoOutraArea(Area areaBase, EPHField ephArea, string[] listaValores, Relation myrelacao)
        {
            string condicoesEph = "";
            string programa = Configuration.Program;

            string crorigem = myrelacao.SourceRelField;
            //Area tabelaEPH = Area.createArea(ephArea.Table, user, module);
            AreaInfo tabelaEPH = Area.GetInfoArea(ephArea.Table);

            if (ephArea.Field.ToLower().Equals(myrelacao.TargetRelField))
            {//se a eph for da key primária

                if (ephArea.Operator == "EN")
                    condicoesEph += "(";
                else
                    condicoesEph += "(not(isnull({" + programa + areaBase.Alias.ToUpper() + "." + crorigem.ToUpper() + "})) AND ";

                condicoesEph += "{" + programa + areaBase.Alias.ToUpper() + "." + crorigem.ToUpper() + "} IN [";

                FieldFormatting fcampo = tabelaEPH.DBFields[ephArea.Field].FieldFormat;

                for (int i = 0; i < listaValores.Length; i++)
                {
                    condicoesEph += CrystalConversion.FromInternal(listaValores[i], fcampo) + ", ";
                }
                condicoesEph = condicoesEph.Remove(condicoesEph.Length - 2);

                if (ephArea.Operator == "EN")
                {
                    condicoesEph += "] OR isnull({" + programa + areaBase.Alias.ToUpper() + "." + crorigem.ToUpper() + "}) ";

                    if (fcampo == FieldFormatting.CARACTERES)
                        condicoesEph += " OR {" + programa + areaBase.Alias.ToUpper() + "." + crorigem.ToUpper() + "}  = \"\"";
                    condicoesEph += ") AND ";
                }
                else
                    condicoesEph += "]) AND ";
            }
            else
            {//se a eph for doutro Qfield da table

                condicoesEph += " (";
                if (ephArea.Operator == "L" || ephArea.Operator == "LN")
                {//se a eph for em árvore usa-se o Qfield da eph como prefixo
                    for (int i = 0; i < listaValores.Length; i++)
                    {
                        condicoesEph += "{" + programa + tabelaEPH.Alias.ToUpper() + "." + ephArea.Field.ToUpper() + "} LIKE ";
                        condicoesEph += CrystalConversion.FromInternal(listaValores[i].ToUpper() + "*", tabelaEPH.DBFields[ephArea.Field].FieldFormat) + " OR ";
                    }
                    // MH - Eph em árvore ou NULL
                    if (ephArea.Operator == "LN")
                    {
                        Field campoEPH = tabelaEPH.DBFields[ephArea.Field];
                        FieldFormatting cFormat = campoEPH.FieldFormat;
                        condicoesEph += "isnull({" + programa + tabelaEPH.Alias.ToUpper() + "." + ephArea.Field.ToUpper() + "}) OR ";
                        if (cFormat == FieldFormatting.CARACTERES)
                            condicoesEph += "{" + programa + tabelaEPH.Alias.ToUpper() + "." + ephArea.Field.ToUpper() + "} = \"\" OR ";
                    }
                }
                else
                {
                    string operador = "";

                    //NH(2016.10.06)
                    //Caso o Operator seja EN (EQUAL or NULL), o comportamento é semelhante ao ==, só mudando na parte final em que se coloca mais a validação do ISNULL e vazio.
                    if (ephArea.Operator == "EN")
                        operador = "=";
                    else
                        operador = ephArea.Operator;

                    //NH(2016.11.23) - Se o Qfield for key em Inteiro, vamos mudar a formatação to Inteiros
                    FieldFormatting fcampo = tabelaEPH.DBFields[ephArea.Field].FieldFormat;

                    for (int i = 0; i < listaValores.Length; i++)
                    {
                        condicoesEph += "{" + programa + tabelaEPH.Alias.ToUpper() + "." + ephArea.Field.ToUpper() + "} " + operador;
                        condicoesEph += CrystalConversion.FromInternal(listaValores[i], fcampo) + " OR ";
                    }

                    //Caso o Operator seja EN (Equal or NULL) vamos colocar a condição ISnull e vazio consiante o tipo de formatting do Qfield
                    if (ephArea.Operator == "EN")
                    {
                        condicoesEph += " isnull({" + programa + areaBase.Alias.ToUpper() + "." + crorigem.ToUpper() + "}) ";

                        if (fcampo == FieldFormatting.CARACTERES)
                            condicoesEph += " OR {" + programa + areaBase.Alias.ToUpper() + "." + crorigem.ToUpper() + "}  = \"\"";
                        condicoesEph += " OR ";
                    }
                }
                condicoesEph = condicoesEph.Remove(condicoesEph.Length - 3);
                condicoesEph += ")";

                condicoesEph += "AND ";
            }

            return condicoesEph;
        }

        private string AuxAdicionaCondicaoMesmaArea(Area areaBase, EPHField ephArea, string[] listaValores)
        {
            string condicoesEph = "";
            string programa = Configuration.Program;
            
            ////////////adicionar condição quando a área é a mesma da eph
            if (ephArea.Operator == "=" || ephArea.Operator == "EN")
            {//se o operador for "=" podemos usar o IN

                if (ephArea.Operator == "EN")
                    condicoesEph += "(";

                //NH(2016.11.23) - Se o Qfield for key em Inteiro, vamos mudar a formatação to Inteiros
                FieldFormatting fcampo = areaBase.DBFields[ephArea.Field].FieldFormat;

                condicoesEph += "{" + programa + areaBase.Alias.ToUpper() + "." + ephArea.Field.ToUpper() + "} IN [";
                for (int i = 0; i < listaValores.Length; i++)
                {
                    condicoesEph += CrystalConversion.FromInternal(listaValores[i], fcampo) + ", ";
                }
                condicoesEph = condicoesEph.Remove(condicoesEph.Length - 2);
                condicoesEph += "]";

                if (ephArea.Operator == "EN")
                {
                    condicoesEph += " OR isnull({" + programa + areaBase.Alias.ToUpper() + "." + ephArea.Field.ToUpper() + "}) ";

                    if (fcampo == FieldFormatting.CARACTERES)
                        condicoesEph += " OR {" + programa + areaBase.Alias.ToUpper() + "." + ephArea.Field.ToUpper() + "}  = \"\"";
                    condicoesEph += ") ";
                }
            }
            else
            {
                condicoesEph += " (";
                if (ephArea.Operator == "L" || ephArea.Operator == "LN")
                {//se a eph for em árvore usa-se o Qfield da eph como prefixo
                    for (int i = 0; i < listaValores.Length; i++)
                    {
                        condicoesEph += "{" + programa + areaBase.Alias.ToUpper() + "." + ephArea.Field.ToUpper() + "} LIKE ";
                        condicoesEph += CrystalConversion.FromInternal(listaValores[i] + "*", areaBase.DBFields[ephArea.Field].FieldFormat) + " OR ";
                    }
                    // MH - Eph em árvore ou NULL
                    if (ephArea.Operator == "LN")
                    {
                        Field campoEPH = areaBase.DBFields[ephArea.Field];
                        FieldFormatting cFormat = campoEPH.FieldFormat;
                        condicoesEph += "isnull({" + programa + areaBase.Alias.ToUpper() + "." + ephArea.Field.ToUpper() + "}) OR ";
                        if (cFormat == FieldFormatting.CARACTERES)
                            condicoesEph += "{" + programa + areaBase.Alias.ToUpper() + "." + ephArea.Field.ToUpper() + "} = \"\" OR ";
                    }
                }
                else
                {
                    for (int i = 0; i < listaValores.Length; i++)
                    {
                        condicoesEph += "{" + programa + areaBase.Alias.ToUpper() + "." + ephArea.Field.ToUpper() + "} " + ephArea.Operator + " ";
                        condicoesEph += CrystalConversion.FromInternal(listaValores[i], areaBase.DBFields[ephArea.Field].FieldFormat) + " OR ";
                    }
                }
                condicoesEph = condicoesEph.Remove(condicoesEph.Length - 3);
                condicoesEph += ")";
            }
            condicoesEph += "AND ";
            return condicoesEph;
        }

        /// <summary>
        /// Método to preencher as limitações de EPH
        /// </summary>
        /// <param name="utilizador">user em sessão</param>
        /// <param name="modulo">module</param>
        private void preencherEPHs(User user, string module, PersistentSupport sp)
        {
            Area areaBase = Area.createArea(area, user, module);
            string programa = Configuration.Program;
            string condicoesEph = "";

            List<EPHOfArea> ephsDaArea = areaBase.CalculateAreaEphs(user.Ephs, null, false);
            foreach (EPHOfArea v in ephsDaArea)
            {
                if (v.Relation2 != null)
                {
                    string condicao1 = "";
                    string condicao2 = "";
                    // First relation can be empty
                    if (v.Relation != null)
                        condicao1 = AuxAdicionaCondicaoOutraArea(areaBase, v.Eph, v.ValuesList, v.Relation);
                    
                    // In order to reuse code, we create a second EPH field from data in area's EPH
                    EPHField EPH2 = new EPHField(v.Eph.Name, v.Eph.Table2, v.Eph.Field2, v.Eph.Operator2, v.Eph.Propagate);
                    condicao2 = AuxAdicionaCondicaoOutraArea(areaBase, EPH2, v.ValuesList, v.Relation2);

                    if (!string.IsNullOrEmpty(condicao1) && !string.IsNullOrEmpty(condicao2))
                    {
                        // retira a última particula AND
                        condicao1 = condicao1.Substring(0, condicao1.Length - 4);
                        condicao2 = condicao2.Substring(0, condicao2.Length - 4);

                        string op = " AND ";
                        if (v.Eph.OR_EPH1_EPH2)
                            op = " OR ";
                        condicoesEph += $"({condicao1} {op} {condicao2}) AND ";
                    }
                    else if (!string.IsNullOrEmpty(condicao1))
                    {
                        condicoesEph += condicao1;
                    }
                    else if (!string.IsNullOrEmpty(condicao2))
                    {
                        condicoesEph += condicao2;
                    }
                }
                else if (v.Relation != null)
                {
                    condicoesEph += AuxAdicionaCondicaoOutraArea(areaBase, v.Eph, v.ValuesList, v.Relation);
                }
                else
                {
                    condicoesEph += AuxAdicionaCondicaoMesmaArea(areaBase, v.Eph, v.ValuesList);
                }
            }                

            // retira a última partícula AND
            if (condicoesEph.Length != 0)
                condicoesEph = condicoesEph.Substring(0, condicoesEph.Length - 4);
           
		    adicionaCondicaoRecordSelectionFormula(condicoesEph);
        }

        /// <summary>
        /// Método to preencher as condições de historial
        /// </summary>
        private void preencherCamposHistorial(User user, string module)
        {
            //AV(2010/01/03) Alterei a função to passar a usar as conversões to formatar as condições

            AreaInfo areaBase = Area.GetInfoArea(area);

            for (int i = 0; i < nomesCamposHistorial.Length; i++)
            {
                //FS(2008-11-03) Ignorar Qvalues de historial que venham a null (em principio não deveria acontecer, caso entre aqui investigar razões)
                if (valoresCamposHistorial[i] == null)
                    continue;
                StringBuilder str_recordselection; //TSX (2008-10-30)
                StringBuilder str_recordselectionCampo = new StringBuilder(""); //TSX (2008-10-30)
                //podemos incializar o tpField com um tipo qualquer pois vamos sempre actualizar a variável quando encontrarmos o Qfield
                FieldType tpField = FieldType.KEY_VARCHAR;
                //to cada name tem que se verificar se vem só o name da area ou se vem o name completo
                //se vier só o name da area tem que se descobrir a relação com a area base
                if (!nomesCamposHistorial[i].Contains("."))//só vem o name da area pelo que assumimos que é uma key primária
                {
                    if (area == nomesCamposHistorial[i])//AV(2011/03/29) o histórico é da área base do rpt
                    {
                  		tpField = areaBase.DBFields[areaBase.PrimaryKeyName].FieldType;
                        str_recordselectionCampo = new StringBuilder("{" + Configuration.Program + areaBase.Alias + "." + areaBase.PrimaryKeyName + "}=");
                    }
                    else
                    {
                        //AV(2011/03/29) o histórico é de uma table directamente acima da área base do rpt
                        if (areaBase.ParentTables.ContainsKey(nomesCamposHistorial[i]))
                        {
                            Relation relacao = (Relation)areaBase.ParentTables[nomesCamposHistorial[i]];
                            
                           	tpField = areaBase.DBFields[relacao.SourceRelField].FieldType;
                            str_recordselectionCampo = new StringBuilder("{" + Configuration.Program + relacao.AliasSourceTab + "." + relacao.SourceRelField + "}=");
                        }
                        else
                        {
                            foreach (string areaReport in areasReport)
                            {
                                AreaInfo areaRpt = Area.GetInfoArea(areaReport);
                                //AV(2011/03/29) o histórico é de uma das áreas do rpt identificada nas definições
                                if (areaReport == nomesCamposHistorial[i])
                                {
                                   	tpField = areaRpt.DBFields[areaRpt.PrimaryKeyName].FieldType;
                                    str_recordselectionCampo = new StringBuilder("{" + Configuration.Program + areaRpt.Alias + "." + areaRpt.PrimaryKeyName + "}=");
                                    break;
                                }
                                else
                                {
                                    //AV(2011/03/29) o histórico é de uma table directamente acima das áreas do rpt identificada nas definições
                                    if (areaRpt.ParentTables.ContainsKey(nomesCamposHistorial[i]))
                                    {
                                        Relation relacao = (Relation)areaRpt.ParentTables[nomesCamposHistorial[i]];
                                       	tpField = areaRpt.DBFields[relacao.SourceRelField].FieldType;
                                        str_recordselectionCampo = new StringBuilder("{" + Configuration.Program + relacao.AliasSourceTab + "." + relacao.SourceRelField + "}=");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {//vem o name da area e do Qfield (table.Qfield)
                        string[] campoCompleto = nomesCamposHistorial[i].Split('.');
                    //AV(2011/03/29) o histórico é dum Qfield da área base do rpt
                    if (area == campoCompleto[0])
                    {
                       	tpField = areaBase.DBFields[campoCompleto[1]].FieldType;
                        str_recordselectionCampo = new StringBuilder("{" + Configuration.Program + nomesCamposHistorial[i] + "}=");
                    }
                    else
                    {
                        foreach (string areaReport in areasReport)
                        {
                            AreaInfo areaRpt = Area.GetInfoArea(areaReport);
                            //AV(2011/03/29) o histórico é dum Qfield de uma das áreas do rpt identificada nas definições
                            if (areaReport == campoCompleto[0])
                            {
								//RMR(2016-10-04) - Before converting to int in case of integer keys, gets the field format, and verifies if this field is any kind of key
                                tpField = areaRpt.DBFields[campoCompleto[1]].FieldType;

                                str_recordselectionCampo = new StringBuilder("{" + Configuration.Program + nomesCamposHistorial[i] + "}=");
                                break;
                            }
                        }
                    }
                }
                //AV(2011/03/29) Encontrámos uma table no rpt to aplicar a condição de histórico
                if (str_recordselectionCampo.Length != 0)
                {
                    //ARR(2010-01-25) separar os codigos quando temos uma seleção multipla.
                    //concatenando assim : ( Qfield = codigo or Qfield = codigo)
                    object[] Nentradas = null;
                    if (valoresCamposHistorial[i] is String)
                    {
                        Nentradas = valoresCamposHistorial[i].ToString().Split(';');
                    }
                    else
                    {
                        Nentradas = new object[1] { valoresCamposHistorial[i] };
                    }
                    str_recordselection = new StringBuilder("(");
                    for (int nr = 0; nr < Nentradas.Length - 1; nr++)
                    {
                        str_recordselection.Append(str_recordselectionCampo + CrystalConversion.FromInternal(Nentradas[nr], tpField.GetFormatting()));
                        str_recordselection.Append(" OR ");
                    }
                    //AV(2011/03/29) o último Qvalue de Nentradas não precisa do OR por isso é feito fora do for
                    str_recordselection.Append(str_recordselectionCampo + CrystalConversion.FromInternal(Nentradas[Nentradas.Length - 1], tpField.GetFormatting()));

                    str_recordselection.Append(")");
                    adicionaCondicaoRecordSelectionFormula(str_recordselection.ToString().ToUpper());
                }
            }
        }

		/// <summary>
        /// função to delimitar a record selection formula existente no mapa
        /// </summary>
        private void DelimitarRecordSelection()
        {
            //Quando é feito o load o ReportDocument tenta optimizar a record selection formula tendo em conta os parentesis
            //No entanto esta analise não tem em conta que poderá existir outros limites que serão adicionados
            //Temos que manter a record selection formula dentro de parentesis
            if (!string.IsNullOrEmpty(reportDocument.RecordSelectionFormula))
                reportDocument.RecordSelectionFormula = "(" + reportDocument.RecordSelectionFormula + ")";

            //se os limites adicionais forem aplicados ao subreport então também temos que delimitar a record de cada subreport
            if (subReportRecord)
            {
                for (int r = 0; r < reportDocument.Subreports.Count; r++)
                {
                    if (!string.IsNullOrEmpty(reportDocument.Subreports[r].RecordSelectionFormula))
                        reportDocument.Subreports[r].RecordSelectionFormula = "(" + reportDocument.Subreports[r].RecordSelectionFormula + ")";
                }
            }
        }

        /// <summary>
        /// função to adicionar uma condição genérica à recordselectionformula
        /// </summary>
        /// <param name="recordSelectionFormula">record selection formula</param>
        /// <param name="condicao">condition a ser adicionada</param>
        /// <returns>a record selection formula completa</returns>
        private void adicionaCondicaoRecordSelectionFormula(string condition)
        {
            if (!condition.Equals(""))
            {
                if (reportDocument.RecordSelectionFormula.Equals(""))
                    reportDocument.RecordSelectionFormula = condition;
                else
                {
                    //reportDocument.RecordSelectionFormula += " AND " + condition;
                    //JG 20/08/2008
                    reportDocument.RecordSelectionFormula = condition + " AND " + reportDocument.RecordSelectionFormula;
                }

				//se for true o RecordSelectionFormula dos subreports passam a ter também a limitação do report
				if (subReportRecord)
				{
					
					for (int r = 0; r < reportDocument.Subreports.Count; r++)
					{
						if (!reportDocument.Subreports[r].Name.ToUpper().StartsWith("NL_"))
						{
							if (reportDocument.Subreports[r].RecordSelectionFormula.Equals(""))
								reportDocument.Subreports[r].RecordSelectionFormula = condition;
							else
								reportDocument.Subreports[r].RecordSelectionFormula = condition + " AND " + reportDocument.Subreports[r].RecordSelectionFormula;
						}
					}
				}
            }
        }

        public class ReportCrystalLimitParam
        {
            public enum LimitSource { SE, DB };

            public LimitSource Source { get; set; }

            /// <summary>
            /// Full field name
            /// </summary>
            public string FullFieldName { get; set; }

            /// <summary>
            /// D - Date | N - Numeric | C - String
            /// </summary>
            public string FieldType { get; set; }

            public ReportCrystalLimitParam(LimitSource lim)
            {
                this.Source = lim;
            }
        }

        /// <summary>
        /// ReportCrystal limit of the type: Selection between limits
        /// </summary>
        public class ReportCrystalLimitParam_SE : ReportCrystalLimitParam
        {
            // Min value
            public string MinFieldName { get; set; }
            public object MinFieldValue  { get; set; }

            // Max value
            public string MaxFieldName { get; set; }
            public object MaxFieldValue { get; set; }

            public ReportCrystalLimitParam_SE() : base(LimitSource.SE) { }
        }

        /// <summary>
        /// ReportCrystal limit of the type: List with tree selection
        /// </summary>
        public class ReportCrystalLimitParam_DB : ReportCrystalLimitParam
        {
            /// <summary>
            /// Tree table - Design
            /// </summary>
            public string FieldValue  { get; set; }

            public ReportCrystalLimitParam_DB() : base(LimitSource.DB) { }
        }

        /// <summary>
        /// Método que devolve o objecto ReportDocument
        /// </summary>
        public ReportDocument ReportDocument
        {
            get { return reportDocument; }
        }

        /// <summary>
        /// Método que devolve o name do Report
        /// </summary>
        public string ReportName
        {
            get { return nomeReport; }
        }
    }
}
#endif
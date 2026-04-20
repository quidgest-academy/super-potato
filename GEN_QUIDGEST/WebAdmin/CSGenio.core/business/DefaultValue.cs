using System;
using System.Data;
using System.Data.SqlTypes;
using System.Text;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
    /// <summary>
    /// Class that describes the Default Values, can be:
    /// -Fixed
    /// -Predefined that do not need to access DB
    /// -Predefined that need to access DB or formulas.
    /// </summary>   
    public class DefaultValue : Formula
    {

        public enum DefaultType { FIXO, PRE_DEF, PRE_DEF_BD, OP_INT};//Default Type

        //Default Type
        public DefaultType tpDefault;
        //Argument that holds the fixed Qvalues
        private object Qvalue;
        //Arguments used in predefined functions
        public delegate object FunctionPreDef(object[] args);//function that will be invoked
        public FunctionPreDef funcaoPreDef;//Variable that will have the Fungco

        //argument used in predefined function with database access
        public delegate object FunctionPreDefBD(object[] args, PersistentSupport sp);//function that will be invoked
        public FunctionPreDefBD funcaoPreDefBD;//Variable that will have the function
        private string nomeCampoConsultado; //Name of field that will be queried to calculate the default value

        //argument used when the function is not predefined
        private InternalOperationFormula formulaDefault;

        /// <summary>
        /// Constructor for default fixed values (fixed value and none)
        /// </summary>
        /// <param name="valor">Default value</param>
        public DefaultValue(object Qvalue)
        {
            tpDefault = DefaultType.FIXO;
            this.Qvalue = Qvalue;
        }

        /// <summary>
        /// Constructor for predefined functions that do not need to refer to BD
        /// </summary>
        /// <param name="f">function that corresponds to the formula</param>
        public DefaultValue(FunctionPreDef f)
        {
            tpDefault = DefaultType.PRE_DEF;
            this.funcaoPreDef = f;
        }

        /// <summary>
        /// Constructor for the predefined functions and that need to go to BD
        /// </summary>
        /// <param name="nomeCampo">Name of field queried to calculate the default value</param>
        /// <param name="f">function that corresponds to the formula</param>
        public DefaultValue(FunctionPreDefBD f, string campoConsultado)
        {
            tpDefault = DefaultType.PRE_DEF_BD;
            this.funcaoPreDefBD = f;
            this.nomeCampoConsultado = campoConsultado;

        }

        /// <summary>
        /// Constructor for user defined default formulas
        /// </summary>
        /// <param name="formulaDefault">Formula that will fill the default value</param>
        public DefaultValue(InternalOperationFormula formulaDefault)
        {
            tpDefault = DefaultType.OP_INT;
            this.formulaDefault = formulaDefault;

        }


        /// <summary>
        /// Function that allows you to get the day 1 of the month and current year
        /// </summary>
        /// <returns>The Day 1 of the current MJS and year</returns>
        public static object getDay1Month(object[] fieldsValue)
        {
            return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }

        /// <summary>
        /// Function that allows you to get the day 1 of last month and current year
        /// </summary>
        /// <returns>The day 1 of last month and current year</returns>
        public static object getDay1LastMonth(object[] fieldsValue)
        {
            return new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(-1).Month, 1);
        }

        /// <summary>
        /// Function that allows you to get today's date
        /// </summary>
        /// <returns>Today's date</returns>
        public static object getToday(object[] fieldsValue)
        {
            return DateTime.Today;
        }
		
		 /// <summary>
        /// Function that allows you to get today's date taking into account the time (H:M: S)
        /// </summary>
        /// <returns>Today's date</returns>
        public static object getNow(object[] fieldsValue)
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Function that allows to get yesterday's date
        /// </summary>
        /// <returns>Yesterday's date</returns>
        public static object getYesterday(object[] fieldsValue)
        {
            return DateTime.Today.AddDays(-1.0);
        }

        /// <summary>
        /// Function that allows you to get the day 31 December of the current year
        /// </summary>
        /// <returns>The Day 31 December of the current year</returns>
        public static object getLastDayDecember(object[] fieldsValue)
        {
            return new DateTime(DateTime.Today.Year,12,31);

        }

        /// <summary>
        /// Function that allows you to get the last useful day
        /// Does not count with today, starts to check if yesterday is useful day
        /// </summary>
        /// <returns>The Last Useful Day</returns>
        public static object getLastWorkingDay(object[] fieldsValue)
        {
            DateTime ontem = DateTime.Today.AddDays(-1);
            while (ontem.DayOfWeek.Equals(DayOfWeek.Saturday) ||
                ontem.DayOfWeek.Equals(DayOfWeek.Sunday))
                ontem = ontem.AddDays(-1);
            return ontem;

        }

        /// <summary>
        /// Function that allows you to get the day 1 January of the current year
        /// </summary>
        /// <returns>The Day 1 January of the current year</returns>
        public static object getJanuary1st(object[] fieldsValue)
        {
            return new DateTime(DateTime.Today.Year, 1, 1);
        }


        static public object getGreaterPlus1(object[] args, PersistentSupport sp)
        {
            return getGreaterPlus1(args, sp, FieldFormatting.CARACTERES);
        }
        static public object getGreaterPlus1_int(object[] args, PersistentSupport sp)
        {
            return getGreaterPlus1(args, sp, FieldFormatting.FLOAT);
        }

        /// <summary>
        /// Function that allows you to get the highest value plus 1
        /// If there is no return 1
        /// </summary>
        /// <param name="args">The area and name of the field</param>	
        /// <returns>The largest plus 1</returns>
        static private object getGreaterPlus1(object[] args, PersistentSupport sp, FieldFormatting fieldType)
        {
            try
            {
				string system = Convert.ToString(args[0]);
				string table = args[1].ToString();
                string primaryKey = args[2].ToString();
                string fieldName = args[3].ToString();
                string nDupPref = args[4] as string;
                //Dim nDupPrefValue As String = args [5] as String; AJA-This form only worked when the object was to type string. Further down what it does is pass this string to within a variable type object. If You do not make this double conversion, and keep the variable only as type object, we can use a prefix that is not just string.
				object nDupPrefValue = args[5];
                Field prefixField = null;
                if (args[7] is Field prefixFieldArg)
                    prefixField = prefixFieldArg;

                SelectQuery qs;
                if (fieldType == FieldFormatting.FLOAT)
                {
                    //If the field is already numeric the query is much simpler and more efficient
                    qs = new SelectQuery()
                    .Select(SqlFunctions.Max(new ColumnReference(table, fieldName)), "max")
                    .From(system, table, table)
                    .Where(CriteriaSet.And()
                        .Equal(table, "zzstate", 0));
                }
                else
                {
                    //Ignore non-numeric values
                    qs = new SelectQuery()
                    .Select(SqlFunctions.Max(SqlFunctions.Cast(new ColumnReference(table, fieldName), DbType.Int32)), "max")
                    .From(system, table, table)
                    .Where(CriteriaSet.And()
                        .Equal(table, "zzstate", 0)
                        .Equal(SqlFunctions.SysCustom("IsNumeric", new ColumnReference(table, fieldName)), 1)
                        .Greater(SqlFunctions.Cast(new ColumnReference(table, fieldName), DbType.Int32), 0));
                }
				// [RC] 24/05/2017-Run the query using update locks on rows
				// This is used to prevent all the deadlocks we're getting under heavy load
				// For the moment, we are adding the locks every time. In the future, we should probably check for conditions under
				// Which this kind of locks is necessary.
				qs.updateLock = true;

                if (!string.IsNullOrEmpty(nDupPref))
                {
                    if (nDupPrefValue != null)
                    {
                        // For key fields, an empty prefix means 'no value', so we normalise it to null
                        // to generate a WHERE ... IS NULL filter. For non-empty values, we convert the
                        // prefix to a database-safe value (e.g. Guid) before applying the equality filter.
                        object prefixRealValue = prefixField.isKey() && prefixField.isEmptyValue(nDupPrefValue) ? null : QueryUtils.ToValidDbValue(nDupPrefValue, prefixField);
                        qs.WhereCondition.Equal(table, nDupPref, prefixRealValue);
                    }
                    else
                    {
                        return "";
                    }
                }

                object Qvalue = sp.ExecuteScalar(qs);
                //The SQL query already ensures that we always receive an integer
                Qvalue = DBConversion.ToInteger(Qvalue) + 1;

                return Qvalue;
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "DefaultValue.getGreaterPlus1", 
				                          string.Format("Error computing the value - [args] {0}: ", args.ToString()) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "DefaultValue.getGreaterPlus1", 
				                          string.Format("Error computing the value - [args] {0}: ", args.ToString()) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function that allows obtaining the value of the last record
        /// If None exists returns 0
        /// </summary>
        /// <param name="args">The area and name of the field</param>
        /// <returns>value of last record</returns>
        public static object getLast(object[] args,PersistentSupport sp)
        {
			string system = Convert.ToString(args[0]);
			string table = args[1].ToString();
            string primaryKey = args[2].ToString();
            string fieldName = args[3].ToString();
            string nDupPref = args[4] as string;
            object nDupPrefValue = args[5];
            FieldFormatting formPrefNDup = FieldFormatting.CARACTERES;
            if (args[6] != null)
                formPrefNDup = (FieldFormatting)args[6];

            SelectQuery qs = new SelectQuery()
                .Select(table, fieldName)
                .From(system, table, table)
                .Where(CriteriaSet.And()
                    .Equal(table, "zzstate", 0));

            if (!string.IsNullOrEmpty(nDupPref) && !Field.isEmptyValue(nDupPrefValue, formPrefNDup))
            {
                qs.WhereCondition.Equal(table, nDupPref, nDupPrefValue);
            }

            qs.OrderBy(table, primaryKey, SortOrder.Descending).PageSize(1);
            
            object Qvalue = sp.ExecuteScalar(qs);
            if (Qvalue == null || Qvalue == DBNull.Value)
                Qvalue = 0;
            return Qvalue;
        }

        /// <summary>
        /// Function that allows obtaining the value of the last record plus 1
        /// If there is no return 1
        /// </summary>
        /// <param name="args">The area and name of the Qfield</param>
        /// <returns>Qvalue of last record plus 1</returns>
        public static object getLastPlus1(object[] args, PersistentSupport sp)
        {
			string system = Convert.ToString(args[0]);
			string table = args[1].ToString();
            string primaryKey = args[2].ToString();
            string fieldName = args[3].ToString();
            //RS (2008-10-30) cannot convert the name of a field with the conversion functions of values!
            string nDupPref = args[4] as string;
            string nDupPrefValue = args[5] as string;
            FieldFormatting formPrefNDup = FieldFormatting.CARACTERES;
            if(args[6]!=null)
                formPrefNDup = (FieldFormatting)args[6];

            SelectQuery qs = new SelectQuery()
                .Select(SqlFunctions.Add(new ColumnReference(table, fieldName), 1), "lastPlusOne")
                .From(system, table, table)
                .Where(CriteriaSet.And()
                    .Equal(table, "zzstate", 0));

            if (nDupPref != null && nDupPrefValue != null)
            {
                qs.WhereCondition.Equal(table, nDupPref, nDupPrefValue);
            }
            // ML (2010.01.21) If You do not have non-duplication prefix returns ""??? 
            //else return ""; 
            qs.OrderBy(table, primaryKey, SortOrder.Descending).PageSize(1);

            object Qvalue = sp.ExecuteScalar(qs);
            if (Qvalue == null || Qvalue == DBNull.Value)
                Qvalue = 1;

            return Qvalue;
        }

        /// <summary>
        /// Calculates The value of this default for the given row
        /// </summary>
        /// <param name="area">The row</param>
        /// <param name="sp">Persistent Support</param>
        /// <param name="fdc">The formula context to optimize database access</param>
        /// <param name="tpFunction">Operation type for the calculation</param>
        /// <returns>The Default Value</returns>
        public object calculateFormulaDefault(Area area, PersistentSupport sp, FormulaDbContext fdc, FunctionType tpFunction)
        {
            try
            {
                if (tpDefault == DefaultType.FIXO)
                    return this.Qvalue;
                else
                {
                    object[] args = new object[0];
                    if (tpDefault == DefaultType.PRE_DEF)
                        return funcaoPreDef(args);
                    else if (tpDefault == DefaultType.OP_INT)
                    {
                        return formulaDefault.calculateInternalFormula(area,sp,fdc,tpFunction);
                    }
                    return null;                    
                } 

            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "DefaultValue.calculaFormulaDefault", 
				                          string.Format("Error computing the value - [area] {0}; [sp] {1}; [tpFuncao] {2}: ", area.ToString(), sp.ToString(), tpFunction.ToString()) + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new BusinessException(null, "DefaultValue.calculaFormulaDefault", 
				                          string.Format("Error computing the value - [area] {0}; [sp] {1}; [tpFuncao] {2}: ", area.ToString(), sp.ToString(), tpFunction.ToString()) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function receives as argument the value of the fields 
        /// </summary>
        /// <param name="valorCampos">value of Fields</param>
        /// <returns>The value of the default formula</returns>
        public object calculateSequentialFormula(IArea area, string nDupPref, object nDupPrefValue, FieldFormatting formPrefNDup, PersistentSupport sp)
        {
            try
            {

                Field prefixField = string.IsNullOrEmpty(nDupPref) ? null : area.DBFields[nDupPref];
                object[] args = [area.QSystem, area.TableName, area.PrimaryKeyName, this.nomeCampoConsultado, nDupPref, nDupPrefValue, formPrefNDup, prefixField];
                return funcaoPreDefBD(args,sp);

            }
            catch (GenioException ex)
            {
				throw new BusinessException(ex.UserMessage, "DefaultValue.calculaFormulaSequencial", 
				                          string.Format("Error computing the value - [area] {0}; [prefNDup] {1}; [valorPrefNDup] {2}; [formPrefNDup] {3}; [sp] {4}: ", area.ToString(), nDupPref, nDupPrefValue.ToString(), formPrefNDup.ToString(), sp.ToString()) + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new BusinessException(null, "DefaultValue.calculaFormulaSequencial", 
				                          string.Format("Error computing the value - [area] {0}; [prefNDup] {1}; [valorPrefNDup] {2}; [formPrefNDup] {3}; [sp] {4}: ", area.ToString(), nDupPref, nDupPrefValue.ToString(), formPrefNDup.ToString(), sp.ToString()) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function to check if the sequential value already exists, if there is
        /// That if you assign another
        /// </summary>
        /// <param name="nomeTabela">Name table</param>
        /// <param name="nomeChavePrimaria">Name Key Primary</param>
        /// <param name="valorChavePrimaria">value of Key Primary</param>
        /// <param name="utilizador">User</param>
        /// <param name="prefNDup">field prefix non-duplication</param>
        /// <param name="valorPrefNDup">value of field prefix no duplication</param>
        /// <param name="formPrefNDup">field formatting that is non-duplicating prefix</param>
        /// <param name="valorCampoSequencial">value of the sequential field</param>
        /// <param name="formCampoSeq">Formatting of the sequential field</param>
        /// <returns>True if the sequential value already exists, false otherwise</returns>
        public bool existsSequentialValue(IArea area, object primaryKeyValue, string nDupPref, object nDupPrefValue, FieldFormatting formPrefNDup, object sequentialFieldValue, FieldFormatting formSeqField, PersistentSupport sp)
        {
            try
            {
                SelectQuery qs = new SelectQuery()
                    .Select(area.TableName, area.PrimaryKeyName)
                    .From(area.QSystem, area.TableName, area.TableName)
                    .Where(CriteriaSet.And()
                        .Equal(area.TableName, nomeCampoConsultado, sequentialFieldValue)
                        .NotEqual(area.TableName, area.PrimaryKeyName, primaryKeyValue));

                if (nDupPref != null && nDupPrefValue != null)
                {
                    if (!nDupPref.Equals("") && !nDupPrefValue.Equals(""))
                    {
                        qs.WhereCondition.Equal(area.TableName, nDupPref, nDupPrefValue);
                    }
                }

                object Qvalue = sp.ExecuteScalar(qs);
                if (Qvalue == null || Qvalue == DBNull.Value)
                    return false;
                else
                    return true;
                   // throw new BusinessException ("$Genio. TraduzirF (" This sequential number already exists, write another, and then write again. ")", "DefaultValue. Existevalorsequential", "This sequential number already exists, write another and write again.");
            }
            catch (GenioException ex)
            {
				throw new BusinessException(ex.UserMessage, "DefaultValue.existeValorSequencial", 
				                          string.Format("Error checking the value - [area] {0}; [valorChavePrimaria] {1}; [prefNDup] {2}; [valorPrefNDup] {3}; [formPrefNDup] {4}; [valorCampoSequencial] {5}; [formCampoSeq] {6}; [sp] {7}: ",
										                area.ToString(), primaryKeyValue.ToString(), nDupPref, nDupPrefValue.ToString(), formPrefNDup.ToString(), sequentialFieldValue.ToString(), formSeqField.ToString(), sp.ToString()) + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new BusinessException(null, "DefaultValue.existeValorSequencial", 
				                          string.Format("Error checking the value - [area] {0}; [valorChavePrimaria] {1}; [prefNDup] {2}; [valorPrefNDup] {3}; [formPrefNDup] {4}; [valorCampoSequencial] {5}; [formCampoSeq] {6}; [sp] {7}: ",
										                area.ToString(), primaryKeyValue.ToString(), nDupPref, nDupPrefValue.ToString(), formPrefNDup.ToString(), sequentialFieldValue.ToString(), formSeqField.ToString(), sp.ToString()) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function to verify that the non-duplication prefix has changed
        /// </summary>
        /// <param name="nomeTabela">Name table</param>
        /// <param name="nomeChavePrimaria">Name Key Primary</param>
        /// <param name="valorChavePrimaria">value of Key Primary</param>
        /// <param name="utilizador">User</param>
        /// <param name="prefNDup">field prefix of NDuplicate</param>
        /// <param name="valorPrefNDup">value for prefix NDuplicate</param>
        /// <returns>True if the non-duplication prefix has been changed, false otherwise</returns>
        public bool valuePrefNDupChanged(IArea area, object primaryKeyValue, string nDupPref, string nDupPrefValue, PersistentSupport sp)
        {
            try
            {
                SelectQuery qs = new SelectQuery()
                    .Select(area.TableName, nDupPref)
                    .From(area.QSystem, area.TableName, area.TableName);

                if (nDupPref != null && nDupPrefValue != null)
                {
                    if (!nDupPref.Equals("") && !nDupPrefValue.Equals(""))
                    {
                        qs.Where(CriteriaSet.And()
                            .Equal(area.TableName, area.PrimaryKeyName, primaryKeyValue));
                    }
                }

                object Qvalue = sp.ExecuteScalar(qs);

                if (Qvalue == null || Qvalue == DBNull.Value)
                    return false;
                else
                    if (DBConversion.ToKey(Qvalue) != nDupPrefValue)
                        return true;
                    else return false;          
            }
            catch (GenioException ex)
            {
				throw new BusinessException(ex.UserMessage, "DefaultValue.valorPrefNDupAlterado", 
				                          string.Format("Error checking the value - [area] {0}; [valorChavePrimaria] {1}; [prefNDup] {2}; [valorPrefNDup] {3}; [sp] {4}: ",
										                area.ToString(), primaryKeyValue.ToString(), nDupPref, nDupPrefValue.ToString(), sp.ToString()) + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new BusinessException(null, "DefaultValue.valorPrefNDupAlterado", 
				                          string.Format("Error checking the value - [area] {0}; [valorChavePrimaria] {1}; [prefNDup] {2}; [valorPrefNDup] {3}; [sp] {4}: ",
										                area.ToString(), primaryKeyValue.ToString(), nDupPref, nDupPrefValue.ToString(), sp.ToString()) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function to verify that the record is being inserted
        /// </summary>
        /// <param name="nomeTabela">Name table</param>
        /// <param name="nomeChavePrimaria">Name Key Primary</param>
        /// <param name="valorChavePrimaria">Qvalue of Key Primary</param>
        /// <param name="utilizador">User</param>
        /// <returns>True if the record does not already exist, false otherwise</returns>
        public bool newRecord(IArea area, object primaryKeyValue, PersistentSupport sp)
        {
            try
            {
                SelectQuery qs = new SelectQuery()
                    .Select(area.TableName, "zzstate")
                    .From(area.QSystem, area.TableName, area.TableName)
                    .Where(CriteriaSet.And()
                        .Equal(area.TableName, area.PrimaryKeyName, primaryKeyValue));

                object Qvalue = sp.ExecuteScalar(qs);

                if (Qvalue == null || Qvalue == DBNull.Value)
                {
                    return true;
                }
                else
                {
                    if (Convert.ToInt32(Qvalue) == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (GenioException ex)
            {
				throw new BusinessException(ex.UserMessage, "DefaultValue.novoRegisto", 
				                          string.Format("Error executing te checking - [area] {0}; [valorChavePrimaria] {1}; [sp] {2}: ",
										                area.ToString(), primaryKeyValue.ToString(), sp.ToString()) + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new BusinessException(null, "DefaultValue.novoRegisto", 
				                          string.Format("Error executing te checking - [area] {0}; [valorChavePrimaria] {1}; [sp] {2}: ",
										                area.ToString(), primaryKeyValue.ToString(), sp.ToString()) + ex.Message, ex);
            }
        }
        
        /// <summary>
        ///  Method that returns or places the default value
        /// </summary>
        public object Value
        {
            get { return Qvalue; }
            set { Qvalue = value; }
        }

        /// <summary>
        ///  Method that returns or places the name of the field that will be queried in DB
        /// </summary>
        public string ConsultedFieldName
        {
            get { return nomeCampoConsultado; }
            set { nomeCampoConsultado = value; }
        }

        /// <summary>
        ///  Method that returns or puts the formula Default
        /// </summary>
        public InternalOperationFormula DefaultFormula
        {
            get { return formulaDefault; }
            set { formulaDefault = value; }
        }
    }
}

using System;
using CSGenio.framework;
using CSGenio.persistence;
using System.Collections.Generic;

namespace CSGenio.business
{
    /// <summary>
    /// Genio Logical type representation
    /// Can be either a boolean or an int
    /// </summary>
    public class Logical  {

        public Logical(bool b)
        {
            boolVal = b;
        }

        public Logical(int i)
        {
            boolVal = (i != 0);
        }

        private readonly bool boolVal;        

        public static implicit operator bool(Logical logical) => logical.boolVal;
        public static implicit operator int(Logical logical) => logical.boolVal ? 1 : 0;
        public static implicit operator Logical(bool b) => new Logical(b);
        public static implicit operator Logical(int i) => new Logical(i);

    }

    /// <summary>
    /// Descreve os tipos possíveis de fórmulas condition.
    /// </summary>
    public class ConditionFormula : Formula
    {
        /// <summary>
        /// função que vai ser invocada
        /// </summary>
        /// <param name="args">parametro da function</param>
        /// <returns>1 se a condition é válida, 0 caso contrário</returns>
        public delegate Logical Function(object[] args,User user,string module,PersistentSupport sp);

        /// <summary>
        /// variável que vai ter a função
        /// </summary>
        private Function function;

        /// <summary>
        /// lista de argumentos por área
        /// </summary>
        private List<ByAreaArguments> argumentosPorArea;
        private int nrArguments; //número de argumentos da função
       

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nomeCampos">Name dos fields que são argumentos</param>
        /// <param name="f">função que corresponde à fórmula</param>
        public ConditionFormula(List<ByAreaArguments> argumentosPorArea,int nrArguments, Function f)
        {
            this.function = f;
            this.argumentosPorArea = argumentosPorArea;
            this.nrArguments = nrArguments;
 
        }

        /// <summary>
        /// Função recebe como argumento o Qvalue dos fields 
        /// </summary>
        /// <param name="valorCampos">Qvalue dos fields</param>
        /// <returns>o Qvalue da formula</returns>
         //SO 2007.05.29 adicionei o module
        public bool calculateFormulaCondition(object[] fieldsValue,User user,string module,PersistentSupport sp)
        {
			try
			{
				// SO 2007.05.29 adicionei o module
				return function(fieldsValue,user,module,sp);
			}
			catch (GenioException ex)
			{
				throw new BusinessException(ex.UserMessage, "ConditionFormula.calculaFormulaCondicao", "Error computing conditional formula: " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				throw new BusinessException(null, "ConditionFormula.calculaFormulaCondicao", "Error computing conditional formula: " + ex.Message, ex);
			}            
        }
		
		/// <summary>
        /// Execute the condition formula
        /// </summary>
        /// <param name="area">An area with the necessary values</param>
        /// <param name="sp">The persistent support to get the data</param>
        /// <param name="tpFunction">Function type</param>
		/// <param name="fdc">Formula DB Context</param>
        public bool ExecuteCondition(Area area, PersistentSupport sp, FunctionType tpFunction, FormulaDbContext fdc)
        {
            object[] fieldsValue = returnValueFieldsInternalFormula(area, ByAreaArguments, sp, fdc, ParameterCount, tpFunction);
            return calculateFormulaCondition(fieldsValue, area.User, area.Module, sp);
		}

        /// <summary>
        /// Execute the condition formula
        /// </summary>
        /// <param name="area">An area with the necessary values</param>
        /// <param name="sp">The persistent support to get the data</param>
        /// <param name="tpFunction">Function type</param>
		//[Obsolete("Please use the overload that uses FormulaDbContext for a more efficient calculation")]
        public bool ExecuteCondition(Area area, PersistentSupport sp, FunctionType tpFunction)
        {
            object[] fieldsValue = returnValueFieldsInternalFormula(area, ByAreaArguments, sp, ParameterCount, tpFunction);
            return calculateFormulaCondition(fieldsValue, area.User, area.Module, sp);
        }


        /// <summary>
        /// Name dos fields que servem de argumentos
        /// </summary>
        public List<ByAreaArguments> ByAreaArguments
        {
            get { return argumentosPorArea; }
            set { argumentosPorArea = value; }
        }

        /// <summary>
        /// Name das tables dos fields que servem de argumentos
        /// </summary>
        public int ParameterCount
        {
            get { return nrArguments; }
            set { nrArguments = value; }
        }

      
        /// <summary>
        /// Aviso mostrado ao user quando a condição de escrita impede o 
        /// preenchimento do Qfield
        /// </summary>
        public string ErrorWarning
        {
            get; set;
        }

        /// <summary>
        /// Se a condição de escrita deve ser validada no "Apply"
        /// </summary>
        public bool Validate
        {
            get; set;
        }

         /// <summary>
        /// Condition type
        /// </summary>
        public ConditionType Type
        {
            get; set;
        }

        /// <summary>
        /// The condition field. On mandantory conditions it will be this field that will be checked
        /// </summary>
        public Field Field
        {
            get;set;
        }

        /// <summary>
        /// Returns the message in the user language
        /// </summary>
        public string GetMessage(User user)
        {
            //If no custom message is given we need to get a predefined one
            if(String.IsNullOrEmpty(ErrorWarning))
            {
                if(Field != null)
                {
                    string msg = Translations.GetByCode("A_CONDITION_FOR_FIEL29176", user.Language);
                    string fieldDesc = Translations.Get(Field.FieldDescription, user.Language);
                    return string.Format(msg, fieldDesc);
                }
                else
                {
                    return Translations.GetByCode("A_CONDITION_IN_THIS_08270", user.Language);
                }
            }

            return Translations.Get(ErrorWarning, user.Language);
        }

        public bool IsCrudCondition()
        {
            switch(Type)
            {
                case ConditionType.VIEW:
                case ConditionType.UPDATE:
                case ConditionType.DELETE:
                case ConditionType.INSERT:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsWriteCondition()
        {
            switch (Type)
            {
                case ConditionType.ERROR:
                case ConditionType.MANDATORY:
                case ConditionType.WARNING:
                case ConditionType.SAVE:
                    return true;
                default:
                    return false;
            }
        }
    }    

    /// <summary>
    /// Descreve os tipos possíveis de fórmulas condition de duplicação.
    /// </summary>
    public class DupConditionFormula : Formula
    {
        /// <summary>
        /// função que vai ser invocada
        /// </summary>
        /// <param name="args">parametro da function</param>
        /// <returns>1 se a condition é válida, 0 caso contrário</returns>
        public delegate Logical Function(object[] args, User user, string module, PersistentSupport sp);

        /// <summary>
        /// variável que vai ter a função
        /// </summary>
        private Function function;

        /// <summary>
        /// lista de argumentos por área
        /// </summary>
        private List<ByAreaArguments> argumentosPorArea;
        private int nrArguments; //número de argumentos da função


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nomeCampos">Name dos fields que são argumentos</param>
        /// <param name="f">função que corresponde à fórmula</param>
        public DupConditionFormula(List<ByAreaArguments> argumentosPorArea, int nrArguments, Function f)
        {
            this.function = f;
            this.argumentosPorArea = argumentosPorArea;
            this.nrArguments = nrArguments;

        }

        /// <summary>
        /// Função recebe como argumento o Qvalue dos fields 
        /// </summary>
        /// <param name="valorCampos">Qvalue dos fields</param>
        /// <returns>o Qvalue da formula</returns>
         //SO 2007.05.29 adicionei o module
        public bool calculateFormulaCondition(object[] fieldsValue, User user, string module, PersistentSupport sp)
        {
            try
            {
                // SO 2007.05.29 adicionei o module
                return function(fieldsValue, user, module, sp);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "ConditionFormula.calculaFormulaCondicao", "Error computing conditional formula: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "ConditionFormula.calculaFormulaCondicao", "Error computing conditional formula: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Execute the condition formula
        /// </summary>
        /// <param name="area">An area with the necessary values</param>
        /// <param name="sp">The persistent support to get the data</param>
        /// <param name="tpFunction">Function type</param>
        public bool ExecuteCondition(Area area, PersistentSupport sp, FunctionType tpFunction)
        {
            object[] fieldsValue = returnValueFieldsInternalFormula(area, ByAreaArguments, sp, ParameterCount, tpFunction);
            return calculateFormulaCondition(fieldsValue, area.User, area.Module, sp);
        }


        /// <summary>
        /// Name dos fields que servem de argumentos
        /// </summary>
        public List<ByAreaArguments> ByAreaArguments
        {
            get { return argumentosPorArea; }
            set { argumentosPorArea = value; }
        }

        /// <summary>
        /// Name das tables dos fields que servem de argumentos
        /// </summary>
        public int ParameterCount
        {
            get { return nrArguments; }
            set { nrArguments = value; }
        }


        /// <summary>
        /// The condition field. On mandantory conditions it will be this field that will be checked
        /// </summary>
        public Field Field
        {
            get; set;
        }

        /// <summary>
        /// The area that invokes this condition.
        /// </summary>
        public string CondArea
        {
            get; set;
        }
    }

    public enum ConditionType {
        ERROR,
        WARNING,
        MANDATORY, 
        INSERT,
        UPDATE,
        VIEW,
        DELETE,
        SAVE
    }


}

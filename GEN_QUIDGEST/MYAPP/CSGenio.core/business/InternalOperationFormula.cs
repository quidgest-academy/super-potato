using System;
using CSGenio.framework;
using CSGenio.persistence;
using System.Collections.Generic;

namespace CSGenio.business
{
	/// <summary>
	/// Describes the possible types of internal formulas.
	/// </summary>
    public class InternalOperationFormula : Formula
	{
        public delegate object Function(object[] args,User user, string module,PersistentSupport sp);//function that will be invoked
        public Function function;//Variable that will have the function
        private List<ByAreaArguments> argumentosPorArea;//Argument list by area
        private int nrArguments; //Number of function arguments

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="argumentosPorArea">Argument list by area</param>
        /// <param name="f">function that corresponds to the formula</param>
        public InternalOperationFormula(List<ByAreaArguments> argumentosPorArea, int nrArguments, Function f)
        {
            this.function = f;
            this.argumentosPorArea = argumentosPorArea;
            this.nrArguments = nrArguments;
        }

        /// <summary>
        /// Calculates The value of this formula for the given row
        /// </summary>
        /// <param name="area">The row</param>
        /// <param name="sp">Persistent Support</param>
        /// <param name="fdc">The formula context to optimize database access</param>
        /// <param name="tpFunction">Operation type for the calculation</param>
        /// <returns>The Calculated Value</returns>
        public object calculateInternalFormula(Area area, PersistentSupport sp, FormulaDbContext fdc, FunctionType tpFunction)
        {
            try
            {
                object[] fieldsValue = returnValueFieldsInternalFormula(area, argumentosPorArea, sp, fdc, nrArguments, tpFunction);
                return function(fieldsValue, area.User, area.Module, sp);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "InternalOperationFormula.calculaFormulaInterna", "Error computing internal formula: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "InternalOperationFormula.calculaFormulaInterna", "Error computing internal formula: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Name of the fields that serve as arguments
        /// </summary>
        public List<ByAreaArguments> ByAreaArguments
        {
            get { return argumentosPorArea; }
            set { argumentosPorArea = value; }
        }

        /// <summary>
        /// Name of the tables of fields that serve as arguments
        /// </summary>
        public int ParameterCount
        {
            get { return nrArguments; }
            set { nrArguments = value; }
        }
	}
}

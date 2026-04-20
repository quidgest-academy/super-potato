using System;
using System.Text;
using CSGenio.framework;
using System.Data.SqlTypes;

namespace CSGenio.persistence
{
	/// <summary>
	/// Summary description for Query.
	/// </summary>
    //SO 20060814 alterações na classe
	[Obsolete]
	public abstract class Query
	{
		/// <summary>
		/// Create a condição to ser usada no where de acordo com o tipo de dados
		/// </summary>
		/// <param name="nomeCampo">Name do Qfield</param>
		/// <param name="valorCampo">Value do Qfield</param>
		/// <param name="format">Formato do Qfield</param>
		/// <param name="tipoLigacao">Type de ligação</param>
		/// <returns></returns>
        /// 
        //SO 20060816
		public static string createCondition(string fieldName,object fieldValue,FieldFormatting format,string sign)
        {
            return fieldName + sign + DBConversion.FromInternal(fieldValue, format);			
		}

		

		
	}
}

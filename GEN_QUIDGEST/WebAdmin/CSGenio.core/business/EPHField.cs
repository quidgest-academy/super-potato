using System;
using System.Collections.Generic;

namespace CSGenio.framework
{
    public class EPHField
    {
        private string name;
        private string table;
        private string Qfield;
        private string operador;
        private string table2;
        private string Qfield2;
        private string operador2;
        private bool or_EPH1_EPH2;
        private bool propagate;

		/// <summary>
		/// 
		/// </summary>
        public EPHField(string name, string table, string Qfield, string operador, bool propagate = false)
        {
            this.name = name;
            this.table = table;
            this.Qfield = Qfield;
            this.operador = operador;
            this.propagate = propagate;
        }
        public EPHField(string name, string table, string Qfield, string operador, string table2, string Qfield2, string operador2, bool or_EPH1_EPH2, bool propagate = false)
        {
            this.name = name;
            this.table = table;
            this.Qfield = Qfield;
            this.operador = operador;
            this.table2 = table2;
            this.Qfield2 = Qfield2;
            this.operador2 = operador2;
            this.or_EPH1_EPH2 = or_EPH1_EPH2;
            this.propagate = propagate;
        }
        
		/// <summary>
		/// 
		/// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Table
        {
            get { return table; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Field
        {
            get { return Qfield; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Operator
        {
            get { return operador; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Table2
        {
            get { return table2; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Field2
        {
            get { return Qfield2; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Operator2
        {
            get { return operador2; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool OR_EPH1_EPH2
        {
            get { return or_EPH1_EPH2; }
        }

        /// <summary>
        /// Indicate whether this EPH field should be propagated in-depth
        /// </summary>
        public bool Propagate
        {
            get { return propagate; }
        }
    }
}

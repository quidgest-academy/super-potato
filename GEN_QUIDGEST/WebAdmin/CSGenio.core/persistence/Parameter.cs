using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace CSGenio.persistence
{
    /// <summary>
    /// RR 24-02-2011
    /// Classe que representa um parametro a usar numa query à BD.
    /// Por agora vai servir apenas to binários, mas pode-se
    /// extender to se utilizar com todo o tipo de fields, o que
    /// torna as queries mais eficientes e reduz a probabilidade
    /// de existirem problemas com as formatações dos Qvalues.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Name do parametro
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Value do parametro
        /// </summary>
        public object Value { get; set; }
        
        /// <summary>
        /// Type do parametro
        /// </summary>
        public DbType Type { get; set; }

        /// <summary>
        /// Constructor do parametro
        /// </summary>
        /// <param name="name">Name do parametro</param>
        /// <param name="value">Value do parametro</param>
        /// <param name="size">Size do parametro</param>
        /// <param name="type">Type do parametro</param>
        public Parameter(string name, object value, DbType type)
        {
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }
    }
}

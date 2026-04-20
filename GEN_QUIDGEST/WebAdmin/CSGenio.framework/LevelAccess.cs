using System;

namespace CSGenio.framework
{
	/// <summary>
	/// Descreve os tipos possíveis de fórmulas internas.
	/// </summary>
	[Serializable]
	public sealed class LevelAccess
	{
        public readonly static LevelAccess DESAUTORIZADO = new LevelAccess(0);		    // Desautorizado
        public readonly static LevelAccess NV0 = new LevelAccess(0); //Desautorizado
        public readonly static LevelAccess NV1 = new LevelAccess(1); //Consulta
        public readonly static LevelAccess NV2 = new LevelAccess(2); //Officer
        public readonly static LevelAccess NV3 = new LevelAccess(3); //Agent
        public readonly static LevelAccess NV99 = new LevelAccess(99); //Administrador

        private readonly int levelValue;	// identifier do level de acesso
        private readonly string id;

		/// <summary>
		/// Construtor privado usado na inicialização dos membros
		/// </summary>
		/// <param name="anID">String identificadora do tipo de fórmula</param>
		public LevelAccess(int levelValue)
		{
			this.id = levelValue.ToString();
            this.levelValue = levelValue;
		}
        /// <summary>
        /// Método que dada uma string correspondente ao level devolve o level de acesso
        /// se nao exists fica desautorizado
        /// </summary>
        /// <param name="valorNivel">Qvalue do level em string</param>
        /// <returns>QLevel de acesso</returns>
        [Obsolete("This is only used to create a new undeclared role dynamically, which is never needed, and if it is should be done by a constructor")]
        public static LevelAccess returnAccessLevel(string levelValue)
        {
            int Qvalue;
            int.TryParse(levelValue, out Qvalue);
            return new LevelAccess(Qvalue);
        }

		/// <summary>
		/// Converte o objecto em string
		/// </summary>
		/// <returns>Identificador do tipo de fórmula</returns>
		public override string ToString()
		{
			return this.id;
		}

		/// <summary>
        /// Compara objectos da classe LevelAccess
		/// </summary>
		/// <param name="obj">Objecto a comparar</param>
		/// <returns>true se é igual, false caso contrário</returns>
		public override bool Equals(object obj)
		{
			if(obj is LevelAccess tf)
				return tf.levelValue.Equals(levelValue);
			return false;
		}

		/// <summary>
		/// Override obrigatório da classe Object
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return levelValue.GetHashCode ();
		}

        /// <summary>
        /// Método que devolve o Qvalue do level
        /// </summary>
        public int LevelValue
        {
            get{return levelValue;}
        }
        
        /// <summary>
        /// Verificar se um nível de acesso é maior do que o do argumento
        /// </summary>
        /// <param name="nivel"></param>
        /// <returns></returns>
        public bool greaterThan(LevelAccess level)
        {
            return this.levelValue > level.LevelValue;
        }

        /// <summary>
        /// Verificar se um nível de acesso é maior ou igual do que o do argumento
        /// </summary>
        /// <param name="nivel"></param>
        /// <returns></returns>
        public bool greaterOrEqualThan(LevelAccess level)
        {
            return this.levelValue >= level.LevelValue;
        }

        /// <summary>
        /// Verificar se um nível de acesso é menor do que o do argumento
        /// </summary>
        /// <param name="nivel"></param>
        /// <returns></returns>
        public bool lessThan(LevelAccess level)
        {
            return this.levelValue < level.LevelValue;
        }

        /// <summary>
        /// Verificar se um nível de acesso é menor ou igual do que o do argumento
        /// </summary>
        /// <param name="nivel"></param>
        /// <returns></returns>
        public bool lessOrEqualThan(LevelAccess level)
        {
            return this.levelValue <= level.LevelValue;
        }
	}
}


using System;

namespace CSGenio.framework
{
	/// <summary>
	/// Classe que representa um par de strings.
	/// Não se usa a class Par porque o método equals não compara o conteúdo
	/// </summary>
	public class Par
	{
        private object primeiro;
        private object second;

        /// <summary>
		/// Constructor da classe
		/// </summary>
		/// <param name="primeiro"></param>
		/// <param name="segundo"></param>
		public Par(object primeiro,object second)
        {
            this.primeiro = primeiro;
            this.second = second;
        }
		
		/// <summary>
		/// Implementação do método equals to o tipo Par
		/// </summary>
		/// <param name="obj">objecto a comparar</param>
		/// <returns>true se forem iguais e false caso contrário</returns>
		public override bool Equals(object obj)
		{
			if(obj is Par)
			{
				Par par = (Par)obj;
				if(par.First.Equals(this.primeiro) && par.Second.Equals(this.second))
					return true;
				else
					return false;
			}
			else
				return false;
		}
		
		/// <summary>
		/// Override do método GetHashCode do tipo Par
		/// </summary>
		/// <returns>devolve o hashcode do objecto Par</returns>
		public override int GetHashCode()
		{
			return this.First.GetHashCode();
		}

        /// <summary>
        /// Método que devolve o primeiro
        /// </summary>
        public object First
        {
            get { return primeiro; }
            set { primeiro = value; }
        }

        /// <summary>
        /// Método que devolve o second
        /// </summary>
        public object Second
        {
            get { return second; }
            set { second = value; }
        } 
	}
}

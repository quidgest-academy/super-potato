using System;

namespace CSGenio.framework
{
    public class ReplicaDestination
    {
		private string sistemaDestinoReplica; // system da table de target da replica
        private string tabelaDestinoReplica;	// name da table de target da réplica
        private string chaveEstrangeira;	// name da key estrangeira
        private string campoDestinoReplica;	// Qfield de target da réplica

		/// <summary>
		/// Construtor
		/// </summary>
		/// <param name="sistemaDestinoReplica">system da table de target</param>
		/// <param name="tabelaDestinoReplica">name da table de target</param>
		/// <param name="chaveEstrangeira">name da key estrangeira</param>
		/// <param name="campoDestinoReplica">Qfield de target da réplica</param>
		public ReplicaDestination(string sistemaDestinoReplica, string tabelaDestinoReplica, string chaveEstrangeira, string campoDestinoReplica)
		{
			this.sistemaDestinoReplica = sistemaDestinoReplica;
			this.tabelaDestinoReplica = tabelaDestinoReplica;
			this.chaveEstrangeira = chaveEstrangeira;
			this.campoDestinoReplica = campoDestinoReplica;
		}

        /// <summary>
        /// QSystem da table do fields de target da réplica
        /// </summary>
        public string ReplicaDestinationSystem
        {
            get { return sistemaDestinoReplica; }
            set { sistemaDestinoReplica = value; }
        }

        /// <summary>
        /// Name do alias da table do fields de target da réplica
        /// </summary>
        public string ReplicaDestinationTable
        {
            get { return tabelaDestinoReplica; }
            set { tabelaDestinoReplica = value; }
        }

        /// <summary>
        /// Name da key estrangeira
        /// </summary>
        public string ForeignKey
        {
            get { return chaveEstrangeira; }
            set { chaveEstrangeira = value; }
        }

        /// <summary>
        /// Name do Qfield de target da réplica
        /// </summary>
        public string ReplicaTargetFields
        {
            get { return campoDestinoReplica; }
            set { campoDestinoReplica = value; }
        }
    }
}

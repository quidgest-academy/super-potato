using System;
using System.Collections.Generic;
using CSGenio.framework;
using System.Collections;

namespace CSGenio.business
{
    /// <summary>
    /// Local de actualização de um registo em relação à sincronização
    /// </summary>
    public enum SyncType { Central, Local }

    /// <summary>
    /// Type de visibilidade to um Qfield ou table na consulta avancada
    /// </summary>
    public enum CavVisibilityType
    {
        /// <summary>
        /// Sempre visível na consulta avançada
        /// </summary>
        Sempre,
        /// <summary>
        /// Só na propria table (so to fields)
        /// </summary>
        Propria,
        /// <summary>
        /// Só em tables relacionadas
        /// </summary>
        Relacionada,
        /// <summary>
        /// Nunca é visivel na consulta avançada
        /// </summary>
        Nunca
    }

    /// <summary>
    /// Meta informação sobre uma área
    /// </summary>
    public class AreaInfo
    {
        //---------------------------------------------------------------------------------
        // To a sincronização:
        //---------------------------------------------------------------------------------
        private TimeSpan m_horaInicioSyncIncremental;
        /// <summary>
        /// Hora de start de tentativas de sincronização incremental
        /// </summary>
        public TimeSpan SyncIncrementalDateStart
        {
            get { return m_horaInicioSyncIncremental; }
            set { m_horaInicioSyncIncremental = value; }
        }

        private TimeSpan m_horaFimSyncIncremental;
        /// <summary>
        /// Hora de fim de tentativas de sincronização incremental
        /// </summary>
        public TimeSpan SyncIncrementalDateEnd
        {
            get { return m_horaFimSyncIncremental; }
            set { m_horaFimSyncIncremental = value; }
        }

        private TimeSpan m_periodoSyncIncremental;
        /// <summary>
        /// Periodo entre tentativas de sicronização incremental
        /// </summary>
        public TimeSpan SyncIncrementalPeriod
        {
            get { return m_periodoSyncIncremental; }
            set { m_periodoSyncIncremental = value; }
        }

        private TimeSpan m_horaSyncCompleta;
        /// <summary>
        /// Hora a que deve ocorrer a sincronização completa todos os days
        /// </summary>
        public TimeSpan SyncCompleteHour
        {
            get { return m_horaSyncCompleta; }
            set { m_horaSyncCompleta = value; }
        }

        private DateTime m_ultimoSync;
        /// <summary>
        /// Data da última sincronização
        /// </summary>
        public DateTime LastSync
        {
            get { return m_ultimoSync; }
            set { m_ultimoSync = value; }
        }

        private DateTime m_proximoSync;
        /// <summary>
        /// Data agendada to a proxima sincronização incremental
        /// </summary>
        public DateTime NextSync
        {
            get { return m_proximoSync; }
            set { m_proximoSync = value; }
        }

        private int m_batchSync;
        /// <summary>
        /// Size do número de registos sincronizados de cada vez
        /// </summary>
        public int BatchSync
        {
            get { return m_batchSync; }
            set { m_batchSync = value; }
        }

        private SyncType m_tipoSync;
        /// <summary>
        /// Type de sincronização
        /// </summary>
        public SyncType SyncType
        {
            get { return m_tipoSync; }
            set { m_tipoSync = value; }
        }
		
		private PersistenceType m_persistenceType;
        /// <summary>
        /// True se esta área representa o domínio, false caso seja uma área
        /// </summary>
        public PersistenceType PersistenceType
        {
            get { return m_persistenceType; }
            set { m_persistenceType = value; }
        }

        private bool m_isDominio;
        /// <summary>
        /// True se esta área representa o domínio, false caso seja uma área
        /// </summary>
        public bool IsDomain
        {
            get { return m_isDominio; }
            set { m_isDominio = value; }
        }

        private Dictionary<string, Field> m_camposBD = new Dictionary<string, Field>(100);
        /// <summary>
        /// Fields na BD
        /// </summary>
        public Dictionary<string, Field> DBFields
        {
            get { return m_camposBD; }
        }

        private List<Field> m_sequenciaCampos = new List<Field>();
        /// <summary>
        /// Lista de fields ordenada pela sequencia correcta
        /// </summary>
        public List<Field> DBFieldsList
        {
            get { return m_sequenciaCampos; }
        }

        /// <summary>
        /// Regista os metadados de um Qfield na informação desta área
        /// </summary>
        /// <param name="campo">O Qfield a registar</param>
        public void RegisterFieldDB(Field Qfield)
        {
            m_camposBD.Add(Qfield.Name, Qfield);
            m_sequenciaCampos.Add(Qfield);
        }

        private Dictionary<string, Relation> m_tabelasMae;
        /// <summary>
        /// Tabelas mãe desta.
        /// Tem apenas as relações directas.
        /// </summary>
        public Dictionary<string, Relation> ParentTables
        {
            get { return m_tabelasMae; }
            set { m_tabelasMae = value; }
        }

        private Dictionary<string, string> m_caminhos;
        /// <summary>
        /// Conjunto de caminhos a partir desta table.
        /// Tem todas as relações indirectas. To cada target diz por que table directamente
        ///  relacionada se deve ir to conseguir chegar ao target.
        /// </summary>
        public Dictionary<string, string> Pathways
        {
            get { return m_caminhos; }
            set { m_caminhos = value; }
        }

        /// <summary>
        /// Devolve o caminho completo to chegar desta area ao target
        /// </summary>
        /// <param name="destino">A area de target a chegar</param>
        /// <returns>Uma lista de relações to chegar ao target ou null caso não exista caminho</returns>
        public List<Relation> GetRelations(string target)
        {
            if (!Pathways.ContainsKey(target))
                return null;

            AreaInfo info = this;
            List<Relation> res = new List<Relation>();

            while (info != null)
            {
                //ver por onde temos de ir to chegar ao target
                string area = info.Pathways[target];
                //adicionar a parcela do caminho
                res.Add(info.ParentTables[area]);
                //se já estamos na área desejada sair
                if (target == area)
                    break;
                //consultar a proxima area
                info = Area.GetInfoArea(area);
            }

            return res;
        }

        private Relation[] m_relacoesDuplicacao;
        /// <summary>
        /// Relações de duplicação
        /// </summary>
        public Relation[] DuplicationRelations
        {
            get { return m_relacoesDuplicacao; }
            set { m_relacoesDuplicacao = value; }
        }

        private ChildRelation[] m_tabelasFilha;
        /// <summary>
        /// Relações com tables filhas
        /// </summary>
        public ChildRelation[] ChildTable
        {
            get { return m_tabelasFilha; }
            set { m_tabelasFilha = value; }
        }
        
        private string m_sistema;
        /// <summary>
        /// Schema a que pertence esta table
        /// </summary>
        public string QSystem
        {
            get { return m_sistema; }
            set { m_sistema = value; }
        }

        private string m_alias;
        /// <summary>
        /// alias (name da área)
        /// </summary>
        public string Alias
        {
            get { return m_alias; }
            set { m_alias = value; }
        }

        private string m_nomeTabela;
        /// <summary>
        /// Name da table
        /// </summary>
        public string TableName
        {
            get { return m_nomeTabela; }
            set { m_nomeTabela = value; }
        }

        private string m_nomeTabSombra;
        /// <summary>
        /// Name da table shadow
        /// </summary>
        public string ShadowTabName
        {
            get { return m_nomeTabSombra; }
            set { m_nomeTabSombra = value; }
        }

        private string m_nomeChavePrimaria;
        /// <summary>
        /// Name do Qfield key primária
        /// </summary>
        public string PrimaryKeyName
        {
            get { return m_nomeChavePrimaria; }
            set { m_nomeChavePrimaria = value; }
        }
		
		private string m_nomeChaveHumana;
        /// <summary>
        /// Name do Qfield key humana.
		/// Caso exista mais do que um Qfield de key humana, 
		/// os seus nomes aparecem separados por uma vírgula 
        /// </summary>
        public string HumanKeyName
        {
            get { return m_nomeChaveHumana; }
            set { m_nomeChaveHumana = value; }
        }

        /// <summary>
        /// Natural ordering field for this table.
        /// This can influence sorting defaults and order of processing during batches.
        /// </summary>
        public string MainOrderField { get; set; } = "";

        private string m_nomeChaveTabSombra;
        /// <summary>
        /// Name do Qfield key primária da table shadow
        /// </summary>
        public string ShadowTabKeyName
        {
            get { return m_nomeChaveTabSombra; }
            set { m_nomeChaveTabSombra = value; }
        }

        private ArrayList m_niveisTabSombra;
        /// <summary>
        /// Níveis cujas alterações são escritas na table shadow
        /// </summary>
        public ArrayList ShadowTabLevels
        {
            get { return m_niveisTabSombra; }
            set { m_niveisTabSombra = value; }
        }

        private Hashtable m_ephs;
        /// <summary>
        /// Eph por àrea
        /// </summary>
        public Hashtable Ephs
        {
            get { return m_ephs; }
            set { m_ephs = value; }
        }

        private string[] m_valoresDefault;
        /// <summary>
        /// Lista de fields com Qvalue default
        /// </summary>
        public string[] DefaultValues
        {
            get { return m_valoresDefault; }
            set { m_valoresDefault = value; }
        }

        private string[] m_valoresDefaultSequenciais;
        /// <summary>
        /// Lista de fields com Qvalue default de números sequenciais
        /// </summary>
        public string[] SequentialDefaultValues
        {
            get { return m_valoresDefaultSequenciais; }
            set { m_valoresDefaultSequenciais = value; }
        }

        private string[] m_camposReplica;
        /// <summary>
        /// Lista de fields que sao formulas replica
        /// </summary>
        public string[] ReplicaFields
        {
            get { return m_camposReplica; }
            set { m_camposReplica = value; }
        }

        private string[] m_camposArgumentosReplicas;
        /// <summary>
        /// Fields que são usados em réplicas
        /// </summary>
        public string[] FieldsParametersReplicas
        {
            get { return m_camposArgumentosReplicas; }
            set { m_camposArgumentosReplicas = value; }
        }

        private string[] m_camposConsultaTabela;
        /// <summary>
        /// Fields que são consultas a tables
        /// </summary>
        public string[] CheckTableFields
        {
            get { return m_camposConsultaTabela; }
            set { m_camposConsultaTabela = value; }
        }

        private string[] m_camposFimPeriodo;
        /// <summary>
        /// Lista de fields que sao formulas fim de período
        /// </summary>
        public string[] EndofPeriodFields
        {
            get { return m_camposFimPeriodo; }
            set { m_camposFimPeriodo = value; }
        }

        private string[] m_camposCarimboIns;
        /// <summary>
        /// Lista de fields que sao carimbados pelo programa 
        /// (datacria, opercria, horacria)
        /// </summary>
        public string[] StampFieldsIns
        {
            get { return m_camposCarimboIns; }
            set { m_camposCarimboIns = value; }
        }

        private string[] m_camposCarimboAlt;
        /// <summary>
        /// Lista de fields que sao carimbados pelo programa na alteração
        /// (datamuda, operChange, horamuda)
        /// </summary>
        public string[] StampFieldsAlt
        {
            get { return m_camposCarimboAlt; }
            set { m_camposCarimboAlt = value; }
        }

        private string[] m_camposOperacaoInterna;
        /// <summary>
        /// Fields que são operações internas
        /// </summary>
        public string[] InternalOperationFields
        {
            get { return m_camposOperacaoInterna; }
            set { m_camposOperacaoInterna = value; }
        }

        private string[] m_camposOperacaoInternaSequenciais;
        /// <summary>
        /// Fields que são operações internas que dependem de números sequenciais
        /// </summary>
        public string[] InternalOperationSequentialFields
        {
            get { return m_camposOperacaoInternaSequenciais; }
            set { m_camposOperacaoInternaSequenciais = value; }
        }

        private string[] m_camposSomaRelacionada;
        /// <summary>
        /// Fields que são somas relacionadas
        /// </summary>
        public string[] RelatedSumFields
        {
            get { return m_camposSomaRelacionada; }
            set { m_camposSomaRelacionada = value; }
        }

        private List<RelatedSumArgument> m_argsSomaRelac;
        /// <summary>
        /// Fields que sao argumentos de formulas que são somas relacionadas
        /// </summary>
        public List<RelatedSumArgument> RelatedSumArgs
        {
            get { return m_argsSomaRelac; }
            set { m_argsSomaRelac = value; }
        }

        //created by [AJA] at [2014.06.06] - concatenha linhas
        //last updated by [    ] at [    .  .  ]
        //last reviewed by [    ] at [    .  .  ]
        private string[] m_camposListAggregate;
        /// <summary>
        /// Fields que são últimos Qvalues
        /// </summary>
        public string[] AggregateListFields
        {
            get { return m_camposListAggregate; }
            set { m_camposListAggregate = value; }
        }

        private List<ListAggregateArgument> m_argsListAggregate;
        /// <summary>
        /// Fields que sao argumentos de formulas que list aggregate
        /// </summary>
        public List<ListAggregateArgument> ArgsListAggregate
        {
            get { return m_argsListAggregate; }
            set { m_argsListAggregate = value; }
        }

        private string[] m_camposUltimoValor;
        /// <summary>
        /// Fields que são últimos Qvalues
        /// </summary>
        public string[] LastValueFields
        {
            get { return m_camposUltimoValor; }
            set { m_camposUltimoValor = value; }
        }

        private List<LastValueArgument> m_argsUltValor;
        /// <summary>
        /// Fields que sao argumentos de formulas que são relações de último Qvalue
        /// </summary>
        public List<LastValueArgument> LastValueArgs
        {
            get { return m_argsUltValor; }
            set { m_argsUltValor = value; }
        }

        private List<History> m_listaHistoricos;
        /// <summary>
        /// Tabelas e Fields usados em CreateHist
        /// </summary>
        public List<History> HistoryList
        {
            get { return m_listaHistoricos; }
            set { m_listaHistoricos = value; }
        }

        private List<String> m_chavesEstrangeirasDocums;
        /// <summary>
        /// Lista de chaves estrangeiras to a table DOCUMS
        /// </summary>
        public List<String> DocumsForeignKeys
        {
            get { return m_chavesEstrangeirasDocums; }
            set { m_chavesEstrangeirasDocums = value; }
        }

        private bool m_temGestaoVersoes;
        /// <summary>
        /// Se têm gestão de versões
        /// </summary>
        public bool HasVersionManagment
        {
            get { return m_temGestaoVersoes; }
            set { m_temGestaoVersoes = value; }
        }

        private QLevel m_nivel;
        /// <summary>
        /// QLevel da table to as várias operações
        /// </summary>
        public QLevel QLevel
        {
            get { return m_nivel; }
            set { m_nivel = value; }
        }

        private string m_designacaoArea;
        /// <summary>
        /// Designação da área
        /// </summary>
        public string AreaDesignation
        {
            get { return m_designacaoArea; }
            set { m_designacaoArea = value; }
        }
		
		private string m_designacaoPluralArea;
        /// <summary>
        /// Designação Plural da área
        /// </summary>
        public string AreaPluralDesignation
        {
            get { return m_designacaoPluralArea; }
            set { m_designacaoPluralArea = value; }
        }

        private TreeTable m_tabelaArvore;
        /// <summary>
        /// Informação sobre a table em árvore caso seja
        /// </summary>
        public TreeTable TreeTable
        {
            get { return m_tabelaArvore; }
            set { m_tabelaArvore = value; }
        }

        /// <summary>
        /// Dicionário de registos desta table controlados pela aplicação
        /// </summary>
        public ControlledRecords ControlledRecords
        {
            get { return m_registosControlados; }
            set { m_registosControlados = value; }
        }
        private ControlledRecords m_registosControlados = null;

        /// <summary>
        /// inteiro identifier da db auxiliar usada to esta table
        /// </summary>
        public int Mdb
        {
            get { return m_mdb; }
            set { m_mdb = value; }
        }
        private int m_mdb = 0;

        private SumsCreatesRecords[] m_somamCriamRegistos;
        /// <summary>
        /// Conjunto de caminhos a partir desta table
        /// </summary>
        public SumsCreatesRecords[] SumCreateRecords
        {
            get { return m_somamCriamRegistos; }
            set { m_somamCriamRegistos = value; }
        }

        private string[] m_todosCamposObter;
        /// <summary>
        /// Lista de fields que têm que ser obtidos
        /// </summary>
        public string[] ObtainAllFields
        {
            get { return m_todosCamposObter; }
            set { m_todosCamposObter = value; }
        }
		
		private ConditionFormula m_proibeRecalculoSe;
		/// <summary>
        /// Condição que indica quando uma table pode ser recalculada
        /// </summary>
		public ConditionFormula ForbidsRecalculationIf
		{
            get { return m_proibeRecalculoSe; }
            set { m_proibeRecalculoSe = value; }
        }
		
        public List<ConditionFormula> WriteConditions {  get;set; } = new List<ConditionFormula>();

        public List<ConditionFormula> CrudConditions {  get;set; } = new List<ConditionFormula>();
        public List<DupConditionFormula> DupConditions {  get;set; } = new List<DupConditionFormula>();

        private string[] m_passwordFields;
        /// <summary>
        /// Password-type fields
        /// </summary>
        public string[] PasswordFields
        {
            get { return m_passwordFields; }
            set { m_passwordFields = value; }
        }
    //---------------------------------------------------------------------------------
    // To o message queueing:
    //---------------------------------------------------------------------------------
    
    private bool m_MsqActive = false;
    /// <summary>
    /// Flag to marcar se pelo menos uma das queues a exportar está configurada
    /// </summary>
    public bool MsqActive 
    {
        get 
        {
            m_MsqActive = ActiveQueuesList != null && ActiveQueuesList.Count > 0;
            return m_MsqActive;
        } 
    }

    private List<GenioServer.business.QueueGenio> m_listaActiveQueues;
    /// <summary>
    /// Lista de todas as queues a exportar e configuradas no configuracoes.xml
    /// </summary>
    public List<GenioServer.business.QueueGenio> ActiveQueuesList
    {
        get
        {
            if (m_listaActiveQueues != null && m_listaActiveQueues.Count > 0)
                return m_listaActiveQueues;

            if (Configuration.MessageQueueing == null)
                return m_listaActiveQueues;
            if (Configuration.MessageQueueing.Queues == null)
                return m_listaActiveQueues;

            m_listaActiveQueues = new List<GenioServer.business.QueueGenio>();
			if(m_listaQueues == null)
                return m_listaActiveQueues;
            foreach (GenioServer.business.QueueGenio queue in m_listaQueues)
            {
                CSGenio.Queue queueOBJ = Configuration.MessageQueueing.Queues.Find(p => p.queue.Equals(queue.Name));
                if (queueOBJ != null) {
                    m_listaActiveQueues.Add(queue);
                    m_MsqActive = true;
                }
                    
            }
            return m_listaActiveQueues;
        }
    } 
	
	private List<GenioServer.business.QueueGenio> m_listaQueues;
    /// <summary>
    /// Lista de todas as queues a exportar
    /// </summary>
    public List<GenioServer.business.QueueGenio> QueuesList
    {
        get { return m_listaQueues; }
        set { m_listaQueues = value; }
    }
		
    private List<string> m_listaSolr;
    /// <summary>
    /// Lista de todas as queues a exportar
    /// </summary>
    public List<string> SolrList
    {
        get { return m_listaSolr; }
        set { m_listaSolr = value; }
    }
        
    //---------------------------------------------------------------------------------
    // To a consulta avançada:
    //---------------------------------------------------------------------------------
    private CavVisibilityType m_visivelCav;
    /// <summary>
    /// Regra de visibilidade to esta table na consulta avançada
    /// </summary>
    public CavVisibilityType VisivelCav
    {
        get { return m_visivelCav; }
        set { m_visivelCav = value; }
    }
	
	private string m_DescriptionCav;
    /// <summary>
    /// CAV table description resource Id
    /// </summary>
    public string DescriptionCav
        {
        get { return m_DescriptionCav; }
        set { m_DescriptionCav = value; }
    }
        
    //---------------------------------------------------------------------------------
    // Type de Key da table:
    //--------------------------------------------------------------------------------
    public FieldType KeyType
    {
        get { return DBFields[PrimaryKeyName].FieldType; }
    }
    }

    
}

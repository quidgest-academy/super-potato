using System.Collections.Generic;

namespace GenioServer.business
{
    public enum CallAck
    {
        OK,
        Rejected,
        Failed
    }

    // Status das Queues da MQQueues
    /// Define o estado a messagem.
    public enum MQueueACK
    {
        //to carimbar o status durante o envio

        /// <summary>
        /// quando a queue deu erro a ser enviada to o servidor de queues.
        /// </summary>
        SendFAIL = 0,

        /// <summary>
        /// quando a queue foi enviada com sucesso to o servidor de queues e está 
        /// apenas à espera da resposta do integrador do system target.
        /// </summary>
        SendINPROGRESS = 1,

        /// <summary>
        /// quando a queue foi enviada com sucesso to o servidor de queues e está
        /// apenas à espera da resposta do integrador do system target.
        /// </summary>
        SendEXPIRED = 2,

        //to carimbar o status da resposta

        /// <summary>
        /// quando a queue chegou ao integrador de target e foi integrada correctamente.
        /// </summary>
        ReplyOK = 3,

        /// <summary>
        /// quando a queue foi rejeitada pelo system target(integrador), nesta situação ver mensagem descritiva. 
        /// </summary>
        ReplyREJECT = 4,

        /// <summary>
        /// quando ocorreu um erro técnico a processar a queue pelo system target(integrador), nesta situação ver mensagem descritiva. 
        /// </summary>
        ReplyFAIL = 5,

        //to carimbar o status do ACK Enviado em reposta da queue recebida

        /// <summary>
        /// quando é envida a Queue de ACK to a source da queue a originou o ACK
        /// </summary>
        SendReplyOK = 6,

        /// <summary>
        /// serve to inibir o envio de ACK. Permite em queues que são broadcasted to multiplos sistemas que alguns não enviem a resposta.
        /// </summary>
        ReplyIGNORE = 7,

        /// <summary>
        /// serve para indicar o progresso do processemto da queue.
        /// </summary>
        ReplyPROGRESS = 8
    }

    public class QueueResponse
    {
        public string MsgId;
        public string Desc;
        public int progress;        
        public MQueueACK Ack;
        public string AckQueue;

        public QueueResponse()
        {
            Desc = "";
            progress = 0;
        }
    }
    public class NQueueTables
    {
        private string table;
        private string Qfield;
        private string tabeladominio;

        public NQueueTables(string table, string Qfield)
        {
            this.table = table;
            this.Qfield = Qfield;
        }

        public NQueueTables(string table, string Qfield, string tabeladominio)
        {
            this.table = table;
            this.Qfield = Qfield;
            this.tabeladominio = tabeladominio;
        }

        public string Table
        {
            get { return table; }
            set { table = value; }
        }

        public string Field
        {
            get { return Qfield; }
            set { Qfield = value; }
        }

        public string DomainTable
        {
            get { return tabeladominio; }
            set { tabeladominio = value; }
        }
    }

    /// <summary>
    /// classe que representa uma queue
    /// TODO: NA geracão associar todas as informações relacionadas (tables acima, tables abaixo) com as queue nesta classe. implica alteração do código de envio de queue 
    /// </summary>
    public class QueueGenio
    {
        private string name;
        private bool naoreexporta;
        private string queuearea;
        private string tabeladominio;
        private List<NQueueTables> tabelasN1;
        private List<NQueueTables> tabelas1N;

        public QueueGenio()
        {
            tabelas1N = new List<NQueueTables>();
            tabelasN1 = new List<NQueueTables>();
        }

        /// <summary>
        /// Devolve e atribui o name da queue
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Área da Queue
        /// </summary>
        public string Queuearea
        {
            get { return queuearea; }
            set { queuearea = value; }
        }

        /// <summary>
        /// Table de dóminio da area da queue
        /// </summary>
        public string DomainTable
        {
            get { return tabeladominio; }
            set { tabeladominio = value; }
        }

        /// <summary>
        /// Devolve e atribui a flag que indica se a queue é reexportada ou não
        /// </summary>
        public bool DoNotReexport
        {
            get { return naoreexporta; }
            set { naoreexporta = value; }
        }

        /// <summary>
        /// Devolve e atribui as tables N1 
        /// </summary>
        public List<NQueueTables> TablesN1
        {
            get { return tabelasN1; }
            set { tabelasN1 = value; }
        }

        /// <summary>
        /// Devolve e atribui as tables 1N
        /// </summary>
        public List<NQueueTables> Tables1N
        {
            get { return tabelas1N; }
            set { tabelas1N = value; }
        }
    }

    public class IsAckResponse
    {
        public bool IsACK { get; set; }
        public string QueueKey { get; set; }
    }

    public class QueueProgressStatus
    {
        public string Message { get; set; }
        public int Count { get; set; }
        public int Total { get; set; }
        public bool Completed { get; set; }
        public string id { get; set; }
    }
}

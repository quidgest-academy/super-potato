using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Administration.Models
{
    public class MessageQueueModel : ModelBase
    {        
        [Display(Name = "MESSAGE_QUEUEING34227", ResourceType = typeof(Resources.Resources))]
        public MessageQueue MQueues { get; set; }

        private QueueStatsModel _stats;
        [Display(Name = "ESTATISTICAS03241", ResourceType = typeof(Resources.Resources))]
        public QueueStatsModel Stats { get { if (_stats == null) _stats = new QueueStatsModel(); return _stats; } set { _stats = value; } }

        [Display(Name = "ESTADO_DA_OPERACAO38065", ResourceType = typeof(Resources.Resources))]
        public string ResultMsg { get; set; }

        [Display(Name = "DADOS_43180", ResourceType = typeof(Resources.Resources))]
        public bool LogDatabaseSelected { get; set; }
        public bool LogDatabaseExists { get; set; }
    }


    public class QueueStatsModel : ModelBase
    {
        [Display(Name = "DATA_DE_INICIO37610", ResourceType = typeof(Resources.Resources))]
        public DateTime StartDate { get; set; }

        [Display(Name = "DATA_DE_FIM18270", ResourceType = typeof(Resources.Resources))]
        public DateTime EndDate { get; set; }

        [Display(Name = "Queue")]
        public DateTime Queue { get; set; }

        public double ToSend
        {
            get
            {
                return StatLines.Sum(p => p.ToSend);
            }
        }

        public double Sended
        {
            get
            {
                if (Total > 0)
                {
                    return (StatLines.Sum(p => p.Sended) / Total) * 100;
                }
                return 0;
            }
        }

        public double Errors
        {
            get
            {
                if (Total > 0)
                {
                    return (StatLines.Sum(p => p.Errors) / Total) * 100;
                }
                return 0;
            }
        }

        public double Total
        {
            get{ return StatLines.Sum(p=>p.Total); }
        }

        private List<ItemQueueStats> _statLines;
        public List<ItemQueueStats> StatLines { get { if (_statLines == null) _statLines = new List<ItemQueueStats>(); return _statLines; } set { _statLines = value; } }

        private List<ItemQueueErrorStats> _erroStatLines;
        public List<ItemQueueErrorStats> ErroStatLines { get { if (_erroStatLines == null) _erroStatLines = new List<ItemQueueErrorStats>(); return _erroStatLines; } set { _erroStatLines = value; } }
    }

    public class ItemQueueErrorStats
    {
        public string QueueId { get; set; }

        public string mqstatus { get; set; }

        public string Errors { get; set; }

        public int Total { get; set; }
    }



    public class ItemQueueStats
    {
        public string QueueId { get; set; }

        public int ToSend { get; set; }

        public int Sended { get; set; }

        public int Errors { get; set; }

        public int Total { get; set; }


        public double ToSendPercentage 
        {
            get
            {
                if(Total > 0)
                {
                    return (ToSend * 100.0) / Total;
                }
                return 0.0;
            }
        }

        public double SendedPercentage
        {
            get
            {
                if (Total > 0)
                {
                    return Sended * 100.0 / Total;
                }
                return 0.0;
            }
        }

        public double ErrorsPercentage
        {
            get
            {
                if (Total > 0)
                {
                    return Errors * 100.0 / Total;
                }
                return 0.0;
            }
        }

    }
}
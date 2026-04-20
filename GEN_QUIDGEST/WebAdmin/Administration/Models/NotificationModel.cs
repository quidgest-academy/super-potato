using CSGenio.business;
using CSGenio.framework;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Administration.Models
{
    /// <summary>
    /// Interface Notification
    /// </summary>
    public class NotificationModel : ModelBase
    {

        /// <summary>
        /// Class to store information on Email Notification to be used when sending messages through email
        /// </summary>
		[Key]
		/// <summary>Campo : "PK da tabela" Tipo: "+" Formula:  ""</summary>
        public string codtpnot { get; set; } //Notification identifier
        /// <summary>
        /// Notification ID "ID de notificação"
        /// </summary>
		[Display(Name = "ID_DE_NOTIFICACAO24601", ResourceType = typeof(Resources.Resources))]
		public string IDNotif { get; set; } //Notification ID

        /// <summary>
        /// Notification Name 
        /// </summary>
		[Display(Name = "NOME47814", ResourceType = typeof(Resources.Resources))]
        public string Notif { get; set; } //Notification name
        /// <summary>
        /// Flag indication to send an email
        /// </summary>
        [Display(Name = "PERMITE_ENVIO_DE_EMA25939", ResourceType = typeof(Resources.Resources))]
        public bool SendsEmail { get; set; }
        /// <summary>
        /// Flag indication to send an alert (on webpage alert zone)
        /// </summary>
        [Display(Name = "PERMITE_ENVIO_DE_ALE16691", ResourceType = typeof(Resources.Resources))]
        public bool SendsAlert { get; set; }
        /// <summary>
        /// Flag indication to store the information sent on database
        /// </summary>
        [Display(Name = "PERMITE_ESCRITA_NA_B48768", ResourceType = typeof(Resources.Resources))]
        public bool SendsToDatabase { get; set; }
        /// <summary>
        /// List of pre-configured Messages set in Genios definitions
        /// </summary>
        [Display(Name = "NO_DE_MENSAGENS_CONF09230" , ResourceType = typeof(Resources.Resources))]
        public int NumMessagesConfig { get; set; }

        [JsonIgnore]
        public List<DbArea> NotifsOnBD { get; set; }

        public List<string> NotifsOnBD_headers { get; set; }
        public List<Dictionary<string, object>> NotifsOnBD_body { get; set; }

        public List<NotificationMessageModel> MessagesConfig { get; set; }

        public MessagesTableMap DatabaseFieldMapping { get; set; }

        public string FormMode { get; set; }
        public string ResultMsg { get; set; }

        /// <summary>
        /// Class FieldMap - mapping between names retrieved by the ManualQuery and the ones used on BD/Application (as placeholders)
        /// </summary>
        public class FieldMap
        {
            /// <summary>
            /// Field retrieved from ManualQuery
            /// </summary>
            public string FieldnameQuery { get; set; }
            /// <summary>
            /// Field to be used in app
            /// </summary>
            public string FieldnameApp { get; set; }
            /// <summary>
            /// Reserved field/Field from query
            /// </summary>
            public bool Reserved { get; set; }
        }

        /// <summary>
        /// Class to store information on BD (with fields to be mapped query vs BD namming)
        /// </summary>
        public class MessagesTableMap
        {
            public String MessagesTable;
            public String MessagesTablePKName;
            public List<FieldMap> TableFieldMap;
        }

        public void MapToModel(Notification m)
        {
            if (m == null)
            {
                Log.Error("Map ViewModel (NotificationModel) to Model (Notification) - Model is a null reference");
                throw new Exception("Model not found");
            }
            try
            {
               //
            }
            catch (Exception)
            {
                Log.Error("Map ViewModel (NotificationModel) to Model (Notification) - Error during mapping");
                throw;
            }
        }

        public void MapFromModel(Notification m)
        {
            if (m == null)
            {
                Log.Error("Map ViewModel (Notification) to Model (NotificationModel) - Model is a null reference");
                throw new Exception("Model not found");
            }
            try
            {
                codtpnot = m.codtpnot;
                Notif = m.Notif;
                IDNotif = m.IDNotif;
                SendsEmail = m.SendsEmail;
                SendsToDatabase = m.SendsToDatabase;
                NumMessagesConfig = m.MessagesConfig.Count;

                NotifsOnBD = m.NotifsOnBD;

                MessagesConfig = new List<NotificationMessageModel>();
                foreach (var messageconfig in m.MessagesConfig)
                {
                    NotificationMessageModel notificationMessageModel = new NotificationMessageModel();
                    notificationMessageModel.MapFromModel(messageconfig);
                    MessagesConfig.Add(notificationMessageModel);
                }

                DatabaseFieldMapping = new MessagesTableMap()
                {
                    MessagesTable = m.DatabaseFieldMapping.MessagesTable,
                    MessagesTablePKName = m.DatabaseFieldMapping.MessagesTablePKName
                };

                DatabaseFieldMapping.TableFieldMap = new List<FieldMap>();
                foreach (var item in m.DatabaseFieldMapping.TableFieldMap)
                {

                    FieldMap fieldMap = new FieldMap()
                    {
                        FieldnameApp = item.FieldnameApp,
                        FieldnameQuery = item.FieldnameQuery,
                        Reserved = item.Reserved
                    };
                    DatabaseFieldMapping.TableFieldMap.Add(fieldMap);
                }
                
                // Mapeamento do NotifsOnBD
                NotifsOnBD_headers = new List<string>();
                NotifsOnBD_body = new List<Dictionary<string, object>>();
                // Headers                
                if(NotifsOnBD.Count > 0)
                {
                    foreach (var fieldmap in DatabaseFieldMapping.TableFieldMap)
                    {
                        var campo_gravacao = fieldmap.FieldnameApp.Replace("[", string.Empty).Replace("]", string.Empty).ToLower();
                        if (NotifsOnBD[0].DBFields.ContainsKey(campo_gravacao))
                        {
                            string header = fieldmap.FieldnameApp.Replace("[", string.Empty).Replace("]", string.Empty);
                            if (!NotifsOnBD_headers.Contains(header))
                                NotifsOnBD_headers.Add(header);
                        }
                    }
                }
                else
                {
                    foreach (var fieldmap in DatabaseFieldMapping.TableFieldMap)
                    {
                        string header = fieldmap.FieldnameApp.Replace("[", string.Empty).Replace("]", string.Empty);
                        if (!NotifsOnBD_headers.Contains(header))
                            NotifsOnBD_headers.Add(header);
                    }
                }
                // Body                
                if(NotifsOnBD.Count > 0)
                {
                    var _table = DatabaseFieldMapping.MessagesTable.ToLower();
                    foreach(var BD_msg in NotifsOnBD)
                    {
                        var row = new Dictionary<string, object>();
                        foreach (var fieldmap in DatabaseFieldMapping.TableFieldMap)
                        {
                            var _fieldMap = fieldmap.FieldnameApp.Replace("[", "").Replace("]", "").ToLower();
                            var campo_gravacao = _table + "." + _fieldMap;
                            if(BD_msg.DBFields.ContainsKey(_fieldMap))
                                if(!row.ContainsKey(_fieldMap))
                                    row.Add(_fieldMap, BD_msg.returnValueField(campo_gravacao));
                        }
                        NotifsOnBD_body.Add(row);
                    }
                }
            }
            catch (Exception)
            {
                Log.Error("Map ViewModel (Notification) to Model (NotificationModel) - Error during mapping");
                throw;
            }
        }

    }


}


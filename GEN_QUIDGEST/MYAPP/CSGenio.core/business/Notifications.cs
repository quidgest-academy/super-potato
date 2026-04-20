using CSGenio.core;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;


namespace CSGenio.business
{
    /// <summary>
    /// Interface Notification
    /// </summary>
    public abstract class Notification
    {
        /// <summary>
        /// Notification PK
        /// </summary>
        public String codtpnot { get; set; }
        /// <summary>
        /// Notification ID
        /// </summary>
        public String IDNotif { get; set; }
        /// <summary>
        /// Notification Name
        /// </summary>
        public String Notif { get; set; }
        /// <summary>
        /// Help text
        /// </summary>
        public String Help { get; set; }
        /// <summary>
        /// Reference to Genio manual query that controls this notification
        /// </summary>
        public ManualQuery GenioQuery { get; set; }
        /// <summary>
        /// Available tag list with mapping fields (tag names vs query names)
        /// </summary>
        public List<Tag> TagsFieldMapping { get; set; }
        /// <summary>
        /// Destinations allowed for this notification type
        /// </summary>
        public List<AllowedDestination> AllowedDestinations { get; set; }
        /// <summary>
        /// Class with BD table destination with fields to store information about the messages sent
        /// </summary>
        public MessagesTableMap DatabaseFieldMapping { get; set; }
        /// <summary>
        /// Link to Alert (ID) to be raised
        /// </summary>
        public String AlertID { get; set; }
        /// <summary>
        /// Flag indication to send an email
        /// </summary>
        public bool SendsEmail { get; set; }
        /// <summary>
        /// Flag indication to send an alert (on webpage alert zone)
        /// </summary>
        public bool SendsAlert { get; set; }
        /// <summary>
        /// Flag indication to store the information sent on database
        /// </summary>
        public bool SendsToDatabase { get; set; }
        /// <summary>
        /// List of pre-configured Messages set in Genios definitions
        /// </summary>
        public List<CSGenioAnotificationmessage> MessagesConfig { get; set; }
        /// <summary>
        /// Datamatrix from ManualQuery. After run, this will also be extended with the reserved columns and store to all information processed
        /// </summary>
        public DataMatrix QueryData { get; set; }
        /// <summary>
        /// Messages saved on database originated by this Notif
        /// </summary>
        public List<DbArea> NotifsOnBD { get; set; }
        /// <summary>
        /// Instanciates the Datamatrix from ManualQuery
        /// </summary>
        public virtual DataMatrix Run(PersistentSupport sp, User user)
        {
            try
            {
                sp.openConnection();

                RunOpen(sp, user);
            }
            catch { }
            finally
            {
                sp.closeConnection();
            }

            return QueryData;
        }

        abstract public DataMatrix RunOpen(PersistentSupport sp, User user);


        /*********************************************************** SUPPORTING CLASSES ***********************/
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
        /// Class Tags/placeholders that will be available to select from and to be used on text messages
        /// </summary>
        public class Tag
        {
            /// <summary>
            /// Tag mapping
            /// </summary>
            public FieldMap FieldMap { get; set; }
            /// <summary>
            /// Tag help
            /// </summary>
            public String Help { get; set; }
        }

        /// <summary>
        /// Allowed destinations that will be available to be set on message configurations
        /// </summary>
        public class AllowedDestination
        {
            /// <summary>
            /// Destination reference
            /// </summary>
            public DestinationType Destination { get; set; }
            /// <summary>
            /// Foreign key fieldname to destination table in ManualQuery
            /// </summary>
            public string FKnameQuery { get; set; }

            /// <summary>
            /// PK with destination identifier
            /// </summary>
            public String DestinationKey { get; set; }
        }
        /// <summary>
        /// Class to store information about the destination of messages (table & relationship key and email)
        /// </summary>
        public class DestinationType
        {
            /// <summary>
            /// PK with destination identifier
            /// </summary>
            public String DestinationKey { get; set; }
            /// <summary>
            /// string with destination name
            /// </summary>
            public string DestinationName { get; set; }
            /// <summary>
            /// Table name
            /// </summary>
            public String DestinationTablename { get; set; }
            /// <summary>
            /// Primary key name
            /// </summary>
            public String DestinationTablePKName { get; set; }
            /// <summary>
            /// Email field in destination table
            /// </summary>
            public string EmailField { get; set; }
        }

        /// <summary>
        /// Class to store information on BD (with fields to be mapped query vs BD namming)
        /// </summary>
        public class MessagesTableMap
        {
            public String MessagesTable;
            public String MessagesTablePKName;
            public List<FieldMap> TableFieldMap;

            public MessagesTableMap() {
                MessagesTable = "";
                MessagesTablePKName = "";
                TableFieldMap = new List<FieldMap>();
            }
        }

        /*********************************************************** END SUPPORTING CLASSES ***********************/

        /*************************************** Builder/Helper Methods to store Genio definitions in appropriate classes *****************/
        /// <summary>
        /// Builder for Allowed Destinations
        /// </summary>
        public AllowedDestination AllowedDestination_build(String destinationKey, String destinationName, String destinationTablename, String destinationTablePKName, String emailField, String fknameQuery)
        {
            DestinationType destinationType = new DestinationType()
            {
                DestinationKey = destinationKey,
                DestinationName = destinationName,
                DestinationTablename = destinationTablename,
                DestinationTablePKName = destinationTablePKName,
                EmailField = emailField
            };

            AllowedDestination allowedDestination = new AllowedDestination()
            {
                Destination = destinationType,
                FKnameQuery = fknameQuery,
                DestinationKey = destinationKey


            };
            return allowedDestination;
        }

        /// <summary>
        /// Builder for Tags with mapping
        /// </summary>
        public Tag Tag_build(String fieldnameApp, String fieldnameQuery, String help)
        {
            Tag tag = new Tag()
            {

                FieldMap = new FieldMap()
                {
                    FieldnameApp = fieldnameApp,
                    FieldnameQuery = fieldnameQuery
                },
                Help = help
            };
            return tag;
        }

        /*************************************** END of Builder/Helper Methods to store Genio definitions in appropriate classes *****************/

        /*************************************** METHODS USED FOR MULTIPLEXING MESSAGES *************************************/
        /// <summary>
        /// Standartization for Manualquery identifiers
        /// </summary>
        public string FormatTagId(String tagId)
        {
            tagId = String.Format("{0,-13}", tagId);
            tagId = tagId.Replace(" ", "_");
            return tagId;
        }
        /// <summary>
        /// Method to Add columns to store all information needed (reserved and support fields) after Multiplexing Manualquery and Messages configured
        /// </summary>
        public void QueryData_AddReservedFields()
        {
            //Updates QueryData with primary ROWID collumn
            DataColumn rowid = new DataColumn("ROWID", typeof(String));
            QueryData.DbDataSet.Tables[0].Columns.Add(rowid);
            foreach (DataRow row in QueryData.DbDataSet.Tables[0].Rows)
            {
                row["ROWID"] = Guid.NewGuid().ToString();
            }
            QueryData.DbDataSet.Tables[0].PrimaryKey = new DataColumn[] { QueryData.DbDataSet.Tables[0].Columns["ROWID"] }; //define primary key


            //Updates QueryData with reserved columns
            foreach (FieldMap fieldmap in DatabaseFieldMapping.TableFieldMap)
            {
                if (fieldmap.Reserved)
                {
                    DataColumn row = new DataColumn(fieldmap.FieldnameQuery.Replace("[", "").Replace("]", ""), typeof(object));
                    try
                    {
                        QueryData.DbDataSet.Tables[0].Columns.Add(row);
                    }
                    catch { } //field already in table
                }
            }
        }
        /// <summary>
        /// Multiplexer that will process every row obtained from QueryData (manualquery) and will replace all placeholders (tags) into Configured Messages (MessagesConfig) one dependent class (FinalMsg) for each ocorrence
        /// </summary>
        public void ConfigMessagesMultiplexer(PersistentSupport sp, User user)
        {
            QueryData_AddReservedFields();
            // In each record of the QueryData runs every message of notification to replace tags defined in OriginalSubject and OriginalMessage to final Subject and Message (so, 1 result in the initial dataset can result in several final messages, depending on the message configuration)
            // To achieve this we need to create a new Dataset, and adding the final results to it.
            DataSet results = QueryData.DbDataSet.Clone();
            //. new CSGenio.persistence.DataMatrix();
            //results.DbDataSet = QueryData.DbDataSet.Clone();

            for (int i = 0; i < QueryData.NumRows; i++)
            {
                String final_subj = string.Empty;
                String final_msg = string.Empty;
                String destination_key = string.Empty;
                String destination_email = string.Empty;
                foreach (CSGenioAnotificationmessage msg in MessagesConfig)
                {
                    if (msg.ValAtivo == 1)
                    {
                        //Tags replacement
                        final_subj = msg.ValAssunto;
                        final_msg = msg.ValMensagem;
                        String tag_value = string.Empty;
                        String tag_app_name = string.Empty;
                        foreach (Tag tag in TagsFieldMapping)
                        {
                            // TODO: The type of field should be generated in the Tag and there should be a formatting function to handle this logic.
                            object fieldVal = QueryData.GetDirect(i, tag.FieldMap.FieldnameQuery.Replace("[", "").Replace("]", ""));
                            if (fieldVal is DateTime)
                            {
                                string dateFormat = fieldVal.ToString();
                                if (dateFormat.EndsWith("12:00:00 AM") || dateFormat.EndsWith("00:00:00"))
                                    tag_value = fieldVal.ToString().Substring(0, 10);
                                else
                                    tag_value = fieldVal.ToString();
                            }
                            else
                                tag_value = fieldVal.ToString();

                            tag_app_name = tag.FieldMap.FieldnameApp;
                            final_subj = final_subj.Replace(tag_app_name, tag_value);
                            final_msg = final_msg.Replace(tag_app_name, tag_value);
                        }
                        //Destination field
                        if (!string.IsNullOrEmpty(msg.ValCoddestn))
                        {
                            AllowedDestination destination = this.AllowedDestinations.Find(x => x.DestinationKey.ToUpper() == msg.ValCoddestn.ToUpper());
                            destination_key = QueryData.GetDirect(i, destination.FKnameQuery.Replace("[", "").Replace("]", "")).ToString();

							if (destination_key == null || destination_key == "")
                                continue;

                            //retrives email information from the destination
                            //String system = CSGenio.framework.Configuration.Program;
                            String dest_table = destination.Destination.DestinationTablename.Replace("[", "").Replace("]", "");
                            String dest_pk_name = destination.Destination.DestinationTablePKName.Replace("[", "").Replace("]", "");
                            String dest_emailfield = destination.Destination.EmailField.Replace("[", "").Replace("]", "");

                            var area = CSGenio.business.Area.createArea(dest_table.ToLowerInvariant(), user, user.CurrentModule) as DbArea;

                            SelectQuery sql = new SelectQuery().Select(area.Alias, dest_emailfield)
                                .From(area.QSystem, area.TableName, area.Alias)
                                .Where(CriteriaSet.And()
                                    .Equal(area.Alias, dest_pk_name, destination_key))
                                .PageSize(1);

                            destination_email = DBConversion.ToString(sp.ExecuteScalar(sql));
                        }
                        else //destination is set manually
                        {
                            destination_email = msg.ValTomanual;
                        }

                        //adds the message replacement results as dependent to the configured message (1 configured msg can get several results for several destinations)
                        string new_guid = Guid.NewGuid().ToString();
                        CSGenioAnotificationmessage.FinalMsg finalmsg = new CSGenioAnotificationmessage.FinalMsg()
                        {
                            ID = new_guid,
                            Subject = final_subj,
                            Message = final_msg,
                            Destination_PK = destination_key,
                            Destination_EMAIL = destination_email,
                            QueryDataID = new_guid
                        };
                        msg.FinalMsgs.Add(finalmsg);

                        //updates Dataquery reserved fields information (so it will be easier to save results on database)
                        DataRow this_row = QueryData.DbDataSet.Tables[0].NewRow();
                        DataRow initial_row = QueryData.DbDataSet.Tables[0].Rows[i];
                        this_row.ItemArray = initial_row.ItemArray.Clone() as object[];


                        //DataRow row = QueryData.DbDataSet.Tables[0].Rows[i];
                        this_row["OPERCRIA"] = "Q_NOTIFS";
                        this_row["DATACRIA"] = DateTime.Now;
                        this_row["IDNOTIF"] = IDNotif;
                        this_row["ROWID"] = finalmsg.ID;
                        this_row["IDMSG"] = finalmsg.ID;
                        this_row["MESSAGE"] = finalmsg.Message;
                        this_row["EMAIL"] = finalmsg.Destination_EMAIL;
						this_row["DESIGNAC"] = msg.ValDesignac;
                        results.Tables[0].Rows.Add(this_row.ItemArray);
                    }
                }
            }
            //Copy final results to QueryData:
            QueryData.DbDataSet.Tables.Clear();
            QueryData.DbDataSet.Tables.Add(results.Tables[0].Copy());
        }
        /*************************************** END METHODS USED FOR MULTIPLEXING MESSAGES *************************************/
        /*************************************** METHODS USED FOR SENDING FINAL DATA (COMMUNICATION BD; EMAIL; ETC) *************************************/
        /// <summary>
        /// Sends the final messages as email (with sending proprieties defined (can be overrided) and to the destination mapping configured in Genio
        /// </summary>
        public void WriteEmails(PersistentSupport sp, User user)
        {
            foreach (CSGenioAnotificationmessage msg in MessagesConfig)
            {
                if (msg.ValAtivo == 1 && msg.ValEmail == 1)
                {
                    foreach (CSGenioAnotificationmessage.FinalMsg finalmsg in msg.FinalMsgs)
                    {
                        EmailServer emailProperties = Configuration.EmailProperties.Find(x=>x.Codpmail.ToUpper() == msg.ValCodpmail.ToUpper());
                        CSGenioAnotificationemailsignature emailSignature = CSGenioAnotificationemailsignature.search(sp, msg.ValCodsigna, user);

                        byte[] data = String.IsNullOrEmpty(emailProperties.Password) ? null : Convert.FromBase64String(emailProperties.Password);
                        string decodedPass = data is null ? "" : Encoding.UTF8.GetString(data);

                        CSmail mail = new CSmail()
                        {
                            //Email properties
                            NomeRemetente = emailProperties.Name,
                            From = emailProperties.From,
                            SmtpServer = emailProperties.SMTPServer,
                            Port = (int)emailProperties.Port,
                            SSL = emailProperties.SSL,
                            AuthType = emailProperties.AuthType,
                            OAuth2Options = emailProperties.OAuth2Options,
                            User = emailProperties.Username,
                            Pass = decodedPass,
                            //Pathimg = path TODO.. de momento no CSmail está a ir buscar os recursos ao disco.... tenho de alterar para stream ou nos documentos da BD

                            To = finalmsg.Destination_EMAIL,
                            CC = msg.ValCc,
                            Bcc = msg.ValBcc,
                            Subject = finalmsg.Subject,
                            BodyHtml = msg.ValHtml == 1,
                            Body = finalmsg.Message,
                            StreamImagens = new List<System.IO.Stream>() { { new MemoryStream(emailSignature.ValImage) } },
                            Textass = emailSignature.ValTextass
                            //TODO: Mapear anexos Anexo = finalmsg.Attachment Mapeamento de campo de anexo para incluir na mensagem final
                            //de momento no CSmail está a ir buscar os recursos ao disco....tenho de alterar para stream ou nos documentos da BD
                        };
                        try
                        {
                            mail.Send();
                            finalmsg.Sent = true;
                            //Update QueryData information for this messageID
                            //updates Dataquery reserved fields information
                            DataRow row = QueryData.DbDataSet.Tables[0].Rows.Find(finalmsg.QueryDataID);
                            row["MAILSENT"] = 1;
                        }
                        catch (Exception e)
                        {

                            finalmsg.Sent = false;
                            finalmsg.ErrorMessage = e.Message;
                            DataRow row = QueryData.DbDataSet.Tables[0].Rows.Find(finalmsg.QueryDataID);
                            row["MAILERR"] = e.Message;
							row["MAILSENT"] = 0;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Saves the final messages on database (table defined in Genio, can be overrided, but be sure to have all the fields and mappings correct)
        /// </summary>
        public void WriteMessagesToBD(PersistentSupport sp, User user)
		{
			var destTable = DatabaseFieldMapping.MessagesTable.ToLowerInvariant();
			var dataTable = QueryData.DbDataSet.Tables[0];
			var areaFields = new HashSet<string>(DbArea.GetInfoArea(destTable).DBFields.Select(kv => kv.Key.ToLowerInvariant()), StringComparer.OrdinalIgnoreCase);

			foreach (var msg in this.MessagesConfig.Where(m => m.ValAtivo == 1 && m.ValGravabd == 1))
			{
				foreach (var finalMsg in msg.FinalMsgs)
				{
					var finalRow = dataTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("IDMSG") == finalMsg.ID);

					if (finalRow == null)
						continue;

					var area = CSGenio.business.Area.createArea(destTable, user, user.CurrentModule) as DbArea;

					foreach (FieldMap fieldmap in DatabaseFieldMapping.TableFieldMap)
					{
						string nameQuery = fieldmap.FieldnameQuery.Replace("[", "").Replace("]", "");
						string BDfield = fieldmap.FieldnameApp.Replace("[", "").Replace("]", "").ToLowerInvariant();

						if (!finalRow.Table.Columns.Contains(nameQuery) || !areaFields.Contains(BDfield))
							continue;

						var value = finalRow[nameQuery] ?? DBNull.Value;
						area.insertNameValueField(BDfield, value);

					}

					area.insert(sp);
				}
			}
		}
    }

	//------------------ DYNAMIC PART:
    /// <summary>
    /// Notification constructor
    /// </summary>
 }

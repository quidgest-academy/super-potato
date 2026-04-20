using CSGenio.business;
using CSGenio.core.messaging;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CSGenio.messaging
{
    /// <summary>
    /// Generic processor mapper that drives a standard ETL process from the source Dataset to a persistence
    /// </summary>
    public class MessageProcessMapper
    {
        /// <summary>
        /// List of table mappers to be applied
        /// </summary>
        public List<IMessageTableMapper> TableMaps { get; set; } = new List<IMessageTableMapper>();

        /// <summary>
        /// Resolve the datasystem according to the message received and business logic.
        /// Defaults to current default year datasystem.
        /// </summary>
        public Func<QueueMessageEnvelope, AreaDataset, string> DatasystemResolver { get; set; } = (e, d) => Configuration.DefaultYear;

        /// <summary>
        /// Resolve the user according to the message received and business logic.
        /// Defaults to a mq user with admin role.
        /// </summary>
        public Func<QueueMessageEnvelope, AreaDataset, string, User> UserResolver { get; set; } = (e, d, s) =>
        {
            var module = Configuration.Program;
            User user = new User("mq", "", "")
            {
                Year = s ?? Configuration.DefaultYear,
                CurrentModule = module,
                Language = CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper() //this should be the base language, not the server culture info
            };            
            user.AddModuleRole(module, Role.ADMINISTRATION);
            return user;
        };

        /// <summary>
        /// Applies the standard ETL process within a transaction
        /// </summary>
        /// <param name="envelope">Message envelope</param>
        /// <param name="dataset">Message dataset</param>
        public void Apply(QueueMessageEnvelope envelope, AreaDataset dataset)
        {
            var context = new MessageMapperContext();

            var sourceDs = DatasystemResolver(envelope, dataset);
            context.Sp = PersistentSupport.getPersistentSupport(sourceDs);
            context.Sp.QueueMode = true;
            context.User = UserResolver(envelope, dataset, sourceDs);
            context.Envelope = envelope;
            context.Dataset = dataset;

            try
            {
                context.Sp.openTransaction();
                foreach (var table in TableMaps)
                    table.Apply(context);
                context.Sp.closeTransaction();
            }
            catch
            {
                context.Sp.rollbackTransaction();
                throw;
            }
        }
    }

    /// <summary>
    /// Mapper context
    /// </summary>
    public class MessageMapperContext
    {
        /// <summary>
        /// Envelope of the sent message
        /// </summary>
        public QueueMessageEnvelope Envelope { get; set; }
        /// <summary>
        /// Allocated and resolved persistent support
        /// </summary>
        public PersistentSupport Sp { get; set; }
        /// <summary>
        /// Allocated and resolved user
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// The source dataset
        /// </summary>
        public AreaDataset Dataset { get; set; }
        /// <summary>
        /// Generic cache scoped to this process (thread safe)
        /// </summary>
        public Dictionary<string, object> Cache { get; set; }
    }

    /// <summary>
    /// Maps one or more tables in the source dataset to operations over the destination persistent support
    /// </summary>
    public interface IMessageTableMapper
    {
        /// <summary>
        /// Maps one or more tables in the source dataset to operations over the destination persistent support
        /// </summary>
        /// <param name="context">Processing context</param>
        void Apply(MessageMapperContext context);
    }

    /// <summary>
    /// Generic table mapper for update/insert and deleted rows
    /// It assumes a single integration key that maps the source rows to the target rows
    /// </summary>
    public class MessageTableMapper : IMessageTableMapper
    {
        /// <summary>
        /// integration primary key
        /// </summary>
        public string IPK { get; set; }

        /// <summary>
        /// dst table on the database
        /// </summary>
        public string DstTable { get; set; }

        /// <summary>
        /// source table on the message dataset
        /// </summary>
        public string SrcTable { get; set; }

        /// <summary>
        /// Row mappers that will be run in order
        /// </summary>
        public List<IMessageRowMapper> FieldMaps { get; set; }

        /// <inheritdoc/>
        public void Apply(MessageMapperContext context)
        {
            var table = context.Dataset.Tables[SrcTable];

            foreach (var row in table.Updated)
            {
                var src = row.Value;
                //check if row already exists
                var dst = Area.searchList(DstTable, context.Sp, context.User,
                    CriteriaSet.And().Equal(IPK, row.Key)
                    ).FirstOrDefault() ?? Area.createArea(DstTable, context.User, context.User.CurrentModule);

                //map source dataset into the destination dataset
                foreach (var fm in FieldMaps)
                    fm.Apply(dst, src, context);

                //update or insert
                if (string.IsNullOrEmpty(dst.QPrimaryKey))
                    dst.inserir_WS(context.Sp);
                else
                    dst.change(context.Sp, null);
            }

            foreach (var row in table.Deleted)
            {
                //look up the row to delete
                var dst = Area.searchList(DstTable, context.Sp, context.User,
                    CriteriaSet.And().Equal(IPK, row)
                    ).FirstOrDefault() ?? Area.createArea(DstTable, context.User, context.User.CurrentModule);
                //if exists, delete it
                dst?.eliminate(context.Sp);
            }

        }
    }

    /// <summary>
    /// Maps data in a source row to a destination row
    /// </summary>
    public interface IMessageRowMapper
    {
        /// <summary>
        /// Maps data in a source row to a destination row
        /// </summary>
        /// <param name="dstRow">Destination</param>
        /// <param name="srcRow">Source</param>
        /// <param name="context">Processing context</param>
        void Apply(IArea dstRow, IArea srcRow, MessageMapperContext context);
    }

    /// <summary>
    /// Maps fields that match exactly the same name in src and dst
    /// </summary>
    public class MessageFieldAutomapper : IMessageRowMapper
    {
        private HashSet<string> Fields = new HashSet<string>();

        public MessageFieldAutomapper(AreaInfo dst, AreaInfo src)
        {
            foreach (var sf in src.DBFields)
                if (dst.DBFields.ContainsKey(sf.Key))
                    Fields.Add(sf.Key);
        }

        /// <inheritdoc/>
        public void Apply(IArea dstRow, IArea srcRow, MessageMapperContext context)
        {
            foreach (var sf in Fields)
                dstRow.insertNameValueField(sf, srcRow.returnValueField(sf));
        }
    }

    /// <summary>
    /// Maps one destination field to an expression calculated over the source
    /// </summary>
    public class MessageFieldExprMapper : IMessageRowMapper
    {
        /// <summary>
        /// Destination field
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Expression to get the field from the messaged row
        /// </summary>
        public Func<IArea, object> Expression { get; set; }

        /// <inheritdoc/>
        public void Apply(IArea dstRow, IArea srcRow, MessageMapperContext context)
        {
            dstRow.insertNameValueField(Field, Expression(srcRow));
        }
    }

    /// <summary>
    /// Maps one destination foreign key to a relation calculated over the integration key of the source
    /// </summary>
    public class MessageRelationMapper : IMessageRowMapper
    {
        /// <summary>
        /// Foreign key
        /// </summary>
        public string FK { get; set; }

        /// <summary>
        /// Related table
        /// </summary>
        public string RelTable { get; set; }

        /// <summary>
        /// Integration key on the related table
        /// </summary>
        public string RelIPk { get; set; }

        /// <summary>
        /// Get the foreign key value send by the message
        /// </summary>
        public Func<IArea, object> SrcFK { get; set; }

        /// <inheritdoc/>
        public void Apply(IArea dstRow, IArea srcRow, MessageMapperContext context)
        {
            //check cache for previouly fetched FK to avoid query if we can
            var ik = SrcFK(srcRow);
            var cacheKey = $"rel.{dstRow.Alias}.{ik}";
            if (!context.Cache.TryGetValue(cacheKey, out var fkval))
            {
                //find out what the pk of the related area is by the integration key
                var dst = Area.searchList(RelTable, context.Sp, context.User,
                    CriteriaSet.And().Equal(RelIPk, ik)
                    ).FirstOrDefault();
                //if there is such a row use the local pk of it for the local fk
                fkval = dst?.QPrimaryKey;
                //add to the cache
                context.Cache[cacheKey] = fkval;
            }
            dstRow.insertNameValueField(FK, fkval);
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business.async
{
    /// <summary>
    /// Represents a policy for partitioning data into units
    /// </summary>
    public abstract class PartitionPolicy
    {
        private IReadOnlyList<WorkUnit> _workUnits;
        private bool _executed;

        public IReadOnlyList<WorkUnit> GetWorkUnits(PersistentSupport sp)
        {
            if (!_executed)
            {
                _workUnits = Breakdown(sp);
                _executed = true;
            }
            return _workUnits;
        }

        protected abstract IReadOnlyList<WorkUnit> Breakdown(PersistentSupport sp);
    }

    /// <summary>
    /// Represents a global partition with no sub-units
    /// </summary>
    public sealed class GlobalPartition : PartitionPolicy
    {
        private readonly IReadOnlyList<WorkUnit> _workUnits = new List<WorkUnit>
        {
            new WorkUnit("global")
        };

        protected override IReadOnlyList<WorkUnit> Breakdown(PersistentSupport sp) => _workUnits;
    }

    /// <summary>
    /// Represents a partition with a single unit
    /// </summary>
    public sealed class SinglePartition : PartitionPolicy
    {
        private readonly IReadOnlyList<WorkUnit> _workUnits;

        public SinglePartition(string identifier)
            : this(new WorkUnit(identifier ?? throw new ArgumentNullException(nameof(identifier))))
        { }

        public SinglePartition(WorkUnit workUnit)
        {
            _workUnits = new List<WorkUnit>
            {
                workUnit ?? throw new ArgumentNullException(nameof(workUnit))
            };
        }

        protected override IReadOnlyList<WorkUnit> Breakdown(PersistentSupport sp) => _workUnits;
    }

    /// <summary>
    /// Represents a partition that combines multiple sub-partitions
    /// </summary>
    public sealed class MultiplePartition : PartitionPolicy
    {
        private readonly Func<string, PartitionPolicy> _innerPolicyFactory;
        private readonly IReadOnlyList<string> _identifiers;

        public MultiplePartition(
            Func<string, PartitionPolicy> innerPolicyFactory,
            IEnumerable<string> identifiers
        )
        {
            _innerPolicyFactory =
                innerPolicyFactory ?? throw new ArgumentNullException(nameof(innerPolicyFactory));
            _identifiers = (
                identifiers ?? throw new ArgumentNullException(nameof(identifiers))
            ).ToList();
        }

        protected override IReadOnlyList<WorkUnit> Breakdown(PersistentSupport sp)
        {
            return _identifiers
                .Select(id => _innerPolicyFactory(id))
                .SelectMany(policy => policy.GetWorkUnits(sp))
                .ToList();
        }
    }

    /// <summary>
    /// Base class for partitions based on database queries
    /// </summary>
    public abstract class QueryPartition : PartitionPolicy
    {
        protected static List<WorkUnit> ListFromMatrix(DataMatrix matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            return Enumerable
                .Range(0, matrix.NumRows)
                .Select(i => new WorkUnit(matrix.GetString(i, 0)))
                .ToList();
        }

        protected static CriteriaSet CreateEmptyCriteria(object field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            return CriteriaSet.Or().Equal(0, field).Equal(field, null);
        }
    }

    /// <summary>
    /// Represents an empty partition with no units
    /// </summary>
    public sealed class EmptyPolicy : PartitionPolicy
    {
        private readonly IReadOnlyList<WorkUnit> _workUnits = new List<WorkUnit>();

        protected override IReadOnlyList<WorkUnit> Breakdown(PersistentSupport sp) => _workUnits;
    }
}

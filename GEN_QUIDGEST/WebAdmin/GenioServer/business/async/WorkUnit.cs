using System;

namespace CSGenio.business.async
{
    /// <summary>
    /// Represents a unit of work with a specific scope. 
    /// A work unit is defined by a path or URI that describes the scope of the work.
    /// </summary>
    public class WorkUnit
    {
        /// <summary>
        /// Gets the scope of the work unit.
        /// The scope defines the path or URI that the work unit pertains to.
        /// </summary>
        public WorkPath Scope { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkUnit"/> class using a <see cref="WorkPath"/>.
        /// </summary>
        /// <param name="workPath">The work path defining the scope of the work unit.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="workPath"/> is <c>null</c>.</exception>
        public WorkUnit(WorkPath workPath)
        {
            Scope = workPath ?? throw new ArgumentNullException(nameof(workPath));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkUnit"/> class using a string that may represent either a path or a URI.
        /// If the string is not a valid URI, it will be treated as a local path.
        /// </summary>
        /// <param name="maybePath">The string representing either a URI or a local path.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="maybePath"/> is <c>null</c>, empty, or consists only of whitespace.</exception>
        public WorkUnit(string maybePath)
        {
            if (string.IsNullOrWhiteSpace(maybePath))
                throw new ArgumentException("Work path cannot be null or empty.", nameof(maybePath));

            if (maybePath.StartsWith("global/"))
            {
                // e.g., global/orgs/quidgest/projects/gen
                Scope = new WorkPath(maybePath);
            }
            else
            {
                // e.g., 93a54176-0564-40f3-a12d-878e666ad573 => global/93a54176-0564-40f3-a12d-878e666ad573
                // e.g., orgs/quidgest/projects/gen => global/orgs/quidgest/projects/gen
                Scope = new WorkPath($"global/{maybePath}");
            }
        }

        /// <summary>
        /// Determines if this work unit conflicts with another work unit.
        /// A conflict occurs if the scopes of the two work units intersect.
        /// </summary>
        /// <param name="other">The other work unit to compare with.</param>
        /// <returns><c>true</c> if the work units conflict (i.e., their scopes intersect); otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is <c>null</c>.</exception>
        public bool CollidesWith(WorkUnit other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            return Scope.Intersects(other.Scope);
        }

        /// <summary>
        /// Returns a string representation of the work unit.
        /// The string will be the string representation of the scope of the work unit.
        /// </summary>
        /// <returns>A string that represents the work unit, typically the string representation of the scope.</returns>
        public override string ToString() => Scope.ToString();
    }
}

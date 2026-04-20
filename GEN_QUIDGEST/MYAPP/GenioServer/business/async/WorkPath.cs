using System;

namespace CSGenio.business.async
{
    /// <summary>
    /// Represents a work path.
    /// </summary>
    public sealed class WorkPath
    {
        /// <summary>
        /// Gets the value of the work path.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkPath"/> class with the specified path.
        /// </summary>
        /// <param name="path">The path to initialize the work path with.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided path is null.</exception>
        public WorkPath(string path)
        {
            Value = path ?? throw new ArgumentNullException(nameof(path));
        }

        /// <summary>
        /// Determines whether this work path intersects with another work path.
        /// </summary>
        /// <param name="other">The other work path to check for intersection.</param>
        /// <returns>
        /// true if the work paths intersect; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided work path is null.</exception>
        public bool Intersects(WorkPath other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            return IsAncestorOrSelf(Value, other.Value) || IsAncestorOrSelf(other.Value, Value);
        }

        /// <summary>
        /// Determines if one path is an ancestor of or the same as another path.
        /// </summary>
        /// <param name="ancestor">The potential ancestor path to check.</param>
        /// <param name="descendant">The potential descendant path to check against.</param>
        /// <returns>
        /// true if ancestor is an ancestor of or the same as descendant;
        /// false otherwise.
        /// </returns>
        private static bool IsAncestorOrSelf(string ancestor, string descendant)
        {
            if (ancestor == null || descendant == null)
                return false;

            // Get segments for comparison
            string[] ancestorSegments = ancestor.Split('/');
            string[] descendantSegments = descendant.Split('/');

            // Descendant must have at least as many segments as ancestor
            if (ancestorSegments.Length > descendantSegments.Length)
                return false;

            // Compare each segment
            for (int i = 0; i < ancestorSegments.Length; i++)
            {
                if (!string.Equals(
                    ancestorSegments[i].TrimEnd('/'),
                    descendantSegments[i].TrimEnd('/'),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => Value.ToString();
    }
}

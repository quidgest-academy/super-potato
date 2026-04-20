using System;

namespace CSGenio
{
    /// <summary>
    /// Attribute to provide CLI command information for configuration properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CliPropertyAttribute : Attribute
    {
        /// <summary>
        /// The command-line argument name (e.g., qa-env, api-url)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Help text displayed when showing command usage
        /// </summary>
        public string Description { get; set; }

        public CliPropertyAttribute(string commandName, string commandDescription)
        {
            Name = commandName;
            Description = commandDescription;
        }
    }
} 
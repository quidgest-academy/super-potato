namespace CSGenio.core.scheduler
{

    /// <summary>
    /// Metadata description of an option for a Scheduled task
    /// </summary>
    public class ScheduledTaskOption
    {
        /// <summary>
        /// Unique id for the property
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// Human readable name for the property
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// True if the option is Optional, false if its Mandatory
        /// </summary>
        public bool Optional { get; set; }
        /// <summary>
        /// A longer help description with details about this option
        /// </summary>
        public string Description { get; set; }
    }
}
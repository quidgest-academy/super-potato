using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSGenio.core.scheduler
{
    /// <summary>
    /// Interface for Scheduled tasks
    /// </summary>
    public interface IScheduledTask
    {
        /// <summary>
        /// Process the task
        /// </summary>
        /// <param name="options">A key-value options dictionary to customize this task run</param>
        /// <param name="stoppingToken">
        ///   A token that can be canceled from outside the task to terminate it early. 
        ///   Process can choose to use it or ignore it.
        /// </param>
        /// <returns>Asyncronous task</returns>
        Task Process(Dictionary<string, string> options, CancellationToken stoppingToken);

        /// <summary>
        /// Supplies a list of options metadata that describe each of the options this process recognizes.
        /// </summary>
        /// <returns>The list of options metadata</returns>
        List<ScheduledTaskOption> GetOptions();
    }


    /// <summary>
    /// Helper methods to parse the options key-values
    /// </summary>
    public static class ScheduledTaskExtensions
    {
        /// <summary>
        /// Get a integer from the options
        /// </summary>
        /// <param name="options">The options dictionary</param>
        /// <param name="name">The name of the option</param>
        /// <param name="defaultValue">The value to return in case the option was not set</param>
        /// <returns>The option value</returns>
        public static int GetIntOption(Dictionary<string, string> options, string name, int defaultValue) {
            if(options.TryGetValue(name, out var str))
                if(int.TryParse(str, out var intval))
                    return intval;
            return defaultValue;
        }

        /// <summary>
        /// Get a boolean from the options
        /// </summary>
        /// <param name="options">The options dictionary</param>
        /// <param name="name">The name of the option</param>
        /// <param name="defaultValue">The value to return in case the option was not set</param>
        /// <returns>The option value</returns>
        public static bool GetBoolOption(Dictionary<string, string> options, string name, bool defaultValue) {
            if(options.TryGetValue(name, out var str))
                return str == "true" || str == "1";
            return defaultValue;
        }

        /// <summary>
        /// Get a string from the options
        /// </summary>
        /// <param name="options">The options dictionary</param>
        /// <param name="name">The name of the option</param>
        /// <param name="defaultValue">The value to return in case the option was not set</param>
        /// <returns>The option value</returns>
        public static string GetStringOption(Dictionary<string, string> options, string name, string defaultValue) {
            if(options.TryGetValue(name, out var str))
                return str;
            return defaultValue;
        }

        /// <summary>
        /// Get an list of values from the options. Assumed to be formated as a comma separated string.
        /// </summary>
        /// <param name="options">The options dictionary</param>
        /// <param name="name">The name of the option</param>
        /// <returns>The option values or empty list if no option was set</returns>
        public static List<string> GetCsvOption(Dictionary<string, string> options, string name) {
            if(options.TryGetValue(name, out var str))
                return str.Split(';').ToList();
            return new List<string>();
        }

    }
}
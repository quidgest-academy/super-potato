using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CSGenio.config
{

    public class AdvancedProperty
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }  
        public string ResourceId { get; set; }  
        public string HelpResourceId { get; set; }  
        public string HelpResourceVerboseId { get; set; }     
    }

    /// <summary>
    /// Handle specific project properties that are specific to this system. 
    /// They will be visible in the more properties area of Webadmin
    /// </summary>
    public static class ExtraProperties
    {
        private static Dictionary<string, string> initialProperties = new Dictionary<string, string>()
        {
            // Specify key-value pairs with the following notation: { "key", "value" },
            // Properties with empty/null value, will NOT be added,
            // but are displayed in the form dropdown for selection.
            // Notice: these properties cannot be deleted on WebAdmin,
            // because they will be recreated from the MANWIN.
// USE /[MANUAL FOR INITPROPERTIES]/
        };

        private static List<AdvancedProperty> initialAdvancedProperties = new List<AdvancedProperty>()
        {            
            // but are displayed in the form dropdown for selection.
            // Notice: these properties cannot be deleted on WebAdmin,
            // because they will be recreated from the MANWIN.         
        };


        /// <summary>
        /// Add initial properties that should have a default value to an existing property list
        /// </summary>
        public static void AddMissingValues(SerializableDictionary<string, string>  propertyList)
        {
            foreach (var entry in initialProperties)
            {
                // Do not add properties with empty value
                if (String.IsNullOrEmpty(entry.Value))
                    continue;
                // Do not add already existing properties
                if (propertyList.ContainsKey(entry.Key))
                    continue;

                propertyList.Add(entry.Key, entry.Value);
            }

            foreach (var entry in initialAdvancedProperties)
            {
                if (
                    String.IsNullOrEmpty(entry.DefaultValue) ||
                    propertyList.ContainsKey(entry.Id)
                )
                { continue;}

                propertyList.Add(entry.Id, entry.DefaultValue);
            }
        }

        /// <summary>
        /// Returns a list of initial properties that don't have null values
        /// </summary>        
        public static SerializableDictionary<string, string> GetInitialValues()
        {
            var result = new SerializableDictionary<string, string>();
            foreach (var entry in initialProperties)
            {
                // Do not add properties with empty value
                if (String.IsNullOrEmpty(entry.Value))
                    continue;

                result.Add(entry.Key, entry.Value);
            }
            
            foreach (var entry in initialAdvancedProperties)
            {
                if (
                    String.IsNullOrEmpty(entry.DefaultValue) ||
                    result.ContainsKey(entry.Id)
                )
                { continue;}

                result.Add(entry.Id, entry.DefaultValue);
            }
            return result;
        }

        public static List<string> GetInitialKeys()
        {
            return initialProperties.Keys.ToList();
        }

        public static List<AdvancedProperty> GetAdvancedProperties()
        {
            return initialAdvancedProperties;
        }
        
        public static bool HasDefaultValue(string key)
        {
            if(initialProperties.ContainsKey(key))
            {
                return !String.IsNullOrEmpty(initialProperties[key]);
            }                
            else
            {
                var property = initialAdvancedProperties.FirstOrDefault(p=> p.Id.Equals(key));
                return property != null && !string.IsNullOrEmpty(property?.DefaultValue);
            }
        }

        public static bool IsPasswordType(string id)
        {
            return initialAdvancedProperties.Exists(p => 
                p.Id.Equals(id) &&
                p.Type.Equals("P")
            );
        }
    }
}

using System.Runtime.Serialization;
using GenioServer.business;

namespace Administration.Models
{
    [DataContract]
    public class ModulesLevel
    {
        [DataMember]
        public string Module { get; set; }

        [DataMember]
        public string ModDescription { get; set; }

        [DataMember]
        public string Level { get; set; }

        [DataMember]
        public string LvlDescription { get; set; }

        public ModulesLevel()
        { }

        public ModulesLevel(string module, string modDesc, string level, string lvlDesc)
        {
            Module = module;
            ModDescription = modDesc;
            Level = level;
            LvlDescription = lvlDesc;
        }
    }
}
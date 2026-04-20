using System.ComponentModel.DataAnnotations;

namespace Administration.Models
{
    public class ConfigMigrationModel : ModelBase
    {
        [Display(Name = "ESTADO07788", ResourceType = typeof(Resources.Resources))]
        public string ResultMsg { get; set; }

        [Display(Name = "VERSAO_ATUAL00037", ResourceType = typeof(Resources.Resources))]
        public int CurVersion { get { return GenioServer.framework.ConfigXMLMigration.CurConfigurationVerion;  } }

        [Display(Name = "VERSAO_DO_FICHEIRO_D63572", ResourceType = typeof(Resources.Resources))]
        public int ConfigVersion { get; set; }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Administration.Models
{
    public class ErrorLogModel : ModelBase
    {
        [Display(Name = "ESTADO07788", ResourceType = typeof(Resources.Resources))]
        public string ResultMsg { get; set; }
        public string ErrorLog { get; set; }

        public List<CSGenio.framework.ClientApplication> Applications { get; set; }
    }
}
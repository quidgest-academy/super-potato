using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Administration.Models
{
    public class ReportFormModel : ModelBase
    {        
        public string Scope { get; set; }

        public bool Dynamic { get; set; }

        public bool Delete { get; set; }
    }
}
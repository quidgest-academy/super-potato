using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administration.Models
{
    public class DashboardModel : ModelBase
    {
        public bool HasConfig { get; set; }
		public bool HasValidConfig { get; set; }
		public bool IsBetaTesting { get; set; }
        public bool HasEnvironment { get; set; }
        public bool HasDiffVersion { get; set; }
        public bool HasDiffIdxVersion { get; set; }
		public bool HasSGBDVersion { get; set; }
		public bool HasDiffUserSettingsVersion { get; set; }
        public string TpSGBD { get; set; }
        public string SGBDVersion { get; set; }
        public string SGBDServer { get; set; }
        public string DBSchema { get; set; }
        public decimal DBSize { get; set; }		
        public int VersionDb { get; set; }
        public int VersionIdxDb { get; set; }
        public int VersionDbGen { get; set; }
        public int VersionIdxDbGen { get; set; }
        public int VersionUpgrIndx { get; set; }
        public int VersionUpgrIndxGen { get; set; }
		public int UserSettingsVersion { get; set; }
		public int CurrentUserSettingsVersion { get; set; }
        public string SODesc { get; set; }
        public string PCDesc { get; set; }
        public string HardwProcDesc { get; set; }
        public string HardwMemDesc { get; set; }
        public string HardwDrivDesc { get; set; }
		
		public string ResultErrors { get; set; }

        public CSGenio.framework.MaintenanceStatus CurrentMaintenance { get { return CSGenio.framework.Maintenance.Current; } }
    }
}

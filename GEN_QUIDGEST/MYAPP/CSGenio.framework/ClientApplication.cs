using System;
using System.Collections.Generic;

namespace CSGenio.framework
{
    public class ClientApplication
    {
        public static readonly ClientApplication MYAPP = 
            new ClientApplication("MYAPP", "My application") 
            {
                Modules = new Dictionary<string, string>
				{
                    { "FOR", "MY_APPLICATION56216" },
                },
                Platform = "VUE",
            };

        public static readonly ClientApplication WEBADMIN = 
            new ClientApplication("WebAdmin", "WebAdmin") 
                { 
                    Security = false
                };

        public ClientApplication(string id, string name)
        {
            Name = name;
            Id = id;
            Security = true;
            Path = true;
            Modules = new Dictionary<string, string>();
            Platform = string.Empty;
        }

        public Dictionary<string, string> Modules {get; private set;}
        public String Name { get; private set; }
        public String Id { get; private set; }
        public String Platform { get; private set; }
        public bool Security { get; set; }
        public bool Path { get; set; }
		
		public static List<ClientApplication> Applications => applications;

        private static readonly List<ClientApplication> applications = new List<ClientApplication>()
        {
            MYAPP,      
            WEBADMIN
        };
    }
}

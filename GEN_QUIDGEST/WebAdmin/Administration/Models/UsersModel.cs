using Administration.AuxClass;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Administration.Models
{
    public class UsersModel : ModelBase
    {
        public UsersModel()
        {
            Modules = new List<UsersModule>();
            Levels = new Dictionary<int, string>();
        }

        [Display(Name = "ESTADO07788", ResourceType = typeof(Resources.Resources))]
        public string ResultMsg { get; set; }

        public IList<UserItem> Users { get; set; } 

		public IList<AuxFunctions.SelectlistElement> IdentityProviders { get; set; }

        public IList<UsersModule> Modules { get; set; }

        public IDictionary<int, string> Levels { get; set; }
    }

    public class UserItem
    {
        public string Cod { get; set; }

        public string Name { get; set; }
    }

    public class UsersModule
    {
        public UsersModule(string module, string description)
        {
            Cod = module;
            Description = description;
        }

        public string Cod { get; set; }

        public string Description { get; set; }
    }

    public class UsersLevel
    {
        public UsersLevel(UsersModule module, int cod, string description)
        {
            Module = module;
            Cod = cod;
            Description = description;
        }

        public UsersModule Module { get; set; }

        public int Cod { get; set; }

        public string Description { get; set; }
    }
}
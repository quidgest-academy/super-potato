using Administration.AuxClass;
using CSGenio.framework;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Administration.Models
{
    public class ManageUsersModel : ModelBase
    {
        public ManageUsersModel()
        {
            Modules = new List<Module>();
            Privileges = new List<ModuleRoleModel>();
            AssignedRoles = ToDictionaryList(new List<ModuleRoleModel>());
        }

        [Display(Name = "ESTADO07788", ResourceType = typeof(Resources.Resources))]
        public string ResultMsg { get; set; }

        [Display(Name = "NOME__48276", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string Username { get; set; }

        [Display(Name = "EMAIL25170", ResourceType = typeof(Resources.Resources))]
        public string Email { get; set; }

        [Display(Name = "TELEFONE37757", ResourceType = typeof(Resources.Resources))]
        public string Phone { get; set; }

        [Display(Name = "ALTERAR_A_PALAVRA_CH54014", ResourceType = typeof(Resources.Resources))]
        public bool PasswordChange { get; set; }

        [Display(Name = "NOVA_15272", ResourceType = typeof(Resources.Resources))]
        public string PasswordNew { get; set; }

        [Display(Name = "CONFIRMAR_64824", ResourceType = typeof(Resources.Resources))]
        public string PasswordConfirm { get; set; }

        [Display(Name = "O_UTILIZADOR_TEM_QUE05121", ResourceType = typeof(Resources.Resources))]
        public bool StatusFirstLogin { get; set; }

        [Display(Name = "INVALIDAR_AUTENTICAC21095", ResourceType = typeof(Resources.Resources))]
        public bool Invalidate2FA { get; set; }
        public bool ShowInvalidate2FA { get; set; }
        public bool BlockInvalidate2FA { get; set; }

        public string Psw2faVL { get; set; }
        public string Psw2faTP { get; set; }

        [Display(Name = "DESACTIVAR_CONTA37602", ResourceType = typeof(Resources.Resources))]
        public bool StatusDisableLogin { get; set; }

        public string CodUser { get; set; }
        public string ModForm { get; set; }

        public List<Module> Modules { get; set; }

        [JsonIgnore]
        public List<ModuleRoleModel> Privileges {get;set;}

        public string SubmitValue { get; set; }

        public  Dictionary<string, List<ModuleRoleModel>> AvaiableRoles {
            get
            {
                return ToDictionaryList(ModuleRoleModel.ALL_MODULE_ROLES);
            }
        }
        

        public  Dictionary<string, List<ModuleRoleModel>> AssignedRoles {
            get;set;
        }

        public Dictionary<string, List<ModuleRoleModel>> ToDictionaryList(List<ModuleRoleModel> list)
        {
            var dictionary = new Dictionary<string, List<ModuleRoleModel>>();
            foreach (var module in Modules)
            {                
                dictionary[module.Cod] = list
                    .Where(x => x.Module == module.Cod)
                    .OrderBy(x => Resources.Resources.ResourceManager.GetString(x.Designation))
                    .ToList();
            }
            return dictionary;
        }

        public List<ModuleRoleModel> FromDictionaryList(Dictionary<string, List<ModuleRoleModel>> dictionary)
        {
            return dictionary.SelectMany(x => x.Value).ToList();

        }
		
		public IList<string> IdentityProviders { get; set; }
    }

    public class UserRole
    {
        public UserRole(Module module, Role role)
        {
            Mod = module;
            Role = role;
        }
        public UserRole() { }

        public Module Mod { get; set; }

        public Role Role { get; set; }
    }

    public class Module
    {
        public Module() { }
        
        public Module(string module, string description)
        {
            Cod = module;
            Description = description;
        }

        public string Cod { get; set; }
        public string Description { get; set; }

        public bool OnlyLevels { get; set; }
    }
}
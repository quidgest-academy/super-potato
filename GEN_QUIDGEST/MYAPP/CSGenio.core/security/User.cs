using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CSGenio.business;

namespace CSGenio.framework
{
	[Serializable]
	public class QUserCfg : ICloneable
	{
		/// <summary>
		/// Year a que corespondam os dados
		/// </summary>
		public string Year { get; protected set; }

		/// <summary>
		/// Lists the highest level for each module
		/// </summary>
		[Obsolete]
		public ReadOnlyDictionary<string, LevelAccess> LevelModules
		{
			get
			{
				Dictionary<string, LevelAccess> accessLevel = new Dictionary<string, LevelAccess> ();
				foreach(var module in ModuleRoles.Keys)
				{
					var levels = ModuleRoles[module]
						.Where(r => r.Type == RoleType.LEVEL);
					int value = 0; //Starts as unauthorized
					if(levels.Any())
						value = levels.Max(r => r.GetLevelInt());
					else if(ModuleRoles[module].Where(r => r.Type == RoleType.SYSTEM && r.Id == Role.ADMINISTRATION.Id).Any())
						value = LevelAccess.NV99.LevelValue;
					var level = LevelAccess.returnAccessLevel(value.ToString());
					accessLevel[module] = level;
				}
				return new ReadOnlyDictionary<string, LevelAccess> (accessLevel);
			}
		}
		public Dictionary<string, List<Role>> ModuleRoles { get; set; } = new Dictionary<string, List<Role>> ();

		/// <summary>
		/// Entradas Permanentes de Historial a que o user é sujeito.
		/// Tem como identifier um par (moduleName, nomeEPH)
		/// </summary>
		public Hashtable Ephs { get; set; }

		/// <summary>
		/// Dicionario com os Qvalues dos fields das ephs.
		/// Tem como identifier um par (moduleName, valorEPH)
		/// </summary>
		public Dictionary<string, Dictionary<string, string>> EphValues { get; set; }

		/// <summary>
		/// Instancias das áreas (os registos) com Entradas Permanentes de Historial a que o user é sujeito.
		/// Tem como identifier um par (moduleName, registoEPH)
		/// </summary>
		public Dictionary<string, Area> EphsRecords { get; set; }

		/// <summary>
		/// An unique key for this user. Might be different for different years
		/// </summary>
		public String Key { get; set;}


		public QUserCfg(string Qyear)
		{
			this.Year = Qyear;

		}

        public QUserCfg(string year, Dictionary<string, List<Role>> moduleRoles, Hashtable ephs, Dictionary<string, Dictionary<string, string>> ephValues, Dictionary<string, Area> ephsRecords, string key) : this(year)
        {
            ModuleRoles = moduleRoles;
            Ephs = ephs;
            EphValues = ephValues;
            EphsRecords = ephsRecords;
            Key = key;
        }

        /// <summary>
        /// Clone QUserCfg
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
			return new QUserCfg(
                year: Year,
                moduleRoles: ModuleRoles?.ToDictionary(kv => kv.Key, kv => kv.Value.Select(role => role).ToList()),
				ephs: (Hashtable)Ephs?.Clone(),
				ephValues: EphValues?.ToDictionary(kv => kv.Key, kv => new Dictionary<string, string>(kv.Value)),
				ephsRecords: EphsRecords?.ToDictionary(kv => kv.Key, kv => kv.Value),
				key: Key
			);
        }
	}

	/// <summary>
	/// Classe que representa um User.
	/// </summary>
	[Serializable]
	public class User : ICloneable
	{
		///Create by [TMV|PG] (26.08.2020)
		public EphsToFill EphTofill { get; set; }

		/// <summary>
		/// Returns true if exist eph to fill in current module
		/// </summary>
		public bool EphOk => EphTofill == null || EphTofill.isOK(CurrentModule);

		private Dictionary<string, QUserCfg> userDataPerYear;

		/// <summary>
		/// Constructor da classe
		/// </summary>
		/// <param name="nome">Login do user</param>
		/// <param name="idSessao">ID da sessão</param>
		/// <param name="anoAplicacao">Year da aplicação</param>
		public User(string name, string idSessao, string anoAplicacao) : this(name, idSessao, anoAplicacao, "") { }

		/// <summary>
		/// Constructor da classe
		/// </summary>
		/// <param name="nome">Login do user</param>
		/// <param name="idSessao">ID da sessão</param>
		/// <param name="anoAplicacao">Year da aplicação</param>
		/// <param name="localizacao">Localização do user</param>
		public User(string name, string idSessao, string anoAplicacao, string location)
		{
			Name = name;
			SessionId = idSessao;
			this.Qyear = anoAplicacao;
			Location = location;

			userDataPerYear = new Dictionary<string, QUserCfg>
			{
				{ this.Qyear, new QUserCfg(this.Qyear) }
			};

			ModuleRoles = new Dictionary<string, List<Role>> ();
		}

		/// <summary>
		/// Constructor with all properties
		/// </summary>
		/// <param name="name">User login</param>
		/// <param name="status">User status</param>
		/// <param name="auth2FA">2FA enable/disable</param>
		/// <param name="auth2FATp">2FA Type</param>
		/// <param name="idSessao">Session Id</param>
		/// <param name="appYear">Application year</param>
		/// <param name="anos">List of years this user has access to.</param>
		/// <param name="publico">If it's Public user</param>
		/// <param name="moduloActual">Module which is being accessed</param>
		/// <param name="linguagemActual">Current language of the application</param>
		/// <param name="location">User location</param>
		/// <param name="tokenAux">Token for use with external resources</param>
		/// <param name="code">Code received by external resources</param>
		/// <param name="validated">Validated external resource parameters</param>
		/// <param name="ephTofill">Initial PHE to be filled</param>
		/// <param name="userDataPerYear"></param>
        public User(string name, int status, bool auth2FA, string auth2FATp, string idSessao, string appYear, List<string> anos, bool publico, string moduloActual, string linguagemActual, string location, string tokenAux, string code, bool validated, EphsToFill ephTofill, Dictionary<string, QUserCfg> userDataPerYear)
        {
            Name = name;
            Status = status;
            Auth2FA = auth2FA;
            Auth2FATp = auth2FATp;
            SessionId = idSessao;
            Qyear = appYear;
            Years = anos;
            Public = publico;
            CurrentModule = moduloActual;
            Language = linguagemActual;
            Location = location;
            TokenAux = tokenAux;
            Code = code;
            Validated = validated;
            EphTofill = ephTofill;
            this.userDataPerYear = userDataPerYear ?? new Dictionary<string, QUserCfg>();

			if (!this.userDataPerYear.Any())
				this.userDataPerYear.Add(this.Qyear, new QUserCfg(this.Qyear));

            if (ModuleRoles == null)
                ModuleRoles = new Dictionary<string, List<Role>>();
        }

		/// <summary>
		/// Método que permite colocar ou devolver o login do user
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Método que permite colocar ou devolver o status do user
		/// </summary>
		public int Status { get; set; }

		/// <summary>
		/// Método que permite colocar ou devolver se o user tem 2FA ativado
		/// </summary>
		public bool Auth2FA { get; set; }

		/// <summary>
		/// Método que permite colocar ou devolver qual o tipo de 2FA que o user tem ativado
		/// </summary>
		public string Auth2FATp { get; set; }

		/// <summary>
		/// Método que permite colocar ou devolver a lista de modulos e respectivo level do user
		/// </summary>
		[Obsolete]
		public ReadOnlyDictionary<string, LevelAccess> LevelModules
		{
			get
			{
				return userDataPerYear[this.Qyear].LevelModules;
			}
		}

		/// <summary>
		/// The roles of this user in each module
		/// </summary>
		public IDictionary<string, List<Role>> RolesPerModule
		{
			get
			{
				return ModuleRoles;
			}
		}

		/// <summary>
		/// Método que permite colocar ou devolver a lista de modulos e respectivo level do user
		/// </summary>
		private Dictionary<string, List<Role>> ModuleRoles
		{
			get
			{
				return userDataPerYear[this.Qyear].ModuleRoles;
			}
			set
			{
				userDataPerYear[this.Qyear].ModuleRoles = value;
			}
		}

		/// <summary>
		/// Método que permite colocar ou devolver as Entradas Permanentes de Historial a que o user é sujeito
		/// </summary>
		public Hashtable Ephs
		{
			get
			{
				return userDataPerYear[this.Qyear].Ephs;
			}
			set
			{
				userDataPerYear[this.Qyear].Ephs = value;
			}
		}

		/// <summary>
		/// Método que permite colocar ou devolver um dicionario com os Qvalues dos fields das ephs
		/// </summary>
		public Dictionary<string, Dictionary<string, string>> EphValues
		{
			get
			{
				return userDataPerYear[this.Qyear].EphValues;
			}
			set
			{
				userDataPerYear[this.Qyear].EphValues = value;
			}
		}

		/// <summary>
		/// Método que permite colocar ou devolver instancias das áreas (os registos) com Entradas Permanentes de Historial a que o user é sujeito
		/// </summary>
		public Dictionary<string, Area> EphsRecords
		{
			get
			{
				return userDataPerYear[this.Qyear].EphsRecords;
			}
			set
			{
				userDataPerYear[this.Qyear].EphsRecords = value;
			}
		}

		/// <summary>
		/// Returns the codpsw of this user. One per year
		/// </summary>
		public string Codpsw
		{
			get {
				return userDataPerYear[this.Qyear].Key;
			}
			set {
				userDataPerYear[this.Qyear].Key = value;
			}
		}

		/// <summary>
		/// Método que permite devolver o id da sessão
		/// </summary>
		public string SessionId { get; set; }


        private string Qyear; //Qyear actual
        /// <summary>
        /// Método que permite devolver o Qyear que está a aceder
        /// </summary>
        public string Year
		{
			get { return Qyear; }
			set
			{
				Qyear = value;
				if(!userDataPerYear.ContainsKey(Qyear))
					userDataPerYear.Add(Qyear, new QUserCfg(Qyear));
			}
		}

		/// <summary>
		/// Returns the inferred year of the database. 0 in case its not parseable.
		/// </summary>
		public int NumericYear
		{
			get
			{
				int res = 0;
				int.TryParse(Qyear, out res);
				return res;
			}
		}

		/// <summary>
		/// Lista de anos em que o user tem permissão de entrar
		/// </summary>
		public List<string> Years { get; set; } = [];

		/// <summary>
		/// Método que verifica se o user tem level definido to o módulo
		/// </summary>
		/// <param name="modulo">name do módulo</param>
		/// <returns>true se tem, false caso contrário</returns>
		[Obsolete]
		public bool hasLevelModule(string module)
		{
			if(LevelModules == null)
				return false;
			else if(!LevelModules.ContainsKey(module))
				return false;
			else
				return true;
		}

		/// <summary>
		/// Método que devolve o level do user to o module
		/// </summary>
		/// <param name="modulo">name do módulo</param>
		/// <returns>level de acesso</returns>
		[Obsolete]
		public int getLevelByModule(string module)
		{
			if(!LevelModules.ContainsKey(module))
				return 0;
			else
				return LevelModules[module].LevelValue;
		}

		/// <summary>
		/// Método que permite preencher e devolver se o user é publico
		/// </summary>
		public bool Public { get; set; }

		/// <summary>
		/// Método que permite colocar ou devolver o módulo a que o user está ligado
		/// </summary>
		public string CurrentModule { get; set; }

		/// <summary>
		/// Método que permite colocar ou devolver a linguagem actual da aplicação
		/// </summary>
		public string Language { get; set; }

		/// <summary>
		/// Verifica se o user tem alguma eph definida
		/// </summary>
		/// <returns>true se e só se o user tem alguma eph definida</returns>
		public bool hasEph()
		{
			return Ephs != null;
		}

		/// <summary>
		/// Verifica se o user tem a EPH da área indicada
		/// </summary>
		/// <param name="areaEph">Name da área (alias)</param>
		/// <returns>true se e só se o user tem eph to o presente módulo</returns>
		public bool hasEph(string areaEph)
		{
			return hasEph(CurrentModule, areaEph);
		}

		/// <summary>
		/// Verifica se o user tem a EPH da área indicada
		/// </summary>
		/// <param name="modulo">Módulo</param>
		/// <param name="areaEph">Name da área (alias)</param>
		/// <returns>true se e só se o user tem eph definida to a area no módulo indicado</returns>
		public bool hasEph(string module, string areaEph)
		{
			if(Ephs != null)
				return Ephs.Contains(module.ToUpper() + "_" + areaEph.ToUpper());
			else
				return false;
		}

		public string[] GetEph(string module, string areaEph)
		{
			if(hasEph(module, areaEph))
				return (string[]) Ephs[module.ToUpper() + "_" + areaEph.ToUpper()];

			return null;
		}

		public void SetEph(string module, string areaEph, string[] valueEph)
		{
			if(hasEph(module, areaEph))
				Ephs[module.ToUpper() + "_" + areaEph.ToUpper()] = valueEph;
		}

		/// <summary>
		/// Devolve o dicionario com os Qvalues dos fields da area da eph
		/// <param name="areaEph">Name da área (alias)</param>
		/// </summary>
		public string[] fieldsEph(string areaEph)
		{
			try
			{
				return (string[]) (this.hasEph(areaEph) ? Ephs[this.CurrentModule + "_" + areaEph.ToUpper()] : null);
			}
			catch(KeyNotFoundException)
			{
				return null;
			}
		}

        /// <summary>
        /// Check the initial EPHs and remove from the list those that are already filled.
        /// </summary>
        public void RevalidateEPHRuntime()
        {
            if (EphOk || EphTofill?.Ephs == null)
                return;
            //note: ToList is used for prevent error: Collection was modified
            // <[module], <[initial eph], list<EPHCondition>>>
            foreach (KeyValuePair<string, Dictionary<string, List<EPHCondition>>> eph in EphTofill.Ephs.ToList())
            {
                var module = eph.Key;
                //  <[initial eph], list<EPHCondition>>
                foreach (KeyValuePair<string, List<EPHCondition>> initialEph in eph.Value.ToList())
                {
                    foreach (var ephCondition in initialEph.Value.ToList())
                    {
                        if (hasEph(module, ephCondition.EPHName))
                            EphTofill.Remove(module, ephCondition);
                    }
                }
            }
        }

		/// <summary>
		/// Localização do user, utilizada to validação
		/// Na Web é o endereço de IP do cliente do qual estamos a receber os pedidos
		/// </summary>
		public string Location {  get; set; }

		/// <summary>
		/// token to utilização a recursos externos
		/// </summary>
		public string TokenAux { get; set; }

		/// <summary>
		/// code to utilização a recursos externos
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// external code and token validated
		/// </summary>
		public bool Validated { get; set; }

		/// <summary>
		/// Verificar se user tem permissões de acesso ao determinado role (MH: to MVC)
		/// </summary>
		/// <param name="role">Role that will be checked against user permissions</param>
		/// <returns></returns>
		public bool VerifyAccess(Role role)
		{
			return VerifyAccess(role, CurrentModule);
		}

		/// <summary>
		/// Checks if a user has access to a given role in the specified module
		/// </summary>
		/// <param name="role"></param>
		/// <param name="module"></param>
		/// <returns></returns>
		public bool VerifyAccess(Role role, string module)
		{
			if(role == null)

				return false;

			var userModuleRoles = GetModuleRoles(module);

			return userModuleRoles.Any(r => role.HasRole(r));
		}

		/// <summary>
		/// Check the user level in the specific module (MH: to MVC)
		/// </summary>
		/// <param name="level">Level</param>
		/// <param name="mod">By default, the current module</param>
		/// <returns></returns>
		[Obsolete]
		public bool CheckModuleLevel(int level, string module = null)
		{
			if(string.IsNullOrEmpty(module))
				module = CurrentModule ?? string.Empty;
			return getLevelByModule(module) == level;
		}

		/// Adds a role module to the list of allowed roles for the user
		public void AddModuleRole(string module, Role role)
		{
			if (role is null || role == Role.INVALID)
				return;
			if (!ModuleRoles.ContainsKey(module))
				ModuleRoles[module] = new List<Role>();
			ModuleRoles[module].Add(role);
		}

		/// Removes a role module to the list of allowed roles for the user
		public void RemoveModuleRole(string module, Role role)
		{
			if(ModuleRoles.ContainsKey(module) && ModuleRoles[module].Contains(role))
				ModuleRoles[module].Remove(role);
		}

		/// <summary>
		/// Returns all roles for a given module. If it doens't have anything the list returns a list with unauthorized role.
		/// </summary>
		/// <param name="module">The module we want to check</param>
		public List<Role> GetModuleRoles(string module)
		{
			if(module == null || !ModuleRoles.ContainsKey(module))
			{
				List<Role> roleList = new List<Role> ();
				roleList.Add(Role.UNAUTHORIZED);
				return roleList;
			}
			else
			{
				return ModuleRoles[module];
			}
		}

		/// <summary>
		/// Checks if the user is an administrator in a given module
		/// </summary>
		/// <param name="module">Module we want to check</param>
		/// <returns></returns>
		public bool IsAdmin(string module)
		{
			if(module == null || !ModuleRoles.ContainsKey(module))
				return false;

			return ModuleRoles[module].Any(r => r.IsAdmin);
		}

		/// <summary>
		/// Gets the list of modules where this user is admin.
		/// </summary>
		/// <returns></returns>
		public bool IsAdminInAnyModule()
		{
			return ModuleRoles.Keys.Any(m => IsAdmin(m));
		}

		/// <summary>
		/// Checks if the user is a guest
		/// </summary>
		/// <returns></returns>
		public bool IsGuest()
		{
			return GenioServer.security.UserFactory.IsGuest(this.Name);
		}

		/// <summary>
		/// Checks if a user is authorized in a given module. All users are authorized in public module
		/// </summary>
		/// <param name="module"></param>
		public bool IsAuthorized(string module)
		{
			if(module == "Public")
				return true;
			var roles = GetModuleRoles(module);
			//True if there is at least one role that is not unauthorized.
			return roles.Any(r => r != Role.UNAUTHORIZED);
		}

		/// <summary>
		/// Check if user has a specific role
		/// </summary>
		/// <param name="module"></param>
		/// <returns></returns>
		public bool HasSpecificRole(Role role, string module)
		{
			if (!ModuleRoles.ContainsKey(module))
				return false;
			return ModuleRoles[module].Any(r => r == role);
		}

		/// <summary>
		/// Check if the user needs to change the password before navigating the application
		/// </summary>
		public bool NeedsToChangePassword()
        {
			return Status == 1;
        }

		/// <summary>
		/// Check if the user needs to change the password before navigating the application
		/// </summary>
		public bool NeedsToSetup2FA()
		{
			return CSGenio.framework.Configuration.Security.Mandatory2FA && !Auth2FA;
		}

		/// <summary>
		/// Clone user object
		/// </summary>
		/// <returns></returns>
        public object Clone()
        {
			return new User(
				name: Name,
				status: Status,
				auth2FA: Auth2FA,
				auth2FATp: Auth2FATp,
				idSessao: SessionId,
				appYear: Qyear,
				anos: new List<string>(Years),
				publico: Public,
				moduloActual: CurrentModule,
				linguagemActual: Language,
				location: Location,
				tokenAux: TokenAux,
				code: Code,
				validated: Validated,
				ephTofill: (EphsToFill)EphTofill?.Clone(),
				userDataPerYear: userDataPerYear?.ToDictionary(kv => kv.Key, kv => (QUserCfg)kv.Value.Clone())
				);
        }
	}

	/// <summary>
	/// Represent the user information to be presented on the inteface
	/// </summary>
	public class UserInfo
	{
		/// <summary>
		/// Get or Set the content o image
		/// </summary>
		public byte[] Image { get; set; }

		/// <summary>
		/// Get or Set the user full name
		/// </summary>
		public string Fullname { get; set; }

		/// <summary>
		/// Get or Set the user position or profile
		/// </summary>
		public string Position { get; set; }

		/// <summary>
		/// Test if the user info is empty
		/// </summary>
		/// <returns></returns>
		public bool IsEmpty()
		{
			if(Image == null && string.IsNullOrEmpty(Fullname) && string.IsNullOrEmpty(Position))
				return true;
			else
				return false;
		}
	}

	///Create by [TMV|PG] (26.08.2020)
	//Contais all the ephs to fill
	public class EphsToFill : ICloneable
	{
		/// <summary>
		/// Validates if existe any eph fo fill for the module
		/// </summary>
		/// <param name="module">module</param>
		/// <returns></returns>
		public bool isOK(string module)
		{
			return string.IsNullOrEmpty(module) || (!string.IsNullOrEmpty(module) && !ephs.ContainsKey(module));
		}

		private Dictionary<string, Dictionary<string, List<EPHCondition>>> ephs;
		public Dictionary<string, Dictionary<string, List<EPHCondition>>> Ephs { get { return this.ephs; } }

		/*
		 * ->Module
		 * -> -> Form
		 * -> -> -> List< EPH condition>
		 *      */

		public EphsToFill()
		{
			ephs = new Dictionary<string, Dictionary<string, List<EPHCondition>>> ();
		}

        public EphsToFill(Dictionary<string, Dictionary<string, List<EPHCondition>>> ephs)
        {
            this.ephs = ephs ?? new Dictionary<string, Dictionary<string, List<EPHCondition>>>();
        }

		/// <summary>
		/// Add a new eph condition to the struct
		/// </summary>
		/// <param name="module">Eph module</param>
		/// <param name="formId">Eph formId </param>
		/// <param name="condition"></param>
		public void AddNew(string module, EPHCondition condition)
		{
			string formId = condition.IntialForm;

			if(!this.ephs.ContainsKey(module))
			{
				this.ephs.Add(module, new Dictionary<string, List<EPHCondition>> ()
				{ { formId, new List<EPHCondition> () { condition } }
				});
			}
			else if(!this.ephs[module].ContainsKey(formId))
			{
				this.ephs[module].Add(formId, new List<EPHCondition> () { condition });
			}
			else if(!this.ephs[module][formId].Contains(condition))
			{
				this.ephs[module][formId].Add(condition);
			}
		}

		/// <summary>
		/// Remove a condition for the module a form id
		/// </summary>
		/// <param name="module">Eph module</param>
		/// <param name="condition">Eph condition</param>
		public void Remove(string module, EPHCondition condition)
		{
			string formId = condition.IntialForm;

			if(this.ephs.ContainsKey(module) && this.ephs[module].ContainsKey(formId) && this.ephs[module][formId].Contains(condition))
			{
				this.ephs[module][formId].Remove(condition);

				if(this.ephs[module][formId].Count == 0)
					this.ephs[module].Remove(formId);

				if(this.ephs[module].Count == 0)
					this.ephs.Remove(module);
			}

		}

		/// <summary>
		/// Removel all condition for a module a form id
		/// </summary>
		/// <param name="module"></param>
		/// <param name="formId"></param>
		public void RemoveAllEph(string module, string formId)
		{
			if(this.ephs.ContainsKey(module) && this.ephs[module].ContainsKey(formId))
			{
				this.ephs[module].Remove(formId);

				if(this.ephs[module].Count == 0)
					this.ephs.Remove(module);
			}
		}

		/// <summary>
		/// Gets as eph to fill for the current module
		/// </summary>
		/// <param name="current">User current module</param>
		/// <returns></returns>
		public string GetForm(string currentModule)
		{
			if(!String.IsNullOrEmpty(currentModule) && ephs.Any() && this.ephs.ContainsKey(currentModule))
			{
				return this.ephs[currentModule].FirstOrDefault().Key;
			}

			return "";
		}

        /// <summary>
        /// Clone EphsToFill
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
			return new EphsToFill(
				ephs: ephs?.ToDictionary(
						outerPair => outerPair.Key, // clone module keys
						outerPair => outerPair.Value.ToDictionary(
							innerPair => innerPair.Key, // clone initial PHE keys
							innerPair => innerPair.Value.Select(item => (EPHCondition)item.Clone()).ToList() // clone each EPHCondition associated with initial PHE
						))
			);
        }
	}
}

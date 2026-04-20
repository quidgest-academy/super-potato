#region Description
/*
 *  Os históricos no MVC existem do lado do cliente e do lado do servidor.
 *  O historial do lado do cliente armazenado no LocalStorage e contem os Qvalues de todos fields preenchidos no form (timeout = 24Horas).
 *  O save desses Qvalues efetuado depois do fecho dos DBEdits, depois de mudar o tab (controlo tab), antes de sair do form (quando user abre um form de apoio ou algo semelhante).
 *  O histórico de navegação do lado do servidor, armazenado no SessionState e por default tem timeout de 20 min de inatividade.
 *  Esse histórico está por niveis em que cada nível tem informação do FormMode, Controller, Action, Id (se aplicável) e normalmente tem nas entradas do historial a área e key do registo aberto no form.
 *  Os ambos históricos estão associados a name da janela do browser em que forem abertos.
 *
 •	> A lista dos níveis do Históricos passou a ser uma lista simples de <Level, HistoryLevel>.
 *  > Cada nível contem Entries, FormMode, Level e location. A lista das entradas do historial também passou ser uma lista simples (Hashtable) <key, value>
 •	> Todas funções do Get / Set passarem de estar no NavigationContext em vez de estar no HistoryLevel.
 •	> Adicionado o parâmetro “nav” a query string - Id da navegação que corresponde a window.name e serve to distinguir os vários tabs/janelas do browser.
 •	> Cada abertura dum menu (newMenu=True na query string) cria uma nova navegação. Isto foi feito to casos quando user abra o menu em nova janela
 *      to que não apaga os níveis na navegação da janela original (no primeiro load o servidor não sabe se é nova janela ou não, mas como tem Id da navegação no URL vai utilizar esta navegação e remove todos níveis).
 •	> Breadcrumbs - no servidor antes de “escolher” histórico de navegação, se for deteta o "bc=True" na query string,
 *      então vai duplicate a navegação atual to que no caso de ter aberto em nova janela, manter os níveis anteriores.
 •	> Adicionado o javascript que faz replace do window.history to retirar o url da janela anterior to que nos casos em que user clica no Back do Browser,
 *      não efetuava navegação com Id da outra janela e estraga a navegação da mesma.
 •	> Todos Partial Views deixarem de criar um nível no histórico, agora todas entradas do historial dos Partial Views e do próprio form ficam ‘misturados’ num só nível.
 *      Como foi alterado a estrutura do historial to simples lista dos níveis em que cada nível anterior representa o parente level, e também to que que no fecho do form com remove do ultimo nível,
 *      apagar o nível do form e dos partial views associados e não só dum partial view que foi ultimo inserido. E também to que na inicialização dum novo form saber quem é o “parente real” do form.
 •	> Query string tem novo parâmetro “lvl” que corresponde ao nível do historial dessa janela. Esse ‘nível’ utilizado quando
 *      por exemplo user abra em nova janela o form da área A e depois abra form de apoio da área B na mesma janela, e to que não ter os níveis “a mais”
 *      que não correspondem a janela atual será removidos todos níveis a seguir do nível indicado nesse parâmetro.
 *      Outra janela adiciona um nível no histórico da navegação da página de source porque no primeiro load da página o servidor não sabe se foi aberto em nova janela,
 *      mas como tem id da navegação da janela de source no query string, assim fica com um nível do historicoss que não lhe pertence.
 •	> Todas páginas agora tenham dois hidden field: CurrentNavigationId e CurrentHistoryLevel. O Qvalue do CurrentNavigationId no load da página (javascript)
 *      será comparado com window.name to verificar se servidor tinha atribuído outro Id to navegação da janela (por exemplo na abertura do menu).
 *      O Qvalue do CurrentHistoryLevel atribuído ao parâmetro “lvl” da query string em todos href’s da página aberta.
 •	> Só o servidor que atribui Id da janela. Se javascript no load deteta que window.name diferente do Id que vem do servidor, esse id será atribuído a janela.
 */
#endregion

using CSGenio.framework;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;
using JsonPropertyNameAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace GenioMVC.Models.Navigation
{
	/// <summary>
	/// Context aggregator for a navigation (HistoryLevel's)
	/// This base class is used for serialization / deserialization.
	/// Note: If using the NavigationContext directly for serializing, the NavigationContext.GetObjectData serialization method will cause a loop.
	/// </summary>
	[Serializable]
	public class NavigationContextBase
	{
		/// <summary>
		/// To facilitar acesso a Id da navegação atual.
		/// Usado nos RouteData dos RedirectToAction e nas View's (hidden field)
		/// </summary>
		public string NavigationId { get; set; }

		/// <summary>
		/// Timeout igual a timeout que definido to Session.
		/// Usado to que poder eliminate as Navegações não usadas mais.
		/// </summary>
		protected DateTime Timeout { get; set; }

		/// <summary>
		/// Lista dos niveis do history
		/// </summary>
		[System.Text.Json.Serialization.JsonInclude]
		public ConcurrentStack<HistoryLevel> History { get; protected set; }

		/// <summary>
		/// Holds values of form menu items that allow going back multiple levels in history.
		/// </summary>
		/// <remarks>FOR: FORM MENU GO BACK</remarks>
		public Dictionary<string, int> GoBack { get; set; }

		/// <summary>
		/// Holds values of form menu items that allow overriding the "skip if just one" feature.
		/// </summary>
		/// <remarks>FOR: OVERRIDE SKIP IF JUST ONE</remarks>
		public Dictionary<string, bool> OverrideSkipIfJustOne { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="NavigationContextBase"/> class.
		/// </summary>
		public NavigationContextBase()
		{
			GoBack = new Dictionary<string, int>();
			OverrideSkipIfJustOne = new Dictionary<string, bool>();
			History = new ConcurrentStack<HistoryLevel>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NavigationContextBase"/> class.
		/// </summary>
		/// <param name="source">The base navigation context source.</param>
		public NavigationContextBase(NavigationContextBase source)
		{
			NavigationId = source.NavigationId;
			Timeout = source.Timeout;
			GoBack = source.GoBack ?? new Dictionary<string, int>();
			OverrideSkipIfJustOne = source.OverrideSkipIfJustOne ?? new Dictionary<string, bool>();
			History = source.History ?? new ConcurrentStack<HistoryLevel>();
		}
	}

	/// <summary>
	/// Context aggregator for a navigation (HistoryLevel's)
	/// </summary>
	[Serializable]
	public class NavigationContext : NavigationContextBase, ISerializable
	{
		[JsonIgnore, NonSerialized]
		private readonly object m_lock = new object();

		public bool IsValid()
		{
			lock(m_lock)
			{
				return Timeout >= DateTime.Now;
			}
		}

		/// <summary>
		/// Original Lista dos niveis do history
		/// </summary>
		[JsonIgnore]
		private ConcurrentStack<HistoryLevel> OriginalHistory { get; set; }


		[JsonIgnore]
		private readonly UserContext m_userContext;

		/// <summary>
		/// Constructor with default timeout value
		/// </summary>
		/// <param name="timeout"></param>
		public NavigationContext(UserContext userContext, int timeout=20)
		{
			m_userContext = userContext;
			this.NavigationId = createWinId(11);
			this.UpdateTimeout(timeout);
			firstLoad();
		}

		/// <summary>
		/// Constructor without "firstLoad" and just based on deserialized data.
		/// </summary>
		/// <param name="source"></param>
		public NavigationContext(UserContext userContext, NavigationContextBase source) : base(source)
		{
			m_userContext = userContext;
		}

		/// <summary>
		/// Update navigation context lifetime
		/// </summary>
		/// <param name="minutes"></param>
		public void UpdateTimeout(int minutes)
		{
			lock(m_lock)
			{
				this.Timeout = DateTime.Now.AddMinutes(minutes);
			}
		}

		/// <summary>
		/// Inicializa lista dos niveis e adiciona o primeiro level 0 ("0 = index" <- ainda falta rever código, se é preciso...)
		/// </summary>
		private HistoryLevel firstLoad()
		{
			History = new ConcurrentStack<HistoryLevel>();
			HistoryLevel firstLevel = new HistoryLevel(NavigationLocation.Any, FormMode.None);
			History.Push(firstLevel);
			//FOR: FORM MENU GO BACK
			GoBack = new Dictionary<string, int>();
			//FOR: OVERRIDE SKIP IF JUST ONE
			OverrideSkipIfJustOne = new Dictionary<string, bool>();

			loadUserEphs(firstLevel);
			return firstLevel;
		}

		private void loadUserEphs(HistoryLevel history)
		{
			string module = m_userContext.User.CurrentModule;
			string prefix = module + "_";
			Hashtable ephs = m_userContext.User.Ephs;

			if (module == null || ephs == null)
				return;

			IDictionaryEnumerator en = ephs.GetEnumerator();
			while (en.MoveNext())
			{
				string ephKey = ((string)en.Key);

				// skip ephs from a different module or with a null value
				if (string.IsNullOrEmpty(ephKey) || !ephKey.StartsWith(prefix) || en.Value == null)
					continue;

				string ephId = ephKey.Substring(prefix.Length);
				CSGenio.framework.EPHCondition eph =
					CSGenio.framework.EPH.GetEphConditionById(ephId);

				if (eph == null)
					continue;

				object value = en.Value;

				if (value is Array values)
				{
					// skip empty arrays
					if (values.Length < 1)
						continue;
					// unbox array if there is only one value
					else if (values.Length == 1)
						value = values.GetValue(0);
				}

				// Note: the same EPH can be defined in multiple modules accessible to the user
				if (eph.EPHFieldType.IsKey() && !history.CheckEntry(eph.EPHTable.ToLower()))
					history.SetEntry(eph.EPHTable.ToLower(), value);
				else if (!history.CheckEntry(eph.EPHTable.ToLower() + "." + eph.EPHField.ToLower()))
					history.SetEntry(eph.EPHTable.ToLower() + "." + eph.EPHField.ToLower(), value);
			}
		}

		/// <summary>
		/// Corresponde ultimo level na lista
		/// </summary>
		[JsonIgnore]
		public HistoryLevel CurrentLevel
		{
			get
			{
				HistoryLevel curLevel = null;
				if (!(History?.TryPeek(out curLevel)).Value)
					curLevel = firstLoad();
				return curLevel;
			}
		}

		/// <summary>
		/// "Parent" do ultimo level.
		/// Null - quando o level não tem nenhum level a cima
		/// </summary>
		[JsonIgnore]
		public HistoryLevel PreviousLevel
		{
			get
			{
				return History?.Skip(1).FirstOrDefault();
			}
		}

		/// <summary>
		/// Generate new navigation ID
		/// </summary>
		/// <param name="Idlen">Width of the new ID, by default equal to 8 characters.</param>
		/// <returns>ID with characters: 0-9|a-z|A-Z</returns>
		public static string createWinId(int Idlen = 8)
		{
			//Generate N length code
			return GenioServer.security.PasswordFactory.StringRandom(Idlen, true);
		}

		internal bool ContainsAction(NavigationLocation location)
		{
			return CheckAction(location.Action);
		}

		/// <summary>
		/// Check if the level exists with the specific action
		/// </summary>
		/// <param name="action">The Action to be searched for</param>
		/// <param name="level">The action must match to the specified level (-1 for any level)</param>
		/// <returns></returns>
		internal bool CheckAction(string action, int level = -1)
		{
			return History.Any(h => h.Location.Action == action && (level == -1 || h.Level == level));
		}

		/// <summary>
		/// Clone the Navigation Context.
		/// Used for clone history to the new window
		/// </summary>
		/// <returns></returns>
		public NavigationContext Clone()
		{
			return new NavigationContext(this.m_userContext)
			{
				History = new ConcurrentStack<HistoryLevel>(History.Reverse().Select(h => h.Clone()))
			};
		}

		#region Get / Set / Remove das entradas do history

		private T getDefaultEntryValue<T>()
		{
			var type = typeof(T);
			if (type.IsValueType)
				return (T)Activator.CreateInstance(type);
			return (T)(null as object);
		}

		/// <summary>
		/// Search history levels for a specific key.
		/// </summary>
		/// <param name="key">Key to lookup</param>
		/// <param name="value">If the key exists, we get the associated Qvalue</param>
		/// <param name="level">Lookup starting history level (bottom-up)</param>
		/// <returns>true if the key is found, false otherwise</returns>
		public bool CheckKey<T>(string key, out T value, int level)
		{
			value = getDefaultEntryValue<T>();
			var hLevel = History.FirstOrDefault(h => h.Level <= level && h.CheckEntry(key));
			if (hLevel != null)
			{
				value = hLevel.GetEntry<T>(key);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Search history levels for a specific key.
		/// </summary>
		/// <param name="key">Key to lookup</param>
		/// <param name="value">If the key exists, we get the associated Qvalue</param>
		/// <param name="level">Lookup starting history level (bottom-up)</param>
		/// <returns>true if the key is found, false otherwise</returns>
		public bool CheckKey(string key, out object value, int level)
		{
			return CheckKey<object>(key, out value, level);
		}

		/// <summary>
		/// Search history levels for a specific key.
		/// </summary>
		/// <param name="key">Key to lookup</param>
		/// <param name="level">Lookup starting history level (bottom-up)</param>
		/// <returns>true if the key is found, false otherwise</returns>
		public bool CheckKey(string key, int level)
		{
			return CheckKey(key, out _, level);
		}

		/// <summary>
		/// Search history levels for a specific key, starting from the bottom level.
		/// </summary>
		/// <param name="key">Key to lookup</param>
		/// <returns>True if the key is found, false otherwise</returns>
		public bool CheckKey(string key) { return this.CheckKey(key, CurrentLevel.Level); }

		/// <summary>
		/// Obter o Qvalue da key
		/// </summary>
		/// <param name="key">Key a procurar</param>
		/// <param name="level">Permite indicar o level a partir do qual procurar (de baixo to cima)</param>
		/// <returns>null - Caso se não encontra</returns>
		public object GetValue(string key, int level)
		{
			CheckKey(key, out object value, level);
			return value;
		}

		/// <summary>
		/// Obter o Qvalue da key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="previousLevel">Se procurar só a partir do level anterior. (por default a partir do ultimo level)</param>
		/// <returns>null - Caso se não encontra</returns>
		/// <remarks>Depois do analise dos manwins, foi detetado um uso muito frequente da inserção to o level anteriro => adicionado previousLevel</remarks>
		public object GetValue(string key, bool previousLevel = false)
		{
			if (previousLevel && PreviousLevel != null)
				return GetValue(key, PreviousLevel.Level);
			return GetValue(key, CurrentLevel.Level);
		}

		/// <summary>
		/// Obter o Qvalue da key em String
		/// </summary>
		/// <param name="key"></param>
		/// <param name="previousLevel">Se procurar só a partir do level anterior. (por default a partir do ultimo level)</param>
		/// <returns>null - Caso se não encontra</returns>
		/// <remarks>Depois do analise dos manwins, foi detetado um uso muito frequente da inserção to o level anteriro => adicionado previousLevel</remarks>
		public string GetStrValue(string key, bool previousLevel = false)
		{
			int level = CurrentLevel.Level;
			if (previousLevel && PreviousLevel != null)
				level = PreviousLevel.Level;

			object hValue = GetValue(key, level);

			// this method should only be used if we expect a single value to be present
			if (hValue is Array)
				return null;

			return Convert.ToString(hValue);
		}

		/// <summary>
		/// Obter o valor da key em tipo especifico
		/// </summary>
		/// <param name="key"></param>
		/// <param name="previousLevel">Se procurar só a partir do level anterior. (por default a partir do ultimo level)</param>
		/// <returns>null/default - Caso se não encontra</returns>
		public T GetValue<T>(string key, bool previousLevel = false)
		{
			var level = CurrentLevel.Level;
			if (previousLevel && PreviousLevel != null)
				level = PreviousLevel.Level;

			CheckKey<T>(key, out T hValue, level);

			return hValue;
		}

		/// <summary>
		/// Obter o Qvalue da key em DateTime
		/// </summary>
		/// <param name="key"></param>
		/// <param name="previousLevel">Se procurar só a partir do level anterior. (por default a partir do ultimo level)</param>
		/// <returns>null - Caso se não encontra</returns>
		/// <remarks>Depois do analise dos manwins, foi detetado um uso muito frequente da inserção to o level anteriro => adicionado previousLevel</remarks>
		public DateTime? GetDateValue(string key, bool previousLevel = false)
		{
			DateTime temp;
			var value = GetValue(key, previousLevel);

			if (value == null || value is Array)
				return null;
			else if (value is DateTime)
				return (DateTime)value;
			else if (DateTime.TryParse(Convert.ToString(value), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out temp))
				return temp;
			return null;
		}

		/// <summary>
		/// Atribuir o valor da key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="previousLevel">Atribuir to o level anterior. (por default to o ultimo level)</param>
		/// <remarks>Depois do analise dos manwins, foi detetado um uso muito frequente da inserção to o level anteriro => adicionado previousLevel</remarks>
		public void SetValue(string key, object value, bool previousLevel = false)
		{
			if (previousLevel && PreviousLevel != null) this.SetValue(key, value, PreviousLevel.Level);
			else this.SetValue(key, value, CurrentLevel.Level);
		}

		public void SetValue(string key, object value, int level)
		{
			History.FirstOrDefault(h=> h.Level == level)?.SetEntry(key, value);
		}

		/// <summary>
		/// Remover a key
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="previousLevel">Remover a key no level anterior. (por default no ultimo level)</param>
		public void ClearValue(string key, bool previousLevel = false)
		{
			if (previousLevel && PreviousLevel != null)
				this.ClearValue(key, PreviousLevel.Level);
			else
				this.ClearValue(key, CurrentLevel.Level);
		}

		/// <summary>
		/// Remover a key num determinado level do Historial
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="level">QLevel</param>
		public void ClearValue(string key, int level)
		{
			History.FirstOrDefault(h => h.Level == level)?.RemoveEntry(key);
		}

		/// <summary>
		/// Remove every entry with the specific key in all history levels.
		/// </summary>
		/// <param name="key">Key to be searched</param>
		public void DestroyEntry(string key)
		{
			foreach (var hist in History)
				ClearValue(key, hist.Level);
		}

		#endregion

		/// <summary>
		/// Verificar se exists algum formulario duma determinada area em determinado mode
		/// </summary>
		/// <param name="formArea">Controller / Area do Form (case insensitive)</param>
		/// <param name="formMode"></param>
		/// <returns></returns>
		public bool checkFormMode(string formArea, FormMode formMode)
		{
			return History.Any(h => h.FormMode == formMode && string.Equals(h.Location.Controller, formArea, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// MH (29/12/2016)
		/// Verificar se exists no historial algum form aberto em mode insersão ou edição.
		/// Utilizado pelo JavaScript to avisar o user que pode perder informação caso se abre um link dum menu qualquer ou outro semelhante.
		/// </summary>
		/// <returns></returns>
		public bool FormHistLock()
		{
			return History.Any(h => h.FormMode == FormMode.New || h.FormMode == FormMode.Edit);
		}

		/// <summary>
		/// Returns a hash table with the list of history entries in all levels
		/// </summary>
		/// <param name="onlyCurrentLevel">Indicates if we want entries, from the current level only</param>
		/// <returns>
		/// Hashtable containing the list of entries in all levels
		/// </returns>
		/// <remarks>
		/// The search is made from the last to the first
		///
		/// CÓDIGO ANTIGO - Verificar necesidade desse código, (Usado algures nos documentos)
		///
		/// </remarks>
		public Hashtable GetAllEntries(bool onlyCurrentLevel = false)
		{
			Hashtable history = new Hashtable();
			// History (stack) - The enumerator returns items in LIFO (last-in, first-out) order
			foreach (var hLevel in History)
			{
				foreach (var key in hLevel.EntriesKeys)
					if (!history.ContainsKey(key))
						history.Add(key, hLevel.GetEntry(key));

				// Get entries from the parent only in some cases
				if (onlyCurrentLevel)
					break;
			}

			return history;
		}

		public void SaveOriginal()
		{
			OriginalHistory = new ConcurrentStack<HistoryLevel>(History.Reverse().Select(h => h.Clone()));
		}

		public class ClientSideHistoryResult
		{
			[JsonPropertyName("historyDiff")]
			public List<object> HistoryDiff { get; set; }

			[JsonPropertyName("navigationId")]
			public string NavigationId { get; set; }
		}

		/// <summary>
		/// Compares two objects for equality, handling JsonElement specially.
		/// If both objects are JsonElement, it performs a deep comparison.
		/// If one is JsonElement and the other is not, it attempts to compare them based on their serialized JSON string representation.
		/// Otherwise, it falls back to standard equality comparison.
		/// </summary>
		/// <param name="obj1">The first object to compare.</param>
		/// <param name="obj2">The second object to compare.</param>
		/// <returns>True if the objects are considered equal, false otherwise.</returns>
		private bool CompareEntryValues(object obj1, object obj2)
		{
			// Special handling when both objects are JsonElement.
			if (obj1 is System.Text.Json.JsonElement jsonElement1 && obj2 is System.Text.Json.JsonElement jsonElement2)
			{
				// To avoid overloading the server, we will opt for a simplified comparison.
				return string.Equals(jsonElement1.GetRawText(), jsonElement2.GetRawText(), StringComparison.Ordinal);
			}
			// If only one is JsonElement, serialize the other to JSON for comparison.
			// This ensures a fair comparison by comparing their JSON string representations.
			else if (obj1 is System.Text.Json.JsonElement || obj2 is System.Text.Json.JsonElement)
			{
				var json1 = obj1 is System.Text.Json.JsonElement elem1 ? elem1.GetRawText() : System.Text.Json.JsonSerializer.Serialize(obj1);
				var json2 = obj2 is System.Text.Json.JsonElement elem2 ? elem2.GetRawText() : System.Text.Json.JsonSerializer.Serialize(obj2);
				return string.Equals(json1, json2, StringComparison.Ordinal);
			}
			// Fallback to standard object equality comparison.
			return Equals(obj1, obj2);
		}

		/// <summary>
		/// Gets the diff between Original history and Current history
		/// </summary>
		/// <returns></returns>
		private List<object> getHistoryDiff()
		{
			var result = new ConcurrentBag<object>();

			if (OriginalHistory != null)
			{
				OriginalHistory.AsParallel().ForAll(originalHistory =>
				{
					var hInfo = new
					{
						uId = originalHistory.uniqueIdentifier,
						// keys for Remove
						remove = new List<object>(),
						// keys for Set
						set = new Dictionary<string, object>(),
					};

					var currentHistory = History.FirstOrDefault(h => h.uniqueIdentifier == originalHistory.uniqueIdentifier);

					if (currentHistory != null)
					{
						// Removed keys
						hInfo.remove.AddRange(originalHistory.EntriesKeys.Except(currentHistory.EntriesKeys));

						foreach (var key in currentHistory.EntriesKeys)
						{
							var currentEntryValue = currentHistory.GetEntry(key);
							var originalEntryValue = originalHistory.GetEntry(key);
							// Added and updated keys
							if (!originalHistory.CheckEntry(key) || !CompareEntryValues(currentEntryValue, originalEntryValue))
								hInfo.set.Add(key, currentEntryValue);
						}
					}
					// The level no more exist
					else
					{
						// Removed keys
						hInfo.remove.AddRange(originalHistory.EntriesKeys);
					}

					if (hInfo.remove.Any() || hInfo.set.Any())
						result.Add(hInfo);
				});
			}
			else
			{
				History.AsParallel().ForAll(currentHistory =>
				{
					var hInfo = new
					{
						uId = currentHistory.uniqueIdentifier,
						// keys for Set
						set = currentHistory.GetEntries(),
					};

					if (hInfo.set.Any())
						result.Add(hInfo);
				});
			}

			return result.ToList();
		}

		/// <summary>
		/// Create list of history entries to be update on the client-side (Vue.js)
		/// </summary>
		/// <returns></returns>
		public ClientSideHistoryResult GetHistoryToUpdateClientSide()
		{
			return new ClientSideHistoryResult()
			{
				HistoryDiff = getHistoryDiff(),
				NavigationId = this.NavigationId
			};
		}

		//create by [TMV] (2020.09.23)
		public struct EphMenus
		{
			public string Action { set; get; }

			public string controller { set; get; }

			public string Title { set; get; }

			public string MenuID { set; get; }

			public string Font { set; get; }

			public string Image { set; get; }
		}

		/// <summary>
		/// Get the currents eph forms for the currente level and module for the avatar menu
		/// </summary>
		/// <returns></returns>
		public List<EphMenus> GetAvtarEphMenus()
		{
			List<EphMenus> menuReturn = new List<EphMenus>();

			try
			{
				CSGenio.framework.User user = m_userContext.User;

				List<string> forms = CSGenio.framework.EPH.GetEphCurrentForm(user);
				string modulo = user.CurrentModule;

				foreach (string form in forms.Distinct())
				{
					try
					{
						string id = form;

						Helpers.Menus.MenuEntry menu = new Helpers.Menus.MenuEntry();

						//search the root menu for the dbedit
						while (!string.IsNullOrEmpty(id))
						{
							menu = GenioMVC.Helpers.Menus.Menus.FindMenu(modulo, id);

							if (String.IsNullOrEmpty(menu.Controller))
								break;

							id = menu.ParentId;
						}

						EphMenus menuToAdd = new EphMenus
						{
							Title = menu.Title,
							Action = menu.Children.FirstOrDefault().Action_MVC,
							controller = menu.Children.FirstOrDefault().Controller,
							MenuID = menu.ID,
							Font = menu.Font,
							Image = menu.Image
						};

						menuReturn.Add(menuToAdd);
					}
					catch (System.Exception e)
					{
						CSGenio.framework.Log.Error("Erro creating eph avatar menu for the menu " + forms + e.Message);
					}
				}
			}
			catch (System.Exception e)
			{
				CSGenio.framework.Log.Error("unexpected Erro creating eph avatar menus" + e.Message);
			}

			return menuReturn;
		}

		#region Session State Serialization

		protected NavigationContext(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException("info");

			NavigationContext navigationContext;

			try
			{
				var _value = info.GetString("NavigationContext");
				var navigationContextBase = Helpers.NavigationSerializer.Deserialize<NavigationContextBase>(_value);
				navigationContext = new NavigationContext(m_userContext, navigationContextBase);
			}
			catch (System.Exception e)
			{
				navigationContext = new NavigationContext(m_userContext);
				CSGenio.framework.Log.Error(string.Format("Error on deserialize Navigation context. {0};{1}", e.Message, e.InnerException?.Message));
			}

			this.NavigationId = navigationContext?.NavigationId;
			this.Timeout = navigationContext?.Timeout ?? DateTime.Now;
			this.GoBack = navigationContext?.GoBack;
			this.OverrideSkipIfJustOne = navigationContext?.OverrideSkipIfJustOne;
			this.History = navigationContext?.History;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			try
			{
				// If using the NavigationContext directly, this serialization method will cause a loop.
				var _value = Helpers.NavigationSerializer.Serialize(new NavigationContextBase(this));
				info.AddValue("NavigationContext", _value);
			}
			catch (System.Exception e)
			{
				info.AddValue("NavigationContext", null);
				CSGenio.framework.Log.Error(string.Format("Error on serialize Navigation context. {0};{1}", e.Message, e.InnerException?.Message));
			}
		}

		#endregion
	}
}

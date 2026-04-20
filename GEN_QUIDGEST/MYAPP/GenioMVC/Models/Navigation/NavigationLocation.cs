using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace GenioMVC.Models.Navigation
{
    public struct NavigationLocation
	{
		public static NavigationLocation Empty = new NavigationLocation();
		public static NavigationLocation Any = new NavigationLocation(null, "index", "home")
		{
			m_any = true
		};

		private bool m_any;

		private string m_name;

		private string m_title;

		public string Name
		{
			get
			{
				if (!string.IsNullOrEmpty(m_title))
					return m_title;
				if (string.IsNullOrEmpty(m_name))
					return string.Empty;
				return Resources.Resources.ResourceManager.GetString(m_name, Resources.Resources.Culture);
			}
		}

		private string m_action;

		[JsonInclude]
		public string Action
		{
			get { return m_action; }
			private set { m_action = value; }
		}

		private string m_controller;

        [JsonInclude]
        public string Controller
		{
			get { return m_controller; }
			private set { m_controller = value; }
		}

		[IgnoreDataMember, NonSerialized]
		private RouteValueDictionary m_routedValues;

        [JsonInclude]
		[JsonConverter(typeof(VariantToObjectDictionaryConverter))]
        public RouteValueDictionary RoutedValues
		{
			get { return m_routedValues; }
			private set { m_routedValues = value; }
		}

		// Vue.js
		public string mode { get; set; }
		public string vueRouteName { get; set; }

		public NavigationLocation(string name, string action, string controller)
		{
			m_any = false;
			m_name = name;
			m_action = action;
			m_controller = controller;
			m_routedValues = new RouteValueDictionary();
			m_title = null;

			mode = null;
			vueRouteName = null;
		}

		public NavigationLocation(string name, string action, string controller, object routedValues)
			: this(name, action, controller)
		{
			m_routedValues = new RouteValueDictionary(routedValues);
		}

		public NavigationLocation SetRoutedValues(object values)
		{
			NavigationLocation result = this; // clone
			result.m_routedValues = new RouteValueDictionary(values);

			return result;
		}

		public RouteValueDictionary getRoutes(object values)
		{
			var newRoutes = m_routedValues == null ? new RouteValueDictionary() : new RouteValueDictionary(m_routedValues);

			if (values != null)
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(values);
				foreach (PropertyDescriptor item in properties)
					newRoutes.Add(item.Name, item.GetValue(values));
			}

			return newRoutes;
		}

		public NavigationLocation Normalize()
		{
			if (this.RoutedValues == null)
				return this;

			return new NavigationLocation(this.Name, this.Action, this.Controller);
		}

		public bool IsSameAction(NavigationLocation other)
		{
			return this == other;
		}

		public override string ToString()
		{
			if (this == NavigationLocation.Empty)
				return "Empty";
			else if (this == NavigationLocation.Any)
				return "Any";

			return this.Name + "(" + this.Action + "," + this.Controller + ")";
		}

		public override bool Equals(object? obj)
		{
			if (obj == null || !(obj is NavigationLocation))
				return false;

			NavigationLocation other = (NavigationLocation)obj;

			if (m_any)
				return other.m_any;

			string? thisId = this.RoutedValues != null ? this.RoutedValues["Id"]?.ToString() : null;
			string? otherId = other.RoutedValues != null ? other.RoutedValues["Id"]?.ToString() : null;

			return this.vueRouteName == other.vueRouteName && thisId == otherId;
		}

		public override int GetHashCode()
		{
			if (m_any)
				return m_any.GetHashCode();

			return (Name == null ? 0 : Name.GetHashCode())
				^ (Action == null ? 0 : Action.GetHashCode())
				^ (Controller == null ? 0 : Controller.GetHashCode())
				^ (RoutedValues == null ? 0 : RoutedValues.GetHashCode());
		}

		public static bool operator ==(NavigationLocation m1, NavigationLocation m2)
		{
			return NavigationLocation.Equals(m1, m2);
		}

		public static bool operator !=(NavigationLocation m1, NavigationLocation m2)
		{
			return !NavigationLocation.Equals(m1, m2);
		}

		/// <summary>
		/// MH (06-11-2015) - To que for possivel usar fields das tables nos titulos,
		/// apareceu necesidade de change também o string do menu de navegação.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public NavigationLocation redefineName(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				NavigationLocation result = this; // clone
				result.m_title = name;
				return result;
			}

			return this;
		}

		internal string ShortDescription()
		{
			string id = "", mode = "";
			if (m_routedValues != null)
			{
				if (m_routedValues.ContainsKey("id") && m_routedValues["id"] != null)
					id = " id:" + m_routedValues["id"].ToString() + ";";
				if (m_routedValues.ContainsKey("mode"))
					mode = m_routedValues["mode"].ToString();
			}

			return string.Format("route: {0}; mode: {1};{2}", vueRouteName, mode, id);
		}
	}
}

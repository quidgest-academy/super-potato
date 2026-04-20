using System.Linq;
using System;
using System.Reflection;
using System.Collections.Generic;
using CSGenio.framework;

namespace CSGenio.business
{
	/// <summary>
	/// Dynamic array thats returns all the modules of the system
	/// </summary>
	/// <seealso cref="CSGenio.business.Array&lt;System.String&gt;" />
	public class ModulesDynamicArray : Array<string>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ModulesDynamicArray"/> class.
		/// </summary>
		protected override Dictionary<string, ArrayElement> LoadDictionary()
		{
			return ClientApplication.Applications
				.SelectMany(m => m.Modules)
				.GroupBy(x => x.Key, (key, group) => group.First())
				.ToDictionary(m => m.Key, m => new ArrayElement() { ResourceId = m.Value });
		}
	}

	/// <summary>
	/// Dynamic array thats returns the list of modules of the specified application
	/// </summary>
	/// <seealso cref="CSGenio.business.Array&lt;System.String&gt;" />
	public class ApplicationModulesDynamicArray : Array<string>
	{
		/// <summary>
		/// The application identifier
		/// </summary>
		private readonly string _appId;

		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationModulesDynamicArray"/> class.
		/// </summary>
		/// <param name="appid">The appid.</param>
		public ApplicationModulesDynamicArray(string appId)
		{
			_appId = appId;
		}

		/// <summary>
		/// Loads the dictionary.
		/// </summary>
		/// <param name="lang">The language.</param>
		/// <returns></returns>
		protected override Dictionary<string, ArrayElement> LoadDictionary()
		{
			ClientApplication appcl = ClientApplication.Applications
				.FirstOrDefault(app => app.Id == _appId);

			if (appcl != null)
				return appcl.Modules
					.ToDictionary(m => m.Key, m => new ArrayElement() { ResourceId = m.Value });
			
			return new Dictionary<string, ArrayElement>();
		}
	}

	/// <summary>
	/// Dynamic array thats returns all the roles of the system
	/// </summary>
	/// <seealso cref="CSGenio.business.Array&lt;System.String&gt;" />
	public class RolesDynamicArray : Array<string>
	{
		/// <summary>
		/// Loads the dictionary.
		/// </summary>
		/// <param name="lang">The language.</param>
		/// <returns></returns>
		protected override Dictionary<string, ArrayElement> LoadDictionary()
		{
            return Role.ALL_ROLES
				.ToDictionary(r => r.Key, r => new ArrayElement() { ResourceId = r.Value.Title, Group = GetGroups(r.Value) });
		}

		/// <summary>
		/// Get roles modules
		/// </summary>
		/// <param name="role">The role</param>
		/// <returns></returns>
		string GetGroups(Role role)
		{
			return string.Join("|", role.AvailableModules);
		}
	}

	/// <summary>
	/// Dynamic array thats returns all the roles
	/// above the specified role id
	/// </summary>
	/// <seealso cref="CSGenio.business.DynamicArray&lt;System.String&gt;" />
	public class RolesAboveDynamicArray : Array<string>
	{
		/// <summary>
		/// The role identifier
		/// </summary>
		private readonly string _roleId;

		/// <summary>
		/// Initializes a new instance of the <see cref="RolesAboveDynamicArray"/> class.
		/// </summary>
		/// <param name="roleId">The role identifier.</param>
		public RolesAboveDynamicArray(string roleId)
		{
			_roleId = roleId;
		}

		/// <summary>
		/// Loads the dictionary.
		/// </summary>
		/// <param name="lang">The language.</param>
		/// <returns></returns>
		protected override Dictionary<string, ArrayElement> LoadDictionary()
		{
			Role role = Role.GetRole(_roleId);

			if (role != null)
				return role.AllRolesAbove()
					.ToDictionary(r => r.Id, r => new ArrayElement() { ResourceId = r.Title });

			return new Dictionary<string, ArrayElement>();
		}
	}

	/// <summary>
	/// Dynamic array thats returns all the roles
	/// below the specified role id
	/// </summary>
	/// <seealso cref="CSGenio.business.DynamicArray&lt;System.String&gt;" />
	public class RolesBelowDynamicArray : Array<string>
	{
		/// <summary>
		/// The role identifier
		/// </summary>
		private readonly string _roleId;

		/// <summary>
		/// Initializes a new instance of the <see cref="RolesBelowDynamicArray"/> class.
		/// </summary>
		/// <param name="roleId">The role identifier.</param>
		public RolesBelowDynamicArray(string roleId)
		{
			_roleId = roleId;
		}

		/// <summary>
		/// Loads the dictionary.
		/// </summary>
		/// <param name="lang">The language.</param>
		/// <returns></returns>
		protected override Dictionary<string, ArrayElement> LoadDictionary()
		{
			Role role = Role.GetRole(_roleId);

			if (role != null)
				return role.AllRolesBelow()
					.ToDictionary(r => r.Id, r => new ArrayElement() { ResourceId = r.Title });

			return new Dictionary<string, ArrayElement>();
		}
	}

	/// <summary>
	/// Dynamic array thats returns all the combinations
	/// of roles and modules, allowing to filter roles by module.
	/// </summary>
	public class RolesPerModuleDynamicArray : Array<string>
	{
		/// <summary>
		/// Loads the dictionary.
		/// </summary>
		protected override Dictionary<string, ArrayElement> LoadDictionary()
		{
			Dictionary<string, ArrayElement> result = new Dictionary<string, ArrayElement>();

			foreach (var moduleRole in Role.MODULE_ROLES)
			{
				string module = moduleRole.Item1;
				Role role = moduleRole.Item2;

				if (result.ContainsKey(role.Id))
				{
					ArrayElement el = result[role.Id];
					el.Group = string.Join("|", el.Group, module);
				}
				else
				{
					result.Add(role.Id, new ArrayElement() { ResourceId = role.Title, Group = module });
				}
			}

			return result;
		}
	}

	/// <summary>
	/// Dynamic array with all scheduled process types in the system
	/// </summary>
	public class ProcessTypeDynamicArray : Array<string>
    {
        /// <summary>
        /// Gets the process types from classes with attribute GenioProcessType defined
		/// This is temporary, all this reflection should be removed when the process types become a table
        /// </summary>
        protected override Dictionary<string, ArrayElement> LoadDictionary()
        {
            Dictionary<string, ArrayElement> result = new Dictionary<string, ArrayElement>();		

			//Right now all the process types are in classes defined in the GenioServer. This is a temporary solution, this should be generated.
			var assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "GenioServer");
			//Get all types with GenioProcesstype attribute
			var types = assembly.GetTypes()
				.Where(t => t.IsDefined(typeof(async.GenioProcessType), false));
			foreach (var type in types)
			{
				//Get the id and resource from the attribute
				var attribute = type.GetCustomAttribute(typeof(async.GenioProcessType)) as async.GenioProcessType;
				var element = new ArrayElement() { Key = attribute.Id, ResourceId = attribute.Resource };
				result.Add(attribute.Id, element);
			}
            return result;
        }
    }
}

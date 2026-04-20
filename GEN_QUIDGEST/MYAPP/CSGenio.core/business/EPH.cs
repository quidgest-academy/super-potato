using System;
using System.Collections;
using System.Collections.Generic;

namespace CSGenio.framework
{
	/// <summary>
	/// Class with the Permanent History Entries of the Program
	/// </summary>
	public abstract class EPH
	{
		protected string moduleName; // Name of the module defined in child classes
		private readonly static Dictionary<string, EPH> todosEphs;
        private readonly static Dictionary<string, EPHCondition> allConditions;

		/// <summary>
		/// Class constructor
		/// Inserts all module with a PHE in the hashtable
		/// </summary>
		static EPH()
		{
            allConditions = new(){
            };

            todosEphs = new() {
            };
		}

		/// <summary>
		/// The name of the module
		/// </summary>
		public string ModuleName
		{
			get { return moduleName; }
		}

		/// <summary>
		/// The names of the levels subjected to a PHE
		/// </summary>
		public abstract string[] Levels
		{
			get;
		}

        /// <summary>
        /// Gets the definition of a PHE condition by it's id
        /// </summary>
        public static EPHCondition GetEphConditionById(string id)
        {
            if (allConditions.TryGetValue(id, out var result))
                return result;
            return null;
        }

		/// <summary>
		/// The PHEs per module, it's set in the child classes
		/// </summary>
		public abstract Dictionary<string, List<EPHCondition>> EphsPerModule
		{
			get;
		}

		/// <summary>
		/// Gets the PHE corresponding to the specified module
		/// </summary>
		/// <param name="nomeModulo">The module name</param>
		/// <returns>The PHE corresponding to the module with the specified name</returns>
		public static EPH getEPH(string moduleName)
		{
			if (todosEphs == null)
				return null;
            if(todosEphs.TryGetValue(moduleName, out var result))
                return result;
            return null;
		}

        /* JMT and TR 2009 09 27
         *
         * The following PHE verification functions were created for the purpose of controlling permissions
         * changing, deleting, inserting and duplicating records in an area.
         *
         * It is not the purpose of these functions to control the display of records, as is normally expected from PHEs.
         *
         * They were created to solve a problem with access levels where users can view all records
         * but only records covered by your PHE can change.
         *
         * This is achieved in conjunction with the REMEPH mancs tag
         * and overrides the introduce, change, duplicate and eliminate functions of areas
         */

        /// <summary>
        /// Supported types of PHE combinations
        /// </summary>
        public enum EPHOperation { And, Or };

        /// <summary>
        /// Checks whether the PHEs are satisfied in two variants of choice. AND or OR.
        /// Applies to all PHEs active for this user in the context of the area.
        /// Only PHEs where the internal codes appear explicitly in the form are covered, i.e.
        /// it is necessary to add a Qfield from the PHE area to the form
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="area">Area</param>
        /// <param name="op">Type of operation</param>
        /// <returns>True if and only if PHEs are satisfied according to the type of operation, false otherwise</returns>
        public static bool verifyEPHs(User user, CSGenio.business.Area area, EPHOperation op)
        {
            Dictionary<string, bool> Qresult = evaluateEPHS(user, area);
            if (Qresult == null || Qresult.Count == 0)
                return true;

            bool sat = false;

            if (op == EPHOperation.And)
            {
                sat = true;
                foreach (bool it in Qresult.Values)
                    if (!it)
                        return false;
            }
            else if (op == EPHOperation.Or)
            {
                foreach (bool it in Qresult.Values)
                    if (it)
                        return true;
            }
            return sat;
        }

        /// <summary>
        /// Combines the results of the evaluated PHEs
        /// </summary>
        /// <param name="res">Dictionary with the result of each PHE</param>
        /// <returns></returns>
        public delegate bool combinesResultsEPHDelegate(Dictionary<string, bool> res);

        /// <summary>
        /// Combines the satisfactions of PHEs according to an external function
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="area">Area</param>
        /// <param name="cre">Function to combine the PHEs</param>
        /// <returns>True if and only if the PHEs satisfy the external function, false otherwise</returns>
        public static bool verifyEPHs(User user, CSGenio.business.Area area, combinesResultsEPHDelegate cre)
        {
            Dictionary<string, bool> Qresult = evaluateEPHS(user, area);
            if (Qresult == null || Qresult.Count == 0)
                return true;

            return cre(Qresult);
        }

        /// <summary>
        /// Evaluates the user's active PHEs in the context of the given area
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="area">Area</param>
        /// <returns>A dictionary with the result of the evaluation of each PHE</returns>
        public static Dictionary<string, bool> evaluateEPHS(User user, CSGenio.business.Area area)
        {
            // Users without PHEs pass the verification
            if (!user.hasEph(area.Alias))
                return null;
            Hashtable ephsUtilizador = user.Ephs;
            Dictionary<string, bool> Qresult = new Dictionary<string, bool>();

            IDictionaryEnumerator en = ephsUtilizador.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Key.ToString().Contains(user.CurrentModule))
                {
                    string[] modulo_eph = en.Key.ToString().Split('_');
                    string eph = modulo_eph[1];
                    string module = modulo_eph[0];
                    string campoRelDestino;
                    string relSourceField;

                    try
                    {
                        campoRelDestino = area.ParentTables[eph.ToLower()].TargetRelField;
                        relSourceField = area.ParentTables[eph.ToLower()].SourceRelField;
                    }
                    catch (KeyNotFoundException)
                    {
                        continue;
                    }

                    if (user.hasEph(eph) && area.Fields.ContainsKey(area.Alias + "." + campoRelDestino))
                    {
                        string valorEPH = user.EphValues[user.CurrentModule + "_" + eph][eph.ToLower() + "." + campoRelDestino];
                        string Qvalue = area.returnValueField(area.Alias + "." + relSourceField) as string;

                        Qresult.Add(user.CurrentModule + "_" + eph, valorEPH.CompareTo(Qvalue) == 0);
                    }
                }
            }
            return Qresult;
        }

        /// <summary>
        /// The menus not subject to a PHE
        /// </summary>
        public abstract Dictionary<string, List<string>> MenusNotSubjectEPH
        {
            get;
        }

		/// <summary>
        /// Checks whether in this module the identifier is subject to the PHE of the area
        /// </summary>
        /// <param name="identificador">Control identifier</param>
        /// <param name="areaeph">Area of the PHE</param>
        /// <returns>True if and only if the identifier is subject to a PHE, false otherwise</returns>
        public abstract bool HasIdentifierSubjectEPH(string identifier, string areaeph);

        // Create by [TMV] (2020.09.24)
        /// <summary>
        /// Searches for the PHE names that corresponds to the module, roles and formID
        /// </summary>
        /// <param name="module">Module name</param>
        /// <param name="roles">A list of roles</param>
        /// <param name="formID">The id of the form</param>
        /// <returns>A list with the PHE names that correspond to the specified parameters</returns>
        public static List<EPHCondition> GetEPHForms(string module, List<Role> roles, string formID)
        {
            List<EPHCondition> ephName = [];

            // Gets the PHE for the module
            EPH eph = EPH.getEPH(module);
            if (eph is null)
                return ephName;

            foreach (Role role in roles)
                if (eph.EphsPerModule.TryGetValue(role.ToString(), out var condicoesEPH))
                    foreach (EPHCondition condition in condicoesEPH)
                        // Checks if the ids match and the list does't contain the id
                        if (condition.IntialForm == formID && !ephName.Exists(x => x.EPHName == condition.EPHName))
                            ephName.Add(condition);

            return ephName;
        }

        /// <summary>
        /// Gets the form for the current module and roles
        /// </summary>
        /// <param name="user">Current user</param>
        /// <returns></returns>
        public static List<string> GetEphCurrentForm(User user)
        {
            string modulo = user.CurrentModule;
            List<string> forms = new List<string>();

            EPH eph = EPH.getEPH(modulo);
            if (eph is null)
                return forms;

            foreach (Role role in user.GetModuleRoles(user.CurrentModule))
                if(eph.EphsPerModule.TryGetValue(role.ToString(), out var condicoesEPH))
                    foreach (EPHCondition cond in condicoesEPH)
                        if (!string.IsNullOrEmpty(cond.IntialForm) && !string.IsNullOrWhiteSpace(cond.IntialForm) && !forms.Contains(cond.IntialForm))
                            forms.Add(cond.IntialForm);

            return forms;
        }
    }

    /// <summary>
    /// Auxiliary class to group together a PHE, its restricting values
    /// and relationships from current area to the areas of PHE fields.
    /// This class is used to restrict queries through a CriteriaSet.
    /// </summary>
    public class EPHOfArea
    {
        public EPHOfArea(EPHField eph, string[] valuesList)
        {
            Eph = eph;
            ValuesList = valuesList;
            Relation = null;
            Relation2 = null;
        }

        /// <summary>
        /// The PHE field associated with current area
        /// </summary>
        public EPHField Eph { get; set; }

        /// <summary>
        /// Values being applied to the PHE field to restrict the query
        /// </summary>
        public string[] ValuesList { get; set; }

        /// <summary>
        /// Relation between current area and the area of PHEs first field.
        /// The PHE field can be from a parent area, but it can also
        /// be from an area farther up if the PHE is being propagated.
        /// </summary>
        public Relation Relation { get; set; }

		/// <summary>
        /// Relation between current area and the area of PHEs second field.
        /// The PHE field can be from a parent area, but it can also
        /// be from an area farther up if the PHE is being propagated.
        /// </summary>
        public Relation Relation2 { get; set; }
    }

    /// <summary>
    /// The auxiliary class to group the necessary information about the
    /// Initial PHE values to place and retrieve it from the Cache.
    /// </summary>
    [Serializable]
    public class InitialEPHCache
    {
        /// <summary>
        /// The module to which the values belong
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// A dictionary with the values for each PHE
        /// </summary>
        public Dictionary<string, string[]> EPHValues { get; set; }

        /// <summary>
        /// The class with Initial EPH values information to place and retrieve it from the Cache
        /// </summary>
        public InitialEPHCache()
        {
            EPHValues = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Merges cached values with current ones
        /// </summary>
        /// <param name="cache">The values of the PHEs of the same module that can be in the cache</param>
        public void MergeCache(InitialEPHCache cache)
        {
            if (cache?.EPHValues != null)
            {
                foreach (var eph in cache.EPHValues)
                {
                    if (!this.EPHValues.ContainsKey(eph.Key))
                        this.EPHValues.Add(eph.Key, eph.Value);
                }
            }
        }
    }
}

using System;

namespace CSGenio.framework
{
	/// <summary>
	/// Represents the permissions level of an object
	/// </summary>
	public class QLevel
	{

		/// <summary>
		/// Constructor
		/// </summary>
		public QLevel(){}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="query">Level to query this object</param>
        /// <param name="create">Level to create this object</param>
        /// <param name="alterAlways">Level to edit this object</param>
        /// <param name="removeAlways">Level to delete this object</param>
        public QLevel(Role query,Role create,Role alterAlways,Role removeAlways)
		{
            Query = query;
            Create = create;
            AlterAlways = alterAlways;
            RemoveAlways = removeAlways;
        }

        /// <summary>
        /// Checks if a role has permissions to query table
        /// </summary>
        /// <param name="accessLevel">User access permissions</param>
        /// <returns>True if the user has permission for the operation in this object</returns>
        [Obsolete("Use HasLevel method instead")]
		public bool canConsult(LevelAccess accessLevel)
		{
           return Query.HasLevel(accessLevel);
		}

        /// <summary>
        /// Checks if a role has permissions to query table
        /// </summary>
        /// <param name="role"></param>
        /// <returns>True if the user has permission for the operation in this object</returns>
        public bool CanConsult(Role role)
        {
            return Query.HasRole(role);
        }


        /// <summary>
        /// Checks if a role has permissions to create a record
        /// </summary>
        /// <param name="accessLevel">User access permissions</param>
        /// <returns>True if the user has permission for the operation in this object</returns>
        [Obsolete("Use HasRole method instead")]
        public bool canCreate(string utStringLevel)
        {
            return Create.HasRole(utStringLevel);
        }

        /// <summary>
        /// Checks if a role has permissions to create a record
        /// </summary>
        /// <param name="role"></param>
        /// <returns>True if the user has permission for the operation in this object</returns>
        public bool CanCreate(Role role)
        {
            return Create.HasRole(role);
        }

        /// <summary>
        /// Checks if a given role can change a table.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="date"></param>
        /// <returns>True if the user has permission for the operation in this object</returns>
        public bool CanChange(Role role, DateTime date)
		{
            return AlterAlways.HasRole(role);
		}

        public bool CanChange(Role role)
        {
            return CanChange(role, DateTime.MinValue);
        }

        /// <summary>
        /// Checks if a given role can change a table.
        /// </summary>
        /// <param name="accessLevel">User access permissions</param>
        /// <param name="date">Date of the record</param>
        /// <returns>True if the user has permission for the operation in this object</returns>
        [Obsolete("Use CanChange method instead")]
		public bool canChange(LevelAccess accessLevel,DateTime date)
		{
            return CanChange(Role.GetRole(accessLevel), date);
		}

        /// <summary>
        /// Checks if a given role can change a table.
        /// </summary>
        /// <param name="accessLevel">User access permissions</param>
        /// <returns>True if the user has permission for the operation in this object</returns>
        [Obsolete("Use CanChange method instead")]
        public bool canChange(LevelAccess accessLevel)
        {
			//RMS (05-03-2012) Correcção to o caso em que não está definida a data, apenas o parâmetro "Sempre"
			return canChange(accessLevel,DateTime.MinValue);
        }

        /// <summary>
        /// Checks if a role has permissions to delete a record in the table
        /// </summary>
        /// <returns>True if the user has permission for the operation in this object</returns>
        public bool CanDelete(Role role, DateTime date)
		{
            ///First check if you can remove always
            return RemoveAlways.HasRole(role);
		}

        public bool CanDelete(Role role)
        {
            return CanDelete(role, DateTime.MinValue);
        }

        /// <summary>
        /// Checks if a role has permissions to delete a record in the table
        /// </summary>
        /// <param name="accessLevel">User access permissions</param>
        /// <param name="date">Date of the record</param>
        /// <returns>True if the user has permission for the operation in this object</returns>
        [Obsolete("Use CanDelete method instead")]
        public bool canCancel(LevelAccess accessLevel,DateTime date)
		{
            return CanDelete(Role.GetRole(accessLevel), date);
		}

        /// <summary>
        /// Checks if a role has permissions to delete a record in the table
        /// </summary>
        /// <param name="accessLevel">User access permissions</param>
        /// <returns>True if the user has permission for the operation in this object</returns>
        [Obsolete("Use CanDelete method instead")]
        public bool canCancel(LevelAccess accessLevel)
        {
			//RMS (05-03-2012) Correcção to o caso em que não está definida a data, apenas o parâmetro "Sempre"
			return canCancel(accessLevel, DateTime.MinValue);
        }


        /// <summary>
        /// Level to edit this object
        /// </summary>
        public Role AlterAlways { get; set; }

        /// <summary>
        /// Level to delete this object
        /// </summary>
        public Role RemoveAlways { get; set; }


        /// <summary>
        /// Level to query this object
        /// </summary>
        public Role Query { get; set; }

        /// <summary>
        /// Level to create this object
        /// </summary>
        public Role Create { get; set; }
	}
}

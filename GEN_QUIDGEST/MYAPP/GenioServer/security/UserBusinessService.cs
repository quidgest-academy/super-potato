using System;
using System.Collections;
using System.Collections.Generic;
using CSGenio.framework;
using CSGenio.business;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using CSGenio;

namespace GenioServer.security
{    
    /// <summary>
    /// Provider for creating users from a registration from and associating them with roles and EPH's
    /// </summary>
    public abstract class BaseUserRegistration
    {
        /// <summary>
        /// Full user registration process
        /// </summary>
        /// <param name="psw">User login information</param>
        /// <param name="area">The business information to associate with this user</param>
        /// <param name="secret">Primary credentials to use for user authentication</param>
        /// <returns>The created user</returns>
        public virtual User Register(CSGenioApsw psw, IArea area, CredentialSecret secret = null)
        {
            var sp = ResolvePersistentSupport(psw, area);

            //prevalidate business record, to prevent having to rollback external user creation
            if (area is DbArea dbArea)
            {
                sp.openConnection();

                dbArea.fillValuesDefault(sp, FunctionType.INS);
                FormulaDbContext fdc = new FormulaDbContext(dbArea);
                fdc.AddInternalOperations();
                dbArea.fillInternalOperations(sp, dbArea, fdc);


                StatusMessage status = Validation.validateFieldsChange(dbArea, sp, area.User);
                if (status.Status == Status.E)
                    throw new FieldValidationException(status, "BaseUserRegistration.Register");

                sp.closeConnection();
            }
            else
            {
                throw new ArgumentException($"Expected area to be of type DbArea, but got {area.GetType().Name}.", nameof(area));
            }

            User newUser = CreateUser(psw, secret);
            try
            {
                sp.openTransaction();
                CreateEph(newUser, area, sp);
                sp.closeTransaction();
            }
            catch (Exception)
            {
                sp.rollbackTransaction();
                throw;
            }

            return newUser;
        }

        /// <summary>
        /// Creates a user in the user directory
        /// </summary>
        /// <param name="psw">User login information</param>
        /// <param name="secret">Primary credentials to use for user authentication</param>
        /// <returns>The created user</returns>
        public virtual User CreateUser(CSGenioApsw psw, CredentialSecret secret = null)
        {
            //Setup the user to be created with its associated roles
            User user = new User(psw.ValNome, "", Configuration.DefaultYear);
            user.Years.Add(Configuration.DefaultYear);
            user.CurrentModule = "Public";
            user.Status = 2;
            CreateRoles(user);

            //Any extra information that was collected is passed to the provider as claims
            //The provider can decide to persist the values, modify them, map them, ignore them, etc.
            //user will have user.Codpsw filled by the provider
            Dictionary<string, object> claims = new Dictionary<string, object>();
            foreach (var field in psw.Fields.Values)
                claims.Add(field.Name, field.Value);
            SecurityFactory.CreateNewUser(user, claims, secret);

            return user;
        }

        /// <summary>
        /// Creates the default user permissions for this user
        /// </summary>
        /// <param name="user">The user</param>
        public virtual void CreateRoles(User user) { }

        /// <summary>
        /// Determines the database connection to use in the registration act
        /// </summary>
        /// <param name="psw">User login information</param>
        /// <param name="area">The business information to associate with this user</param>
        /// <returns>A persistent support</returns>
        public virtual PersistentSupport ResolvePersistentSupport(CSGenioApsw psw, IArea area)
			=> PersistentSupport.getPersistentSupport(Configuration.DefaultYear);

        /// <summary>
        /// Associates the user to the business information
        /// </summary>
        /// <param name="newUser">The newly created user</param>
        /// <param name="area">The business information to associate with this user</param>
        /// <param name="sp">Persistent support</param>
        public virtual void CreateEph(User newUser, IArea area, PersistentSupport sp) { }
    }
}
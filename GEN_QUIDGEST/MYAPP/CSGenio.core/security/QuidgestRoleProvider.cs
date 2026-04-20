using CSGenio;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GenioServer.security;

/// <summary>
/// Role provider for Quidgest psw table user management
/// </summary>
[Description("Retrieves the roles of a given user from the application database.")]
[DisplayName("Application Database roles")]
public class QuidgestRoleProvider : BaseRoleProvider
{
    /// <summary>
    /// Optionally set a aux database to fetch the password from
    /// </summary>
    [SecurityProviderOption(optional: true)]
    [Description("Optionally set a aux database to fetch the password from")]
    public string AuxDb { get; set; }

    /// <inheritdoc/>
    public QuidgestRoleProvider(RoleProviderCfgEl config) : base(config)
    {
    }

    /// <summary>
    /// Internal "administrator" user for database operations
    /// </summary>
    private static readonly User _loginuser = CreateAdmin();
    
    private static User CreateAdmin() {
        var admin = new User("login", "", Configuration.DefaultYear);
        admin.AddModuleRole("Public", Role.ADMINISTRATION);
        admin.CurrentModule = "Public";
        return admin;
    }

    private CSGenioApsw GetUserFromDb(GenioIdentity identity, PersistentSupport sp)
    {
        var field = identity.IdProperty switch
        {
            GenioIdentityType.InternalId => CSGenioApsw.FldNome,
            GenioIdentityType.ExternalId => CSGenioApsw.FldUserid,
            GenioIdentityType.Email => CSGenioApsw.FldEmail,
            _ => throw new NotImplementedException()
        };

        var list = CSGenioApsw.searchList(sp, _loginuser, CriteriaSet.And().Equal(field, identity.Name));
        if (list.Count == 0)
            return null; //throw auth exception
        else
            return list[0];
    }

    private static void ReadRoles(User user, CSGenioApsw psw, PersistentSupport sp)
    {
        //no user, no roles
        if (psw == null)
            return;

        //fetch the user authorization rows
        var uaList = CSGenioAuserauthorization.searchList(sp, _loginuser, CriteriaSet.And()
            .Equal(CSGenioAuserauthorization.FldCodpsw, psw.QPrimaryKey)
            .Equal(CSGenioAuserauthorization.FldSistema, Configuration.Program));

        //setup the user year dependent data
        user.Codpsw = psw.ValCodpsw; //internal uuid
        user.Name = psw.ValNome; //internal id

        //deactivated users are not authorized anywhere
        user.Status = (int)psw.ValStatus;
        if (user.Status == 2)
            return;

        //setup 2fa control variables
        string tpPsw2FA = psw.ValPsw2fatp;
        user.Auth2FA = !(string.IsNullOrEmpty(tpPsw2FA) || tpPsw2FA == GenioServer.security.Auth2FAModes.None.ToString());
        user.Auth2FATp = user.Auth2FA ? tpPsw2FA : "";

        var modulos = Configuration.Modules;
        foreach (var ua in uaList)
        {
            if (!modulos.Contains(ua.ValModulo))
                continue;

            if (!String.IsNullOrEmpty(ua.ValRole))
                user.AddModuleRole(ua.ValModulo, Role.GetRole(ua.ValRole));
            else if (ua.ValNivel > 0)
                user.AddModuleRole(ua.ValModulo, Role.GetRole(ua.ValNivel.ToString()));
        }

        if (user.RolesPerModule.Count > 0)
            user.Years.Add(user.Year);
    }

    private PersistentSupport GetPersistence(string Qyear) => string.IsNullOrEmpty(AuxDb)
                ? PersistentSupport.getPersistentSupport(Qyear)
                : PersistentSupport.getPersistentSupportAux(AuxDb);

    /// <inheritdoc/>
    public override User Authorize(GenioIdentity identity)
    {
        IList<string> anos = new List<string>(Configuration.Years);
        if (Configuration.Years.Count == 0)
            anos.Add(Configuration.DefaultYear);

        User user = new User(identity.Name, "", Configuration.DefaultYear);

        //read the roles separately for each datasystem
        foreach (string Qyear in anos)
        {
            PersistentSupport sp = GetPersistence(Qyear);
            sp.openConnection();

            //fetch the internal uuid and id from the external identity
            var psw = GetUserFromDb(identity, sp);

            //user saves internally the role information per year according to the currently set year
            user.Year = Qyear;
            ReadRoles(user, psw, sp);

            sp.closeConnection();
        }

        if (user.Years.Count == 0)
            throw new FrameworkException("Login não foi encontrado.", "QuidgestRoleProvider.Authorize", "Não foi possivel encontrar a chave correspondente ao login " + identity.Name);
		
		user.Year = user.Years[0]; //set to the first year found, otherwise it might be set to a year without roles, and fails authorization later

        return user;
    }


    /// <inheritdoc/>
    public override bool HasUserDirectory => true;


    private void ValidateSecret(CredentialSecret credential)
    {
        if (credential is PasswordSecret userpass)
        {
            PersistentSupport sp = GetPersistence(Configuration.DefaultYear);

            var uf = new UserFactory(sp, _loginuser);
            sp.openConnection();
            string error = uf.CheckNewPassword(userpass.Username, userpass.NewPass, userpass.ConfirmPass);
            if (error != "")
                throw new InvalidPasswordException(error, "QuidgestRoleProvider.ValidateSecret", error);
            sp.closeConnection();
        }
    }

    private void RegisterSecret(CSGenioApsw psw, CredentialSecret credential)
    {
        if (credential is ExternalIdSecret external)
        {
            //UserId-Email matching
            if (external.Identity.IdProperty == GenioIdentityType.Email)
                psw.ValEmail = external.Identity.Name;
            //External userId matching
            else
                psw.ValUserid = external.Identity.Name;
        }
        else if (credential is PasswordSecret userpass)
        {
            psw.ValPasswordDecrypted = userpass.NewPass;
        }
        else if (credential is TwoFaSecret twofaSecret)
        {
            psw.ValPsw2favl = twofaSecret.Value;
            psw.ValPsw2fatp = twofaSecret.Mode.ToString();
        }
    }

    /// <inheritdoc/>
    public override void CreateNewUser(User user, Dictionary<string, object> claims = null, CredentialSecret credential = null)
    {
        if (credential is not null)
            ValidateSecret(credential);

        var yearList = string.IsNullOrEmpty(AuxDb)
            //if aux db is not set, we create the user for all years
            ? user.Years
            //if aux db is set, we only create the user for the current year
            : [user.Year];

        foreach (var Qyear in yearList)
        {
            //user gets the role information per year according to the currently set year
            user.Year = Qyear;

            PersistentSupport sp = GetPersistence(Qyear);
            try
            {
                sp.openTransaction();

                CSGenioApsw psw = new CSGenioApsw(_loginuser);
                foreach (var claim in claims)
                    psw.insertNameValueField(claim.Key, claim.Value);
                psw.QPrimaryKey = "";
                if(credential is not null)
                    RegisterSecret(psw, credential);
                psw.ValNome = user.Name;
                psw.ValStatus = user.Status;
                psw.insert(sp);

                //the user will be assigned the internal code of the default year database
                if (string.IsNullOrEmpty(user.Codpsw) || Qyear == Configuration.DefaultYear)
                    user.Codpsw = psw.QPrimaryKey;

                foreach (var moduleKvp in user.RolesPerModule)
                {
                    foreach (var role in moduleKvp.Value)
                    {
                        CSGenioAuserauthorization ua = new CSGenioAuserauthorization(_loginuser);
                        ua.ValCodpsw = psw.QPrimaryKey;
                        ua.ValSistema = Configuration.Program;
                        ua.ValRole = role.Id;
                        ua.ValNivel = role.GetLevelInt();
                        ua.ValModulo = moduleKvp.Key;
                        ua.insert(sp);
                    }
                }

                sp.closeTransaction();
            }
            catch (Exception)
            {
                sp.rollbackTransaction();
                throw;
            }
        }
    }

    /// <inheritdoc/>
    public override void SetUserEnabled(User user, int status)
    {
        var yearList = string.IsNullOrEmpty(AuxDb)
            //if aux db is not set, we create the user for all years
            ? user.Years
            //if aux db is set, we only create the user for the current year
            : [user.Year];

        foreach (var Qyear in yearList)
        {
            PersistentSupport sp = GetPersistence(Qyear);
            try
            {
                sp.openTransaction();
                CSGenioApsw psw = CSGenioApsw.search(sp, user.Codpsw, _loginuser);
                if (psw is not null)
                {
                    psw.ValStatus = status;
                    psw.update(sp);
                }
                sp.closeTransaction();
            }
            catch (Exception)
            {
                sp.rollbackTransaction();
                throw;
            }
        }
    }

    /// <inheritdoc/>
    public override List<User> ListUsers(int numRecords = 50, int page = 0, CriteriaSet criteria = null)
    {
        List<User> users = new List<User>();
        return users;
    }


    /// <inheritdoc/>
    public override void SetupUserDirectory()
    {
        //create roles and applications as necessary in the user directory.
        //For quidgest role provider this is not necessary.
    }

    /// <inheritdoc/>
    public override void RegisterExternalId(User user, GenioIdentity identity)
    {
        var QYear = string.IsNullOrEmpty(user.Year) ? Configuration.DefaultYear : user.Year;
        PersistentSupport sp = GetPersistence(QYear);

        //save data to PSW
        sp.openConnection();
        var userPsw = CSGenioApsw.search(sp, user.Codpsw, user);
        //UserId-Email matching
        if (identity.IdProperty == GenioIdentityType.Email)
            userPsw.ValEmail = identity.Name;
        //External userId matching
        else
            userPsw.ValUserid = identity.Name;
        userPsw.updateDirect(sp);
        sp.closeConnection();
    }


    /// <inheritdoc/>
    public override void StoreCredential(User user, CredentialSecret credential)
    {
        if (credential is not null)
            ValidateSecret(credential);

        var yearList = string.IsNullOrEmpty(AuxDb)
            //if aux db is not set, we create the user for all years
            ? user.Years
            //if aux db is set, we only create the user for the current year
            : [user.Year];

        foreach (var Qyear in yearList)
        {
            //user gets the role information per year according to the currently set year
            user.Year = Qyear;

            PersistentSupport sp = GetPersistence(Qyear);
            try
            {
                sp.openTransaction();

                CSGenioApsw psw = CSGenioApsw.search(sp, user.Codpsw, _loginuser);
                if (credential is not null)
                    RegisterSecret(psw, credential);
                psw.update(sp);

                sp.closeTransaction();
            }
            catch (Exception)
            {
                sp.rollbackTransaction();
                throw;
            }
        }
    }


}

using CSGenio;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GenioServer.security
{

    [CredentialProvider(typeof(UserPassCredential))]
    [CredentialProvider(typeof(CertificateCredential))]
    [CredentialProvider(typeof(DomainCredential))]
    [Description("Establishes identity according to the current application database.")]
    [DisplayName("Application Database identity")]
    public class QuidgestIdentityProvider : BaseIdentityProvider
    {
        /// <summary>
        /// Optionally set a aux database to fetch the password from
        /// </summary>
        [SecurityProviderOption(optional:true)]
        [Description("Optionally set a aux database to fetch the password from")]
        public string AuxDb { get; set; }


        /// <inheritdoc/>
        public QuidgestIdentityProvider(IdentityProviderCfgEl config) : base(config)
        {
        }

        /// <inheritdoc/>
        public override bool HasUsernameAuth() => true;

        /// <inheritdoc/>
        public override GenioIdentity Authenticate(Credential credential)
        {
            var anos = new List<string>(Configuration.Years);
            if (Configuration.Years.Count == 0)
            {
                anos.Add(Configuration.DefaultYear);
            }

            GenioIdentity id = null;

            foreach (string Qyear in anos)
            {
                PersistentSupport sp = string.IsNullOrEmpty(AuxDb) 
                    ? PersistentSupport.getPersistentSupport(Qyear)
                    : PersistentSupport.getPersistentSupportAux(AuxDb);

                sp.openConnection();

                if(credential is UserPassCredential upCredential)
                    id = Authenticate(upCredential, sp);
                else if (credential is CertificateCredential certCredential)
                    id = Authenticate(certCredential, sp);
                else if (credential is DomainCredential domCredential)
                    id = Authenticate(domCredential, sp);
                else
                    throw new FrameworkException("The type " + credential.GetType().FullName + " is not supported for QuidgestIdentityProvider authentication.", "QuidgestIdentityProvider", "Credential type not supported");

                sp.closeConnection();

                if (id != null)
                    break;
            }

            return id;
        }

        private GenioIdentity Authenticate(UserPassCredential credential, PersistentSupport sp)
        {
            SelectQuery select = new SelectQuery()
                .Select("psw", "password")
                .Select("psw", "pswtype")
                .Select("psw", "salt")
                .Select("psw", "status")
                .Select("psw", "attempts")
				.Select("psw", "datexp")
                .From(Area.AreaPSW)
                .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
            
            var results = sp.executeReaderOneRow(select);
            if (results.Count == 0)
                return null;
            string pass = DBConversion.ToString(results[0]);
            string pswtype = DBConversion.ToString(results[1]);
            string salt = DBConversion.ToString(results[2]);
            int status = DBConversion.ToInteger(results[3]);
            int attempts = DBConversion.ToInteger(results[4]);
			DateTime datexp = DBConversion.ToDateTime(results[5]);

            int maxAttempts = Configuration.Security.MaxAttempts;

            if (pass == null)
                return null;

            if (PasswordFactory.IsOK(credential.Password, pass, salt, pswtype))
            {
                if (maxAttempts != 0)
                {
                    UpdateQuery updok = new UpdateQuery()
                    .Update(Area.AreaPSW)
                    .Set(CSGenioApsw.FldAttempts, 0)
                    .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
                    sp.Execute(updok);
                }
				
				//Date expiration Validation
                if (Configuration.Security.ExpirationDateBool)
                {
                    try
                    {
                        bool statusChange = false;
                        if (datexp == DateTime.MinValue)
                            statusChange = true;

                        int daysExpirecy = Int32.Parse(Configuration.Security.ExpirationDate);
                        DateTime expirationCheckDate = datexp.AddDays(daysExpirecy);

                        if (expirationCheckDate <= DateTime.Now)
                            statusChange = true;

                        if (statusChange)
                        {
                            UpdateQuery updok = new UpdateQuery()
                            .Update(Area.AreaPSW)
                            .Set(CSGenioApsw.FldStatus, 1)
                            .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
                            sp.Execute(updok);
                        }
                    }
                    catch (FormatException)
                    {
                        Log.Error("Password expiration days have a wrong format. Please review administration website to fix it.");
                    }
                }
				
                return new GenioIdentity
                {
                    Name = credential.Username,
                    IsAuthenticated = true,
                    AuthenticationType = this.GetType().Name,
                    IdProperty = GenioIdentityType.InternalId
                };
            }

            //we failed authentication, so increment the attempts field
            UpdateQuery upd = new UpdateQuery()
                .Update(Area.AreaPSW)
                .Set(CSGenioApsw.FldAttempts, attempts + 1)
                .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
            //if we reached the max attempts, set the status to 2 (deactivated)
            if (maxAttempts != 0 && attempts + 1 == maxAttempts)
                upd = upd.Set(CSGenioApsw.FldStatus, 2);
            sp.Execute(upd);

            return null;
        }

        private GenioIdentity Authenticate(CertificateCredential credential, PersistentSupport sp)
        {
            SelectQuery select = new SelectQuery()
                .Select("psw", "nome")
                .From(Area.AreaPSW)
                .Where(CriteriaSet.And().Equal("psw", "certsn", credential.Certificate.returnSerialNumber()));
            string name = sp.ExecuteScalar(select) as string;
            if (name == null)
                return null;

            return new GenioIdentity
            {
                Name = name,
                IsAuthenticated = true,
                AuthenticationType = this.GetType().Name,
                IdProperty = GenioIdentityType.InternalId
            };
        }

        private GenioIdentity Authenticate(DomainCredential credential, PersistentSupport sp)
        {
            SelectQuery select = new SelectQuery()
                .Select("psw", "nome")
                .From(Area.AreaPSW)
                .Where(CriteriaSet.And().Equal("psw", "nome", credential.DomainUser));
            string name = sp.ExecuteScalar(select) as string;
            if (name == null)
                return null;

            return new GenioIdentity
            {
                Name = name,
                IsAuthenticated = true,
                AuthenticationType = this.GetType().Name,
                IdProperty = GenioIdentityType.InternalId
            };
        }

        public override CredentialSecret NewCredentialCreate(string username, string originalChallenge, string assertion)
        {
            PasswordSecret credentialSecret = new PasswordSecret();
            credentialSecret.Username = username;
            credentialSecret.OldPass = originalChallenge;
            credentialSecret.NewPass = assertion;
            credentialSecret.ConfirmPass = assertion;
            return credentialSecret;
        }
    }
}

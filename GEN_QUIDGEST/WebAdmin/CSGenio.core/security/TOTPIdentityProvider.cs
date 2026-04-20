using CSGenio;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using OtpNet;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;

namespace GenioServer.security
{

    [CredentialProvider(typeof(UserPassCredential))]
    public class TOTPIdentityProvider : BaseIdentityProvider
    {
        /// <summary>
        /// Issuer name displayed in the TOTP qrcode when registering
        /// </summary>
        [SecurityProviderOption(optional: true)]
        [Description("Issuer name displayed in the TOTP qrcode when registering.")]
        public string Issuer { get; set; } = Configuration.Program;

        /// <inheritdoc/>
        public TOTPIdentityProvider(IdentityProviderCfgEl config) : base(config)
        {
        }

        public TOTPIdentityProvider() : base(new IdentityProviderCfgEl() { Name = "Totp" })
        {
        }

        /// <inheritdoc/>
        public override GenioIdentity Authenticate(Credential credential)
        {
            IList<string> anos = new List<string>(Configuration.Years);
            if (Configuration.Years.Count == 0)
            {
                anos.Add(Configuration.DefaultYear);
            }
            Type classname = credential.GetType();
            GenioIdentity id = null;

            foreach (string Qyear in anos)
            {
                PersistentSupport sp = PersistentSupport.getPersistentSupport(Qyear);
                sp.openConnection();

                bool known = false;
                if (classname == typeof(UserPassCredential))
                {
                    id = Authenticate(credential as UserPassCredential, sp);
                    known = true;
                }
                if (!known)
                    throw new InvalidOperationException("The type " + credential.GetType().FullName + " is not supported for QuidgestIdentityProvider authentication.");

                sp.closeConnection();

                if (id != null)
                    break;
            }

            return id;
        }

        private bool IsOk(string valPsw2favl, string pass)
        {
            var secretBytes = Encoding.ASCII.GetBytes(valPsw2favl);
            var p = new Totp(secretBytes);
            var genCode = p.ComputeTotp();
            return pass == genCode;
        }

        private GenioIdentity Authenticate(UserPassCredential credential, PersistentSupport sp)
        {
            SelectQuery select = new SelectQuery()
                .Select("psw", "psw2favl")
                .Select("psw", "psw2fatp")
                .Select("psw", "status")
                .Select("psw", "attempts")
                .From(Area.AreaPSW)
                .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
            
            var results = sp.executeReaderOneRow(select);
            if (results.Count == 0)
                return null;
            string pass = DBConversion.ToString(results[0]);
            string pswtype = DBConversion.ToString(results[1]);
            int status = DBConversion.ToInteger(results[2]);
            int attempts = DBConversion.ToInteger(results[3]);

            int maxAttempts = Configuration.Security.MaxAttempts;

            if (pass == null || status == 2)
                return null;

            if (IsOk(pass, credential.Password))
            {
                if (maxAttempts != 0)
                {
                    UpdateQuery updQ = new UpdateQuery()
                        .Update(Area.AreaPSW)
                        .Set(CSGenioApsw.FldAttempts, 0)
                        .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
                    sp.Execute(updQ);
                }

                return new()
                {
                    Name = credential.Username,
                    IsAuthenticated = true,
                    AuthenticationType = this.GetType().Name,
                    IdProperty = GenioIdentityType.InternalId
                };
            }

            //add one more attempt if user fail the pass
            UpdateQuery upd = new UpdateQuery()
                .Update(Area.AreaPSW)
                .Set(CSGenioApsw.FldAttempts, attempts + 1)
                .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
            if (maxAttempts != 0 && attempts + 1 == maxAttempts)
                upd.Set(CSGenioApsw.FldStatus, 2);
            sp.Execute(upd);

            return null;
        }
		       

        /// <inheritdoc/>
        public override string NewCredentialRequest(string username)
        {
            string secret = PasswordFactory.StringRandom(20, true);
            byte[] secretByte = Encoding.ASCII.GetBytes(secret);
            var uri = new OtpUri(OtpType.Totp, secretByte, username, Issuer);
            return uri.ToString();
        }

        /// <inheritdoc/>
        public override CredentialSecret NewCredentialCreate(string username, string originalChallenge, string assertion)
        {
            var uri = new Uri(originalChallenge);
            var otpParams = HttpUtility.ParseQueryString(uri.Query);
            var secretBase64 = otpParams.Get("secret");
            var secretBytes = Base32Encoding.ToBytes(secretBase64);
            var secret = Encoding.ASCII.GetString(secretBytes);

            if (!IsOk(secret, assertion))
                throw new FrameworkException("Verification code does not match the Totp Qrcode", "TotpIdentityProvider.NewCredentialCreate", "");

            return new TwoFaSecret()
            {
                Username = username,
                Mode = Auth2FAModes.TOTP,
                Value = secret
            };
        }

    }

}

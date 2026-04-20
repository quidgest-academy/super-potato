using CSGenio;
using CSGenio.framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Principal;
using System.Text;

namespace GenioServer.security
{
    [CredentialProvider(typeof(UserPassCredential))]
    [CredentialProvider(typeof(DomainCredential))]
    [Description("Performs a local LDAP connection to validate user credentials")]
    [DisplayName("LDAP Simple")]
    public class LdapIdentityProvider : BaseIdentityProvider
    {
        [SecurityProviderOption()]
        [Description("Local domain. In the form of 'example.org'")]
        public string Domain { get; set; }
		
        [SecurityProviderOption(optional:true)]
        [Description("Port number of the LDAP service. Leave blank for default. LDAPS usually 636.")]
		public string Port { get; set; }

        /// <inheritdoc/>
        public LdapIdentityProvider(IdentityProviderCfgEl config) : base(config)
        {
        }

        /// <inheritdoc/>
        public override bool HasUsernameAuth() => true;

        public override GenioIdentity Authenticate(Credential credential)
        {
            Type classname = credential.GetType();
            GenioIdentity id = null;

            if (classname == typeof(UserPassCredential))
            {
                id = Authenticate_p(credential as UserPassCredential);
            }
            if (classname == typeof(DomainCredential))
            {
                id = Authenticate_p(credential as DomainCredential);
            }

            return id;
        }

        private GenioIdentity Authenticate_p(UserPassCredential credential)
        {
            try
            {
				string connection = "LDAP://" + Domain;
                if (!string.IsNullOrEmpty(Port))
                    connection += ":" + Port;
				
                DirectoryEntry entry = new DirectoryEntry(connection, Domain + "\\" + credential.Username, credential.Password);
                object nativeObject = entry.NativeObject;
                return new()
                {
                    Name = credential.Username,
                    IsAuthenticated = true,
                    AuthenticationType = this.GetType().Name,
                    IdProperty = GenioIdentityType.InternalId,
                };
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("GenioServer.security.LdapIdentityProvider.Autheticate [user] {0} [domain] {1}, [Exception message] {2}", credential.Username, Domain, ex.Message));
            }
            return null;
        }

        private GenioIdentity Authenticate_p(DomainCredential credential)
        {
            //if (DirectoryEntry.Exists("LDAP://" + Domain + "/" + credential.DomainUser))
				return new()
                {
                    Name = credential.DomainUser,
                    IsAuthenticated = true,
                    AuthenticationType = this.GetType().Name,
                    IdProperty = GenioIdentityType.InternalId,
                };

            //return null;
        }

        /// <summary>
        /// Searches for all users in the domain
        /// </summary>
        /// <param name="adminUser">Administrator username</param>
        /// <param name="adminPsw">Administrator password</param>
        /// <returns>A list with the username of each user</returns>
        public List<string> ImportUsers(string adminUser, string adminPsw)
        {
            //get complete domain value separated by dot
            var domainValue = Domain;
            if (!string.IsNullOrEmpty(Port))
                domainValue += ":" + Port;

            //derive DC root from the domain
            string[] split = Domain.Split('.');
            StringBuilder root = new StringBuilder();
            foreach (string s in split)
            {
                root.Append("DC=");
                root.Append(s);
                root.Append(',');
            }
            if(root.Length > 0)
                root.Length -= 1;

            using (var ldap = new LdapConnection(new LdapDirectoryIdentifier(domainValue)))
            {
                ldap.SessionOptions.ProtocolVersion = 3;
                ldap.Credential = new NetworkCredential(adminUser, adminPsw);
                ldap.AuthType = AuthType.Basic;

                ldap.Bind();

                var search = ldap.SendRequest(
                    new SearchRequest(
                        root.ToString(),
                        "(&(&(objectCategory=person)(objectClass=user)(!useraccountcontrol:1.2.840.113556.1.4.803:=2)))",
                        System.DirectoryServices.Protocols.SearchScope.Subtree)
                    ) as SearchResponse;

                List<string> result = new List<string>();
                if (search != null)
                    foreach (SearchResultEntry entry in search.Entries)
                        result.Add(entry?.Attributes["samaccountname"][0].ToString() ?? "");

                return result;
            }
        }
	}
}

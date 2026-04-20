using System;
using CSGenio.framework;

namespace GenioServer.security
{
    public abstract class Credential
    {
        public string Year { get; set; }
    }

    public class UserPassCredential : Credential
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class CertificateCredential : Credential
    {
        public ClientCertificate Certificate { get; set; }
    }

    public class DomainCredential : Credential
    {
        public string DomainUser { get; set; }
    }

    public class TokenCredential : Credential
    {
        public string Token { get; set; }
        public string Auth { get; set; }
        public string OriginUrl { get; set; }
    }

    public class Password
    {
        public Password(string newPass, string confirmPass)
        {
            New = newPass;
            Confirm = confirmPass;
        }
        public string New { get; set; }
        public string Confirm { get; set; }
    }

    public class InvalidPasswordException : FrameworkException
    {
        public InvalidPasswordException(string errorMsg, string site, string cause) : base(errorMsg, site, cause) { }
    }

    /// <summary>
    /// Intention of setting in the <see cref="User"/> the secret of a <see cref="Credential"/>
    /// </summary>
    /// <remarks>
    /// The type of the class is important to be unique enough, even if it carries the same data types,
    /// so that the <see cref="IRoleProvider"/> can use the type to persist the secret correctly.
    /// </remarks>
    public abstract class CredentialSecret
    {
        public string Username { get; set; }
    }

    public class PasswordSecret : CredentialSecret
    {
        public string NewPass { get; set; }
        public string ConfirmPass { get; set; }
        public string OldPass { get; set; }
    }

    public class TwoFaSecret : CredentialSecret
    {
        public Auth2FAModes Mode { get; set; }
        public string Value { get; set; }
    }

    public class CertificateSecret : CredentialSecret
    {
        public string Code { get; set; }
    }
    
    public class ExternalIdSecret : CredentialSecret
    {
        public GenioIdentity Identity { get; set; }
    }
}

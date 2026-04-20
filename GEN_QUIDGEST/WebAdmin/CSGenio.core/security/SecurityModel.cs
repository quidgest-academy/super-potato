using System;
using System.Collections.Generic;
using System.Text;

namespace GenioServer.security
{
    /// <summary>
    /// Autentication mode when multiple identity providers are used
    /// </summary>
    public enum AuthenticationMode
    {        
        /// <summary>
        /// The user must be valid in at least one of the providers configured
        /// </summary>
        AcceptOnFirstSucess,
        /// <summary>
        /// The users will have a specific login button per provider configured
        /// </summary>
        OneButtonPerProvider
    }

    public enum MultiSessionMode
    {
        /// <summary>
        /// No restrictions will be applied (this is the default)
        /// </summary>
        Loose,
        /// <summary>
        /// Will only allow additional sessions on the same ip address as the initial session
        /// </summary>
        PerIp,
        /// <summary>
        /// The most rigorous mode will only allow 1 session per use, meaning,
        /// if something goes wrong with the active session, the user might have to wait till a session time-out
        /// </summary>
        Strict
    }

    public enum Auth2FAModes
    {
        None,
        TOTP,
        WebAuth
    }

    public enum PasswordStrength
    {
        Pobre,
        Fraco,
        Bom,
        Forte
    }

    public enum PasswordAlgorithms
    {
        QUI,
        ARG
    }

}

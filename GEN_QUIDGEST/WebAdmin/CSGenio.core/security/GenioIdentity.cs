using System.Collections.Generic;
using System.Security.Principal;

namespace GenioServer.security;

/// <summary>
/// Declares how the Name property of the GenioIdentity should be interpreted
/// </summary>
public enum GenioIdentityType
{
    /// <summary>
    /// Name will refer to the internal user id of the user directory
    /// </summary>
    InternalId,
    /// <summary>
    /// Name will refer to the external provider user id associated with that user
    /// </summary>
    ExternalId,
    /// <summary>
    /// Name will refer to the unique email registered with the user
    /// </summary>
    Email
}

/// <summary>
/// Result of a authentication performed by a GenioIdentityProvider.
/// </summary>
public class GenioIdentity : IIdentity
{
    /// <inheritdoc/>
    public string AuthenticationType { get; set; }
    /// <inheritdoc/>
    public bool IsAuthenticated { get; set; }
    /// <inheritdoc/>
    public string Name { get; set; }
    /// <summary>
    /// Declares how the Name property of the GenioIdentity should be interpreted
    /// </summary>
    public GenioIdentityType IdProperty { get; set; } = GenioIdentityType.InternalId;
    /// <summary>
    /// List of claims collected during the authentication process.
    /// </summary>
    public Dictionary<string, string> Claims { get; private set; } = new Dictionary<string, string>();
}

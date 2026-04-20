using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace CSGenio.core.CSMail;

/// <summary>
/// Defines a contract for token providers, ensuring a method for obtaining an access token.
/// </summary>
public interface ITokenProvider
{
    /// <summary>
    /// Asynchronously retrives an OAuth2 access token.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for task management.</param>
    /// <returns>A task that returns the access token as a string upon completions.</returns>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}

public class OAuth2TokenManager
{
    /// <summary>
    /// Instantiates the appropriate token provider based on whether a certificate or client secret is provided.
    /// </summary>
    /// <returns>An instance of <see cref="ITokenProvider"/> configured for OAuth2 token retrival.</returns>
	public static ITokenProvider GetTokenProvider(CSGenio.config.OAuth2Options options)
	{
		// Use certificate-based authentication if a certificate is provided
		if(!string.IsNullOrEmpty(options.CertificateThumbprintDecrypted))
		{
            // Create an X509Certificate2 object from the raw data
            var cert = GetCertificateFromCurrentUserStore(options.CertificateThumbprintDecrypted);
			return new OAuth2ServiceWithCertificate(options.ClientId, cert, options.TokenEndpoint, options.Scope);
		}
        // Fallback to client secret authentication
        else if(!string.IsNullOrEmpty(options.ClientSecretDecrypted))
        {
		    return new OAuth2Service(options.ClientId, options.ClientSecretDecrypted, options.TokenEndpoint, options.Scope);
        }

        throw new Exception("Neither the decrypted Client Secret nor the decrypted Certificate Thumbprint has been defined. Please check the configuration and ensure that the values are decrypted before being used");
	}

    /// <summary>
    /// Retrieves a certificate from the CurrentUser certificate store based on its thumbprint.
    /// </summary>
    /// <param name="thumbprint">The thumbprint of the certificate to retrieve.</param>
    /// <returns>An <see cref="X509Certificate2"/> object representing the certificate.</returns>
    /// <exception cref="Exception">Thrown if the certificate is not found.</exception>
    public static X509Certificate2 GetCertificateFromCurrentUserStore(string thumbprint)
    {
        if (string.IsNullOrEmpty(thumbprint))
            throw new ArgumentNullException(nameof(thumbprint), "Thumbprint cannot be null or empty.");

        // Remove any whitespace and convert to uppercase
        thumbprint = thumbprint.Replace(" ", string.Empty).ToUpperInvariant();

        // Open the CurrentUser certificate store
        using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
		store.Open(OpenFlags.ReadOnly);

		// Find the certificate by thumbprint
		var certificates = store.Certificates.Find(
			X509FindType.FindByThumbprint,
			thumbprint,
			validOnly: false);

		if (certificates.Count == 0)
		{
			throw new Exception($"Certificate with thumbprint '{thumbprint}' not found in the CurrentUser store.");
		}

		return certificates[0];
    }
}

/// <summary>
/// Represents the structure of an OAuth2 token response.
/// </summary>
public class OAuth2TokenResponse
{
    /// <summary>
    /// The access token issued by the OAuth2 provider.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// The type of the token issued (e.g., Bearer).
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    /// <summary>
    /// The lifetime in seconds of the access token.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}

/// <summary>
/// Provides an implementation of OAuth2 using client credentials and client secret.
/// The Client Credentials flow is used when an application needs to access its own resources rather than a user's resources. 
/// In this flow, the application sends its client ID and client secret to the authorization server, 
/// and in return receives an access token that can be used to access protected resources.
///      +---------+                                  +---------------+
///      |         |>--(A)- Client Authentication --->| Authorization |
///      | Client  |                                  |     Server    |
///      |         |<--(B)---- Access Token ---------<|               |
///      +---------+                                  +---------------+
/// </summary>
/// <param name="clientId">The OAuth2 client identifier.</param>
/// <param name="clientSecret">The OAuth2 client secret.</param>
/// <param name="tokenEndpoint">The OAuth2 token endpoint URI.</param>
/// <param name="scopes">The cope(s) required for the OAuth2 token.</param>
public class OAuth2Service(string clientId, string clientSecret, string tokenEndpoint, string[] scopes) : ITokenProvider
{
    /// <inheritdoc/>
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();

        // Prepare the request body with the required parameters
        var requestBody = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "grant_type", "client_credentials" },
            { "scope", string.Join(" ", scopes) }
        };

        try
        {
            // Send a POST request to the token endpoint
            var response = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestBody), cancellationToken);

            // Read the response content
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching access token: {content}");
            }

            // Deserialize the response content
            var tokenResponse = JsonSerializer.Deserialize<OAuth2TokenResponse>(content);

            return tokenResponse?.AccessToken;
        }
        catch (Exception ex)
        {
            throw new Exception($"OAuth2Service - Failed to acquire OAuth2 access token.", ex);
        }
    }
}

/// <summary>
/// Provides an implementation of OAuth2 using client credentials with certificate-based authentication.
/// The Client Credentials flow is used when an application needs to access its own resources rather than a user's resources. 
/// In this flow, the application sends its client ID and JSON Web Token (JWT) which was signed by its cryptographic key to the authorization server, 
/// and in return receives an access token that can be used to access protected resources.
///      +---------+                                  +---------------+
///      |         |>--(A)- Client Authentication --->| Authorization |
///      | Client  |                                  |     Server    |
///      |         |<--(B)---- Access Token ---------<|               |
///      +---------+                                  +---------------+
/// </summary>
/// <param name="clientId">The OAuth2 client identifier.</param>
/// <param name="certificate">The X509 certificate used for authentication.</param>
/// <param name="tokenEndpoint">The OAuth2 token endpoint URI.</param>
/// <param name="scopes">The scope(s) required for the OAuth2 token.</param>
public class OAuth2ServiceWithCertificate(string clientId, X509Certificate2 certificate, string tokenEndpoint, string[] scopes) : ITokenProvider
{
    /// <inheritdoc/>
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        // Create a JWT token signed with the certificate
        var jwtToken = CreateJwtToken();
        using var client = new HttpClient();

        // Prepare the request body with the required parameters
        var requestBody = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "grant_type", "client_credentials" },
            { "scope", string.Join(" ", scopes) },
            { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
            { "client_assertion", jwtToken }
        };

        try
        {
            // Send a POST request to the token endpoint
            var response = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestBody), cancellationToken);

            // Read the response content
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching access token: {content}");
            }

            // Deserialize the response content
            var tokenResponse = JsonSerializer.Deserialize<OAuth2TokenResponse>(content);

            return tokenResponse?.AccessToken;
        }
        catch (Exception ex)
        {
            throw new Exception($"OAuth2ServiceWithCertificate - Failed to acquire OAuth2 access token.", ex);
        }
    }

    /// <summary>
    /// Creates a signed JWT token to be used as a client assertion for OAuth2 token request.
    /// </summary>
    /// <returns>A JWT token string.</returns>
    private string CreateJwtToken()
    {
        /*
             When authenticating the client at the token endpoint, you generate and sign a JSON Web Token (JWT) with the following claims:

                iss (Issuer): REQUIRED. The client_id of the OAuth client.
                sub (Subject): REQUIRED. The client_id of the OAuth client.
                aud (Audience): REQUIRED. The token endpoint URI. The recipient of the token.
                exp (Expiration time): REQUIRED. The expiration time of the token.
                jti (JWT ID): REQUIRED. A unique identifier for the token to prevent replay attacks.

             The JWT is then used as the client_assertion parameter when requesting the access token.

			 https://learn.microsoft.com/en-us/entra/identity-platform/certificate-credentials
         */

        var now = DateTimeOffset.UtcNow;

        // JWT header specifying the signing algorithm and certificate key
        var jwtHeader = new JwtHeader(new X509SigningCredentials(certificate, SecurityAlgorithms.RsaSha256));

        // JWT payload with issuer, subject, audience, and expiration details
        var jwtPayload = new JwtPayload(
            clientId,
            tokenEndpoint,
            null,
            now.UtcDateTime,
            now.AddMinutes(10).UtcDateTime,
			now.DateTime)
        {
            { JwtRegisteredClaimNames.Sub, clientId }, // Subject: the client ID
            { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() } // JWT ID: a unique identifier for the token to prevent replay attacks
        };

        // Create the JWT token and write it as a string
        var jwtToken = new JwtSecurityToken(jwtHeader, jwtPayload);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(jwtToken);
    }
}
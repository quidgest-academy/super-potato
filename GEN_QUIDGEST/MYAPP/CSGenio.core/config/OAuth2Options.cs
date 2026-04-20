using System;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CSGenio.config;

/// <summary>
/// Enum repserenting the authentication type for a service.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<AuthType>))]
public enum AuthType
{
	None,
	BasicAuth,
	OAuth2
}

/// <summary>
/// Options for configuring OAuth2 authentication, including client credentials or certificate-based auth.
/// </summary>
public class OAuth2Options
{
	/// <summary>
	/// Gets aor sets the OAuth2 client identifier.
	/// </summary>
	public string ClientId { get; set; }

	/// <summary>
	/// Gets or sets the OAuth2 encrypted client secret. Should not be used if certificate authentication is enabled.
	/// </summary>
	[JsonIgnore]
	public string ClientSecret { get; set; }

	/// <summary>
	/// Gets or sets the OAuth2 decrypted client secret. Should not be used if certificate authentication is enabled.
	/// </summary>
	[XmlIgnore]
	[JsonIgnore]
	public string ClientSecretDecrypted
	{
		get
		{
			if (ClientSecret == null) return null;
			return Encoding.Unicode.GetString(Convert.FromBase64String(ClientSecret));
		}
		set
		{
			ClientSecret = Convert.ToBase64String(Encoding.Unicode.GetBytes(value ?? string.Empty));
		}
	}
	
	/// <summary>
	/// Gets or sets the encrypted string representation Thumbprint of the CurrentUser certificate used for authentication.
	/// </summary>
	[JsonIgnore]
	public string CertificateThumbprint { get; set; }

	/// <summary>
	/// Gets or sets the string decrypted representation Thumbprint of the CurrentUser certificate used for authentication.
	/// </summary>
	[XmlIgnore]
	[JsonIgnore]
	public string CertificateThumbprintDecrypted
	{
		get
		{
			if (CertificateThumbprint == null) return null;
			return Encoding.Unicode.GetString(Convert.FromBase64String(CertificateThumbprint));
		}
		set
		{
			CertificateThumbprint = Convert.ToBase64String(Encoding.Unicode.GetBytes(value ?? string.Empty));
		}
	}

	/// <summary>
	/// Gets or sets the OAuth2 token endpoint URI.
	/// </summary>
	public string TokenEndpoint { get; set; }

	/// <summary>
	/// Gets or sets the scope(s) required for the OAuth2 token.
	/// </summary>
	public string[] Scope { get; set; }
}
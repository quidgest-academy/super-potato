using CSGenio;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GenioServer.security;

[CredentialProvider(typeof(WebAuthCredential))]
[Description("Establishes identity using an external OpenIdConnect provider.")]
[DisplayName("WebAuthn")]
public class WebauthnIdentityProvider : BaseIdentityProvider
{
    /// <summary>
    /// The url that will own the fido2 generated keys. Keys are bound to origins. The origin must match the service web domain.
    /// </summary>
    [SecurityProviderOption()]
    [Description("The url that will own the fido2 generated keys. Keys are bound to origins. The origin must match the service web domain.")]
    public string Origin { get; set; } = "http://localhost:5294";

    private readonly Fido2 _fido2;

    /// <inheritdoc/>
    public WebauthnIdentityProvider(IdentityProviderCfgEl config) : base(config)
    {
        Uri uri = new Uri(Origin);

        _fido2 = new Fido2(new Fido2Configuration()
        {
            ServerDomain = uri.Host,
            ServerName = uri.Host,
            TimestampDriftTolerance = 1000,
            Origins = new HashSet<string>() { uri.AbsoluteUri }
        });
    }

    /// <inheritdoc/>
    public override string AuthenticateChallenge(string username)
    {
        var credentialId = FetchFromDatabase(username).KeyId;
        var assertion = _fido2.GetAssertionOptions(new GetAssertionOptionsParams
        {
            AllowedCredentials = [new PublicKeyCredentialDescriptor(credentialId)],
            UserVerification = UserVerificationRequirement.Preferred,
            Extensions = new AuthenticationExtensionsClientInputs
            {
                Extensions = true
            }
        });
        return assertion.ToJson();
    }

    /// <inheritdoc/>
    public override GenioIdentity Authenticate(Credential credential)
    {
        if (credential is WebAuthCredential wan)
            return Authenticate(wan);

        return null;
    }


    private GenioIdentity Authenticate(WebAuthCredential credential)
    {
        var assertionResponse = JsonSerializer.Deserialize<AuthenticatorAssertionRawResponse>(credential.Assertion) ?? throw new Exception("nope");
        //username = UTF8(Base64(assertionResponse.Response.UserHandle))

        var pk = FetchFromDatabase(credential.Username).PubKey;

        var result = _fido2.MakeAssertionAsync(new MakeAssertionParams
        {
            AssertionResponse = assertionResponse,
            OriginalOptions = AssertionOptions.FromJson(credential.OriginalOptions),
            StoredPublicKey = pk,
            StoredSignatureCounter = 0,//sigcounter,
            IsUserHandleOwnerOfCredentialIdCallback = (IsUserHandleOwnerOfCredentialIdParams args, CancellationToken token) =>
            {
                return Task.FromResult(true);
            }
        }).Result;

        return new GenioIdentity()
        {
            Name = credential.Username,
            IdProperty = GenioIdentityType.InternalId
        };
    }

    private WanKeyStore FetchFromDatabase(string username)
    {
        var sp = PersistentSupport.getPersistentSupport(Configuration.DefaultYear);
        SelectQuery select = new SelectQuery()
            .Select("psw", "psw2favl")
            .From(Area.AreaPSW)
            .Where(CriteriaSet.And().Equal("psw", "nome", username));
        sp.openConnection();
        string psw2favl = DBConversion.ToString(sp.executeScalar(select));
        sp.closeConnection();
        if (string.IsNullOrEmpty(psw2favl))
            throw new FrameworkException("Webauthn key not found", "WebauthnIdentityProvider.FetchFromDatabase", "");
        var res = JsonSerializer.Deserialize<WanKeyStore>(psw2favl);
        return res;
    }


    /// <inheritdoc/>
    public override string NewCredentialRequest(string username)
    {
        var challenge = _fido2.RequestNewCredential(new RequestNewCredentialParams
        {
            User = new Fido2User()
            {
                Name = username,
                Id = Encoding.UTF8.GetBytes(username),
                DisplayName = username
            },
            AuthenticatorSelection = AuthenticatorSelection.Default,
            AttestationPreference = AttestationConveyancePreference.Direct,
            Extensions = new AuthenticationExtensionsClientInputs
            {
                CredProps = true
            }
        });
        return challenge.ToJson();
    }

    /// <inheritdoc/>
    public override CredentialSecret NewCredentialCreate(string username, string originalChallenge, string assertion)
    {
        var attestationResponse = JsonSerializer.Deserialize<AuthenticatorAttestationRawResponse>(assertion) ?? throw new Exception("nope");

        var credentials = _fido2.MakeNewCredentialAsync(new MakeNewCredentialParams()
        {
            AttestationResponse = attestationResponse,
            OriginalOptions = CredentialCreateOptions.FromJson(originalChallenge),
            IsCredentialIdUniqueToUserCallback = (IsCredentialIdUniqueToUserParams args, CancellationToken cancel) =>
            {
                return Task.FromResult(true);
            }
        }).Result;

        return new TwoFaSecret()
        {
            Username = username,
            Mode = Auth2FAModes.WebAuth,
            Value = JsonSerializer.Serialize(new WanKeyStore()
            {
                KeyId = credentials.Id,
                PubKey = credentials.PublicKey,
            })
        };
    }

    public struct WanKeyStore
    {
        public byte[] KeyId { get; set; }
        public byte[] PubKey { get; set; }
    }
}

public class WebAuthCredential : Credential
{
    public string Username { get; set; }
    public string OriginalOptions { get; set; }
    public string Assertion { get; set; }
}


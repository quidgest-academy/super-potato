using AspNetCore.LegacyAuthCookieCompat;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace GenioMVC;


public class LegacyFormsAuthenticationOptions: AuthenticationSchemeOptions
{
    public const string DefaultScheme = "LegacyForms";

    public string DecryptionKey { get; set; } = "AC7387D7E54B156377D81930CF237888854B5B5B515CF2D6356541255E696144";
    public string ValidationKey { get; set; } = "30101052676849B0B494466B7A99656346328E8964748448E422D7344467A45777D972414947271744423422851D6742C9A09A65212C276C7F839157501291C6";
    public string Algorithm { get; set; } = "SHA1";
    public string AuthenticationScheme { get; set; } = DefaultScheme;
    public PathString LoginPath { get; set; } = new("/");
    public PathString AccessDeniedPath { get; set; } = new("/account/accessdenied");
    public TimeSpan ExpireTimeSpan { get; set; } = TimeSpan.FromDays(1);
    public string CookieName { get; set; } = "App.ASPXAUTH";
    public bool CheckIpAddress { get; set; } = false;
    public bool AutorenewOnRequest { get; set; } = true;
}


public class LegacyFormsAuthentication : SignInAuthenticationHandler<LegacyFormsAuthenticationOptions>
{
    private readonly IConfiguration _config;
    LegacyFormsAuthenticationTicketEncryptor GetEncryptor()
    {
        var algo = Options.Algorithm switch
        {
            "SHA1" => ShaVersion.Sha1,
            "HMACSHA256" => ShaVersion.Sha256,
            "HMACSHA512" => ShaVersion.Sha512,
            _ => ShaVersion.Sha1
        };

        return new LegacyFormsAuthenticationTicketEncryptor(Options.DecryptionKey, Options.ValidationKey, algo, CompatibilityMode.Framework45);
    }

    public LegacyFormsAuthentication(IOptionsMonitor<LegacyFormsAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, IConfiguration config)
        : base(options, logger, encoder)
    {
        _config = config;
    }


    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authCookie = Request.Cookies[Options.CookieName];
        if (authCookie == null)
            return Task.FromResult(AuthenticateResult.NoResult());

        FormsAuthenticationTicket formsTicket;
        try
        {
            formsTicket = GetEncryptor().DecryptCookie(authCookie);
        } 
        catch (Exception ex)
        {
            return Task.FromResult(AuthenticateResult.Fail(ex));
        }

        //check time
        if(formsTicket.Expired)
            return Task.FromResult(AuthenticateResult.Fail("authentication cookie has expired"));

        //check ip address
        if (Options.CheckIpAddress && Context.GetIpAddress() != formsTicket.UserData)
            return Task.FromResult(AuthenticateResult.Fail("cookie was not issued for this ip address"));

        //check for autorenew when the cookie is close to expire
        if (Options.AutorenewOnRequest && (formsTicket.Expiration - DateTime.Now).TotalSeconds < 60)
            IssueCookie(formsTicket.Name, formsTicket.UserData);

        var claimsIdentity = new ClaimsIdentity(new List<Claim> {
            new Claim(ClaimTypes.Name, formsTicket.Name)
        }, Options.AuthenticationScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var ticket = new AuthenticationTicket(claimsPrincipal, Options.AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }


    private void IssueCookie(string username, string userdata)
    {
        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
            1,
            username,
            DateTime.UtcNow,
            DateTime.UtcNow + Options.ExpireTimeSpan,
            false,
            userdata,
            "/");

        string encTicket = GetEncryptor().Encrypt(ticket);

        //The cookie should be created with SameSite Strict, however the current implementation
        // of openid registration is based on the callback from third party provider sharing the cookie.
        //The OpenIdProvider implementation needs to be changed to instead send the codpsw needed to keep state
        // in the State payload, rather than rely on cookie sharing, which is very insecure.
        var options = new CookieOptions()
        {
            HttpOnly = true,
            Expires = ticket.Expiration,
            SameSite = SameSiteMode.Strict,
            Secure = true,
        };
        _config.GetSection("CookieAuthOptions").Bind(options);
        
        Response.Cookies.Append(Options.CookieName, encTicket, options);
    }

    protected override Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties? properties)
    {
        if (user?.Identity?.Name == null)
            throw new InvalidOperationException("user claims does not contain a name property");

        string userdata = string.Empty;

        if (Options.CheckIpAddress)
            userdata = Context.GetIpAddress();

        IssueCookie(user.Identity.Name, userdata);

        return Task.CompletedTask;
    }

    protected override Task HandleSignOutAsync(AuthenticationProperties? properties)
    {
        Response.Cookies.Delete(Options.CookieName);
        return Task.CompletedTask;
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    }

}

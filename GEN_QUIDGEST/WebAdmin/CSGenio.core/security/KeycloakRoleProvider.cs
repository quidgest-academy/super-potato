using CSGenio;
using CSGenio.core.di;
using CSGenio.framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;

namespace GenioServer.security;

/// <summary>
/// Role provider for Keycloak identity and access management service
/// </summary>
[Description("Retrieves the roles of a given user from the application database.")]
[DisplayName("Keycloak Access Management")]
public class KeycloakRoleProvider : BaseRoleProvider
{
    /// <summary>
    /// Base url of the Keycloak service
    /// </summary>
    [SecurityProviderOption()]
    [Description("Base url of the Keycloak service")]
    public string BaseUrl { get; set; }

    /// <summary>
    /// Custom Keycloak realm
    /// </summary>
    [SecurityProviderOption(optional: true)]
    [Description("Custom Keycloak realm")]
    public string Realm { get; set; } = "master";

    /// <summary>
    /// Application ID registered in Keycloak
    /// </summary>
    [SecurityProviderOption()]
    [Description("Application ID registered in Keycloak")]
    public string ClientId { get; set; }

    /// <summary>
    /// Application service account with rights to manage user directory
    /// </summary>
    [SecurityProviderOption()]
    [Description("Application service account with rights to manage user directory")]
    public string AdminUser { get; set; }

    /// <summary>
    /// Access password for the admin service account
    /// </summary>
    [SecurityProviderOption()]
    [Description("Access password for the admin service account")]
    public string AdminPass { get; set; }

    /// <inheritdoc/>
    public override bool HasUserDirectory => true;

    /// <inheritdoc/>
    public KeycloakRoleProvider(RoleProviderCfgEl config) : base(config)
    {
    }

    private HttpClient SetupAuthorizedHttp()
    {
        var http = GenioDI.HttpFactory.CreateClient("keycloak");

        // Obtain access token from Keycloak (should be cached for performance in high login load scenarios)
        var access_token = GetKeycloakAccessToken(http);
        if (string.IsNullOrEmpty(access_token))
            return null;
        // Set the access token in the request headers for subsequent requests
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
        return http;
    }

    private string GetKeycloakAccessToken(HttpClient http)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string> {
            { "grant_type", "password" },
            { "client_id", "admin-cli" },
            { "username", AdminUser },
            { "password", AdminPass }
        });

        var resp = http.PostAsync($"{BaseUrl}/realms/{Realm}/protocol/openid-connect/token", content).Result;
        resp.EnsureSuccessStatusCode();
        string jsonResult = resp.Content.ReadAsStringAsync().Result;

        var objAccess = JsonNode.Parse(jsonResult);
        return objAccess?["access_token"]?.GetValue<string>();
    }

    private List<string> GetKeycloakUserRoles(HttpClient http, string userId)
    {
        var resp = http.GetAsync($"{BaseUrl}/admin/realms/{Realm}/users/{userId}/role-mappings/clients/{ClientId}").Result;
        resp.EnsureSuccessStatusCode();
        string jsonResult = resp.Content.ReadAsStringAsync().Result;
        var objRoles = JsonNode.Parse(jsonResult);
        List<string> roleNames = [];
        foreach (var objNode in objRoles.AsArray())
        {
            string roleName = objNode["name"]?.GetValue<string>();
            if (!string.IsNullOrEmpty(roleName))
                roleNames.Add(roleName);
        }
        return roleNames;
    }

    private List<KeyValuePair<string,string>> GetKeycloakAppRoles(HttpClient http)
    {
        var resp = http.GetAsync($"{BaseUrl}/admin/realms/{Realm}/clients/{ClientId}/roles").Result;
        resp.EnsureSuccessStatusCode();
        string jsonResult = resp.Content.ReadAsStringAsync().Result;
        var objRoles = JsonNode.Parse(jsonResult);
        List<KeyValuePair<string, string>> roles = [];
        foreach (var objNode in objRoles.AsArray())
        {
            string roleId = objNode["id"]?.GetValue<string>();
            string roleName = objNode["name"]?.GetValue<string>();

            if (!string.IsNullOrEmpty(roleId) && !string.IsNullOrEmpty(roleName))
                roles.Add(new(roleId, roleName));
        }
        return roles;
    }

    private void CreateKeycloakRole(string key, string title, HttpClient http)
    {
        // Prepare the JSON payload for the new role
        var payload = new Dictionary<string, object>
        {
            { "name", key },
            { "description", title }
        };
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        // POST to Keycloak to create the role
        try
        {
            var resp = http.PostAsync($"{BaseUrl}/admin/realms/{Realm}/clients/{ClientId}/roles", content).Result;
            resp.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
    }

    private string CreateKeycloakUser(string username, string email, bool enabled, string password, HttpClient http)
    {
        var payload = new Dictionary<string, object>
        {
            { "username", username },
            { "email", email },
            { "emailVerified", false },
            { "enabled", enabled }
        };
        if(!string.IsNullOrEmpty(password))
        {
            var credentials = new Dictionary<string, object>
            {
                { "type", "password" },
                { "value", password },
            };
            List<Dictionary<string, object>> credential_list = [credentials];
            payload.Add("credentials", credential_list);
        }

        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        // POST to Keycloak to create the user
        try
        {
            var resp = http.PostAsync($"{BaseUrl}/admin/realms/{Realm}/users", content).Result;
            resp.EnsureSuccessStatusCode();

            //Header location will contain the user id within a url that looks like:
            //http://localhost:28080/admin/realms/master/users/c6d4806e-64bb-4ab1-bfd9-1746dd06f1fc
            //so we need to parse it down to the id
            if (resp.Headers.TryGetValues("location", out var values))
                return values.First().Split('/').Last();
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
        return "";
    }

    private void AddKeycloakUserRoles(string userId, List<KeyValuePair<string, string>> roles, HttpClient http)
    {
        var payload = new List<Dictionary<string, object>>();
        foreach (var role in roles)
        {
            payload.Add(new Dictionary<string, object>
            {
                { "id", role.Key },
                { "name", role.Value },
            });
        }
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        // POST to Keycloak to create the role
        try
        {
            var resp = http.PostAsync($"{BaseUrl}/admin/realms/{Realm}/users/{userId}/role-mappings/clients/{ClientId}", content).Result;
            resp.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
    }

    private string SetKeycloakUserEnabled(string userId, bool userEnabled, bool emailVerified, HttpClient http)
    {
        var payload = new Dictionary<string, object>
        {
            { "emailVerified", emailVerified },
            { "enabled", userEnabled }
        };

        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        // POST to Keycloak to change the user status
        try
        {
            var resp = http.PostAsync($"{BaseUrl}/admin/realms/{Realm}/users/{userId}", content).Result;
            resp.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
        return "";
    }

    private string GetKeycloakUserid(HttpClient http, string query, string username)
    {
        var resp = http.GetAsync($"{BaseUrl}/admin/realms/{Realm}/users?{query}={username}").Result;
        resp.EnsureSuccessStatusCode();
        string jsonResult = resp.Content.ReadAsStringAsync().Result;
        var objRoles = JsonNode.Parse(jsonResult).AsArray();
        if (objRoles.Count == 0)
            return null;
        return objRoles[0]["id"]?.GetValue<string>();
    }

    private string GetKeycloakUserInfo(HttpClient http, string userid)
    {
        var resp = http.GetAsync($"{BaseUrl}/admin/realms/{Realm}/users/{userid}").Result;
        resp.EnsureSuccessStatusCode();
        string jsonResult = resp.Content.ReadAsStringAsync().Result;
        var objRoles = JsonNode.Parse(jsonResult);
        return objRoles["username"]?.GetValue<string>();
    }

    /// <inheritdoc/>
    public override User Authorize(GenioIdentity identity)
    {
        using var http = SetupAuthorizedHttp();
        if(http is null) 
            return null;

        // Search the user according to the type of identity provided
        string userid;
        string username;
        switch(identity.IdProperty)
        {
            case GenioIdentityType.InternalId:
            default:
                userid = GetKeycloakUserid(http, "username", identity.Name);
                username = identity.Name;
                break;
            case GenioIdentityType.ExternalId:
                var split = identity.Name.Split('@');
                userid = split[0];
                username = GetKeycloakUserInfo(http, userid);                
                break;
            case GenioIdentityType.Email:
                userid = GetKeycloakUserid(http, "email", identity.Name);
                username = identity.Name;
                break;
        }

        // Fetch the user roles from Keycloak
        List<string> roleNames = GetKeycloakUserRoles(http, userid);
        // Create and return a User object with the roles
        User user = new User(username, "", Configuration.DefaultYear);
        user.Auth2FA = false;
        var all_modules = Configuration.Modules;

        List<string> anos = Configuration.Years.Count == 0
            ? [Configuration.DefaultYear]
            : Configuration.Years;
        foreach (string Qyear in anos)
        {
            //user saves internally the role information per year according to the currently set year
            user.Year = Qyear;
            user.Codpsw = userid;
            foreach (string role in roleNames)
            {
                int ix = role.IndexOf('@');
                //generic roles have to be added to all modules
                if(ix == -1)
                {
                    var r = Role.GetRole(role);
                    if (r != Role.INVALID)
                        foreach (var m in r.AvailableModules)
                            user.AddModuleRole(m, r);
                }
                //module specific roles are only added to that module
                else
                {
                    var role_split = role.Split('@');
                    var r = Role.GetRole(role_split[0]);
                    if (r != Role.INVALID)
                        user.AddModuleRole(role_split[1], r);
                }
            }
            if (user.RolesPerModule.Count > 0)
                user.Years.Add(user.Year);
        }

        if (user.Years.Count == 0)
            throw new FrameworkException("Login sem autorizações.", "KeycloakRoleProvider.Authorize", "Não existem autorizações para o login " + identity.Name);

        return user;
    }

    /// <inheritdoc/>
    public override void SetupUserDirectory()
    {
        using var http = SetupAuthorizedHttp();
        if (http is null)
            return;

        //list the existing roles
        var current_roles = GetKeycloakAppRoles(http).Select(x => x.Value).ToList();

        //find out if new roles should be created
        //(we don't delete roles, since they might be in use, or we might not "own" them)
        foreach (var role in Role.ALL_ROLES)
        {
            if (!current_roles.Contains(role.Key))
            {
                string title = Translations.GetByCode(role.Value.Title);
                if (role.Value.Type == RoleType.LEVEL)
                    title += " (level)";
                CreateKeycloakRole(role.Key, title, http);
            }

            foreach (var m in role.Value.AvailableModules)
            {
                //system roles are always applied globally, they don't need module specific roles
                if (role.Value.Type == RoleType.SYSTEM)
                    continue;

                var roleid = $"{role.Key}@{m}";
                if (!current_roles.Contains(roleid))
                {
                    string title = Translations.GetByCode(role.Value.Title) + " in module " + m;
                    if (role.Value.Type == RoleType.LEVEL)
                        title += " (level)";
                    CreateKeycloakRole(roleid, title, http);
                }
            }
        }
    }

    /// <inheritdoc/>
    public override void CreateNewUser(User user, Dictionary<string, object> claims = null, CredentialSecret credential = null)
    {
        using var http = SetupAuthorizedHttp();
        if (http is null)
            return;

        //create the user
        string email = claims.TryGetValue("email", out var emailobj)
            ? emailobj.ToString()
            : "";
        string password = "";
        if (credential is PasswordSecret userpass)
            password = userpass.NewPass;
        string userId = CreateKeycloakUser(user.Name, email, user.Status == 0, password, http);

        //fetch the ids of the current available application roles
        var app_roles = GetKeycloakAppRoles(http);

        //add the roles to the user
        List<KeyValuePair<string, string>> user_roles = [];
        foreach (var moduleRoles in user.RolesPerModule)
            foreach(var role in moduleRoles.Value)
            {
                var roleName = $"{role}@{moduleRoles.Key}";
                var roleIx = app_roles.FindIndex(x => x.Value == roleName);
                if (roleIx != -1)
                    user_roles.Add(app_roles[roleIx]);
            }
        AddKeycloakUserRoles(userId, user_roles, http);
    }

    /// <inheritdoc/>
    public override void SetUserEnabled(User user, int status)
    {
        using var http = SetupAuthorizedHttp();
        if (http is null)
            return;

        SetKeycloakUserEnabled(userId: user.Codpsw,
            userEnabled: user.Status == 0, 
            emailVerified: user.Status == 0 || user.Status == 1,
            http: http);
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;

using CSGenio.business;
using CSGenio.core.di;
using CSGenio.core.persistence;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Helpers;
using GenioMVC.Helpers.Menus;
using GenioMVC.Models;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;
using GenioServer.security;

namespace GenioMVC.Controllers
{
	public class AccountController(IUserContextService userContextService) : ControllerBase(userContextService)
	{
		//
		// GET: /Account/LogOn
// USE /[MANUAL FOR CUSTOM_LOGON_GET]/
		[HttpGet]
		[AllowAnonymous]
		public ActionResult LogOn()
		{
			LogOnModel model = new();

			foreach(var ip in SecurityFactory.IdentityProviderList)
			{
				//2FA methods are only available after primary auth is completed
				//and only 1 of them will be chosen depending on each user account.
				if(ip.Is2FA) continue;

				//The primary methods declare their credential input type
				//that will determine what kind on UI's will be necessary.
				AuthRedirectMethodModel method = new() {
					Id = ip.Id,
					Description = ip.Description,
					CredentialType = SecurityFactory.GetCredentialType(ip),
					Redirect = ip.GetRedirectLoginUrl(AuthRedirectMethodModel.MapRedirectEndpoint(ip, Url, Request))
				};

				model.AuthRedirectMethods.Add(method);
			}
			model.AuthMode = SecurityFactory.AuthenticationMode;

			return JsonOK(model);
		}

		/// <summary>
		/// Validates the provided model and adds any validation errors to the ModelState.
		/// </summary>
		/// <param name="model">The model to be validated.</param>
		/// <param name="userContext">The userContext.</param>
		public void ValidateModel(IValidatable model, UserContext userContext)
		{
			var validationResult = model.Validate(userContext);

			foreach (var (field, errorMessages) in validationResult.ModelErrors)
				foreach (var errorMessage in errorMessages)
					ModelState.AddModelError(field, errorMessage);
		}

		//
		// POST: /Account/LogOn
		[HttpPost]
		[AllowAnonymous]
// USE /[MANUAL FOR CUSTOM_LOGON_POST]/

		public ActionResult LogOn([FromBody]LogOnModel model, string returnUrl)
		{
			ValidateModel(model, UserContext.Current);
			if (!ModelState.IsValid)
				return JsonERROR();

			return IdentityProviderLoginGeneric(model.ProviderId
				, (ip, token) => new UserPassCredential()
				{
					Username = model.UserName,
					Password = model.Password
				}
				, returnUrl
				, isCallback: false);
		}

		private ActionResult request2FAuthentication(string returnUrl, User user)
		{
			//determine the provider id and type to use
			IIdentityProvider provider = null;
			if(user.Auth2FATp == "TOTP")
				provider = SecurityFactory.IdentityProviderList.First(x => x.Is2FA && SecurityFactory.GetCredentialType(x) == "UserPassCredential");
			if (user.Auth2FATp == "WebAuth")
				provider = SecurityFactory.IdentityProviderList.First(x => x.Is2FA && SecurityFactory.GetCredentialType(x) == "WebAuthCredential");

			//generate the challenge for this provider
			if (provider is null)
				return JsonERROR();
			var challenge = provider.AuthenticateChallenge(user.Name);

			//create a token that will assert the success of the primary authentication
			var token = new AuthStateToken()
			{
				SessionId = HttpContext.Session.Id,
				Username = user.Name,
				Challenge = challenge,
				Timestamp = DateTime.UtcNow
			};
			var state = JsonSerializer.Serialize(token);
			CreateStateCookie("Challenge", state);

			return Json(new
			{
				Success = true,
				Auth2FA = true,
				Auth2FATp = user.Auth2FATp,
				ProviderId = provider.Id,
				Challenge = challenge,
				Username = user.Name,
				Redirect = returnUrl
			});
		}

		/// <summary>
		/// Asserts that the user has already done a sucessful primary authentication
		/// </summary>
		/// <remarks>Use this in 2FA to validate the request and to carry the challenge payload when needed</remarks>
		public class AuthStateToken
		{
			public string SessionId { get; set; }
			public DateTime Timestamp { get; set; }
			public string Username { get; set; }
			public string Challenge { get; set; }
		}

		private string ValidateLoginState(User? user, AuthStateToken? token)
		{
			if (user is null)
				return Resources.Resources.ENTRADA_INCORRETA__T45717;

			if (token is not null)
			{
				if (token.SessionId != HttpContext.Session.Id
					|| token.Username != user.Name
					|| (token.Timestamp - DateTime.UtcNow).TotalMinutes > 10)
					return Resources.Resources.ESTADO_DE_AUTENTICAC50027;
			}

			if (user.Status == 2)
			{
				string errorMessage = Resources.Resources.ESTE_UTILIZADOR_ENCO01685;
				Log.Error($"{errorMessage}. User: {user.Name}");
				return errorMessage;
			}

			bool isConfigurationValid = DatabaseVersionReader.IsConfigurationUpToDate();
			if (!isConfigurationValid)
			{
				string errorMessage = Resources.Resources.E_NECESSARIO_PROCEDE36325;
				Log.Error($"{errorMessage}. Found: {Configuration.GetDbVersion(user.Year)}, Expected: {Configuration.VersionDbGen}");
				return errorMessage;
			}

			bool isValidDb = DatabaseVersionReader.IsDatabaseUpToDate(user);
			if (!isValidDb)
			{
				string errorMessage = Resources.Resources.E_NECESSARIO_ATUALIZ49371;
				Log.Error($"{errorMessage}. Found: {Configuration.GetDbVersion(user.Year)}, Expected: {Configuration.VersionDbGen}");
				return errorMessage;
			}

			return null;
		}


		public class WebAuthLoginRequest
		{
			public string ProviderId { get; set; }
			public string Assertion { get; set; }
			public string Username { get; set; }
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult AuthenticationWebauth([FromBody] WebAuthLoginRequest model, string returnUrl)
		{
			return IdentityProviderLoginGeneric(model.ProviderId,
				(ip, token) => new WebAuthCredential()
				{
					Assertion = model.Assertion,
					Username = model.Username,
					OriginalOptions = token.Challenge
				},
				returnUrl,
				isCallback: false);
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult AuthenticationTotp([FromBody] LogOnModel model, string returnUrl)
		{
			return IdentityProviderLoginGeneric(model.ProviderId,
				(ip, token) => new UserPassCredential()
				{
					Username = model.UserName,
					Password = model.Password
				},
				returnUrl,
				isCallback: false);
		}

		/// <summary>
		/// Sends the email for password recovery
		/// </summary>
		/// <remarks>TODO: Add Captcha to the RecoverPassword interface (Vue.js)</remarks>
		[HttpPost]
		[AllowAnonymous]
		public ActionResult RecoverPassword([FromBody]PasswordRecoverViewModel model)
		{
			try
			{
				ValidateModel(model, UserContext.Current);

				if (!ModelState.IsValid)
					return JsonERROR();

				var captchaData = model.CaptchaData;
				bool isValidCaptcha = QCaptcha.Validate(captchaData.UserEnteredCaptchaCode, captchaData.CaptchaId, HttpContext.Session);
				QCaptcha.SetCaptcha(captchaData.CaptchaId, null, HttpContext.Session);
				if (!isValidCaptcha)
				{
					ModelState.AddModelError("userEnteredCaptchaCode", Resources.Resources.INVALID_CAPTCHA29660);
					return JsonERROR(Resources.Resources.INVALID_CAPTCHA29660);
				}

				User user = SecurityFactory.Authorize(new()
				{
					Name = model.Email,
					AuthenticationType = "RecoverPassword",
					IsAuthenticated = false,
					IdProperty = GenioIdentityType.Email
				});
				user.Location = HttpContext.GetHostName();

				string emailBody = "";
				string appName = Configuration.Application.Name;
				// TODO: this should be obtained directly from user that already has its language filled by Usercontext
				string lang = RouteData.Values["culture"]?.ToString() ?? "";

				if (user != null)
				{
					ResourceUser rec = new(user.Name, user.Codpsw);
					var ticket = QResources.CreateTicketEncryptedBase64(user.Name, user.Location, rec);

					string userName = user.Name;
					string? urlToken = Url.Action("RecoverPasswordChange", "Account", new { ticket }, Request.Scheme);

					emailBody = UserRegistration.GetEmailForLanguage("PasswordChangeEmail", lang);
					emailBody = string.Format(emailBody, appName, userName, urlToken);
				}
				else
				{
					emailBody = UserRegistration.GetEmailForLanguage("InvalidEmailTemplate", lang);
					string? baseUrl = Url.Action("LogOn", "Account", null, Request.Scheme);
					emailBody = string.Format(emailBody, appName, baseUrl);
				}

				UserFactory userFactory = new(null, m_userContext.User);
				userFactory.SendPasswordRecoveryMail(model.Email, emailBody);
				model.IsEmailSent = true;
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
				return JsonERROR(HandleException(e));
			}

			return JsonOK(model);
		}

		/// <summary>
		/// Receives a ticket, validates it and shows the view to change password
		/// </summary>
		[AllowAnonymous]
		public ActionResult RecoverPasswordChange(string ticket)
		{
			try
			{
				var ticketContent = QResources.DecryptTicketBase64(ticket);
				ResourceUser resource = ticketContent[2] as ResourceUser;

				//Check if ticket expired
				if (GenFunctions.DateDiffPart(resource.CreationDate, DateTime.UtcNow, "M") < 60)
				{
					//Store the id in session for later use
					HttpContext.Session.SetString("userId", resource.Name);
					return RedirectToVuePage("RecoverPasswordChange", includeCulture: true, includeSystemAndModule: true);
				}

				return RedirectToVuePage("ErrorTicketConfirm");
			}
			catch
			{
				return RedirectToVuePage("ErrorTicketConfirm");
			}
		}

		/// <summary>
		/// Persist the password change
		/// </summary>
		[HttpPost]
		[AllowAnonymous]
		public ActionResult RecoverPasswordChange([FromBody]PasswordRecoverChangeModel model)
		{
			ValidateModel(model, UserContext.Current);
			if (!ModelState.IsValid)
				return JsonERROR();

			try
			{
				User u = UserContext.Current.User;
				PersistentSupport sp = PersistentSupport.getPersistentSupport(u.Year, u.Name);
				var userFactory = new UserFactory(sp, u);
				//Get the user id from the session
				string userId = HttpContext.Session.GetString("userId");

				var user = userFactory.GetUser(userId);
				Password password = new(model.NewPassword, model.ConfirmPassword);

				string encOldPass = user.ValPassword;
				// checks if the new password is equal to the old one, if yes, return an error
				var isSamePassword = PasswordFactory.IsOK(password.New, encOldPass, user.ValSalt, user.ValPswtype);
				if (isSamePassword)
				{
					ModelState.AddModelError("error", Resources.Resources.A_NOVA_PALAVRA_PASSE58485);
					return JsonERROR();
				}

				//Change password
				userFactory.ChangePassword(user, model.NewPassword, model.ConfirmPassword);
				user.UserRecord = false;

				try
				{
					sp.openTransaction();

					user.User.Name = "PasswordRecovery";
					user.fillStampChange();
					user.update(sp);

					sp.closeTransaction();
				}
				catch
				{
					sp.rollbackTransaction();
					throw;
				}

				//Cleanup
				HttpContext.Session.Remove("userId");
				return JsonOK();
			}
			catch (InvalidPasswordException e)
			{
				Log.Error(e.Message);
				return JsonERROR(HandleException(e));
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
				return JsonERROR(HandleException(e));
			}
		}

		[HttpGet]
		public ActionResult NewCredentialRequest(string providerId)
		{
			try
			{
				var options = SecurityFactory.NewCredentialRequest(providerId, m_userContext.User.Name);
				CreateStateCookie("Challenge", options);
				return Json(new { Success = true, options = options });
			}
			catch (Exception e)
			{
				return JsonERROR(HandleException(e));
			}
		}

		public class StoreCredentialRequest
		{
			public string ProviderId { get; set; }
			public string Credential { get; set; }
		}

		[HttpPost]
		public ActionResult StoreCredential([FromBody] StoreCredentialRequest request)
		{
			try
			{
				string originalChallenge = ConsumeStateCookie("Challenge");
				SecurityFactory.StoreCredential(request.ProviderId, m_userContext.User, originalChallenge, request.Credential);
				return Json(new { Success = true, Message = "Registration Successful!" });
			}
			catch (Exception e)
			{
				return JsonERROR(HandleException(e));
			}
		}

		private ActionResult IdentityProviderLoginGeneric(string providerId, Func<IIdentityProvider, AuthStateToken, Credential> createCredential, string returnUrl = "", bool isCallback = true)
		{
			try
			{
				//check for the existence of a previous 2FA challenge and validate it against this auth request
				var state = ConsumeStateCookie("Challenge");
				var token = string.IsNullOrEmpty(state) ? null : JsonSerializer.Deserialize<AuthStateToken>(state);

				//find the identity provide and collect the credentials from the interface
				var ip = SecurityFactory.IdentityProviderList.First(i => i.Id == providerId);

				Credential credential = createCredential(ip, token);
				User? user = null;

				if (
					ip.HasUsernameAuth() &&
					SecurityFactory.AuthenticationMode.Equals(AuthenticationMode.AcceptOnFirstSucess)
				)
				{
					user = SecurityFactory.Authenticate(credential);
				}
				else
				{
					user = SecurityFactory.Authenticate(credential, providerId);
				}

				//validate the authentication state
				string loginError = ValidateLoginState(user, token);
				if (!string.IsNullOrEmpty(loginError))
				{
					// Increment invalid login counter
					GenioDI.MetricsOtlp.IncrementCounter("login_counter", 1, new List<KeyValuePair<string, object>>()
					{
						new("Ip", HttpContext.GetHostName()),
						new("Failed", true)
					});
					throw new BusinessException(loginError, "IdentityProviderLoginGeneric", loginError);
				}

				//set the new authentication state and direct the user to the adequate page
				var reply = finalizeAuthentication(user, returnUrl, token);
				return isCallback
					? RedirectToVuePage("")
					: reply;
			}
			catch (Exception ex)
			{
				HandleException(ex);
				// TODO: When an authentication error occurs, return to Logon page and present the user with a perceptible error message.
				return isCallback
					? RedirectToVuePage("Error", null, false)
					: JsonERROR();
			}
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult OpenIdConnectLogin([FromRoute] string providerId, [FromForm] string code, [FromForm] string id_token)
		{
			return IdentityProviderLoginGeneric(providerId, (ip, token) => new TokenCredential()
			{
				Auth = code,
				Token = id_token,
				OriginUrl = AuthRedirectMethodModel.MapRedirectEndpoint(ip, Url, Request)
			});
		}

		private ActionResult IdentityProviderRegisterGeneric(string providerId, Func<IIdentityProvider, Credential> createCredential)
		{
			try
			{
				var ip = SecurityFactory.IdentityProviderList.First(i => i.Id == providerId);
				var credential = createCredential(ip);

				var identity = ip.Authenticate(credential);
				SecurityFactory.RegisterExternalId(UserContext.Current.User, identity);
				return RedirectToVuePage("Profile");
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message);
			}

			// TODO: When an authentication error occurs, return to Logon page and present the user with a perceptible error message.
			return RedirectToVuePage("");
		}

		[HttpGet]
		[ActionName("OpenIdConnectRegister")]
		public ActionResult OpenIdConnectRegister_Get([FromRoute] string providerId, [FromQuery] string code, [FromQuery] string id_token, [FromQuery] string state)
		{
			return IdentityProviderRegisterGeneric(providerId, (ip) => new TokenCredential()
			{
				Auth = code,
				Token = id_token,
				OriginUrl = AuthRedirectMethodModel.MapRedirectEndpoint(ip, Url, Request, "Register")
			});
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult OpenIdConnectRegister([FromRoute] string providerId, [FromForm] string code, [FromForm] string id_token, [FromForm] string state)
		{
			//reflects the request back to the server through a browser initiated Get
			//this is necessary because an external request will not bring the authentication cookies of the person registering because its considered cross-site
			//However if we do the registration part in a subsequent request then the cookies are sent and we can use them to get the user state.
			//This has the advantage of not needing any internal state being sent and maintained by the external provider.
			string endpoint = Url.Action("OpenIdConnectRegister", "Account", new { providerId, code, id_token, state });
			return ClientSideRedirect(endpoint);
		}

		/// <summary>
		/// After user have authenticated on Governement CMD identity provider will callback to our application to that funcion.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult CMDLogin([FromRoute] string providerId)
		{
			//Government CMD sends the results in the Url hash (instead of the standard that sends them in the url query)
			//The server will not receive them in this method call, so we need to render a html page that captures them in javascript and sends them back as url query
			string endpoint = Url.Action("CMDLoginParams", "Account", new { providerId });
			return ClientSideRedirect(endpoint, captureHash: true);
		}

		[HttpGet]
		[AllowAnonymous]
		public ActionResult CMDLoginParams([FromRoute] string providerId, string access_token, string token_type, string expires_in)
		{
			return IdentityProviderLoginGeneric(providerId, (ip, token) => new TokenCredential()
			{
				Token = access_token,
			});
		}

		[HttpGet]
		public ActionResult CMDRegisterParams([FromRoute] string providerId, string access_token, string token_type, string expires_in)
		{
			return IdentityProviderRegisterGeneric(providerId, (ip) => new TokenCredential()
			{
				Token = access_token
			});
		}

		[HttpGet]
		[AllowAnonymous]
		public ActionResult CMDRegister([FromRoute] string providerId)
		{
			//reflects the request back to the server through a browser initiated Get
			//this is necessary because an external request will not bring the authentication cookies of the person registering because its considered cross-site
			//However if we do the registration part in a subsequent request then the cookies are sent and we can use them to get the user state.
			//This has the advantage of not needing any internal state being sent and maintained by the external provider.
			string endpoint = Url.Action("CMDRegisterParams", "Account", new { providerId });
			return ClientSideRedirect(endpoint, captureHash: true);
		}

		[HttpGet]
		[AllowAnonymous]
		public ActionResult CASLogin([FromRoute] string providerId, string ticket)
		{
			return IdentityProviderLoginGeneric(providerId, (ip, token) => new TokenCredential()
			{
				Token = ticket,
				OriginUrl = AuthRedirectMethodModel.MapRedirectEndpoint(ip, Url, Request)
			});
		}

		private ActionResult finalizeAuthentication(User user, string returnUrl, AuthStateToken token)
		{
			if (user == null)
				return Json(new { Success = false, Message = Resources.Resources.DADOS_DE_LOGIN_INCOR44791 });

			//If the user reqires 2FA, and we are still in our primary authentication, redirect the user to his 2F authentication
			if (user.Auth2FA && token is null)
				return request2FAuthentication(returnUrl, user);

			user = UserFactory.ReadEphs(user);
			user.SessionId = HttpContext.Session.Id;
			user.Location = HttpContext.GetHostName();
			UserContext.Current.User = user;

			//set the authentication cookie for this user
			var claimsIdentity = new ClaimsIdentity(new List<System.Security.Claims.Claim>
			{
				new(ClaimTypes.Name, user.Name)
			}, LegacyFormsAuthenticationOptions.DefaultScheme);

			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			HttpContext.SignInAsync(claimsPrincipal).Wait();

			// log login (audit)
			CSGenio.framework.Audit.registLoginOut(user, Resources.Resources.ENTRADA31905, Resources.Resources.ENTRADA_ATRAVES_DA_P48446, HttpContext.GetHostName(), HttpContext.GetIpAddress());
			GlobalAppSessions.Instance.AddOrUpdate(HttpContext.Session.Id, user.Name, HttpContext.GetHostName());
			GenioDI.MetricsOtlp.IncrementCounter("login_counter", 1, new List<KeyValuePair<string, object>>() {
				new("Ip", user.Location),
				new("Failed", false)
			});

			//if 2FA is mandatory to the system then force the user into his profile page
			if (GenFunctions.emptyN(user.Status) == 0 && user.Status == 1 || (Configuration.Security.Mandatory2FA && !user.Auth2FA))
				returnUrl = Url.Action("Profile", "Home");
			//if the redirect is the root or invalid, direct the user to the home page
			else if (!Url.IsLocalUrl(returnUrl) || returnUrl.Length <= 1 || !returnUrl.StartsWith("/") || returnUrl.StartsWith("//") || returnUrl.StartsWith("/\\"))
				returnUrl = Url.Action("Index", "Home");

			return Json(new { Success = true, Redirect = returnUrl });
		}

		//
		// GET: /Account/LogOff
		[HttpPost]
		public ActionResult LogOff()
		{
			DestroySession();
			return JsonOK();
		}

		[AllowAnonymous]
		public ActionResult RecoverPassword()
		{
			return JsonOK(new PasswordRecoverViewModel());
		}

		private User AuthenticateUser(BasicUserModel model, string year)
		{
			try
			{
				var principal = SecurityFactory.Authenticate(new UserPassCredential() { Username = model.UserName, Password = model.Password, Year = year });
				if (principal == null)
				{
					CSGenio.framework.Audit.registLoginOut(UserContext.Current.User,
						model.UserName,
						Resources.Resources.TENTATIVA38682,
						Resources.Resources.LOGIN_OU_PASSWORD_IN32183,
						HttpContext.GetHostName(),
						HttpContext.GetIpAddress());

					throw new BusinessException(Resources.Resources.LOGIN_OU_PASSWORD_IN32183, "InterfaceXml.pedidoEXW()", Resources.Resources.LOGIN_OU_PASSWORD_IN32183);
				}

				principal = UserFactory.ReadEphs(principal);
				principal.SessionId = HttpContext.Session.Id;
				principal.Location = HttpContext.GetHostName();
				return principal;
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message);
				return null;
			}
		}

		[HttpGet]
		public ActionResult GetImage()
		{
			User usr = UserContext.Current.User;

			// html for user avatar image
			string dataImage = "";
			string ckey = "userInfo." + usr.Codpsw;

			UserInfo usrInfo = QCache.Instance.User.Get(ckey) as UserInfo;

			if (usrInfo == null)
			{
				usrInfo = UserProfileInfo.getUserImage(UserContext.Current.PersistentSupport, usr);

				if (!usrInfo.IsEmpty())
					QCache.Instance.User.Put(ckey, usrInfo, TimeSpan.FromMinutes(1));
			}

			if (usrInfo.Image != null && usrInfo.Image.Length > 0 )
			{
				dataImage = HtmlHelpers.ImageBase64(usrInfo.Image);
			}
			else
			{
				var avatar = System.IO.File.ReadAllBytes("./Content/img/user_avatar.png");
				dataImage = HtmlHelpers.ImageBase64(avatar);
			}

			return Json(new { Success = true, img = dataImage, fullname = usrInfo.Fullname, position = usrInfo.Position });
		}

		[HttpGet]
		public ActionResult UserAvatar()
		{
			User usr = UserContext.Current.User;

			// base64 image for user avatar image
			string dataImage = "";
			string ckey = "userInfo." + usr.Codpsw;

			UserInfo usrInfo = QCache.Instance.User.Get(ckey) as UserInfo;

			if (usrInfo == null)
			{
				usrInfo = UserProfileInfo.getUserImage(UserContext.Current.PersistentSupport, usr);

				if (!usrInfo.IsEmpty())
					QCache.Instance.User.Put(ckey, usrInfo, TimeSpan.FromMinutes(1));
			}

			if (usrInfo.Image != null && usrInfo.Image.Length > 0)
			{
				string img = HtmlHelpers.ImageBase64(usrInfo.Image);
				if (img != null)
					dataImage = img;
			}

			var usrAvatarMenu = UserAvatarMenu.GetMenus(UserContext.Current.PersistentSupport, UserContext.Current.User);
			var ePHUsrAvatarMenu = EPHUserAvatarMenu.GetMenus(UserContext.Current);
			var avatar = new { image = dataImage, fullname = usrInfo.Fullname, position = usrInfo.Position };

			var has2FAOptions = SecurityFactory.IdentityProviderList.Any(p => p.Is2FA);
			var hasOpenIdAuth = Configuration.Security.IdentityProviders.Exists(ip => ip.Type == "GenioServer.security.OpenIdConnectIdentityProvider");

			return Json(new { Success = true, Avatar = avatar, UserAvatarMenus = usrAvatarMenu, EPHUserAvatarMenus = ePHUsrAvatarMenu, Has2FAOptions = has2FAOptions, HasOpenIdAuth = hasOpenIdAuth });
		}

		// GET: /Account/GetIfUserLogged
		[AllowAnonymous]
		public ActionResult GetIfUserLogged()
		{
			var user = UserContext.Current.User;
			return Json(new { username = user.Name != "guest" ? user.Name : "" });
		}
	}
}

using System;
using System.ComponentModel.DataAnnotations;

using CSGenio.framework;
using GenioMVC.Models.Navigation;
using GenioMVC.ViewModels;
using GenioServer.security;
using Microsoft.AspNetCore.Mvc;

namespace GenioMVC.Models
{
	public interface IValidatable
	{
		CrudViewModelValidationResult Validate(UserContext userContext);
	}

	public abstract class BasicUserModel: IValidatable
	{
		public string UserName { get; set; }

		public string Password { get; set; }

		/// <summary>
		/// Validates form fields
		/// </summary>
		/// <returns></returns>
		public abstract CrudViewModelValidationResult Validate(UserContext userContext);
	}

	public abstract class AChangePasswordModel: IValidatable
	{
		public string NewPassword { get; set; }

		public string ConfirmPassword { get; set; }

		public abstract CrudViewModelValidationResult Validate(UserContext userContext);
	}

	public class PasswordRecoverChangeModel : AChangePasswordModel
	{
		public string UserId { get; set; }

		public override CrudViewModelValidationResult Validate(UserContext userContext)
		{
			CrudViewModelFieldValidator validator = new(userContext.User.Language);

			validator.Required("NewPassword", Resources.Resources.NOVA_PALAVRA_CHAVE09647, NewPassword);
			validator.Password("ConfirmPassword", NewPassword, ConfirmPassword);

			return validator.GetResult();
		}
	}

	public abstract class ChangePasswordModel : AChangePasswordModel
	{
		public string OldPassword { get; set; }

		public string Password { get; set; }

		public bool Enable2FAOptions { get; set; }
	}

	public class ProfileModel : ChangePasswordModel
	{
		/// <summary>
		/// Authentication models that need redirection to external url
		/// </summary>
		public List<AuthRedirectMethodModel> AuthRedirectMethods { get; set; } = [];

		/// <summary>
		/// Current 2FA method associated with this user
		/// </summary>
		public string Current2FA { get; set; }

		public string ValCodpsw { get; set; }

		public string ValNome { get; set; }

		public override CrudViewModelValidationResult Validate(UserContext userContext)
		{
			CrudViewModelFieldValidator validator = new(userContext.User.Language);

			validator.Required("ValNome", Resources.Resources.UTILIZADOR52387, ValNome);
			validator.Required("OldPassword", Resources.Resources.PALAVRA_CHAVE_ACTUAL29965, OldPassword);
			validator.Required("NewPassword", Resources.Resources.NOVA_PALAVRA_CHAVE09647, NewPassword);
			validator.Password("NewPassword", NewPassword, ConfirmPassword);

			return validator.GetResult();
		}

		public void Save(UserContext userContext)
		{
			Models.Psw item = null;

			// Precisamos posicionar a ficha to não "estragar" o Qvalue do zzstate
			try
			{
				item = Models.Psw.Find(userContext.User.Codpsw, userContext, "FPSW");
			}
			finally
			{
				if (item == null)
					item = new Models.Psw(userContext);
			}

			item.ValPasswordDecrypted = NewPassword;
			item.ValStatus = 0;
			item.Save();
		}

		public void Apply(UserContext userContext)
		{
			Models.Psw item = null;

			// Precisamos posicionar a ficha to não "estragar" o Qvalue do zzstate
			try
			{
				item = Models.Psw.Find(userContext.User.Codpsw, userContext,"FPSW");
			}
			finally
			{
				if (item == null)
					item = new Models.Psw(userContext);
			}

			item.ValPasswordDecrypted = NewPassword;
			item.ValStatus = 0;
			item.Apply();
		}
	}


	public class AuthRedirectMethodModel
	{
		/// <summary>
		/// Unique identifier for this provider instance
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// A human readable title for this provider instance
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Type of credential interface requested by this provider
		/// </summary>
		public string CredentialType { get; set; }
		/// <summary>
		/// The external uri to redirect the user during login with this provider
		/// </summary>
		public string Redirect { get; set; }

		public static string MapRedirectEndpoint(IIdentityProvider provider, IUrlHelper url, HttpRequest request, string operation = "Login")
		{
			string actionname = "OpenIdConnect";
			if (provider is CASIdentityProvider)
				actionname = "CAS";
			else if (provider is CMDIdentityProvider)
				actionname = "CMD";
			else if (provider is OpenIdConnectIdentityProvider)
				actionname = "OpenIdConnect";
			actionname += operation;

			string relative = url.RouteUrl("authRedirectRoute", new {
				providerId = provider.Id,
				action = actionname
			});
			return AbsoluteUrlUtils.RelativeToAbsolute(request, relative);
		}
	}

	public class LogOnModel : BasicUserModel
	{
		/// <summary>
		/// Provider we are using to authenticate with
		/// </summary>
		public string ProviderId { get; set; }

		/// <summary>
		/// Authentication models that need authentication
		/// </summary>
		public List<AuthRedirectMethodModel> AuthRedirectMethods { get; set; } = [];

		/// <summary>
		/// Checks if the application is setup to allow password recovery
		/// </summary>
		/// <returns></returns>
		public bool HasPasswordRecovery
		{
			get { return SecurityFactory.HasPasswordManagement() && !string.IsNullOrEmpty(Configuration.PasswordRecoveryEmail); }
		}

		/// <summary>
		/// Authentication mode
		/// </summary>
		public AuthenticationMode AuthMode { get; set; }

		public override CrudViewModelValidationResult Validate(UserContext userContext)
		{
			CrudViewModelFieldValidator validator = new(userContext.User.Language);

			validator.Required("UserName", Resources.Resources.UTILIZADOR52387, UserName);
			validator.Required("Password", Resources.Resources.PALAVRA_CHAVE39832, Password);

			return validator.GetResult();
		}
	}

	public class VueCaptchaModel
	{
		public string CaptchaId { get; set; }

		public string UserEnteredCaptchaCode { get; set; }
	}

	public class PasswordRecoverViewModel: IValidatable
	{
		public string Email { get; set; }

		public bool IsEmailSent { get; set; }

		public VueCaptchaModel CaptchaData { get; set; }

		public CrudViewModelValidationResult Validate(UserContext userContext)
		{
			CrudViewModelFieldValidator validator = new(userContext.User.Language);

			validator.Required("Email", Resources.Resources.EMAIL25170, Email);
			validator.Email("Email", Email);

			return validator.GetResult();
		}
	}
}

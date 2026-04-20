namespace GenioMVC.ViewModels;

/// <summary>
/// Model for the 2FA
/// </summary>
public class TwoFAViewModel
{
	public string User2FATp { get; set; }
	public List<Models.AuthRedirectMethodModel> Providers { get; set; } = [];
}

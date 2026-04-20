using GenioMVC.Models.Navigation;

namespace GenioMVC;

public interface IUserContextService
{
    UserContext Current  {
        get;
    }
}

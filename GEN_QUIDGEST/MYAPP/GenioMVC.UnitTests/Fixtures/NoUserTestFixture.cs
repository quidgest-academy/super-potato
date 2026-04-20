using GenioMVC;
using GenioMVC.Models.Navigation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace GenioMVC.UnitTests
{
    /// <summary>
    /// This fixture can be used when you need to call constructors that don't require information from the user context
    /// </summary>
    [TestFixture]
    public class NoUserTestFixture
    {
        protected IUserContextService _userContextService;

        [SetUp]
        public void Setup()
        {
            Mock<IUserContextService> mockUserContext = new Mock<IUserContextService>();
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<ISession> mockSession = new Mock<ISession>();

            var httpContext = new DefaultHttpContext();
            httpContext.Session = mockSession.Object;
            var userContext = new UserContext(httpContext, mockConfig.Object);

            mockUserContext.SetupGet(userContextService => userContextService.Current).Returns(userContext);

            _userContextService = mockUserContext.Object;
        }


    }
}

using CSGenio.business.async;

namespace DbAdmin.IntegrationTest
{
    /// <summary>
    /// Class is necessary to be able to register classes outside GenioServer assembly
    /// </summary>
    public class TestJobFinder : JobFinder
    {
        private Dictionary<string, Type> registeredTypes = new Dictionary<string, Type>();

        public void RegisterType(Type type) 
        { 
            AddType(registeredTypes, type);
        }

        protected override Dictionary<string, Type> GetJobTypes()
        {
            return registeredTypes;
        }
    }

}

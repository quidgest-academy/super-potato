using System.ServiceModel;

namespace Administration.Models
{
    [ServiceContract]
    public interface IUserManagementService
    {
        [OperationContract]
        public List<ModulesLevel> GetPermissions();

        [OperationContract]
        public bool CreateUserWithPassAndLevels(string username, string password, List<ModulesLevel> levels);

        [OperationContract]
        public bool CreateUserWithPass(string username, string password);

        [OperationContract]
        public bool CreateUserWithLevels(string username, List<ModulesLevel> levels);

        [OperationContract]
        public bool CreateUser(string username);

        [OperationContract]
        public bool DeleteUser(string username);

        [OperationContract]
        public string Test(string s);
    }
}
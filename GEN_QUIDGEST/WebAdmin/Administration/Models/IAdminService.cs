using System.ServiceModel;
using CSGenio.business;

namespace Administration.Models
{
   [ServiceContract]
   public interface IAdminService
   {
      [OperationContract]
      public QApiCallAck QApi(List<Administration.AuxClass.KeyValuePair<string, string>> args);

      [OperationContract]
      public void ProcessMessage(string queueName, string year, string message);

      [OperationContract]
      public List<GlobalFunctions.FunctionInformation> GetAllSchedulerFuncs();

      [OperationContract]
      public string[] GetAllMessages(string queueName, string year);

      [OperationContract]
      public string GetOneMessage(string queueName, string year);

      [OperationContract]
      public QApiCallAck WebAdminApi(List<Administration.AuxClass.KeyValuePair<string, string>> args);

      [OperationContract]
      public QApiCallAck Maintenance(List<Administration.AuxClass.KeyValuePair<string, string>> args);
   }
}
using Administration.Models;
using CSGenio.business;
using Microsoft.AspNetCore.Mvc;

namespace Administration.Controllers
{
    /// <summary>
    /// DTO for getting all messages (queues)
    /// </summary>
    public class GetAllMessagesDTO
    {
        /// <summary>
        /// The name of the queue.
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// The database year.
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// Validates the DTO data.
        /// </summary>
        /// <returns>True if the DTO is valid, otherwise false.</returns>
        public virtual bool IsValid()
        {
            return !string.IsNullOrEmpty(QueueName) && !string.IsNullOrEmpty(Year);
        }
    }

    /// <summary>
    /// DTO for processing a message (queue).
    /// </summary>
    public class ProcessMessageDTO : GetAllMessagesDTO
    {
        /// <summary>
        /// The content of the message to be processed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Validates the DTO data including the message.
        /// </summary>
        /// <returns>True if the DTO is valid, otherwise false.</returns>
        public override bool IsValid()
        {
            return base.IsValid() && !string.IsNullOrEmpty(Message);
        }
    }

    /// <summary>
    /// Controller for admin-related API endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RestAdminController : Microsoft.AspNetCore.Mvc.ControllerBase
    {        
        private readonly IAdminService _adminService;
        public RestAdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Health check endpoint to verify if the API is alive.
        /// </summary>
        /// <returns>True if the API is running.</returns>
        [HttpGet("IsAlive")]
        [Produces("application/json")]
        public IActionResult IsAlive()
        {
            return Ok(true);
        }

        /// <summary>
        /// Executes a QApi call with the provided key-value pairs.
        /// </summary>
        /// <param name="requestData">List of key-value pairs as input.</param>
        /// <returns>A response indicating the result of the QApi call.</returns>
        [HttpPost("QApi")]
        public ActionResult<QApiCallAck> QApi(List<AuxClass.KeyValuePair<string, string>> requestData)
        {
            if (requestData == null)
                return BadRequest(requestData);
            
            return Ok(_adminService.QApi(requestData));
        }

        /// <summary>
        /// Retrieves all scheduler functions available.
        /// </summary>
        /// <returns>A list of scheduler functions.</returns>
        [HttpGet("GetAllSchedulerFuncs")]
        public ActionResult<IEnumerable<GlobalFunctions.FunctionInformation>> GetAllSchedulerFuncs()
        {
            return Ok(_adminService.GetAllSchedulerFuncs());
        }

        /// <summary>
        /// Processes a queue message.
        /// </summary>
        /// <param name="requestData">The data needed to process a message, including queue name, year, and message content.</param>
        /// <returns>A confirmation of the message being processed.</returns>
        [HttpPost("ProcessMessage")]        
        public IActionResult ProcessMessage([FromBody] ProcessMessageDTO requestData)
        {
            if(requestData == null || !requestData.IsValid())
                return BadRequest(requestData);

            _adminService.ProcessMessage(requestData.QueueName,requestData.Year,requestData.Message);
            return Ok();
        }

        /// <summary>
        /// Retrieves all messages from a specific queue.
        /// </summary>
        /// <param name="requestData">The data needed to retrieve messages, including queue name and year.</param>
        /// <returns>A list of queue messages.</returns>
        [HttpGet("GetAllMessages")]
        public ActionResult<IEnumerable<string>> GetAllMessages(GetAllMessagesDTO requestData)
        {
            if (requestData == null || !requestData.IsValid())
                return BadRequest(requestData);

            return Ok(_adminService.GetAllMessages(requestData.QueueName, requestData.Year));
        }

        /// <summary>
        /// Retrieves a single queue message.
        /// </summary>
        /// <param name="requestData">The data needed to retrieve one message, including queue name and year.</param>
        /// <returns>A single queue message.</returns>
        [HttpGet("GetOneMessage")]
        public ActionResult<string> GetOneMessage(GetAllMessagesDTO requestData)
        {
            if (requestData == null || !requestData.IsValid())
                return BadRequest(requestData);

            return Ok(_adminService.GetOneMessage(requestData.QueueName, requestData.Year));
        }

        /// <summary>
        /// Executes WebAdminAPI call.
        /// </summary>
        /// <param name="requestData">List of key-value pairs as input.</param>
        /// <returns>A response indicating the result of the WebAdmin API call.</returns>
        [HttpPost("WebAdminApi")]
        public ActionResult<QApiCallAck> WebAdminApi(List<AuxClass.KeyValuePair<string, string>> requestData)
        {            
            if(requestData == null)
                return BadRequest(requestData);

            return Ok(_adminService.WebAdminApi(requestData));
        }

        /// <summary>
        /// Executes a maintenance API call.
        /// </summary>
        /// <param name="requestData">List of key-value pairs as input.</param>
        /// <returns>A response indicating the result of the maintenance API call.</returns>
        [HttpPost("Maintenance")]
        public ActionResult<QApiCallAck> Maintenance(List<AuxClass.KeyValuePair<string, string>> requestData)
        {
            if (requestData == null)
                return BadRequest(requestData);

            return Ok(_adminService.Maintenance(requestData));
        }


// USE /[MANUAL FOR ADMIN_API_ENDPOINTS]/

    }
}

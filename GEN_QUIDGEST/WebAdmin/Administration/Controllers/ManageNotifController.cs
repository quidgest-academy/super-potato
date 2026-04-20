using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Administration.Controllers
{
    public class ManageNotifController : ControllerBase
    {
        //
        // GET: /ManageNotif/

        public IActionResult Index()
        {
            return Ok();
        }

    }
}

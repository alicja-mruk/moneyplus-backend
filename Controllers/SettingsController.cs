using AlicjowyBackendv3.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlicjowyBackendv3.Controllers
{
    public class SettingsController : Controller
    {
        [Route("/api/settings")]
        //[Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return StatusCode(501, new ResponseMessageStatus { StatusCode = "501", Message = "Jeszcze ni mo, jak bedzie to bedzie. Bądź cierpliwa" });
        }
    }
}

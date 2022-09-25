using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlicjowyBackendv3.Controllers
{
    public class CategoriesController : Controller
    {
        [Route("/api/categories")]
        [HttpGet]
        //[Authorize]
        public IActionResult Index()
        {
            return StatusCode(501, new ResponseMessageStatus { StatusCode = "501", Message = "Jeszcze ni mo, jak bedzie to bedzie. Bądź cierpliwa" });
        }
    }
}

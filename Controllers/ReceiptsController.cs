using Microsoft.AspNetCore.Mvc;

namespace AlicjowyBackendv3.Controllers
{
    public class ReceiptsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

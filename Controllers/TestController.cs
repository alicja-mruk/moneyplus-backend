using Microsoft.AspNetCore.Mvc;

namespace AlicjowyBackendv3.Controllers
{
    public class TestController : Controller
    {
        [Route("/api/test")]
        public IActionResult TestWindow()
        {
            return View();
        }
    }
}

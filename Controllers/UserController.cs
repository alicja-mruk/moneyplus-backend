using AlicjowyBackendv3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;
using Npgsql;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AlicjowyBackendv3.Controllers
{
    public class UserController : Controller
    {
        private readonly moneyplus_dbContext _context;

        public UserController(moneyplus_dbContext context)
        {
            _context = context;
        }

        [Route("api/profile")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<User>> GET()
        {
            var guid = User.FindFirstValue("user guid");
            var user = _context.Users.SingleOrDefault(c => c.id == guid);
            return user;
        }
    }
}

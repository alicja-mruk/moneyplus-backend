using AlicjowyBackendv3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;
using Npgsql;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AlicjowyBackendv3.Helpers;

namespace AlicjowyBackendv3.Controllers
{
    public class UserController : Controller
    {
        private readonly moneyplus_dbContext _context;

        public UserController(moneyplus_dbContext context)
        {
            _context = context;
        }

        /// <response code="200">Returns all informations about user</response>
        /// <response code="401">Access denied. This error means the token was invalid or token expired</response>
        [Route("api/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessageStatus), StatusCodes.Status401Unauthorized)]
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

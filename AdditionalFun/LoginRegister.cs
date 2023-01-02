using AlicjowyBackendv3.Helpers;
using AlicjowyBackendv3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AlicjowyBackendv3.AdditionalFun
{
    public class LoginRegister : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly moneyplus_dbContext _context;

        public LoginRegister(IConfiguration config, moneyplus_dbContext context)
        {
            _configuration = config;
            _context = context;
        }

        #region LoginRegister
        /// <response code="200">User has been succesfully logged in and in response he get a pair of tokens</response>
        /// <response code="400">Access denied. Username or password was incorrect</response>
        [Route("/api/login")]
        [HttpPost]
        [ProducesResponseType(typeof(Tokens), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessageStatus), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> Login([FromBody] UserDataTransferObject request)
        {
            User user = _context.Users.Where(c => c.email == request.Email.ToLower()).SingleOrDefault();

            if (user == null)
            {
                return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Username or password is incorrect" });
            }

            bool passwordCorrect = VerifyPasswordHash(request.Password, Convert.FromBase64String(user.passwordHash), Convert.FromBase64String(user.passwordSalt));

            if (user.email == request.Email.ToLower() && passwordCorrect)
            {
                Tokens token = new Tokens();
                token.accessToken = CreateToken(user);
                RefreshToken fulltoken = GenerateRefreshToken(user);
                token.refreshToken = fulltoken.Token;
                user.refreshToken = fulltoken.Token;
                user.tokenExpires = fulltoken.Expires;
                await _context.SaveChangesAsync();
                return Ok(token);
            }
            else
                return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Username or password is incorrect" });
        }

        /// <response code="201">Returns a message that the user was created successfuly</response>
        /// <response code="409">Conflict has been detected beacause the user with the same email address already exists</response>
        [Route("/api/register")]
        [HttpPost]
        [ProducesResponseType(typeof(ResponseMessageStatus), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseMessageStatus), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<User>> Register([FromBody] RegisterDataTransferObject request)
        {
            User user = new User();
            CreatePasswordHash(request.password, out byte[] passwordHash, out byte[] passwordSalt);

            user.id = Guid.NewGuid().ToString();
            user.firstName = request.firstName;
            user.lastName = request.lastName;
            user.passwordHash = Convert.ToBase64String(passwordHash);
            user.passwordSalt = Convert.ToBase64String(passwordSalt);
            user.age = Convert.ToInt16(request.age);
            user.email = request.email.ToLower();

            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (/*NpgsqlException*/DbUpdateException ex)
            {
                if (ex.InnerException is NpgsqlException sqlex)
                    if (sqlex.SqlState.Equals("23505"))
                        return Conflict(new ResponseMessageStatus { StatusCode = "409", Message = "User with this email address already exists" });
            }
            return Created(string.Empty, new ResponseMessageStatus { StatusCode = "201", Message = "User created" });
        }
        #endregion

        #region PasswordHashes
        protected void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        protected bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        #endregion

        #region TokenOperations
        /// <response code="200">Returns a new pair of tokens</response>
        /// <response code="400">This error means the refresh token was invalid or refresh token expired</response>
        [Route("/api/refresh")]
        [HttpPost]
        [ProducesResponseType(typeof(Tokens), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessageStatus), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> RefreshToken([FromBody] Tokens refreshToken)
        {
            User user = _context.Users.Where(c => c.refreshToken == refreshToken.refreshToken).SingleOrDefault();

            if (user == null)
            {
                return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid refresh token or token expired" });
            }

            Tokens tokens = new Tokens();
            if (!user.refreshToken.Equals(refreshToken.refreshToken) || user.tokenExpires < DateTime.Now)
            {
                return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid refresh token or token expired" });
            }

            tokens.accessToken = CreateToken(user);
            RefreshToken fulltoken = GenerateRefreshToken(user);
            tokens.refreshToken = fulltoken.Token;
            user.refreshToken = fulltoken.Token;
            user.tokenExpires = fulltoken.Expires;
            await _context.SaveChangesAsync();
            return Ok(tokens);
        }

        private RefreshToken GenerateRefreshToken(User user)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Parse(DateTime.Now.AddMinutes(2).ToString("yyyy-MM-dd HH:mm:ss")),
                Created = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            };

            user.refreshToken = refreshToken.Token;
            user.tokenExpires = refreshToken.Expires;

            return refreshToken;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("user guid", user.id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        #endregion
    }
}

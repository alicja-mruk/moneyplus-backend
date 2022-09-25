using AlicjowyBackendv3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Npgsql;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AlicjowyBackendv3
{
    public class LoginRegister : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginRegister(IConfiguration config)
        {
            _configuration = config;
        }

        #region LoginRegister
        [Route("/api/login")]
        [HttpPost]
        public async Task<ActionResult<UserModel>> Login([FromBody] UserDataTransferObject request)
        {
            UserModel user = new UserModel();

            NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
            //NpgsqlConnection conn = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM users WHERE email='" + request.Email + "'";
            NpgsqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                user.userId = Convert.ToInt32(reader["user_id"]);
                user.firstName = reader["first_name"].ToString();
                user.lastName = reader["last_name"].ToString();
                user.passwordHash = Convert.FromBase64String(reader["password_hash"].ToString());
                user.passwordSalt = Convert.FromBase64String(reader["password_salt"].ToString());
                user.age = Convert.ToInt32(reader["age"]);
                user.email = reader["email"].ToString();
                reader.Close();
            }
            else
                return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Username or password is incorrect" });

            bool passwordCorrect = VerifyPasswordHash(request.Password, user.passwordHash, user.passwordSalt);
            if (user.email == request.Email && passwordCorrect)
            {
                Tokens token = new Tokens();
                token.accessToken = CreateToken(user);
                RefreshToken fulltoken = GenerateRefreshToken(user);
                token.refreshToken = fulltoken.Token;
                cmd.CommandText = "UPDATE users SET refresh_token = '" + fulltoken.Token + "', token_expires = '" + fulltoken.Expires + "' WHERE user_id = " + user.userId;
                cmd.ExecuteReader();
                return Ok(token);
            }
            else
                return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Username or password is incorrect" });
        }

        [Route("/api/register")]
        [HttpPost]
        public async Task<ActionResult<UserModel>> Register([FromBody] RegisterDataTransferObject request)
        {
            UserModel user = new UserModel();
            CreatePasswordHash(request.password, out byte[] passwordHash, out byte[] passwordSalt);

            user.firstName = request.firstName;
            user.lastName = request.lastName;
            user.passwordHash = passwordHash;
            user.passwordSalt = passwordSalt;
            user.age = request.age;
            user.email = request.email;

            NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
            //NpgsqlConnection conn = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT MAX(user_id) FROM users";
            NpgsqlDataReader check = cmd.ExecuteReader();
            check.Read();
            if (check["max"] != DBNull.Value)
            {
                check.Close();
                cmd.CommandText = "INSERT INTO users(user_id, first_name, last_name, password_hash, password_salt, age, email) VALUES ((SELECT MAX(user_id)+1 FROM users), '" + user.firstName + "', '" + user.lastName + "', '" + Convert.ToBase64String(user.passwordHash) + "', '" + Convert.ToBase64String(user.passwordSalt) + "', " + user.age + ", '" + user.email + "')";
            }
            else
            {
                check.Close();
                cmd.CommandText = "INSERT INTO users(first_name, last_name, password_hash, password_salt, age, email) VALUES ('" + user.firstName + "', '" + user.lastName + "', '" + Convert.ToBase64String(user.passwordHash) + "', '" + Convert.ToBase64String(user.passwordSalt) + "', " + user.age + ", '" + user.email + "')";
            }
            try
            {
                NpgsqlDataReader reader = cmd.ExecuteReader();
            }
            catch (NpgsqlException ex)
            {
                if (ex.SqlState.Equals("23505"))
                    return Conflict(new ResponseMessageStatus { StatusCode = "409", Message = "User with this email address already exists" });
            }            
            return Created(String.Empty, new ResponseMessageStatus { StatusCode = "201", Message = "User created" });
        }
        #endregion

        #region PasswordHashes
        protected void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        protected bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        #endregion

        #region TokenOperations
        [Route("/api/refresh")]
        [HttpPost]
        public async Task<ActionResult<string>> RefreshToken([FromHeader] string refreshToken)
        {
            UserModel user = new UserModel();

            NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
            //NpgsqlConnection conn = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from Users where refresh_token = '" + refreshToken + "'";
            NpgsqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            if(reader.HasRows)
            {
                user.userId = Convert.ToInt32(reader["user_id"]);
                user.refreshToken = reader["refresh_token"].ToString();
                user.tokenExpires = Convert.ToDateTime(reader["token_expires"].ToString());
                reader.Close();
            }
            else
                return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid refresh token or token expired" });

            Tokens tokens = new Tokens();
            if (!user.refreshToken.Equals(refreshToken) || user.tokenExpires < DateTime.Now)
            {
                return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid refresh token or token expired" });
            }

            tokens.accessToken = CreateToken(user);
            RefreshToken fulltoken = GenerateRefreshToken(user);
            tokens.refreshToken = fulltoken.Token;
            cmd.CommandText = "UPDATE users SET refresh_token = '" + fulltoken.Token + "', token_expires = '" + fulltoken.Expires + "' WHERE user_id = " + user.userId;
            cmd.ExecuteReader();

            return Ok(tokens);
        }

        private RefreshToken GenerateRefreshToken(UserModel user)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddMinutes(4),
                Created = DateTime.Now
            };

            user.refreshToken = refreshToken.Token;
            user.tokenExpires = refreshToken.Expires;

            return refreshToken;
        }

        private string CreateToken(UserModel user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("user id", user.userId.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        #endregion
    }
}

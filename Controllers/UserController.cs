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
        [Route("/api/profile")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserModel>> GET()
        {
            var guid = User.FindFirstValue("user guid");
            UserModel show_user_profile = new UserModel();
            //NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
            NpgsqlConnection conn = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from Users where user_guid = '" + guid + "'";
            NpgsqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            show_user_profile.id = reader["user_guid"].ToString();
            show_user_profile.firstName = reader["first_name"].ToString();
            show_user_profile.lastName = reader["last_name"].ToString();
            show_user_profile.age = Convert.ToInt32(reader["age"]);
            show_user_profile.email = reader["email"].ToString();

            return show_user_profile;
        }
    }
}

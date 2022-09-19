using AlicjowyBackendv3.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Net;

namespace AlicjowyBackendv3.Controllers
{
    public class UserController : Controller
    {
        [Route("/api/profile/{id?}")]
        [HttpGet("{id}")]
        public IEnumerable<UserModel> GET(int? id)
        {
            List<UserModel> show_users = new List<UserModel>();
            //NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
            NpgsqlConnection conn = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            if (id != null)
                cmd.CommandText = "select * from Users where user_id=" + id;
            else
                cmd.CommandText = "select * from Users";
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var users_list = new UserModel();
                users_list.user_id = Convert.ToInt32(reader["user_id"]);
                users_list.first_name = reader["first_name"].ToString();
                users_list.last_name = reader["last_name"].ToString();
                users_list.age = Convert.ToInt32(reader["age"]);
                show_users.Add(users_list);
            }
            //return View(show_users);
            if (show_users.Count == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return null;
            }
            else
                return show_users;
        }
    }
}

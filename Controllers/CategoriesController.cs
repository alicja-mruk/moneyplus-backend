using AlicjowyBackendv3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Security.Claims;

namespace AlicjowyBackendv3.Controllers
{
    public class CategoriesController : Controller
    {
        [Route("/api/categories")]
        [HttpGet]
        //[Authorize]
        public List<CategoriesModel> GET()
        {
            //var id = User.FindFirstValue("user id");
            List<CategoriesModel> get_categories = new List<CategoriesModel>();
            NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
            //NpgsqlConnection conn = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from categories";
            NpgsqlDataReader reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                CategoriesModel category = new CategoriesModel();
                category.categoryId = Convert.ToInt32(reader["category_id"]);
                category.categoryName = reader["category_name"].ToString();
                category.iconName = reader["icon_name"].ToString();
                category.color = reader["color"].ToString();
                category.typeOfCategory = reader["type_of_category"].ToString();
                get_categories.Add(category);
            }

            return get_categories;
            //return StatusCode(501, new ResponseMessageStatus { StatusCode = "501", Message = "Jeszcze ni mo, jak bedzie to bedzie. Bądź cierpliwa" });
        }
    }
}

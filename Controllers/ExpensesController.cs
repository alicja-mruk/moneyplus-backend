using AlicjowyBackendv3.Helpers;
using AlicjowyBackendv3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace AlicjowyBackendv3.Controllers
{
    public class ExpensesController : Controller
    {
        [Route("/api/expenses/{id?}")]
        [HttpGet]
        [Authorize]
        public List<ExpensesReadDataTransferObject> GET(string? id)
        {
            var guid = User.FindFirstValue("user guid");
            List<ExpensesReadDataTransferObject> get_expenses = new List<ExpensesReadDataTransferObject>();
            //NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
            NpgsqlConnection conn = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            if (id == null)
                cmd.CommandText = "SELECT * FROM expenses WHERE user_guid = '" + guid + "'";
            //cmd.CommandText = "SELECT expenses.expense_guid, expenses.expense_name, categories.category_name, expenses.expense_value, expenses.creation_date FROM expenses INNER JOIN categories ON expenses.category_id=categories.category_id WHERE user_guid = '" + guid + "'";
            else
                cmd.CommandText = "SELECT * FROM expenses WHERE user_guid = '" + guid + "' AND expense_guid = '" + id + "'";
            //cmd.CommandText = "SELECT expenses.expense_guid, expenses.expense_name, categories.category_name, expenses.expense_value, expenses.creation_date FROM expenses INNER JOIN categories ON expenses.category_id=categories.category_id WHERE user_guid = '" + guid + "' AND expense_guid = '" + id + "'";
            NpgsqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ExpensesReadDataTransferObject expenses = new ExpensesReadDataTransferObject();
                expenses.id = reader["expense_guid"].ToString();
                //expenses.categoryName = reader["category_name"].ToString();
                expenses.expenseName = reader["expense_name"].ToString();
                expenses.expenseValue = reader["expense_value"].ToString();
                expenses.creationDate = Convert.ToDateTime(reader["creation_date"].ToString());

                //NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
                NpgsqlConnection conn2 = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
                conn2.Open();
                NpgsqlCommand cmd2 = new NpgsqlCommand();
                cmd2.Connection = conn2;
                cmd2.CommandType = CommandType.Text;
                cmd2.CommandText = "select * from categories where category_id = '" + reader["category_id"].ToString() + "'";
                NpgsqlDataReader reader2 = cmd2.ExecuteReader();
                CategoriesModel category = new CategoriesModel();
                reader2.Read();

                category.id = reader2["category_id"].ToString();
                category.categoryName = reader2["category_name"].ToString();
                category.iconName = reader2["icon_name"].ToString();
                category.color = reader2["color"].ToString();
                category.typeOfCategory = reader2["type_of_category"].ToString();

                expenses.category = category;

                get_expenses.Add(expenses);
            }

            return get_expenses;
        }

        [Route("/api/expenses/edit")]
        [Authorize]
        public async Task<ActionResult<ExpensesWriteDataTransferObject>> ADD([FromBody] ExpensesWriteDataTransferObject request)
        {
            //NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
            NpgsqlConnection conn = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            if (Request.Method == "POST")//add
            {
                ExpensesWriteDataTransferObject expens = new ExpensesWriteDataTransferObject();

                var guid = User.FindFirstValue("user guid");
                try
                {
                    expens.categoryId = request.categoryId;
                    expens.expenseName = request.expenseName;
                    expens.expenseValue = request.expenseValue;
                    expens.creationDate = DateTime.Now;
                }
                catch (Exception ex)
                {
                    return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid data type" });
                }

                cmd.CommandText = "INSERT INTO expenses(expense_guid, user_guid, category_id, expense_name, expense_value, creation_date) VALUES ('" + Guid.NewGuid().ToString() + "', '" + guid + "', " + expens.categoryId + ", '" + expens.expenseName + "', '" + expens.expenseValue + "', '" + expens.creationDate.ToString("yyyy.MM.dd HH:mm:ss") + "')";
                NpgsqlDataReader reader = cmd.ExecuteReader();
                return Created(string.Empty, new ResponseMessageStatus { StatusCode = "201", Message = "Expense created" });
            }
            else if (Request.Method == "DELETE")//remove
            {
                ExpensesModel expens = new ExpensesModel();

                try
                {
                    expens.id = request.id;
                }
                catch (Exception ex)
                {
                    return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid data type" });
                }

                cmd.CommandText = "DELETE FROM expenses WHERE expense_guid = '" + expens.id + "'";
                NpgsqlDataReader reader = cmd.ExecuteReader();
                return Ok(new ResponseMessageStatus { StatusCode = "200", Message = "Expense deleted" });
            }
            else if (Request.Method == "PUT")//update
            {
                ExpensesWriteDataTransferObject expens = new ExpensesWriteDataTransferObject();

                var guid = User.FindFirstValue("user guid");
                try
                {
                    expens.id = request.id;
                    expens.categoryId = request.categoryId;
                    expens.expenseName = request.expenseName;
                    expens.expenseValue = request.expenseValue;
                    expens.creationDate = DateTime.Now;
                }
                catch (Exception ex)
                {
                    return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid data type" });
                }

                cmd.CommandText = "UPDATE expenses SET category_id = " + expens.categoryId + ", expense_name = '" + expens.expenseName + "', expense_value = '" + expens.expenseValue + "' WHERE expense_guid = '" + expens.id + "'";
                NpgsqlDataReader reader = cmd.ExecuteReader();
                return Ok(new ResponseMessageStatus { StatusCode = "200", Message = "Expense updated" });
            }
            return StatusCode(405, new ResponseMessageStatus { StatusCode = "405", Message = "Method not allowed" });
        }
    }
}

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
        [Route("/api/expenses")]
        [HttpGet]
        [Authorize]
        public List<ExpensesReadDataTransferObject> GET()
        {
            var guid = User.FindFirstValue("user guid");
            List<ExpensesReadDataTransferObject> get_expenses = new List<ExpensesReadDataTransferObject>();
            //NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
            NpgsqlConnection conn = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT expenses.expense_guid, expenses.expense_name, categories.category_name, expenses.expense_value, expenses.creation_date FROM expenses INNER JOIN categories ON expenses.category_id=categories.category_id WHERE user_guid = '" + guid + "'";
            NpgsqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ExpensesReadDataTransferObject expenses = new ExpensesReadDataTransferObject();
                expenses.expenseGuid = reader["expense_guid"].ToString();
                expenses.categoryName = reader["category_name"].ToString();
                expenses.expenseName = reader["expense_name"].ToString();
                expenses.expenseValue = reader["expense_value"].ToString();
                expenses.creationDate = Convert.ToDateTime(reader["creation_date"].ToString());
                get_expenses.Add(expenses);
            }

            return get_expenses;
        }

        [Route("/api/expenses/add")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ExpensesWriteDataTransferObject>> ADD([FromBody] ExpensesWriteDataTransferObject request)
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
            catch(Exception ex)
            {
                return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid data type" });
            }

            //NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
            NpgsqlConnection conn = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO expenses(expense_guid, user_guid, category_id, expense_name, expense_value, creation_date) VALUES ('" + Guid.NewGuid().ToString() + "', '" + guid + "', " + expens.categoryId + ", '" + expens.expenseName + "', '" + expens.expenseValue + "', '" + expens.creationDate.ToString("yyyy.MM.dd HH:mm:ss") + "')";
            //try
            //{
                NpgsqlDataReader reader = cmd.ExecuteReader();
            //}
            //catch (NpgsqlException ex)
            //{
            //    if (ex.SqlState.Equals("23505"))
            //        return Conflict(new ResponseMessageStatus { StatusCode = "409", Message = "User with this email address already exists" });
            //}
            return Created(string.Empty, new ResponseMessageStatus { StatusCode = "201", Message = "Expense created" });
        }

        [Route("/api/expenses/remove")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ExpensesModel>> Remove([FromBody] ExpensesModel request)
        {
            ExpensesModel expens = new ExpensesModel();

            try
            {
                expens.expenseGuid = request.expenseGuid;
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid data type" });
            }

            //NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
            NpgsqlConnection conn = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "DELETE FROM expenses WHERE expense_guid = '" + expens.expenseGuid + "'";
            //try
            //{
            NpgsqlDataReader reader = cmd.ExecuteReader();
            //}
            //catch (NpgsqlException ex)
            //{
            //    if (ex.SqlState.Equals("23505"))
            //        return Conflict(new ResponseMessageStatus { StatusCode = "409", Message = "User with this email address already exists" });
            //}
            return Ok(new ResponseMessageStatus { StatusCode = "200", Message = "Expense deleted" });
        }
    }
}

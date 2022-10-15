using AlicjowyBackendv3.Helpers;
using AlicjowyBackendv3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Security.Claims;

namespace AlicjowyBackendv3.Controllers
{
    public class ReceiptsController : Controller
    {
        [Route("/api/receipts")]
        [HttpGet]
        [Authorize]
        public List<ReceiptsReadDataTransferObject> GET()
        {
            var guid = User.FindFirstValue("user guid");
            List<ReceiptsReadDataTransferObject> get_receipts = new List<ReceiptsReadDataTransferObject>();
            //NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
            NpgsqlConnection conn = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT receipts.receipts_guid, receipts.receipts_name, categories.category_name, receipts.receipts_value, receipts.creation_date FROM receipts INNER JOIN categories ON receipts.category_id=categories.category_id WHERE user_guid = '" + guid + "'";
            NpgsqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ReceiptsReadDataTransferObject receipt = new ReceiptsReadDataTransferObject();
                receipt.receiptsGuid = reader["receipts_guid"].ToString();
                receipt.categoryName = reader["category_name"].ToString();
                receipt.receiptsName = reader["receipts_name"].ToString();
                receipt.receiptsValue = reader["receipts_value"].ToString();
                receipt.creationDate = Convert.ToDateTime(reader["creation_date"].ToString());
                get_receipts.Add(receipt);
            }

            return get_receipts;
        }

        [Route("/api/receipts/add")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReceiptsWriteDataTransferObject>> ADD([FromBody] ReceiptsWriteDataTransferObject request)
        {
            ReceiptsWriteDataTransferObject receipt = new ReceiptsWriteDataTransferObject();

            var guid = User.FindFirstValue("user guid");
            try
            {
                receipt.categoryId = request.categoryId;
                receipt.receiptsName = request.receiptsName;
                receipt.receiptsValue = request.receiptsValue;
                receipt.creationDate = DateTime.Now;
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
            cmd.CommandText = "INSERT INTO receipts(receipts_guid, user_guid, category_id, receipts_name, receipts_value, creation_date) VALUES ('" + Guid.NewGuid().ToString() + "', '" + guid + "', " + receipt.categoryId + ", '" + receipt.receiptsName + "', '" + receipt.receiptsValue + "', '" + receipt.creationDate.ToString("yyyy.MM.dd HH:mm:ss") + "')";
            //try
            //{
            NpgsqlDataReader reader = cmd.ExecuteReader();
            //}
            //catch (NpgsqlException ex)
            //{
            //    if (ex.SqlState.Equals("23505"))
            //        return Conflict(new ResponseMessageStatus { StatusCode = "409", Message = "User with this email address already exists" });
            //}
            return Created(string.Empty, new ResponseMessageStatus { StatusCode = "201", Message = "Receipt created" });
        }

        [Route("/api/receipts/remove")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReceiptsModel>> Remove([FromBody] ReceiptsModel request)
        {
            ReceiptsModel receipt = new ReceiptsModel();

            try
            {
                receipt.receiptsGuid = request.receiptsGuid;
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
            cmd.CommandText = "DELETE FROM receipts WHERE receipts_guid = '" + receipt.receiptsGuid + "'";
            //try
            //{
            NpgsqlDataReader reader = cmd.ExecuteReader();
            //}
            //catch (NpgsqlException ex)
            //{
            //    if (ex.SqlState.Equals("23505"))
            //        return Conflict(new ResponseMessageStatus { StatusCode = "409", Message = "User with this email address already exists" });
            //}
            return Ok(new ResponseMessageStatus { StatusCode = "200", Message = "Receipt deleted" });
        }
    }
}

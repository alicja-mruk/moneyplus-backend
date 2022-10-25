using AlicjowyBackendv3.Helpers;
using AlicjowyBackendv3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Security.Claims;
using System.Xml.Linq;

namespace AlicjowyBackendv3.Controllers
{
    public class ReceiptsController : Controller
    {
        [Route("/api/receipts/{id?}")]
        [HttpGet]
        [Authorize]
        public List<ReceiptsReadDataTransferObject> GET(string? id)
        {
            var guid = User.FindFirstValue("user guid");
            List<ReceiptsReadDataTransferObject> get_receipts = new List<ReceiptsReadDataTransferObject>();
            //NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
            NpgsqlConnection conn = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            if (id == null)
                cmd.CommandText = "SELECT * FROM receipts WHERE user_guid = '" + guid + "'";
            //cmd.CommandText = "SELECT receipts.receipts_guid, receipts.receipts_name, categories.category_name, receipts.receipts_value, receipts.creation_date FROM receipts INNER JOIN categories ON receipts.category_id=categories.category_id WHERE user_guid = '" + guid + "'";
            else
                cmd.CommandText = "SELECT * FROM receipts WHERE user_guid = '" + guid + "' AND receipts_guid = '" + id + "'";
            //cmd.CommandText = "SELECT receipts.receipts_guid, receipts.receipts_name, categories.category_name, receipts.receipts_value, receipts.creation_date FROM receipts INNER JOIN categories ON receipts.category_id=categories.category_id WHERE user_guid = '" + guid + "' AND receipts_guid = '" + id + "'";
            NpgsqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ReceiptsReadDataTransferObject receipt = new ReceiptsReadDataTransferObject();
                receipt.id = reader["receipts_guid"].ToString();
                //receipt.categoryName = reader["category_name"].ToString();
                receipt.receiptsName = reader["receipts_name"].ToString();
                receipt.receiptsValue = reader["receipts_value"].ToString();
                receipt.creationDate = Convert.ToDateTime(reader["creation_date"].ToString());


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

                receipt.category = category;

                get_receipts.Add(receipt);
            }

            return get_receipts;
        }

        [Route("/api/receipts/edit")]
        [Authorize]
        public async Task<ActionResult<ReceiptsWriteDataTransferObject>> ADD([FromBody] ReceiptsWriteDataTransferObject request)
        {
            //NpgsqlConnection conn = new NpgsqlConnection("User ID=postgres;Password=123;Host=localhost;Port=5432;Database=moneyplusAlpha;");
            NpgsqlConnection conn = new NpgsqlConnection("User ID=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23;Host=moneyplus-server.postgres.database.azure.com;Port=5432;Database=moneyplus_db;");
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            if (Request.Method == "POST")//add
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

                cmd.CommandText = "INSERT INTO receipts(receipts_guid, user_guid, category_id, receipts_name, receipts_value, creation_date) VALUES ('" + Guid.NewGuid().ToString() + "', '" + guid + "', '" + receipt.categoryId + "', '" + receipt.receiptsName + "', '" + receipt.receiptsValue + "', '" + receipt.creationDate.ToString("yyyy.MM.dd HH:mm:ss") + "')";
                NpgsqlDataReader reader = cmd.ExecuteReader();
                return Created(string.Empty, new ResponseMessageStatus { StatusCode = "201", Message = "Receipt created" });
            }
            else if (Request.Method == "DELETE")//remove
            {
                ReceiptsModel receipt = new ReceiptsModel();

                try
                {
                    receipt.id = request.id;
                }
                catch (Exception ex)
                {
                    return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid data type" });
                }

                cmd.CommandText = "DELETE FROM receipts WHERE receipts_guid = '" + receipt.id + "'";
                NpgsqlDataReader reader = cmd.ExecuteReader();
                return Ok(new ResponseMessageStatus { StatusCode = "200", Message = "Receipt deleted" });
            }
            else if (Request.Method == "PUT")//update
            {
                ReceiptsWriteDataTransferObject receipt = new ReceiptsWriteDataTransferObject();

                var guid = User.FindFirstValue("user guid");
                try
                {
                    receipt.id = request.id;
                    receipt.categoryId = request.categoryId;
                    receipt.receiptsName = request.receiptsName;
                    receipt.receiptsValue = request.receiptsValue;
                    receipt.creationDate = DateTime.Now;
                }
                catch (Exception ex)
                {
                    return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid data type" });
                }

                cmd.CommandText = "UPDATE receipts SET category_id = " + receipt.categoryId + ", receipts_name = '" + receipt.receiptsName + "', receipts_value ='" + receipt.receiptsValue + "' WHERE receipts_guid = '" + receipt.id + "'";
                NpgsqlDataReader reader = cmd.ExecuteReader();
                return Ok(new ResponseMessageStatus { StatusCode = "200", Message = "Receipt updated" });
            }
            return StatusCode(405, new ResponseMessageStatus { StatusCode = "405", Message = "Method not allowed" });
        }
    }
}

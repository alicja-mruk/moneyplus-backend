using AlicjowyBackendv3.Helpers;
using AlicjowyBackendv3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using System.Security.Claims;
using System.Xml.Linq;

namespace AlicjowyBackendv3.Controllers
{
    public class ReceiptsController : Controller
    {
        private readonly moneyplus_dbContext _context;

        public ReceiptsController(moneyplus_dbContext context)
        {
            _context = context;
        }

        [Route("/api/receipts/{id?}")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GET(string? id, string? year, string? month, string? day)
        {
            var guid = User.FindFirstValue("user guid");
            List<Receipt> receipts = new List<Receipt>();

            if (year == null && month == null && day == null)
            {
                if (id == null)
                    receipts = await _context.Receipts.Where(c => c.userGuid == guid).ToListAsync();
                else
                    receipts = await _context.Receipts.Where(c => c.id == id && c.userGuid == guid).ToListAsync();
            }
            else
            {
                if (day != null)
                {
                    if (month == null)
                        month = DateTime.Now.Month.ToString();
                    if (year == null)
                        year = DateTime.Now.Year.ToString();
                    receipts = await _context.Receipts.Where(c => c.userGuid == guid && c.creationDate.Year.ToString() == year && c.creationDate.Month.ToString() == month && c.creationDate.Day == Convert.ToInt32(day)).ToListAsync();
                }
                else if (month != null)
                {
                    if (year == null)
                        year = DateTime.Now.Year.ToString();
                    receipts = await _context.Receipts.Where(c => c.userGuid == guid && c.creationDate.Year.ToString() == year && c.creationDate.Month == Convert.ToInt32(month)).ToListAsync();
                }
                else
                {
                    receipts = await _context.Receipts.FromSqlRaw("SELECT e.receipts_guid, e.category_id, e.creation_date, e.receipts_name, e.user_guid, e.receipts_value FROM receipts AS e WHERE (e.user_guid = '" + guid + "') AND (date_part('year', e.creation_date) = " + year + ")").ToListAsync();
                }
            }

            for (int i = 0; i < receipts.Count(); i++)
            {
                receipts[i].Category = _context.Categories.Where(c => c.id == receipts[i].categoryId).Single();
            }
            List<ReceiptsReadDataTransferObject> receipts_list = new List<ReceiptsReadDataTransferObject>();
            for (int i = 0; i < receipts.Count(); i++)
            {
                ReceiptsReadDataTransferObject receipt = new ReceiptsReadDataTransferObject();
                CategoryExtension category = new CategoryExtension();
                category.id = receipts[i].Category.id.ToString();
                category.categoryName = receipts[i].Category.categoryName;
                category.iconName = receipts[i].Category.iconName;
                category.color = receipts[i].Category.color;
                category.typeOfCategory = receipts[i].Category.typeOfCategory;

                receipt.Category = category;
                receipt.id = receipts[i].id;
                receipt.name = receipts[i].name;
                receipt.value = receipts[i].value;
                receipt.creationDate = receipts[i].creationDate;
                receipts_list.Add(receipt);
            }
            return Ok(receipts_list);
        }

        [Route("/api/receipts/edit")]
        [HttpPost, HttpPut, HttpDelete]
        [Authorize]
        public async Task<ActionResult<Receipt>> EDIT([FromBody] ReceiptsWriteDataTransferObject request)
        {
            Receipt receipt = new Receipt();

            var guid = User.FindFirstValue("user guid");
            try
            {
                receipt.id = request.id;
                receipt.categoryId = request.categoryId;
                receipt.name = request.name;
                receipt.value = Convert.ToDecimal(request.value);
                int contains_date = DateTime.Compare(request.creationDate, new DateTime(0001, 1, 1, 00, 0, 0));
                if (contains_date == 0)
                    receipt.creationDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                else
                    receipt.creationDate = request.creationDate;
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid data type" });
            }

            if (Request.Method == "POST")//add
            {
                Receipt receipt_add = new Receipt { id = Guid.NewGuid().ToString(), userGuid = guid, categoryId = receipt.categoryId, name = receipt.name, value = receipt.value, creationDate = receipt.creationDate };
                try
                {
                    _context.Receipts.Add(receipt_add);
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateException ex)
                {
                    if (ex.InnerException is NpgsqlException sqlex)
                        if (sqlex.SqlState.Equals("23503"))
                            return NotFound(new ResponseMessageStatus { StatusCode = "404", Message = "Category with this id does not exist" });
                }
                return Created(string.Empty, new ResponseMessageStatus { StatusCode = "201", Message = "Receipt created" });
            }
            else if (Request.Method == "DELETE")//remove
            {
                Receipt receipt_delete = await _context.Receipts.FindAsync(receipt.id);
                if (receipt_delete == null)
                    return NotFound(new ResponseMessageStatus { StatusCode = "404", Message = "Receipt with this id does not exist" });
                _context.Receipts.Remove(_context.Receipts.Find(receipt.id));
                _context.SaveChanges();
                return Ok(new ResponseMessageStatus { StatusCode = "200", Message = "Receipt deleted" });
            }
            else if (Request.Method == "PUT")//update
            {
                try
                {
                    Receipt receipt_update = await _context.Receipts.FindAsync(receipt.id);
                    if (receipt.categoryId != null)
                        receipt_update.categoryId = receipt.categoryId;
                    if (receipt.name != null)
                        receipt_update.name = receipt.name;
                    if (receipt.value != 0)
                        receipt_update.value = Convert.ToDecimal(receipt.value);
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    return NotFound(new ResponseMessageStatus { StatusCode = "404", Message = "Receipt with this id does not exist" });
                }
                return Ok(new ResponseMessageStatus { StatusCode = "200", Message = "Receipt updated" });
            }
            return StatusCode(405, new ResponseMessageStatus { StatusCode = "405", Message = "Method not allowed" });
        }
    }
}

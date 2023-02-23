using AlicjowyBackendv3.Helpers;
using AlicjowyBackendv3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Npgsql;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace AlicjowyBackendv3.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly moneyplus_dbContext _context;

        public ExpensesController(moneyplus_dbContext context)
        {
            _context = context;
        }

        /// <response code="200">Returns the list of all expenses or list of expenses selected by id or time range</response>
        [Route("/api/expenses/{id?}")]
        [ProducesResponseType(typeof(ExpensesReadDataTransferObject), StatusCodes.Status200OK)]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GET(string? id, string? year, string? month, string? day)
        {
            var guid = User.FindFirstValue("user guid");
            List<Expense> expenses = new List<Expense>();

            if (year == null && month == null && day == null)
            {
                if (id == null)
                    expenses = await _context.Expenses.Where(c => c.userGuid == guid).ToListAsync();
                else
                    expenses = await _context.Expenses.Where(c => c.id == id && c.userGuid == guid).ToListAsync();
            }
            else
            {
                if (day != null)
                {
                    if (month == null)
                        month = DateTime.Now.Month.ToString();
                    if (year == null)
                        year = DateTime.Now.Year.ToString();
                    expenses = await _context.Expenses.Where(c => c.userGuid == guid && c.creationDate.Year.ToString() == year && c.creationDate.Month.ToString() == month && c.creationDate.Day == Convert.ToInt32(day)).ToListAsync();
                }
                else if (month != null)
                {
                    if (year == null)
                        year = DateTime.Now.Year.ToString();
                    expenses = await _context.Expenses.Where(c => c.userGuid == guid && c.creationDate.Year.ToString() == year && c.creationDate.Month == Convert.ToInt32(month)).ToListAsync();
                }
                else
                {
                    expenses = await _context.Expenses.FromSqlRaw("SELECT e.expense_guid, e.category_id, e.creation_date, e.expense_name, e.user_guid, e.expense_value FROM expenses AS e WHERE (e.user_guid = '" + guid + "') AND (date_part('year', e.creation_date) = " + year + ")").ToListAsync(); //no jest to gówno ale co zrobić jak to niżej nie chce działać
                    //expenses = await _context.Expenses.Where(c => c.userGuid == guid && c.creationDate.Year == DateTime.Now.Year).ToListAsync();
                }
            }

            for (int i = 0; i < expenses.Count(); i++)
            {
                expenses[i].Category = _context.Categories.Where(c => c.id == expenses[i].categoryId).Single();
            }
            List<ExpensesReadDataTransferObject> expenses_list = new List<ExpensesReadDataTransferObject>();
            for (int i = 0; i < expenses.Count(); i++)
            {
                ExpensesReadDataTransferObject expense = new ExpensesReadDataTransferObject();
                CategoryExtension category = new CategoryExtension();
                category.id = expenses[i].Category.id.ToString();
                category.categoryName = expenses[i].Category.categoryName;
                category.iconName = expenses[i].Category.iconName;
                category.color = expenses[i].Category.color;
                category.typeOfCategory = expenses[i].Category.typeOfCategory;

                expense.Category = category;
                expense.id = expenses[i].id;
                expense.name = expenses[i].name;
                expense.value = expenses[i].value;
                expense.creationDate = expenses[i].creationDate;
                expenses_list.Add(expense);
            }
            return Ok(expenses_list);
        }

        //[Route("/api/expenses/edit")]
        //[HttpPost, HttpPut, HttpDelete]
        //[Authorize]
        //public async Task<ActionResult<Expense>> EDIT([FromBody] ExpensesWriteDataTransferObject request)
        //{
        //    Expense expense = new Expense();

        //    var guid = User.FindFirstValue("user guid");
        //    try
        //    {
        //        expense.id = request.id;
        //        expense.categoryId = request.categoryId;
        //        expense.name = request.name;
        //        expense.value = Convert.ToDecimal(request.value);
        //        int contains_date = DateTime.Compare(request.creationDate, new DateTime(0001, 1, 1, 00, 0, 0));
        //        if (contains_date == 0)
        //            expense.creationDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        //        else
        //            expense.creationDate = request.creationDate;
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid data type" });
        //    }

        //    if (Request.Method == "POST")//add
        //    {
        //        Expense expense_add = new Expense { id = Guid.NewGuid().ToString(), userGuid = guid, categoryId = expense.categoryId, name = expense.name, value = expense.value, creationDate = expense.creationDate };
        //        try
        //        {
        //            _context.Expenses.Add(expense_add);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch(DbUpdateException ex)
        //        {
        //            if (ex.InnerException is NpgsqlException sqlex)
        //                if (sqlex.SqlState.Equals("23503"))
        //                    return NotFound(new ResponseMessageStatus { StatusCode = "404", Message = "Category with this id does not exist" });
        //        }
        //        return Created(string.Empty, new ResponseMessageStatus { StatusCode = "201", Message = "Expense created" });
        //    }
        //    else if (Request.Method == "DELETE")//remove
        //    {
        //        Expense expense_delete = await _context.Expenses.FindAsync(expense.id);
        //        if (expense_delete == null)
        //            return NotFound(new ResponseMessageStatus { StatusCode = "404", Message = "Expense with this id does not exist" });
        //        _context.Expenses.Remove(_context.Expenses.Find(expense.id));
        //        _context.SaveChanges();
        //        return Ok(new ResponseMessageStatus { StatusCode = "200", Message = "Expense deleted" });
        //    }
        //    else if (Request.Method == "PUT")//update
        //    {
        //        try
        //        {
        //            Expense expense_update = await _context.Expenses.FindAsync(expense.id);
        //            if(expense.categoryId != null)
        //                expense_update.categoryId = expense.categoryId;
        //            if (expense.name != null)
        //                expense_update.name = expense.name;
        //            if (expense.value != 0)
        //                expense_update.value = Convert.ToDecimal(expense.value);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch(Exception ex)
        //        {
        //            return NotFound(new ResponseMessageStatus { StatusCode = "404", Message = "Expense with this id does not exist" });
        //        }
        //        return Ok(new ResponseMessageStatus { StatusCode = "200", Message = "Expense updated" });
        //    }
        //    return StatusCode(405, new ResponseMessageStatus { StatusCode = "405", Message = "Method not allowed" });
        //}

        /// <response code="201">Expense has been successfully created</response>
        /// <response code="400">The data provided was in the wrong format</response>
        /// <response code="404">This error means the user tries to select a category that does not exist</response>
        [Route("/api/expenses/edit")]
        [ProducesResponseType(typeof(ResponseMessageStatus), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseMessageStatus), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessageStatus), StatusCodes.Status404NotFound)]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Expense>> Post([FromBody] ExpensesWriteDataTransferObject request)
        {
            Expense expense = new Expense();

            var guid = User.FindFirstValue("user guid");
            try
            {
                expense.id = request.id;
                expense.categoryId = request.categoryId;
                expense.name = request.name;
                expense.value = Convert.ToDecimal(request.value);
                int contains_date = DateTime.Compare(request.creationDate, new DateTime(0001, 1, 1, 00, 0, 0));
                if (contains_date == 0)
                    expense.creationDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                else
                    expense.creationDate = request.creationDate;
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid data type" });
            }

            Expense expense_add = new Expense { id = Guid.NewGuid().ToString(), userGuid = guid, categoryId = expense.categoryId, name = expense.name, value = expense.value, creationDate = expense.creationDate };
            try
            {
                _context.Expenses.Add(expense_add);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is NpgsqlException sqlex)
                    if (sqlex.SqlState.Equals("23503"))
                        return NotFound(new ResponseMessageStatus { StatusCode = "404", Message = "Category with this id does not exist" });
            }
            return Created(string.Empty, new ResponseMessageStatus { StatusCode = "201", Message = "Expense created" });
        }

        /// <response code="200">Expense has been successfully updated</response>
        /// <response code="400">The data provided was in the wrong format</response>
        /// <response code="404">This error means the user is trying to update an expense that does not exist</response>
        [Route("/api/expenses/edit")]
        [ProducesResponseType(typeof(ResponseMessageStatus), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessageStatus), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessageStatus), StatusCodes.Status404NotFound)]
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<Expense>> Put([FromBody] ExpensesWriteDataTransferObject request)
        {
            Expense expense = new Expense();

            var guid = User.FindFirstValue("user guid");
            try
            {
                expense.id = request.id;
                expense.categoryId = request.categoryId;
                expense.name = request.name;
                expense.value = Convert.ToDecimal(request.value);
                int contains_date = DateTime.Compare(request.creationDate, new DateTime(0001, 1, 1, 00, 0, 0));
                if (contains_date == 0)
                    expense.creationDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                else
                    expense.creationDate = request.creationDate;
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid data type" });
            }

            try
            {
                Expense expense_update = await _context.Expenses.FindAsync(expense.id);
                if (expense.categoryId != null)
                    expense_update.categoryId = expense.categoryId;
                if (expense.name != null)
                    expense_update.name = expense.name;
                if (expense.value != 0)
                    expense_update.value = Convert.ToDecimal(expense.value);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return NotFound(new ResponseMessageStatus { StatusCode = "404", Message = "Expense with this id does not exist" });
            }
            return Ok(new ResponseMessageStatus { StatusCode = "200", Message = "Expense updated" });
        }

        /// <response code="200">Expense has been successfully deleted</response>
        /// <response code="400">The data provided was in the wrong format</response>
        /// <response code="404">This error means the user is trying to delete an expense that does not exist</response>
        [Route("/api/expenses/edit")]
        [ProducesResponseType(typeof(ResponseMessageStatus), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessageStatus), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessageStatus), StatusCodes.Status404NotFound)]
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<Expense>> Delete([FromBody] ExpensesWriteDataTransferObject request)
        {
            Expense expense = new Expense();

            var guid = User.FindFirstValue("user guid");
            try
            {
                expense.id = request.id;
                expense.categoryId = request.categoryId;
                expense.name = request.name;
                expense.value = Convert.ToDecimal(request.value);
                int contains_date = DateTime.Compare(request.creationDate, new DateTime(0001, 1, 1, 00, 0, 0));
                if (contains_date == 0)
                    expense.creationDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                else
                    expense.creationDate = request.creationDate;
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseMessageStatus { StatusCode = "400", Message = "Invalid data type" });
            }

            Expense expense_delete = await _context.Expenses.FindAsync(expense.id);
            if (expense_delete == null)
                return NotFound(new ResponseMessageStatus { StatusCode = "404", Message = "Expense with this id does not exist" });
            _context.Expenses.Remove(_context.Expenses.Find(expense.id));
            _context.SaveChanges();
            return Ok(new ResponseMessageStatus { StatusCode = "200", Message = "Expense deleted" });
        }

        //To samo do zrobienia co w receipts
    }
}

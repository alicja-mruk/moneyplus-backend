using AlicjowyBackendv3.Helpers;
using AlicjowyBackendv3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using System.Security.Claims;

namespace AlicjowyBackendv3.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly moneyplus_dbContext _context;

        public CategoriesController(moneyplus_dbContext context)
        {
            _context = context;
        }

        [Route("api/categories/{id?}")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GET(int? id)
        {
            List<Category> categories = new List<Category>();
            if (id == null)
                categories = await _context.Categories.ToListAsync();
            else
                categories = await _context.Categories.Where(c => c.id == id).ToListAsync();
            List<CategoryExtension> categories_list = new List<CategoryExtension>();
            for (int i = 0; i < categories.Count(); i++)
            {
                CategoryExtension category = new CategoryExtension();
                category.id = categories[i].id.ToString();
                category.categoryName = categories[i].categoryName;
                category.iconName = categories[i].iconName;
                category.color = categories[i].color;
                category.typeOfCategory = categories[i].typeOfCategory;
                categories_list.Add(category);
            }
            return Ok(categories_list);
        }

        [Route("/api/categories/add")]
        [HttpGet]
        //[Authorize]
        public IActionResult AddCategory()
        {
            return View();
        }
    }
}

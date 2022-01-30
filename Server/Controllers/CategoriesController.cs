using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Shared.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDBContext _appDBContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoriesController(AppDBContext appDBContext, IWebHostEnvironment webHostEnvironment)
        {
            _appDBContext = appDBContext;
            _webHostEnvironment = webHostEnvironment;
        }

        #region CRUD operations

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var categories = await _appDBContext.Categories.ToListAsync();
            return Ok(categories);
        }

        //website.com/api/categories/withposts
        [HttpGet("withposts")]
        public async Task<IActionResult> GetWithPosts()
        {
            List<Category> categories = await _appDBContext.Categories
                .Include(category => category.Posts)
                .ToListAsync();

            return Ok(categories);
        }

        //website.com/api/categories/3
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await GetCategoryByCategoryId(id, false);
            return Ok(category);
        }

        [HttpGet("withposts/{id}")]
        public async Task<IActionResult> GetWithPosts(int id)
        {
            var category = await GetCategoryByCategoryId(id, true);
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category categoryToCreate)
        {
            try
            {
                if (categoryToCreate == null)
                    return BadRequest(ModelState);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _appDBContext.Categories.AddAsync(categoryToCreate);
                bool changePersistedToDatabase = await PersistChangesToDatabase();

                if (!changePersistedToDatabase)
                    return StatusCode(500, "Something went wrong on our side. Please contact the administrator");

                return Created("Create", categoryToCreate);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Something went wrong on our side. Please contact the administrator. Error message: {e.Message} .");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category updatedCategory)
        {
            try
            {
                if (id < 1 || updatedCategory == null || id != updatedCategory.CategoryId)
                    return BadRequest(ModelState);

                bool exists = await _appDBContext.Categories.AnyAsync(category => category.CategoryId == id);

                if (!exists)
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _appDBContext.Categories.Update(updatedCategory);
                bool changePersistedToDatabase = await PersistChangesToDatabase();

                if (!changePersistedToDatabase)
                    return StatusCode(500, "Something went wrong on our side. Please contact the administrator");

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Something went wrong on our side. Please contact the administrator. Error message: {e.Message} .");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id < 1 )
                    return BadRequest(ModelState);

                bool exists = await _appDBContext.Categories.AnyAsync(category => category.CategoryId == id);

                if (!exists)
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var categoryToDelete = await GetCategoryByCategoryId(id, false);

                if (categoryToDelete.ThumbnailimagePath != "upload/placeholder.jpg")
                {
                    string fileName = categoryToDelete.ThumbnailimagePath.Split('/').Last();
                    System.IO.File.Delete($"{_webHostEnvironment.ContentRootPath}\\wwwroot\\uploads\\{fileName}");
                }

                _appDBContext.Categories.Remove(categoryToDelete);

                bool changePersistedToDatabase = await PersistChangesToDatabase();

                if (!changePersistedToDatabase)
                    return StatusCode(500, "Something went wrong on our side. Please contact the administrator");

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Something went wrong on our side. Please contact the administrator. Error message: {e.Message} .");
            }
        }


        #endregion

        #region Utility methods

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<bool> PersistChangesToDatabase()
        {
            var amountOfChanges = await _appDBContext.SaveChangesAsync();
            return amountOfChanges > 0;
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<Category> GetCategoryByCategoryId(int categoryId, bool withPosts)
        {
            var categoryToGet = null as Category;
            if (withPosts == true)
            {
                categoryToGet = await _appDBContext.Categories
                    .Include(category => category.Posts)
                    .FirstAsync(category => category.CategoryId == categoryId);
            }
            else
            {
                categoryToGet = await _appDBContext.Categories
                    .FirstAsync(category => category.CategoryId == categoryId);
            }

            return categoryToGet;
        }

        #endregion
    }
}

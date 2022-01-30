using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Shared.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly AppDBContext _appDBContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostsController(AppDBContext appDBContext, IWebHostEnvironment webHostEnvironment)
        {
            _appDBContext = appDBContext;
            _webHostEnvironment = webHostEnvironment;
        }

        #region CRUD operations

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var posts = await _appDBContext.Posts
                .Include(post => post.Category)
                .ToListAsync();
            return Ok(posts);
        }


        //website.com/api/posts/3
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var post = await GetPostByPostId(id);
            return Ok(post);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Post postToCreate)
        {
            try
            {
                if (postToCreate == null)
                    return BadRequest(ModelState);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (postToCreate.Published)
                    postToCreate.PublishDate = DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm");

                await _appDBContext.Posts.AddAsync(postToCreate);
                bool changePersistedToDatabase = await PersistChangesToDatabase();

                if (!changePersistedToDatabase)
                    return StatusCode(500, "Something went wrong on our side. Please contact the administrator");

                return Created("Create", postToCreate);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Something went wrong on our side. Please contact the administrator. Error message: {e.Message} .");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Post updatedPost)
        {
            try
            {
                if (id < 1 || updatedPost == null || id != updatedPost.PostId)
                    return BadRequest(ModelState);

                var oldPost = await _appDBContext.Posts.FindAsync(id);

                if (oldPost == null)
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (oldPost.Published == false && updatedPost.Published)
                    updatedPost.PublishDate = DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm");

                _appDBContext.Entry(oldPost).State = EntityState.Detached;

                _appDBContext.Posts.Update(updatedPost);

                bool changePersistedToDatabase = await PersistChangesToDatabase();

                if (!changePersistedToDatabase)
                    return StatusCode(500, "Something went wrong on our side. Please contact the administrator");

                return Created("Create",updatedPost);
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
                if (id < 1)
                    return BadRequest(ModelState);

                bool exists = await _appDBContext.Posts.AnyAsync(post => post.PostId == id);

                if (!exists)
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var postToDelete = await GetPostByPostId(id);

                if (postToDelete.ThumbnailimagePath != "upload/placeholder.jpg")
                {
                    string fileName = postToDelete.ThumbnailimagePath.Split('/').Last();
                    System.IO.File.Delete($"{_webHostEnvironment.ContentRootPath}\\wwwroot\\uploads\\{fileName}");
                }

                _appDBContext.Posts.Remove(postToDelete);

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
        private async Task<Post> GetPostByPostId(int postId)
        {
            var postToGet = null as Post;
            postToGet = await _appDBContext.Posts
                 .Include(post => post.Category)
                 .FirstAsync(post => post.PostId == postId);

            return postToGet;
        }

        #endregion
    }
}

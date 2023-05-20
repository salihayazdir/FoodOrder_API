using FoodOrder_API.Data;
using FoodOrder_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder_API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public CategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return Ok(await _db.Categories.ToListAsync());
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }
            var category = await _db.Categories.FirstOrDefaultAsync(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (category == null)
            {
                return BadRequest();
            }
            if (category.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();

            return Ok(category);

        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }
            var category = _db.Categories.FirstOrDefault(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category categoryBody)
        {
            if (categoryBody == null || id != categoryBody.Id)
            {
                return BadRequest();
            }
            var category = _db.Categories.FirstOrDefault(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            category.Name = categoryBody.Name;
            category.Description = categoryBody.Description;

            _db.Categories.Update(category);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}

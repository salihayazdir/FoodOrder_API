using FoodOrder_API.Data;
using FoodOrder_API.Models;
using FoodOrder_API.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder_API.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ItemsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
            return Ok(await _db.Items.ToListAsync());
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            if(id < 1) 
            {
                return BadRequest();
            }
            var item = await _db.Items.FirstOrDefaultAsync(u => u.Id == id);
            if(item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<Item>> CreateItem([FromBody] Item item)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(item == null)
            {
                return BadRequest();
            }
            if(item.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            await _db.Items.AddAsync(item);
            await _db.SaveChangesAsync();

            return Ok(item);
        
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            if(id < 1)
            {
                return BadRequest();
            }
            var item = _db.Items.FirstOrDefault(u=>u.Id == id);
            if(item == null)
            {
                return NotFound();
            }
            _db.Items.Remove(item);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] Item itemBody)
        {
            if(itemBody == null || id != itemBody.Id)
            {
                return BadRequest();
            }
            var item = _db.Items.FirstOrDefault(u => u.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            item.Name = itemBody.Name;
            item.Description = itemBody.Description;
            item.Price = itemBody.Price;

            _db.Items.Update(item);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    
    }
}

using FoodOrder_API.Data;
using FoodOrder_API.Models;
using FoodOrder_API.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;

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
        public async Task<ActionResult<AnyType>> GetItems()
        {
            var items = (
                from it in _db.Items
                join ct in _db.Categories on it.CategoryId equals ct.Id
                select new
                {
                    id = it.Id,
                    name = it.Name,
                    price = it.Price,
                    description = it.Description,
                    category = ct.Name
                }
            );

            return Ok(items);
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
        public async Task<ActionResult<Item>> CreateItem([FromBody] AddItemDTO item)
        {

            if(item == null)
            {
                return BadRequest();
            }

            var newItem = new Item
            {
                CategoryId = item.CategoryId,
                Name = item.Name,
                Price = item.Price,
                Description = item.Description,
            };
            await _db.Items.AddAsync(newItem);
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

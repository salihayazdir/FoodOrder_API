using FoodOrder_API.Data;
using FoodOrder_API.Models;
using FoodOrder_API.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder_API.Controllers
{
    [Route("api/addresses")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public AddressesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            return Ok(await _db.Addresses.ToListAsync());
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }
            var item = await _db.Addresses.FirstOrDefaultAsync(u => u.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Address>> CreateAddress([FromBody] Address item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (item == null)
            {
                return BadRequest();
            }
            if (item.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            await _db.Addresses.AddAsync(item);
            await _db.SaveChangesAsync();

            return Ok(item);

        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }
            var item = _db.Addresses.FirstOrDefault(u => u.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            _db.Addresses.Remove(item);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] Address itemBody)
        {
            if (itemBody == null || id != itemBody.Id)
            {
                return BadRequest();
            }
            var item = _db.Addresses.FirstOrDefault(u => u.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            item.Name = itemBody.Name;
            item.Details = itemBody.Details;

            _db.Addresses.Update(item);
            await _db.SaveChangesAsync();

            return NoContent();
        }

    }
}

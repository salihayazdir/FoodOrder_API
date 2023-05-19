using FoodOrder_API.Data;
using FoodOrder_API.Models;
using FoodOrder_API.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder_API.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ItemDTO>> GetItems()
        {
            return Ok(ItemStore.itemList);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemDTO> GetItem(int id)
        {
            if(id < 1) 
            {
                return BadRequest();
            }
            var item = ItemStore.itemList.FirstOrDefault(u => u.Id == id);
            if(item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        public ActionResult<ItemDTO> CreateItem([FromBody] ItemDTO itemDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(itemDTO == null)
            {
                return BadRequest();
            }
            if(itemDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            itemDTO.Id = ItemStore.itemList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            ItemStore.itemList.Add(itemDTO);

            return Ok(itemDTO);
        
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteItem(int id)
        {
            if(id < 1)
            {
                return BadRequest();
            }
            var item = ItemStore.itemList.FirstOrDefault(u=>u.Id == id);
            if(item == null)
            {
                return NotFound();
            }
            ItemStore.itemList.Remove(item);
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateItem(int id, [FromBody] ItemDTO itemDTO)
        {
            if(itemDTO == null || id != itemDTO.Id)
            {
                return BadRequest();
            }
            var item = ItemStore.itemList.FirstOrDefault(u => u.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            item.Name = itemDTO.Name;
            item.Description = itemDTO.Description;
            item.Price = itemDTO.Price;

            return NoContent();
        }
    
    }
}

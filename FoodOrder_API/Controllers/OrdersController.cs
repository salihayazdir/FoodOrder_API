using FoodOrder_API.Data;
using FoodOrder_API.Models;
using FoodOrder_API.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FoodOrder_API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public OrdersController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AnyType>> GetOrders()
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var bearerToken = HttpContext.Request.Headers.Authorization.ToString();
            bearerToken = bearerToken.Substring(bearerToken.LastIndexOf(" ") + 1);
            var jwt = new JwtSecurityToken(bearerToken);
            var claimlist = jwt.Claims.ToList();
            var nameid = jwt.Claims.FirstOrDefault(c => c.Type == "nameid").Value;
            var role = jwt.Claims.FirstOrDefault(c => c.Type == "role").Value;

            if (nameid.IsNullOrEmpty())
            {
                return BadRequest();
            }

            if(role == "admin")
            {
                var allOrderItems = (
                from oi in _db.OrderItems
                join it in _db.Items on oi.ItemId equals it.Id
                select new
                {
                    itemName = it.Name,
                    quantity = oi.Quantity,
                    itemPrice = it.Price,
                    orderHeader = oi.OrderHeaderId,
                }
            );

                var allOrderHeaderDetails = (
                    from oh in _db.OrderHeaders
                    join ad in _db.Addresses on oh.AddressId equals ad.Id
                    join us in _db.Users on oh.UserId equals us.Id
                    orderby oh.DateTime descending
                    select new
                    {
                        id = oh.Id,
                        address = ad.Details,
                        user = us.Name,
                        date = oh.DateTime,
                        notes = oh.Notes,
                        status = oh.Status,
                    }

                );

                var resAdmin = new
                {
                    orderHeaderDetails = allOrderHeaderDetails,
                    orderItems = allOrderItems,
                };

                return Ok(resAdmin);
            }

            var orderItems = (
                from oi in _db.OrderItems
                join it in _db.Items on oi.ItemId equals it.Id
                where oi.OrderHeader.UserId == int.Parse(nameid)
                select new
                {
                    itemName = it.Name,
                    quantity = oi.Quantity,
                    itemPrice = it.Price,
                    orderHeader = oi.OrderHeaderId,
                }
            );

            var orderHeaderDetails = (
                from oh in _db.OrderHeaders
                join ad in _db.Addresses on oh.AddressId equals ad.Id
                join us in _db.Users on oh.UserId equals us.Id
                where oh.UserId == int.Parse(nameid)
                orderby oh.DateTime descending
                select new
                {
                    id = oh.Id,
                    address = ad.Details,
                    user = us.Name,
                    date = oh.DateTime,
                    notes = oh.Notes,
                    status = oh.Status,
                }

            );

            var res = new
            {
                orderHeaderDetails = orderHeaderDetails,
                orderItems = orderItems,
            };

            return Ok(res);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AnyType>> GetOrder(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }
            var orderHeader = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.Id == id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            var orderItems = (
                from oi in _db.OrderItems
                join it in _db.Items on oi.ItemId equals it.Id
                where oi.OrderHeaderId == id
                select new
                {
                    itemName = it.Name,
                    quantity = oi.Quantity,
                    itemPrice = it.Price,
                }
            );

            var orderHeaderDetails = (
                from oh in _db.OrderHeaders
                join ad in _db.Addresses on oh.AddressId equals ad.Id
                join us in _db.Users on oh.UserId equals us.Id
                where oh.Id == id
                select new
                {
                    address = ad.Details,
                    user = us.Name,
                    date = oh.DateTime,
                    notes = oh.Notes,
                }
            );

            var res = new
            {
                orderHeaderDetails = orderHeaderDetails,
                orderItems = orderItems,
            };

            return Ok(res);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<AnyType>> CreateOrder([FromBody] NewOrderDTO req)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var bearerToken = HttpContext.Request.Headers.Authorization.ToString();
            bearerToken = bearerToken.Substring(bearerToken.LastIndexOf(" ") + 1);
            var jwt = new JwtSecurityToken(bearerToken);
            var claimlist = jwt.Claims.ToList();
            var nameid = jwt.Claims.FirstOrDefault(c => c.Type == "nameid").Value;
            var role = jwt.Claims.FirstOrDefault(c => c.Type == "role").Value;


            var newHeader = new OrderHeader
            {
                UserId = int.Parse(nameid),
                AddressId = req.AddressId,
                Notes = req.Notes,
                DateTime = DateTime.Now,
                Status = 1
            };

            await _db.OrderHeaders.AddAsync(newHeader);
            _db.SaveChanges();


            foreach(OrderItemRequestDTO item in req.OrderItems)
            {
                var newOrderItem = new OrderItem
                {
                    ItemId = item.ItemId,
                    Quantity = item.Quantity,
                    OrderHeaderId = newHeader.Id,
                };

                await _db.AddAsync(newOrderItem);
            }

            await _db.SaveChangesAsync();

            return Ok();

        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }
            var category = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _db.OrderHeaders.Remove(category);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderStatusChangeDTO orderStatusChangeDTO)
        {
            //if (orderStatusChangeDTO == null || id != orderStatusChangeDTO.status)
            //{
            //    return BadRequest();
            //}
            var orderHeader = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            orderHeader.Status = orderStatusChangeDTO.status;

            _db.OrderHeaders.Update(orderHeader);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}

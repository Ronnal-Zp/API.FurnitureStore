using API.FurnitureStore.Data;
using API.FurnitureStore.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.FurnitureStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly APIFurnitureStoreContext _context;

        public OrdersController(APIFurnitureStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> Get()
        {
            return await _context.Orders.Include(o => o.OrderDetails).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var order = await _context.Orders
                                .Include(o => o.OrderDetails)
                                .FirstOrDefaultAsync(o => o.Id == id);
            
            if (order == null) return NotFound("Order not found");

            return Ok(order);   
        }

        [HttpPost]
        public async Task<IActionResult> Post(Order order)
        {
            if (order.OrderDetails == null) return BadRequest("Order should have at least one details");

            await _context.Orders.AddAsync(order);
            await _context.OrderDetails.AddRangeAsync(order.OrderDetails);

            await _context.SaveChangesAsync();

            return CreatedAtAction("Post", order.Id, order);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Order order)
        {
            if (order == null) return BadRequest("Order not found");
            if (order.Id <= 0) return BadRequest("Id order not found");

            var orderDB = await _context.Orders
                            .Include(o => o.OrderDetails)
                            .FirstOrDefaultAsync(o => o.Id == order.Id);

            if(orderDB == null) return NotFound("Order not exists in database");

            orderDB.OrderNumber = order.OrderNumber;
            orderDB.OrderDate = order.OrderDate;
            orderDB.DeliveryDate = order.DeliveryDate;
            orderDB.ClientId = order.ClientId;

            _context.OrderDetails.RemoveRange(orderDB.OrderDetails);

            _context.Orders.Update(orderDB);
            _context.OrderDetails.AddRange(order.OrderDetails);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Order order)
        {
            if (order == null) return BadRequest("Order not found");

            var orderDB = await _context.Orders
                                    .Include(o => o.OrderDetails)
                                    .FirstOrDefaultAsync(o =>o.Id == order.Id); 

            if(orderDB == null) return NotFound("Order not exists in database");

            _context.OrderDetails.RemoveRange(orderDB.OrderDetails);
            _context.Orders.Remove(orderDB);

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}

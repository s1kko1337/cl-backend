﻿using cl_backend.DbContexts;
using cl_backend.DTO;
using cl_backend.Extensions;
using cl_backend.Models.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace cl_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public OrdersController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/orders - получить все заказы (admin) или свои заказы (user)
        [HttpGet]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            IQueryable<Order> query = _context.Orders
                .Include(o => o.OrderItems);

            // Если пользователь не admin, показываем только его заказы
            if (userRole != "admin")
            {
                query = query.Where(o => o.UserId == userId);
            }

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders.Select(o => o.ToDTO()));
        }

        // GET: api/orders/5 - получить конкретный заказ
        [HttpGet("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Проверяем права доступа
            if (userRole != "admin" && order.UserId != userId)
            {
                return Forbid();
            }

            return order.ToDTO();
        }

        // GET: api/orders/user/5 - получить все заказы конкретного пользователя (только admin)
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByUser(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders.Select(o => o.ToDTO()));
        }

        // POST: api/orders - создать новый заказ
        [HttpPost]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<OrderDTO>> CreateOrder(OrderCreateDTO orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    UserId = userId,
                    CustomerName = orderDto.CustomerName,
                    CustomerPhone = orderDto.CustomerPhone,
                    DeliveryAddress = orderDto.DeliveryAddress,
                    PaymentMethod = orderDto.PaymentMethod,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                decimal totalAmount = 0;

                // Обрабатываем каждый товар в заказе
                foreach (var itemDto in orderDto.OrderItems)
                {
                    var product = await _context.Products.FindAsync(itemDto.ProductId);

                    if (product == null)
                    {
                        return BadRequest($"Product with ID {itemDto.ProductId} not found.");
                    }

                    // Проверяем наличие товара
                    if (product.StockQuantity < itemDto.Quantity)
                    {
                        return BadRequest($"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {itemDto.Quantity}");
                    }

                    // Уменьшаем количество товара на складе
                    product.StockQuantity -= itemDto.Quantity;

                    // Создаем элемент заказа
                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        PriceAtPurchase = product.Price,
                        Quantity = itemDto.Quantity,
                        Subtotal = product.Price * itemDto.Quantity
                    };

                    totalAmount += orderItem.Subtotal;
                    order.OrderItems.Add(orderItem);
                }

                order.TotalAmount = totalAmount;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var createdOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, createdOrder?.ToDTO());
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // PUT: api/orders/5 - обновить статус заказа (только admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateOrder(int id, OrderUpdateDTO orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.UpdateFromDTO(orderDto);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // DELETE: api/orders/5 - удалить заказ
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Проверяем права доступа
            if (userRole != "admin" && order.UserId != userId)
            {
                return Forbid();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Возвращаем товары на склад
                foreach (var orderItem in order.OrderItems)
                {
                    var product = await _context.Products.FindAsync(orderItem.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += orderItem.Quantity;
                    }
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
using System.Security.Claims;
using System.Threading.Tasks;
using day1.DTOs.Dish;
using day1.DTOs;
using day1.ProjectHelper;
using day1.Services.CartAndOrder;
using day1.Services.Dish;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using day1.DTOs.CartAndOrderDto;

namespace day1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishAndOrderController : ControllerBase
    {
        public readonly IDishServer _dishServer;
        public readonly IWebHostEnvironment _webHostEnvironment;
        public readonly ICartAndOrderServer _cartAndOrderServer;
        public DishAndOrderController(IDishServer dishServer, IWebHostEnvironment webHostEnvironment, ICartAndOrderServer cartAndOrderServer)
        {
            _dishServer = dishServer;
            _webHostEnvironment = webHostEnvironment;
            _cartAndOrderServer = cartAndOrderServer;
        }
        [HttpGet("GetDishAll")]
        public IActionResult GetDishAll()
        {
            var users = _dishServer.GetDishes();
            return Ok(users);
        }
        [HttpPost("GetDishPageIndex")]
        public async Task<IActionResult> GetPageIndexSizeAsyncDish(PageRequestDto dto)
        {
           var result = await _dishServer.GetPageDishAsync(dto.PageIndex, dto.PageSize, dto.KeyWord);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("AddDish")]
        public IActionResult GetDishAll(DishDTO dishDTO)
        {
            var dish = _dishServer.AddDish(dishDTO);
            return Ok(dish);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("UpLoadImage")]
        public IActionResult UpLoadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("请选择要上传的图片");
            }
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            var imageUrl = $"/uploads/{fileName}";
            var fullurl = Request.ToFullUrl(imageUrl);
            return Ok(fullurl);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteDishAsync")]
        public async Task<IActionResult> DeletDishAsync(long dishid) 
        {
            var result = await _dishServer.DeleteDishAsync(dishid);
            if (!result)
            {
                return NotFound(new { message = "菜品不存在" });
            }
            return Ok(new { message = "删除成功" });
        }

        [Authorize]
        [HttpPut("UpdateToCart")]
        public async Task<IActionResult> UpdateToCart(long dishid, int quantity)
        {
            var userId = GetCurrentUserId();
            var cartdto= await _cartAndOrderServer.UpdateToCart(userId, dishid, quantity);
            return Ok(cartdto);
        }
        [Authorize]
        [HttpGet("GetCart")]
        public async Task<IActionResult> GetCart() 
        {
            var userId = GetCurrentUserId();
            var carts = await _cartAndOrderServer.GetCarts(userId);
            return Ok(carts);
        }
        [Authorize]
        [HttpDelete("ClearCart")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetCurrentUserId();
            await _cartAndOrderServer.ClearCart(userId);
            return Ok(new { message = "购物车已清空" });
        }

        [Authorize]
        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var order = await _cartAndOrderServer.PlaceOrder(userId, dto.Remark);
                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetOrders()
        {
            var userId = GetCurrentUserId();
            var orders = await _cartAndOrderServer.GetUserOrders(userId);
            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _cartAndOrderServer.GetAllOrders();
            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusDto dto)
        {
            var result = await _cartAndOrderServer.UpdateOrderStatus(dto.OrderId, dto.Status);
            if (!result)
            {
                return NotFound(new { message = "订单不存在" });
            }
            return Ok(new { message = "订单状态更新成功" });
        }

        [Authorize]
        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(long orderId)
        {
            var userId = GetCurrentUserId();
            var result = await _cartAndOrderServer.CancelOrder(orderId, userId);
            if (!result)
            {
                return BadRequest(new { message = "订单不存在或无法取消" });
            }
            return Ok(new { message = "订单已取消" });
        }

        [Authorize]
        [HttpPost("SearchOrders")]
        public async Task<IActionResult> SearchOrders([FromBody] OrderSearchDto searchDto)
        {
            var userId = User.IsInRole("Admin") ? (long?)null : GetCurrentUserId();
            var result = await _cartAndOrderServer.SearchOrders(searchDto, userId);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetOrderStatistics")]
        public async Task<IActionResult> GetOrderStatistics()
        {
            var statistics = await _cartAndOrderServer.GetOrderStatistics();
            return Ok(statistics);
        }

        private long GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return claim != null ? long.Parse(claim) : throw new UnauthorizedAccessException();
        }
    }
}

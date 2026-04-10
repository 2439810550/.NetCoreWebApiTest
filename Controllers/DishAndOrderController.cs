using System.Threading.Tasks;
using day1.DTOs.Dish;
using day1.ProjectHelper;
using day1.Services.Dish;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace day1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishAndOrderController : ControllerBase
    {
        public readonly IDishServer _dishServer;
        public readonly IWebHostEnvironment _webHostEnvironment;
        public DishAndOrderController(IDishServer dishServer, IWebHostEnvironment webHostEnvironment)
        {
            _dishServer = dishServer;
            _webHostEnvironment = webHostEnvironment;
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
    }
}

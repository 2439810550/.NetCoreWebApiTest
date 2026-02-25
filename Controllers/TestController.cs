using System.Runtime.CompilerServices;
using day1.Common;
using day1.Models;
using day1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace day1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IUserService _userService;
        public TestController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize]
        [HttpGet("user")]
        public IActionResult GetUserAll()
        {
            var users = _userService.GetAllUsers();
            return Ok(ApiResponse<List<User>>.SuccessResponse(users));
        }

        [HttpPost("user")]
        public IActionResult AddUser(DTOs.CreateUserDTO createUserDTO)
        {
            try
            {
                var user = _userService.CreateUser(createUserDTO);
                return Ok(ApiResponse<User>.SuccessResponse(user));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult.Fail(ex.Message));
            }
        }
        [HttpGet("id")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(ApiResponse<User>.SuccessResponse(user));
        }

        [HttpPost("login")]
        public IActionResult Login(DTOs.CreateUserDTO createUserDTO)
        {
            var user = _userService.Login(createUserDTO);
            if (user == null)
            {
                return BadRequest(ApiResult.Fail("用户名或密码错误"));
            }
            return Ok(ApiResponse<DTOs.LoginResponseDto>.SuccessResponse(user));
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteByUserName")]
        public IActionResult DeleteByUserName(string username)
        {
            try
            {
                _userService.DeleteByUserName(username);
                return Ok(ApiResponse<object>.SuccessResponse("删除成功"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult.Fail(ex.Message));
            }
        }
    }
}

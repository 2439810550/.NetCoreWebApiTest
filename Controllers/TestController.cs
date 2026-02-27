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
        private readonly ITokenService _tokenService;
        public TestController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }
        [Authorize]
        [HttpGet("user")]
        public IActionResult GetUserAll()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpPost("user")]
        public IActionResult AddUser(DTOs.CreateUserDTO createUserDTO)
        {
                var user = _userService.CreateUser(createUserDTO);
                return Ok(user);
        }
        [HttpGet("id")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public IActionResult Login(DTOs.CreateUserDTO createUserDTO)
        {
            var user = _userService.Login(createUserDTO);
            return Ok(user);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public IActionResult ReFresh(string refreshtoken) 
        {
                var result= _tokenService.RefreshToken(refreshtoken);
                return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteByUserName")]
        public IActionResult DeleteByUserName(string username)
        {
                _userService.DeleteByUserName(username);
                return Ok("删除成功!");
        }
    }
}

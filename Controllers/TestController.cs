using System.Runtime.CompilerServices;
using day1.Services;
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
    }
}

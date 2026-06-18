﻿using day1.Domain.Enum;
using day1.Domain;
using day1.Services;
using day1.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using day1.Domain.Security;

namespace day1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("user")]
        public IActionResult GetUserAll()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsersPaged(int page = 1, int size = 10, string? keyword = null)
        {
            var result = await _userService.GetUsersPagedAsync(page, size, keyword);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("user")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto dto)
        {
            await _userService.UpdateUserAsync(dto.Id, dto);
            return Ok("更新成功!");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("user")]
        public IActionResult AddUser(CreateUserDTO createUserDTO)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState
                    .Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                throw new BusinessException(
                    BusinessErrorCode.ValidationError,
                    string.Join("; ", errorMessages));
            }
            var user = _userService.CreateUser(createUserDTO);
                return Ok(user);
        }
        [Authorize(Policy = Permissions.User.Read)]
        [HttpGet("id")]
        public IActionResult GetById(long id)
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpPost("login")]
        public IActionResult Login(CreateUserDTO createUserDTO)
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

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteByUserId")]
        public IActionResult DeleteByUserId(long userid)
        {
            _userService.DeleteByUserId(userid);
            return Ok("删除成功!");
        }
    }
}

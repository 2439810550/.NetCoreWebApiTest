using day1.Data;
using day1.Models;
using day1.Repositories;
using day1.DTOs;
using day1.ProjectHelper;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Configuration.UserSecrets;
using day1.Domain;
using day1.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace day1.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IRoleAndPermissionRepository _roleAndPermissionRepository;
        public UserService(IUserRepository userRepository,ITokenService tokenService,IDateTimeProvider dateTimeProvider,IRoleAndPermissionRepository roleAndPermissionRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _dateTimeProvider = dateTimeProvider;
            _roleAndPermissionRepository = roleAndPermissionRepository;
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public User CreateUser(CreateUserDTO createUserDto)
        {

            CheckPassword(createUserDto.PassWord); // ✅ 密码校验逻辑
            var exct= _userRepository.ExctisUserName(createUserDto.UserName);
            if (exct)
            {
                throw new BusinessException(Domain.Enum.BusinessErrorCode.UserAlreadyExists,"用户名已存在");
            }
            var user = new User
            {
                UserName = createUserDto.UserName,
                PassWord = PasswordHelper.HashPassword(createUserDto.PassWord),
                CreateTime =_dateTimeProvider.UtcNow
            };
            _userRepository.Add(user);  // ✅ 调用 Repository
            return user;
        }

        private void CheckPassword(string password)
        {
            if (password.Length < 6)
                throw new Exception("密码长度不能小于6位");

            if (!password.Any(char.IsDigit) || !password.Any(char.IsLetter))
                throw new Exception("密码必须包含字母和数字");
        }

        public User? GetById(long id)
        {
            return _userRepository.GetById(id);
        }
        /// <summary>
        /// 用户登录，验证用户名和密码，生成 JWT 令牌，并处理账户锁定逻辑
        /// </summary>
        /// <param name="createUserDTO"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public LoginResponseDto Login(DTOs.CreateUserDTO createUserDTO)
        {
           var user= _userRepository.GetByUserName(createUserDTO.UserName);
            if (user is null)
                throw new BusinessException(Domain.Enum.BusinessErrorCode.UserNameOrPasswordError,"用户名或密码错误");
            if (user.LockOutEndTime.HasValue && user.LockOutEndTime > _dateTimeProvider.UtcNow)
                throw new BusinessException(Domain.Enum.BusinessErrorCode.AccountLocked, $"账户已锁定，请于{user.LockOutEndTime.Value.ToLocalTime()}后再试");
            bool Success = PasswordHelper.VerifyPassword(createUserDTO.PassWord,user.PassWord);
            if (!Success) 
            {
                HandleFailedLogin(user);
                throw new BusinessException(Domain.Enum.BusinessErrorCode.UserNameOrPasswordError, "用户名或密码错误");
            }
            user.FailedLoginCount = 0;
            user.LockOutEndTime = null;
            var roles = _roleAndPermissionRepository.GetUserRoles(user.Id);
            var token=_tokenService.CreateAccessToken(user,roles);
            var refreshToken =_tokenService.CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime =_dateTimeProvider.UtcNow.AddDays(7);

            _userRepository.UpdateUser(user);
            return new LoginResponseDto
            {
                Id=user.Id,
                UserName = user.UserName,
                Token = token,
                RefreshToken=refreshToken,
            };
        }

        private void HandleFailedLogin(User user)
        {
            user.FailedLoginCount++;

            if (user.FailedLoginCount >= 5)
            {
                user.LockOutEndTime = _dateTimeProvider.UtcNow.AddMinutes(5);
                user.FailedLoginCount = 0; // 重置失败计数
            }
            _userRepository.UpdateUser(user);
        }

        public void DeleteByUserName(string username)
        {
          var user= _userRepository.GetByUserName(username);
            if (user==null)
            {
                throw new BusinessException(Domain.Enum.BusinessErrorCode.UserNotFound,"用户不存在");
            }
            var roles = _roleAndPermissionRepository.GetUserRoles(user.Id);
            if (roles.Contains("Admin")) 
            {
                throw new BusinessException(Domain.Enum.BusinessErrorCode.NotDeleteAdminUser, "管理员用户不能被删除");
            }
            _userRepository.DeleteByUserName(username);
        }

    }
}

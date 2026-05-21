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

        public async Task<PagedResult<UserListDto>> GetUsersPagedAsync(int page, int size, string? keyword)
        {
            var (users, total) = await _userRepository.GetUsersPagedAsync(page, size, keyword);
            var items = users.Select(u => new UserListDto
            {
                Id = u.Id,
                UserName = u.UserName,
                CreateTime = u.CreateTime,
                FailedLoginCount = u.FailedLoginCount,
                LockOutEndTime = u.LockOutEndTime,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            }).ToList();
            return new PagedResult<UserListDto>
            {
                Items = items,
                TotalCount = total,
                PageIndex = page,
                PageSize = size
            };
        }

        public async Task UpdateUserAsync(long id, UpdateUserDto dto)
        {
            var user = _userRepository.GetById(id)
                ?? throw new BusinessException(Domain.Enum.BusinessErrorCode.UserNotFound, "用户不存在");
            if (!string.IsNullOrWhiteSpace(dto.UserName) && dto.UserName != user.UserName)
            {
                if (_userRepository.ExctisUserName(dto.UserName))
                    throw new BusinessException(Domain.Enum.BusinessErrorCode.UserAlreadyExists, "用户名已存在");
                user.UserName = dto.UserName;
            }
            // 处理角色更新
            if (dto.Roles != null)
            {
                var existingRoles = _roleAndPermissionRepository.GetUserRoles(id);
                var allRoles = _roleAndPermissionRepository.GetAllRoles();
                // 添加新角色
                foreach (var roleName in dto.Roles.Except(existingRoles))
                {
                    var role = allRoles.FirstOrDefault(r => r.Name == roleName);
                    if (role != null)
                        _roleAndPermissionRepository.AddRoleByUser(id, role.Id);
                }
                // 移除被去掉的角色（但不允许移除最后一个角色，且不可移除 Admin 给别人）
                foreach (var roleName in existingRoles.Except(dto.Roles))
                {
                    if (roleName == "Admin" && existingRoles.Count == 1)
                        continue; // 保护：不能移除最后一个 Admin 角色
                    var role = allRoles.FirstOrDefault(r => r.Name == roleName);
                    if (role != null && existingRoles.Count > 1)
                        _roleAndPermissionRepository.DeleteRoleByUser(id, role.Id);
                }
            }
            _userRepository.UpdateUser(user);
        }

        public User CreateUser(CreateUserDTO createUserDto)
        {
            CheckPassword(createUserDto.PassWord);
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
            _userRepository.Add(user);
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

        public LoginResponseDto Login(CreateUserDTO createUserDTO)
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
                user.FailedLoginCount = 0;
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

        public void DeleteByUserId(int userId)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
            {
                throw new BusinessException(Domain.Enum.BusinessErrorCode.UserNotFound, "用户不存在");
            }
            var roles = _roleAndPermissionRepository.GetUserRoles(user.Id);
            if (roles.Contains("Admin"))
            {
                throw new BusinessException(Domain.Enum.BusinessErrorCode.NotDeleteAdminUser, "管理员用户不能被删除");
            }
            _userRepository.DeleteByUserName(user.UserName);
        }
    }
}

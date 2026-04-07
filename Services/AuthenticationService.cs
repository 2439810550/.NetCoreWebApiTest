using day1.DTOs;
using day1.Models;
using day1.Repositories;
using day1.ProjectHelper;
using day1.Domain;
using day1.Interfaces;

namespace day1.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IRoleAndPermissionRepository _roleAndPermissionRepository;
        
        public AuthenticationService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IDateTimeProvider dateTimeProvider,
            IRoleAndPermissionRepository roleAndPermissionRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _dateTimeProvider = dateTimeProvider;
            _roleAndPermissionRepository = roleAndPermissionRepository;
        }
        
        public LoginResponseDto Login(CreateUserDTO createUserDTO)
        {
            var user = _userRepository.GetByUserName(createUserDTO.UserName);
            if (user is null)
                throw new BusinessException(Domain.Enum.BusinessErrorCode.UserNameOrPasswordError, "用户名或密码错误");
            
            if (user.LockOutEndTime.HasValue && user.LockOutEndTime > _dateTimeProvider.UtcNow)
                throw new BusinessException(Domain.Enum.BusinessErrorCode.AccountLocked, $"账户已锁定，请于{user.LockOutEndTime.Value.ToLocalTime()}后再试");
            
            bool Success = PasswordHelper.VerifyPassword(createUserDTO.PassWord, user.PassWord);
            if (!Success)
            {
                HandleFailedLogin(user);
                throw new BusinessException(Domain.Enum.BusinessErrorCode.UserNameOrPasswordError, "用户名或密码错误");
            }
            
            user.FailedLoginCount = 0;
            user.LockOutEndTime = null;
            var roles = _roleAndPermissionRepository.GetUserRoles(user.Id);
            var token = _tokenService.CreateAccessToken(user, roles);
            var refreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = _dateTimeProvider.UtcNow.AddDays(7);

            _userRepository.UpdateUser(user);
            return new LoginResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = token,
                RefreshToken = refreshToken,
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
    }
}

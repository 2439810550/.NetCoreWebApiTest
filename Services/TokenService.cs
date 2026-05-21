using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using day1.Domain;
using day1.DTOs;
using day1.Interfaces;
using day1.Models;
using day1.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace day1.Services
{
    public class TokenService : ITokenService
    {

        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IRoleAndPermissionRepository _roleAndPermissionRepository;

        public TokenService(IConfiguration configuration,IUserRepository userRepository,IDateTimeProvider dateTimeProvider,IRoleAndPermissionRepository roleAndPermissionRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _dateTimeProvider = dateTimeProvider;
            _roleAndPermissionRepository = roleAndPermissionRepository;
        }
        public string CreateAccessToken(User user,List<string> roles)
        {
            #region 老的Token生成方式
            //var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("ThisIsMySuperSecretKey1234567890123456"));
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            //    claims: new[]
            //    {
            //        new System.Security.Claims.Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            //        new System.Security.Claims.Claim(ClaimTypes.Name,user.UserName),
            //        new System.Security.Claims.Claim(ClaimTypes.Role,user.Role)
            //    },
            //    expires: DateTime.Now.AddHours(2),
            //    signingCredentials: creds
            //);
            //return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
            #endregion
            #region 新的Token生成方式，使用配置文件中的密钥和其他参数
            var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
                //new Claim(ClaimTypes.Role, user.Role)
        };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: _dateTimeProvider.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
            #endregion

        }

        public string CreateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public LoginResponseDto RefreshToken(string refreshToken)
        {
            var user = _userRepository.GetByRefreshToken(refreshToken);
            if (user == null || user.RefreshTokenExpiryTime < _dateTimeProvider.UtcNow)
            {
                throw new BusinessException(Domain.Enum.BusinessErrorCode.InvalidRefreshToken, "Token不存在或已过期");
            };
            var roles = _roleAndPermissionRepository.GetUserRoles(user.Id);
            var accesstoken = CreateAccessToken(user, roles);
            var newrefreshToken = CreateRefreshToken();
            user.RefreshToken = newrefreshToken;
            _userRepository.UpdateUser(user);
            return new LoginResponseDto()
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = accesstoken,
                RefreshToken = newrefreshToken
            };
        }
    }
}

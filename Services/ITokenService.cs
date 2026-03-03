using day1.Models;

namespace day1.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(User user,List<string> roles);
        string CreateRefreshToken();

        DTOs.LoginResponseDto RefreshToken(string refreshToken);
    }
}

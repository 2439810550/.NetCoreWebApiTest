using day1.Models;

namespace day1.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
        string CreateRefreshToken();

        DTOs.LoginResponseDto RefreshToken(string refreshToken);
    }
}

using day1.DTOs;

namespace day1.Services
{
    public interface IAuthenticationService
    {
        LoginResponseDto Login(CreateUserDTO createUserDTO);
    }
}

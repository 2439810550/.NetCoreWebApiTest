using day1.Models;
using day1.DTOs;

namespace day1.Services
{
    public interface IUserService
    {
        List<User> GetAllUsers();

        User CreateUser(DTOs.CreateUserDTO createUserDto);

        User? GetById(int id);

        DTOs.LoginResponseDto Login(DTOs.CreateUserDTO createUserDTO);

        void DeleteByUserName(string username);

        DTOs.LoginResponseDto RefreshToken(string refreshToken);


    }
}

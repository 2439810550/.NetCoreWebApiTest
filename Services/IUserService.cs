using day1.Models;
using day1.DTOs;

namespace day1.Services
{
    public interface IUserService
    {
        List<User> GetAllUsers();

        User CreateUser(DTOs.CreateUserDTO createUserDto);

        User? GetById(long id);

        DTOs.LoginResponseDto Login(DTOs.CreateUserDTO createUserDTO);

        void DeleteByUserName(string username);
        void DeleteByUserId(long userId);

        Task<PagedResult<UserListDto>> GetUsersPagedAsync(int page, int size, string? keyword);
        Task UpdateUserAsync(long id, UpdateUserDto dto);

    }
}

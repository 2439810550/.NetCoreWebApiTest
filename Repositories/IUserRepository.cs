using day1.DTOs;
using day1.Models;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore.Update.Internal;
namespace day1.Repositories
{
    public interface IUserRepository
    {
        CreateUserDTO Add(User user);
        List<User> GetAllUsers();
        User? GetById(long id);
        bool ExctisUserName(string userName);

        User? GetByUserName(string UserName);

        int DeleteByUserName(string username);
        void UpdateUser(User user);

        User? GetByRefreshToken(string refreshToken);

        User? GetByUserId(long userId);

        Task<(List<User> Users, int Total)> GetUsersPagedAsync(int page, int size, string? keyword);
    }
}

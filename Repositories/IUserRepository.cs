using day1.Models;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore.Update.Internal;
namespace day1.Repositories
{
    public interface IUserRepository
    {
        void Add(User user);
        List<User> GetAllUsers();
        User? GetById(long id);
        bool ExctisUserName(string userName);

        User? GetByUserName(string UserName);

        int DeleteByUserName(string username);
        void UpdateUser(User user);

        User? GetByRefreshToken(string refreshToken);

        List<string> GetUserRoles(long userid);
        List<string> GetByUserId();
    }
}

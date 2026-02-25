using day1.Models;
using Microsoft.AspNetCore.Routing.Constraints;
namespace day1.Repositories
{
    public interface IUserRepository
    {
        void Add(User user);
        List<User> GetAllUsers();
        User? GetById(int id);
        bool ExctisUserName(string userName);

        User? GetByUserName(string UserName);

        int DeleteByUserName(string username);
    }
}

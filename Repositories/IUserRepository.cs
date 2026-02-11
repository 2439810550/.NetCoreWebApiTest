using day1.Models;
namespace day1.Repositories
{
    public interface IUserRepository
    {
        void Add(User user);
        List<User> GetAllUsers();
        User? GetById(int id);
        bool ExctisUserName(string userName);
    }
}

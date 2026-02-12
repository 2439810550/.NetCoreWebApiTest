using day1.Data;
using day1.Models;
namespace day1.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public void Add(User user)
        {
            _context.Add(user);
            _context.SaveChanges();
        }

        public User? GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
        public bool ExctisUserName(string userName)
        {
            return _context.Users.Any(u => u.UserName == userName);
        }
        public User? GetByUserName(string UserName)
        {
            return _context.Users.FirstOrDefault(u => u.UserName==UserName);
        }
    }
}

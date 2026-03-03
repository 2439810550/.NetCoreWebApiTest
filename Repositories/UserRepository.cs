using day1.Data;
using day1.Models;
using Microsoft.EntityFrameworkCore;
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
            _context.Users.Add(user);
            _context.SaveChanges();
            //只允许注册用户角色 1管理员 2用户
            _context.UserRoles.Add(new UserRole {UserId=user.Id,RoleId=2 }) ;
            _context.SaveChanges();
        }

        public User? GetById(long id)
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

        public int DeleteByUserName(string username)
        {
            return _context.Users.Where(u=>u.UserName==username).ExecuteDelete();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public User? GetByRefreshToken(string refreshToken)
        {
            return _context.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
        }

        public List<string> GetUserRoles(long userid)
        {
            return _context.UserRoles.Where(u=>u.UserId==userid).Select(u => u.Role.Name).ToList();
        }

        public List<string> GetByUserId()
        {
            throw new NotImplementedException();
        }
    }
}

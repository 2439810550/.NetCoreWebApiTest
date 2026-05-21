using day1.Data;
using day1.DTOs;
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
        public CreateUserDTO Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            // 默认分配 User 角色
            var userRole = _context.Roles.FirstOrDefault(r => r.Name == "User");
            if (userRole != null)
            {
                _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = userRole.Id });
                _context.SaveChanges();
            }
            return new CreateUserDTO {UserName=user.UserName,PassWord=user.PassWord,CreatTime=user.CreateTime };
        }

        public User? GetById(long id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public async Task<(List<User> Users, int Total)> GetUsersPagedAsync(int page, int size, string? keyword)
        {
            var query = _context.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(u => u.UserName.Contains(keyword));
            }
            var total = await query.CountAsync();
            var users = await query
                .OrderByDescending(u => u.CreateTime)
                .Skip((page - 1) * size)
                .Take(size)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync();
            return (users, total);
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


        public User? GetByUserId(long userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }

    }
}


using day1.Data;
using day1.Models;
using Microsoft.EntityFrameworkCore;

namespace day1.Repositories
{
    public class RoleAndPermissionRepository : IRoleAndPermissionRepository
    {
        public readonly  AppDbContext _context;
        public RoleAndPermissionRepository(AppDbContext appDbContext) 
        {
            _context = appDbContext;
        }
        public List<string> GetUserRoles(long userid)
        {
            return _context.UserRoles.Where(u => u.UserId == userid)
                .Include(ur => ur.Role)
                .Select(u => u.Role.Name)
                .ToList();
        }
        /// <summary>
        /// 获取用户权限列表，通过用户的角色关联到权限表，返回权限名称列表
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<string> GetUserPemissions(long userid)
        {
            return _context.UserRoles.Where(ur => ur.UserId == userid)
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Code)
                .Distinct()
                .ToList();
        }

        public void DeleteRoleByUser(long userId, long roleid)
        {
           // 只删除指定用户和指定角色之间的关联
          _context.UserRoles.Where(ur => ur.UserId == userId && ur.RoleId == roleid).ExecuteDelete();
            
        }

        public void AddRoleByUser(long userId, long roleid)
        {
            _context.UserRoles.Add(new UserRole { RoleId = roleid, UserId = userId });
            _context.SaveChanges();
        }
    }
}

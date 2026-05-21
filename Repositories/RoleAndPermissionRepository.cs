
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

        public List<Role> GetAllRoles()
        {
            return _context.Roles.ToList();
        }

        public List<Permission> GetAllPermissions()
        {
            return _context.Permissions.ToList();
        }

        public Role? GetRoleById(long id)
        {
            return _context.Roles.Find(id);
        }

        public Role AddRole(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return role;
        }

        public void UpdateRole(Role role)
        {
            _context.Roles.Update(role);
            _context.SaveChanges();
        }

        public void DeleteRole(long id)
        {
            _context.Roles.Where(r => r.Id == id).ExecuteDelete();
        }

        public Permission? GetPermissionById(long id)
        {
            return _context.Permissions.Find(id);
        }

        public Permission AddPermission(Permission permission)
        {
            _context.Permissions.Add(permission);
            _context.SaveChanges();
            return permission;
        }

        public void UpdatePermission(Permission permission)
        {
            _context.Permissions.Update(permission);
            _context.SaveChanges();
        }

        public void DeletePermission(long id)
        {
            _context.Permissions.Where(p => p.Id == id).ExecuteDelete();
        }

        public List<Permission> GetRolePermissions(long roleId)
        {
            return _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission)
                .ToList();
        }

        public void AssignPermissionToRole(long roleId, long permissionId)
        {
            if (!_context.RolePermissions.Any(rp => rp.RoleId == roleId && rp.PermissionId == permissionId))
            {
                _context.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permissionId });
                _context.SaveChanges();
            }
        }

        public void RemovePermissionFromRole(long roleId, long permissionId)
        {
            _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && rp.PermissionId == permissionId)
                .ExecuteDelete();
        }
    }
}

using day1.Data;
using Microsoft.EntityFrameworkCore;

namespace day1.Models
{
    public class DBInitializer
    {
        public readonly AppDbContext _context;
        public DBInitializer(AppDbContext context)
        {
            _context = context;
        }

        public async Task InitializeAsync()
        {
            await SendRolesAsync();
            await SendPermissionAsync();
            await SendRolesPermissionAsync();
        }

        private async Task SendRolesPermissionAsync()
        {
            // 获取所有权限
            var allPermissions = await _context.Permissions.ToListAsync();
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");

            if (adminRole == null || userRole == null) return;

            // 获取当前已分配的权限ID集合
            var existingAdminPermissionIds = (await _context.RolePermissions
                .Where(rp => rp.RoleId == adminRole.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync()).ToHashSet();
            var existingUserPermissionIds = (await _context.RolePermissions
                .Where(rp => rp.RoleId == userRole.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync()).ToHashSet();

            // 需要为 Admin 添加的权限（所有未分配的）
            var adminPermissionsToAdd = allPermissions
                .Where(p => !existingAdminPermissionIds.Contains(p.Id))
                .Select(p => new RolePermission { RoleId = adminRole.Id, PermissionId = p.Id });

            // 需要为 User 添加的权限（包含 "Read" 且未分配的）
            var userPermissionsToAdd = allPermissions
                .Where(p => p.Code.Contains("Read") && !existingUserPermissionIds.Contains(p.Id))
                .Select(p => new RolePermission { RoleId = userRole.Id, PermissionId = p.Id });

            // 批量添加
            if (adminPermissionsToAdd.Any())
                await _context.RolePermissions.AddRangeAsync(adminPermissionsToAdd);

            if (userPermissionsToAdd.Any())
                await _context.RolePermissions.AddRangeAsync(userPermissionsToAdd);

            if (adminPermissionsToAdd.Any() || userPermissionsToAdd.Any())
                await _context.SaveChangesAsync();
        }

        private async Task SendPermissionAsync()
        {
            List<Permission> permissions = new List<Permission>
            {
                new Permission { Code = "User.Read",Name="用户读取" },
                new Permission { Code = "User.Write",Name="用户写入" },
                new Permission { Code = "User.Delete",Name="用户删除" },
                new Permission { Code = "User.Edit",Name="用户修改" }
            };
            var existingPermissions = (await _context.Permissions.Select(p=>p.Code).ToListAsync()).ToHashSet();
            var newPermissions = permissions.Where(p => !existingPermissions.Contains(p.Code)).ToList();
            if (newPermissions.Any())
            {
                await _context.Permissions.AddRangeAsync(newPermissions);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SendRolesAsync()
        {
            List<Role> roles = new List<Role>() 
            {
                new Role { Name = "Admin" },
                new Role { Name = "User" } 
            };
            var existingRoles = (await _context.Roles.Select(r => r.Name).ToListAsync()).ToHashSet();
            var newRoles=roles.Where(r => !existingRoles.Contains(r.Name)).ToList();
            if (newRoles.Any())
            {
                await _context.Roles.AddRangeAsync(newRoles);
                await _context.SaveChangesAsync();
            }
        }
    }
}

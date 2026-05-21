using day1.Models;
namespace day1.Repositories
{
    public interface IRoleAndPermissionRepository
    {

        List<string> GetUserRoles(long userid);
        List<string> GetUserPemissions(long userid);
        void DeleteRoleByUser(long userId,long roleid);
        void AddRoleByUser(long userId,long roleid);
        List<Role> GetAllRoles();
        List<Permission> GetAllPermissions();
        Role? GetRoleById(long id);
        Role AddRole(Role role);
        void UpdateRole(Role role);
        void DeleteRole(long id);
        Permission? GetPermissionById(long id);
        Permission AddPermission(Permission permission);
        void UpdatePermission(Permission permission);
        void DeletePermission(long id);
        List<Permission> GetRolePermissions(long roleId);
        void AssignPermissionToRole(long roleId, long permissionId);
        void RemovePermissionFromRole(long roleId, long permissionId);
    }
}

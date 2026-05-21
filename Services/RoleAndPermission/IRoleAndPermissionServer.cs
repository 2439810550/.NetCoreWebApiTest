namespace day1.Services.RoleAndPermission
{
    public interface IRoleAndPermissionServer
    {
        List<string> GetRoleList(long userid);
        List<string> GetPermissionListByUserId(long userid);
        void DeleteRoleByUser(long userId,long roleid);
        void AddRoleByUser(long userId, long roleid);
        List<Models.Role> GetAllRoles();
        List<Models.Permission> GetAllPermissions();
        Models.Role AddRole(Models.Role role);
        void UpdateRole(Models.Role role);
        void DeleteRole(long id);
        Models.Permission AddPermission(Models.Permission permission);
        void UpdatePermission(Models.Permission permission);
        void DeletePermission(long id);
        List<Models.Permission> GetRolePermissions(long roleId);
        void AssignPermissionToRole(long roleId, long permissionId);
        void RemovePermissionFromRole(long roleId, long permissionId);
    }
}


using day1.Repositories;

namespace day1.Services.RoleAndPermission
{
    public class RoleAndPermissionServer : IRoleAndPermissionServer
    {
        public readonly IRoleAndPermissionRepository _roleAndPermissionRepository;
        public RoleAndPermissionServer(IRoleAndPermissionRepository roleAndPermissionRepository)
        {
            _roleAndPermissionRepository = roleAndPermissionRepository;
        }
        /// <summary>
        /// 删除用户的所有角色关联关系，接受用户ID作为参数，在数据库中删除用户与角色的关联关系
        /// </summary>
        /// <param name="userId"></param>
        public void DeleteRoleByUser(long userId, long roleid)
        {
            _roleAndPermissionRepository.DeleteRoleByUser(userId, roleid);
        }

        public void AddRoleByUser(long userId, long roleid)
        {
            _roleAndPermissionRepository.AddRoleByUser(userId, roleid);
        }

        public List<string> GetPermissionListByUserId(long userid)
        {
            return _roleAndPermissionRepository.GetUserPemissions(userid);
        }

        public List<string> GetRoleList(long userid)
        {
            return _roleAndPermissionRepository.GetUserRoles(userid);
        }

        public List<Models.Role> GetAllRoles()
        {
            return _roleAndPermissionRepository.GetAllRoles();
        }

        public List<Models.Permission> GetAllPermissions()
        {
            return _roleAndPermissionRepository.GetAllPermissions();
        }

        public Models.Role AddRole(Models.Role role)
        {
            return _roleAndPermissionRepository.AddRole(role);
        }

        public void UpdateRole(Models.Role role)
        {
            _roleAndPermissionRepository.UpdateRole(role);
        }

        public void DeleteRole(long id)
        {
            _roleAndPermissionRepository.DeleteRole(id);
        }

        public Models.Permission AddPermission(Models.Permission permission)
        {
            return _roleAndPermissionRepository.AddPermission(permission);
        }

        public void UpdatePermission(Models.Permission permission)
        {
            _roleAndPermissionRepository.UpdatePermission(permission);
        }

        public void DeletePermission(long id)
        {
            _roleAndPermissionRepository.DeletePermission(id);
        }

        public List<Models.Permission> GetRolePermissions(long roleId)
        {
            return _roleAndPermissionRepository.GetRolePermissions(roleId);
        }

        public void AssignPermissionToRole(long roleId, long permissionId)
        {
            _roleAndPermissionRepository.AssignPermissionToRole(roleId, permissionId);
        }

        public void RemovePermissionFromRole(long roleId, long permissionId)
        {
            _roleAndPermissionRepository.RemovePermissionFromRole(roleId, permissionId);
        }
    }
}

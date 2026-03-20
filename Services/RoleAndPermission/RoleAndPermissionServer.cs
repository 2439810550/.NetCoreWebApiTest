
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
    }
}

namespace day1.Services.RoleAndPermission
{
    public interface IRoleAndPermissionServer
    {
        List<string> GetRoleList(long userid);
        /// <summary>
        /// 获取用户权限列表，通过用户的角色关联到权限表，返回权限名称列表
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        List<string> GetPermissionListByUserId(long userid);

        /// <summary>
        /// 删除账户下角色 关联关系，接受用户ID作为参数，在数据库中删除用户与角色的关联关系
        /// </summary>
        /// <param name="userId"></param>
        void DeleteRoleByUser(long userId,long roleid);
        
        /// <summary>
        /// 为用户分配角色，接受用户ID和角色ID作为参数，在数据库中添加用户与角色的关联关系
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleid"></param>
        void AddRoleByUser(long userId, long roleid);

    }
}

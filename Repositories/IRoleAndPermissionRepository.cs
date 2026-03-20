using day1.Models;
namespace day1.Repositories
{
    public interface IRoleAndPermissionRepository
    {

        List<string> GetUserRoles(long userid);
        List<string> GetUserPemissions(long userid);
        void DeleteRoleByUser(long userId,long roleid);
        void AddRoleByUser(long userId,long roleid);
    }
}

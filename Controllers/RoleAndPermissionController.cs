using day1.Services.RoleAndPermission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace day1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleAndPermissionController : ControllerBase
    {
        public readonly IRoleAndPermissionServer _roleAndPermissionServer;
        public RoleAndPermissionController(IRoleAndPermissionServer roleAndPermissionServer)
        {
            _roleAndPermissionServer = roleAndPermissionServer;
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("roles")]
        public IActionResult DeleteRolesByUserId(long userid, long roleid)
        {
            _roleAndPermissionServer.DeleteRoleByUser(userid, roleid);
            return Ok("删除成功!");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("adduserroles")]
        public IActionResult AddRolesByUserId(long userid, long roleid)
        {
            _roleAndPermissionServer.AddRoleByUser(userid, roleid);
            return Ok("添加成功!");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("roles/{userid}")]
        public IActionResult GetUserRoles(long userid)
        {
            var roles = _roleAndPermissionServer.GetRoleList(userid);
            return Ok(roles);
        }
        [HttpGet("getpermissions/{userid}")]
        public IActionResult GetUserPermissions(long userid)
        {
            var permissions = _roleAndPermissionServer.GetPermissionListByUserId(userid);
            return Ok(permissions);
        }
    }
}
        

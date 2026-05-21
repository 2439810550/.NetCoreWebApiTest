using System.Security.Claims;
using day1.Models;
using day1.Services.RoleAndPermission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace day1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleAndPermissionController : ControllerBase
    {
        public readonly IRoleAndPermissionServer _roleAndPermissionServer;
        public RoleAndPermissionController(IRoleAndPermissionServer roleAndPermissionServer)
        {
            _roleAndPermissionServer = roleAndPermissionServer;
        }
        // ==================== 用户-角色关联 ====================
        [HttpDelete("roles")]
        public IActionResult DeleteRolesByUserId(long userid, long roleid)
        {
            _roleAndPermissionServer.DeleteRoleByUser(userid, roleid);
            return Ok("删除成功!");
        }
        [HttpPost("adduserroles")]
        public IActionResult AddRolesByUserId(long userid, long roleid)
        {
            _roleAndPermissionServer.AddRoleByUser(userid, roleid);
            return Ok("添加成功!");
        }
        [Authorize]
        [HttpGet("roles/{userid}")]
        public IActionResult GetUserRoles(long userid)
        {
            if (GetCurrentUserId() != userid && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            var roles = _roleAndPermissionServer.GetRoleList(userid);
            return Ok(roles);
        }
        [Authorize]
        [HttpGet("getpermissions/{userid}")]
        public IActionResult GetUserPermissions(long userid)
        {
            if (GetCurrentUserId() != userid && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            var permissions = _roleAndPermissionServer.GetPermissionListByUserId(userid);
            return Ok(permissions);
        }

        private long GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return claim != null ? long.Parse(claim) : throw new UnauthorizedAccessException();
        }

        // ==================== 角色 CRUD ====================
        [HttpGet("roles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleAndPermissionServer.GetAllRoles();
            return Ok(roles);
        }
        [HttpPost("role")]
        public IActionResult AddRole([FromBody] Role role)
        {
            var created = _roleAndPermissionServer.AddRole(role);
            return Ok(created);
        }
        [HttpPut("role")]
        public IActionResult UpdateRole([FromBody] Role role)
        {
            _roleAndPermissionServer.UpdateRole(role);
            return Ok("更新成功!");
        }
        [HttpDelete("role/{id}")]
        public IActionResult DeleteRole(long id)
        {
            _roleAndPermissionServer.DeleteRole(id);
            return Ok("删除成功!");
        }

        // ==================== 权限 CRUD ====================
        [HttpGet("permissions")]
        public IActionResult GetAllPermissions()
        {
            var permissions = _roleAndPermissionServer.GetAllPermissions();
            return Ok(permissions);
        }
        [HttpPost("permission")]
        public IActionResult AddPermission([FromBody] Permission permission)
        {
            var created = _roleAndPermissionServer.AddPermission(permission);
            return Ok(created);
        }
        [HttpPut("permission")]
        public IActionResult UpdatePermission([FromBody] Permission permission)
        {
            _roleAndPermissionServer.UpdatePermission(permission);
            return Ok("更新成功!");
        }
        [HttpDelete("permission/{id}")]
        public IActionResult DeletePermission(long id)
        {
            _roleAndPermissionServer.DeletePermission(id);
            return Ok("删除成功!");
        }

        // ==================== 角色-权限关联 ====================
        [HttpGet("role/{roleId}/permissions")]
        public IActionResult GetRolePermissions(long roleId)
        {
            var permissions = _roleAndPermissionServer.GetRolePermissions(roleId);
            return Ok(permissions);
        }
        [HttpPost("role/assignPermission")]
        public IActionResult AssignPermissionToRole(long roleId, long permissionId)
        {
            _roleAndPermissionServer.AssignPermissionToRole(roleId, permissionId);
            return Ok("分配成功!");
        }
        [HttpDelete("role/removePermission")]
        public IActionResult RemovePermissionFromRole(long roleId, long permissionId)
        {
            _roleAndPermissionServer.RemovePermissionFromRole(roleId, permissionId);
            return Ok("移除成功!");
        }
    }
}

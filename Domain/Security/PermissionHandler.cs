using day1.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace day1.Domain.Security
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRoleAndPermissionRepository _roleAndPermissionRepository;
        public PermissionHandler(IRoleAndPermissionRepository roleAndPermissionRepository, IHttpContextAccessor contextAccessor)
        {
            _roleAndPermissionRepository = roleAndPermissionRepository;
            _contextAccessor = contextAccessor;
        }
        /// <summary>
        /// 处理权限验证逻辑，首先从用户的 Claims 中获取用户 ID，然后从 HttpContext.Items 中尝试获取用户的权限列表。如果没有找到，则从数据库中查询用户的权限并缓存到 HttpContext.Items 中。最后检查用户的权限列表是否包含所需的权限，如果包含则授权成功，否则授权失败。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userid = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userid == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            var httpcontext = _contextAccessor.HttpContext;
            if (!httpcontext.Items.TryGetValue("UserPerMissions", out var permissionsObj))
            {
                var permissions = _roleAndPermissionRepository.GetUserPemissions(long.Parse(userid));
                httpcontext.Items["UserPerMissions"] = permissions;
                permissionsObj = permissions;
            }
            var permissionList = (IEnumerable<string>)permissionsObj;
            if (permissionList != null && permissionList.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}

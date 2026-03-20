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

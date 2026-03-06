using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace day1.Domain.Security
{
    public class PermissionRequirement:IAuthorizationRequirement
    {
        /// <summary>
        /// 需要的权限名称，可以是一个字符串，表示用户需要具备的权限标识
        /// </summary>
        public string Permission { get; }
        /// <summary>
        /// 指定使用哪个认证方案来验证用户的身份，默认为JwtBearerDefaults.AuthenticationScheme
        /// </summary>
        //public string AuthenticationScheme { get; }
        //public PermissionRequirement(string permission, string authenticationScheme)
        //{
        //    Permission = permission;
        //    AuthenticationScheme = authenticationScheme;
        //}
        public PermissionRequirement(string permission)
        {
            Permission= permission;
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace day1.Domain.Security
{
    public static class AuthorizationExtensions
    {
        public static void AddPermissionPolicies(this AuthorizationOptions options)
        {
            // 获取Permissions类的所有嵌套类型
            var nestedTypes = typeof(Permissions).GetNestedTypes();
            
            foreach (var nestedType in nestedTypes)
            {
                // 获取嵌套类型中的所有公共静态常量字段
                var fields = nestedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                
                foreach (var field in fields)
                {
                    // 检查字段是否是常量且类型为string
                    if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                    {
                        // 获取权限值
                        var permission = field.GetValue(null) as string;
                        
                        // 注册权限策略
                        if (!string.IsNullOrEmpty(permission))
                        {
                            options.AddPolicy(permission, policy =>
                                policy.Requirements.Add(new PermissionRequirement(permission)));
                        }
                    }
                }
            }
        }
    }
}
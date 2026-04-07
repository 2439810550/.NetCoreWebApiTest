using day1.Repositories;
using day1.Services;
using day1.Services.RoleAndPermission;
using day1.Interfaces;
using day1.DYSdk.DYApi;
using Microsoft.AspNetCore.Authorization;
using day1.Domain.Security;
using day1.Models;
using day1.Common;

namespace day1.Extensions
{
    /// <summary>
    /// 服务集合扩展方法，用于批量注册依赖注入服务
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 批量注册应用程序服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // 注册 Repository 层服务
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleAndPermissionRepository, RoleAndPermissionRepository>();
            
            // 注册 Service 层服务
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRoleAndPermissionServer, RoleAndPermissionServer>();
            services.AddScoped<IDouYinVideoApiService, DouYinVideoApiService>();
            
            // 注册其他服务
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<DYVideoServer>();
            services.AddScoped<DBInitializer>();
            
            return services;
        }
    }
}

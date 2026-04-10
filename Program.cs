using day1.Services;
using Microsoft.EntityFrameworkCore;
using day1.Data;
using day1.Repositories;
using day1.DTOs;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using day1.Middlewares;
using Serilog;
using day1.Filter;
using day1.Common;
using day1.Interfaces;
using Microsoft.AspNetCore.Mvc;
using day1.DYSdk.DYApi;
using day1.Models;
using day1.Domain.Security;
using Microsoft.AspNetCore.Authorization;
using day1.Services.RoleAndPermission;
using day1.Services.Dish;
using Microsoft.Extensions.FileProviders;
var builder = WebApplication.CreateBuilder(args);
/// 配置 JWT 认证密钥
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("未配置 Jwt:Key");
var key = Encoding.UTF8.GetBytes(jwtKey);
//var key =Encoding.UTF8.GetBytes("ThisIsMySuperSecretKey1234567890123456");
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueCors", policy =>
    {
        policy
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
/// 注册 UserService 和 UserRepository 服务依赖注入
builder.Services.AddScoped<IUserService, UserService>();
/// 配置 Entity Framework Core 使用 SQL Server 数据库，并注册 AppDbContext 服务依赖注入
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
/// 注册 UserRepository 服务依赖注入
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleAndPermissionRepository, RoleAndPermissionRepository>();
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped<IRoleAndPermissionServer, RoleAndPermissionServer>();
builder.Services.AddScoped<IDishRepository,DishRepository>();
builder.Services.AddScoped<IDishServer,DishServer>();
/// 配置 FluentValidation自动注册 CreateUserDtoValidator
builder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateUserDtoValidator>());
/// 配置 JWT 认证服务，默认认证方案为 JWT Bearer，使用 JWT Bearer 选项，进行 Token 验证配置
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});
/// 配置权限策略，使用 AddPermissionPolicies 扩展方法自动注册所有权限策略
builder.Services.AddAuthorization(options =>
{
    options.AddPermissionPolicies();
});
/// 注册 PermissionHandler 服务依赖注入，用于权限验证，确保使用该处理器验证用户权限
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

// 向容器添加服务
builder.Services.AddControllers();
// 配置 Swagger/OpenAPI 的说明：https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
/// 配置 Swagger 并支持 JWT 认证
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "请输入 Bearer + 空格 + Token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
/// 配置 Serilog 日志记录器
builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.Console();
    config.WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
});
/// 全局注册 ApiResponseFilter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseFilter>();
});
builder.Services.AddSingleton<IDateTimeProvider,DateTimeProvider>();
/// 配置 ApiBehaviorOptions，禁用默认的模型验证响应，以便使用自定义的 ApiResponseFilter 处理模型验证错误
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
/// 全局注册 ValidationFilter，以便在模型验证失败时抛出自定义的 BusinessException 异常
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
builder.Services.AddHttpClient<IDouYinVideoApiService, DouYinVideoApiService>(client => 
{
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped<DYVideoServer>();
builder.Services.AddScoped<DBInitializer>();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
/// 全局使用自定义异常处理中间件
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("VueCors");
/// 启用认证和授权中间件
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


/// 应用启动时执行一次初始化，自动创建数据库，确保数据库结构正确
using (var scope = app.Services.CreateScope())
{
   
    scope.ServiceProvider.GetRequiredService<DBInitializer>().InitializeAsync().Wait();

}
app.UseStaticFiles();
app.Run();


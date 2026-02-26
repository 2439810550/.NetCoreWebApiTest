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
var builder = WebApplication.CreateBuilder(args);
///配置 JWT 认证服务
var key =Encoding.UTF8.GetBytes("ThisIsMySuperSecretKey1234567890123456");
///注册 UserService 和 UserRepository 到依赖注入容器中
builder.Services.AddScoped<IUserService, UserService>();
///配置 Entity Framework Core 使用 SQL Server 数据库，并注册 AppDbContext 到依赖注入容器中
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
///注册 UserRepository 到依赖注入容器中
builder.Services.AddScoped<IUserRepository, UserRepository>();
///配置 FluentValidation，自动注册 CreateUserDtoValidator
builder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateUserDtoValidator>());
///配置 JWT 认证服务，设置默认认证方案为 JWT Bearer，并配置 JWT Bearer 选项，包括 Token 验证参数
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

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
///配置 Swagger 以支持 JWT 认证
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
///配置 Serilog 日志记录器
builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.Console();
    config.WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
});
///全局注册 ApiResponseFilter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseFilter>();
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
///全局使用自定义异常处理中间件
app.UseMiddleware<ExceptionMiddleware>();
///启用认证和授权中间件
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();

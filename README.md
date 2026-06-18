# Q&L Kitchen - 后端 API

基于 ASP.NET Core 8 的后端服务，集成多平台短视频去水印解析。

## 功能

- 多平台去水印 API：自动提取分享口令中的链接，调用 media-parser 解析
- 视频/图片代理下载（防盗链绕过）
- 用户认证与 JWT Token 管理
- 点餐系统（菜品管理、订单管理）
- 角色权限管理（RBAC）
- Serilog 日志记录
- Swagger API 文档

## 技术栈

- **.NET 8** / ASP.NET Core Web API
- **Entity Framework Core 8** + SQL Server
- **JWT Bearer** 认证
- **FluentValidation** 参数校验
- **Serilog** 日志
- **Swashbuckle** Swagger

## 本地开发

```bash
# 修改 appsettings.json 中的数据库连接字符串
dotnet run           # 启动 (localhost:5272)
dotnet build         # 编译
dotnet publish -c Release  # 发布
```

## 依赖服务

- **SQL Server**：数据库
- **media-parser**：短视频去水印解析服务 (localhost:8051)
- **Clash Verge**（可选）：解析 X/Twitter、YouTube 等外网平台时需代理

## API 接口

| 路由 | 说明 |
|------|------|
| `GET /api/DYVideo?videoUrl=...` | 去水印解析 |
| `GET /api/DYVideo/proxy?url=...&download=true` | 视频/图片代理 |
| `POST /api/Auth/login` | 用户登录 |
| `GET /api/Auth/users` | 用户列表 |
| `/api/RoleAndPermission/*` | 角色权限管理 |
| `/swagger` | API 文档 |

## 部署

发布后与前端（wwwroot）合一部署到 IIS，端口 5272。

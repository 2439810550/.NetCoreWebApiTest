# 鸽子摇修复 — 上下文摘要

**日期：** 2026-06-12
**项目：** Q&L 厨房 (ASP.NET Core 8 + Vue 3 + Element Plus)
**服务器：** 47.99.133.151:5273 (IIS in-process, 自签名 SSL)

---

## 1. 问题描述

点击侧边栏「🐦 鸽子摇」→ 要求登录 token → 页面空白

**预期行为：** 鸽子摇是公共页面，不需要登录即可访问

---

## 2. 根本原因

### 原因 1：中间件管道顺序错误
之前的 `Program.cs` 尝试用 `UseWhen`/`Map` 为 `/pigeon-shake/` 创建特殊分支，但分支放在了认证中间件之后。另外 `app.Map` 存在重载冲突（ASP.NET Core 的 `WebApplication` 同时实现了 `IApplicationBuilder.Map` 和 `IEndpointRouteBuilder.Map`）。

### 原因 2：AI 模型文件缺失
`wwwroot/pigeon-shake/assets/models/` 目录不存在，CDN（jsdelivr/unpkg）在中文服务器上经常被墙，导致 face-api.js 加载失败。

### 原因 3：库/模型格式不兼容
原 CDN 配置加载 `face-api.js@0.22.2`（原始库）但模型来自 `@vladmandic/face-api`（fork），两者文件格式不同（`.shard1` vs `.bin`）。

---

## 3. 已完成的修复

### 修复 1：Program.cs 中间件管道（最终版）

```csharp
// 180-195 行 — 这是正确的最终版本
app.UseCors("VueCors");

// 静态文件 + 默认文档（必须在认证和控制器之前）
// /pigeon-shake/ → /pigeon-shake/index.html 由 UseDefaultFiles 自动处理
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SPA 回退
app.MapFallbackToFile("index.html");
```

**关键点：** `UseDefaultFiles` + `UseStaticFiles` 在认证之前运行，所以 `/pigeon-shake/` 下的所有文件无需认证即可访问。零特殊分支。

### 修复 2：本地 AI 模型文件

已下载到 `wwwroot/pigeon-shake/assets/models/`：
```
face-api.min.js                              (1.3 MB)  ← @vladmandic/face-api@1.7.15
tiny_face_detector_model.bin                 (189 KB)
tiny_face_detector_model-weights_manifest.json (3.2 KB)
face_landmark_68_tiny_model.bin              ( 76 KB)
face_landmark_68_tiny_model-weights_manifest.json (4.8 KB)
```

库和模型来自同一 fork，格式兼容。

### 修复 3：Vue 前端路由确认（已验证无需改动）

- `src/router/index.ts`: `/pigeonshake` 已在 `publicPaths` 数组中
- `src/layout/DLLayout.vue`: 菜单项指向 `/pigeonshake`
- `src/views/PigeonShake.vue`: iframe 加载 `/pigeon-shake/`

---

## 4. 部署包

📦 `D:\1\projectExcel\qianhou\qianhou-deploy-package.zip` (7.0 MB)

## 5. 部署步骤

```powershell
# 1. 上传 qianhou-deploy-package.zip 到服务器
# 2. RDP 到 47.99.133.151
# 3. 停止 IIS 应用池（IIS 管理器 → Application Pools → 找到对应池 → Stop）
# 4. 删除旧文件（保留 appsettings.json 等配置文件）
# 5. 解压到 publish 目录
# 6. 启动应用池
# 7. 清除浏览器缓存后访问 https://47.99.133.151:5273/
```

## 6. 验证清单

- [ ] 不登录，访问首页正常显示
- [ ] 点击「🐦 鸽子摇」，不弹出登录窗口
- [ ] 鸽子摇页面显示（iframe 加载成功）
- [ ] 摄像头授权后能正常工作
- [ ] 人脸检测功能正常
- [ ] 浏览器 F12 Console 无 404/401 错误

## 7. 关键文件路径

| 文件 | 路径 |
|------|------|
| 后端入口 | `D:\1\projectExcel\qianhou\day1\Program.cs` |
| IIS 配置 | `D:\1\projectExcel\qianhou\day1\publish\web.config` |
| Vue 路由 | `D:\1\projectExcel\qianhou\vue-project-gl\src\router\index.ts` |
| 布局组件 | `D:\1\projectExcel\qianhou\vue-project-gl\src\layout\DLLayout.vue` |
| 鸽子摇页面 | `D:\1\projectExcel\qianhou\vue-project-gl\src\views\PigeonShake.vue` |
| 鸽子摇 HTML | `D:\1\projectExcel\qianhou\day1\wwwroot\pigeon-shake\index.html` |
| 人脸检测 JS | `D:\1\projectExcel\qianhou\day1\wwwroot\pigeon-shake\js\faceDetector.js` |
| AI 模型 | `D:\1\projectExcel\qianhou\day1\wwwroot\pigeon-shake\assets\models\` |

## 8. 如果问题仍存在

1. 打开浏览器 F12 → Console 查看错误
2. 打开 Network 标签 → 检查 `/pigeon-shake/` 请求返回的状态码
3. 检查 IIS 日志（`C:\inetpub\logs\LogFiles\`）
4. 如果 `/pigeon-shake/` 返回 302 重定向到登录页 → 说明中间件配置未生效（publish 目录未更新）
5. 如果 `/pigeon-shake/` 返回 200 但页面空白 → 检查 Console 是否有 face-api.js CDN 加载错误

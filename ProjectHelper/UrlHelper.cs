namespace day1.ProjectHelper
{
    public static class UrlHelper
    {
        /// <summary>
        /// 将相对路径转换为完整 URL（基于当前请求）
        /// </summary>
        public static string ToFullUrl(this HttpRequest request, string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return relativePath;
            if (relativePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return relativePath; // 已经是完整 URL

            // 确保相对路径以 / 开头
            if (!relativePath.StartsWith("/")) relativePath = "/" + relativePath;

            return $"{request.Scheme}://{request.Host}{relativePath}";
        }
    }
}

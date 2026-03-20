namespace day1.Models
{
    public class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }

        public string PassWord { get; set; }

        /// <summary>
        /// 用户创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public string? RefreshToken { get; set; }
        /// <summary>
        /// 刷新令牌过期时间
        /// </summary>
        public DateTime? RefreshTokenExpiryTime { get; set; }

        /// <summary>
        /// 连续登录失败次数
        /// </summary>
        public int FailedLoginCount { get; set; }
        /// <summary>
        /// 账户锁定结束时间，如果当前时间小于该时间，则账户处于锁定状态
        /// </summary>
        public DateTime? LockOutEndTime { get; set; }
        
        /// <summary>
        /// 
        /// 
        /// 用户角色关联
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}

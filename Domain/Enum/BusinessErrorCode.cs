namespace day1.Domain.Enum
{
    public enum BusinessErrorCode
    {
        //用户名或密码错误
        UserNameOrPasswordError = 1000,
        //刷新token失败
        InvalidRefreshToken = 1001,
        //用户不存在
        UserNotFound = 1002,
        //密码错误
        PasswordError = 1003,
        //用户已存在
        UserAlreadyExists = 1004,
        //不能删除管理员用户
        NotDeleteAdminUser=1005,
        /// <summary>
        /// 账户被锁定，连续登录失败次数超过限制，账户被暂时锁定一段时间
        /// </summary>
        AccountLocked = 1006,
        /// <summary>
        /// 模型验证错误，输入的数据不符合要求，例如用户名或密码格式不正确，或者其他参数验证失败
        /// </summary>
        ValidationError = 1007
    }
}

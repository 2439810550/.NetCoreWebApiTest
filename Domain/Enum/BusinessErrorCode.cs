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
        NotDeleteAdminUser=1005
    }
}

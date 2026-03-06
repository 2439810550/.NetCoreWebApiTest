namespace day1.Domain.Security
{
    public static class Permissions
    {
        // 用户相关权限
        public static class User
        {
            public const string Read = "User.Read";
            public const string Create = "User.Create";
            public const string Update = "User.Update";
            public const string Delete = "User.Delete";
        }
        
        // 其他模块权限可以在这里添加
        public static class Order
        {
            public const string Read = "Order.Read";
            public const string Create = "Order.Create";
            public const string Update = "Order.Update";
            public const string Delete = "Order.Delete";
        }
    }
}
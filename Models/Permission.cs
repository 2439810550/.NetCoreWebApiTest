namespace day1.Models
{
    public class Permission
    {
        public long Id { get; set; }
        public string Code { get; set; } = null!;   // 比如：User.Create
        public string Name { get; set; } = null!;

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}

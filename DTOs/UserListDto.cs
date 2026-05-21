namespace day1.DTOs
{
    public class UserListDto
    {
        public long Id { get; set; }
        public string UserName { get; set; } = null!;
        public DateTime CreateTime { get; set; }
        public List<string> Roles { get; set; } = new();
        public int FailedLoginCount { get; set; }
        public DateTime? LockOutEndTime { get; set; }
    }

    public class UpdateUserDto
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public List<string>? Roles { get; set; }
    }
}

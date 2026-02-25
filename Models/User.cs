namespace day1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string PassWord { get; set; }

        public string Role { get; set; } = "User";

        public DateTime CreateTime { get; set; }
    }
}

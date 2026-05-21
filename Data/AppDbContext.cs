using Microsoft.EntityFrameworkCore;
using day1.Models;


namespace day1.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Order_item> Order_Items { get; set; }
        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // 1. Order 与 User 一对多（一个用户多个订单）
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);   // 防止用户被删时订单级联删除

            modelBuilder.Entity<Order_item>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);    // 订单删除时级联删除订单项

            modelBuilder.Entity<Order_item>()
               .HasOne(oi => oi.Dish)
               .WithMany(d => d.OrderItems)
               .HasForeignKey(oi => oi.DishId)
               .OnDelete(DeleteBehavior.Restrict);   // 菜品被删时不删除订单项

            modelBuilder.Entity<Order>()
               .HasIndex(o => o.Order_No)
               .IsUnique();
            
            modelBuilder.Entity<Cart>().HasOne(oi=>oi.User).WithMany(o=>o.Carts).HasForeignKey(oi=>oi.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Cart>().HasOne(oi => oi.Dish).WithMany(o => o.Carts).HasForeignKey(oi => oi.DishId).OnDelete(DeleteBehavior.Restrict);

            //复合唯一索引：同一用户对同一菜品只能有一条购物车记录
            modelBuilder.Entity<Cart>()
                .HasIndex(c => new { c.UserId, c.DishId })
                .IsUnique();
        }
    }
}

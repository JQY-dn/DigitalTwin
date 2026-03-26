using DigitalTwin.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Infrastructure.Tools
{
    /// <summary>
    /// EF Core 数据库上下文。
    /// 新增实体时：
    ///   1. 在此文件添加 DbSet&lt;T&gt;
    ///   2. 运行 CLI：dotnet ef migrations add &lt;MigrationName&gt;
    ///   3. 运行 CLI：dotnet ef database update
    /// </summary>
    public class AppDbContext : DbContext
    {
        // ── DbSet（每张表对应一个） ───────────────────────────────────────────────
        // 示例：public DbSet<Product> Products => Set<Product>();
        // 在此处添加你的实体 ↓

        // ← 加这行
        public DbSet<Equipment> Equipments { get; set; }



        // ── 构造函数 ─────────────────────────────────────────────────────────────

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ── 模型配置 ─────────────────────────────────────────────────────────────

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 自动扫描当前程序集中所有继承 IEntityTypeConfiguration<T> 的配置类
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }

    // ── Design-Time 工厂（CLI 迁移用）────────────────────────────────────────────
    // dotnet ef migrations add / database update 时会调用此工厂创建 DbContext
    // 不需要运行时 DI，只需要提供连接字符串即可

    /// <summary>
    /// 仅供 EF CLI 工具使用，不参与运行时 DI。
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // ⚠️ 仅用于 CLI 迁移，请替换为你的开发连接字符串
            optionsBuilder.UseSqlServer(
                ConnectionStringProvider.Get());

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

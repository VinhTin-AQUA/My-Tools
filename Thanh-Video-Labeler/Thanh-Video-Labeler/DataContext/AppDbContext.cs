using Microsoft.EntityFrameworkCore;
using Thanh_Video_Labeler.DataContext.Configuartions;
using Thanh_Video_Labeler.Models;

namespace Thanh_Video_Labeler.DataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<VideoInfo> VideoInfo { get; set; }
        public DbSet<Config>  Config { get; set; }
        public DbSet<Sheet>  Sheet { get; set; }
        
        // AWS
        public DbSet<VideoAwsInfo>  VideoAwsInfo { get; set; }
        public DbSet<VideoAwsConfig>  VideoAwsConfig { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new VideoInfoConfiguration());
            
            base.OnModelCreating(modelBuilder);
            
            ConfigureIdentityTables(modelBuilder);
            ConfigDefaultValues(modelBuilder);
        }
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken =
            default)
        {
            // var now = DateTime.Now;
            // var currentUserId = GetCurrentUserId();
            // foreach (var entry in ChangeTracker.Entries<Entity>())
            // {
            //     if (entry.State == EntityState.Added)
            //     {
            //         entry.Entity.CreatedAt = now;
            //         entry.Entity.CreatedBy = currentUserId;
            //     }
            //     else if (entry.State == EntityState.Modified)
            //     {
            //         entry.Entity.LatestUpdatedAt = now;
            //         entry.Entity.LatestUpdatedBy = currentUserId;
            //     }
            // }
            return await base.SaveChangesAsync(cancellationToken);
        }
        
        private void ConfigDefaultValues(ModelBuilder modelBuilder)
        {
            // foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            // foreach (var property in entityType.GetProperties())
            // {
            //     // Skip primary key properties
            //     if (property.IsPrimaryKey() || property.IsForeignKey())
            //         continue;
            //     // Check if the property is explicitly marked as [Required]
            //     var isRequired =
            //         property.PropertyInfo?.GetCustomAttributes(typeof(RequiredAttribute), false).Length != 0;
            //     // Set default values for properties that are not required
            //     switch (isRequired)
            //     {
            //         case false when property.ClrType == typeof(Guid):
            //             property.SetDefaultValue(Guid.Empty);
            //             break;
            //         case false when property.ClrType == typeof(string):
            //             property.SetDefaultValue(string.Empty);
            //             break;
            //         case false when property.ClrType == typeof(bool):
            //             property.SetDefaultValue(false);
            //             break;
            //         case false when property.ClrType == typeof(int):
            //             property.SetDefaultValue(0);
            //             break;
            //         case false when property.ClrType == typeof(double):
            //             property.SetDefaultValue(0.0);
            //             break;
            //         case false when property.ClrType == typeof(decimal):
            //             property.SetDefaultValue(0.0m);
            //             break;
            //         case false when property.ClrType == typeof(DateTime):
            //             property.SetDefaultValueSql("CURRENT_TIMESTAMP");
            //             break;
            //     }
            // }
            // // Apply Fluent API configurations from the current assembly
            // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        
        private void ConfigureIdentityTables(ModelBuilder modelBuilder)
        {
            // Đổi tên bảng Users và Roles
            // modelBuilder.Entity<ApplicationUser>().ToTable("Identity_Users");
            // modelBuilder.Entity<ApplicationRole>().ToTable("Identity_Roles");
            // // Loại bỏ các bảng Identity không sử dụng
            // modelBuilder.Ignore<IdentityUserClaim<Guid>>();
            // modelBuilder.Ignore<IdentityUserRole<Guid>>();
            // modelBuilder.Ignore<IdentityUserLogin<Guid>>();
            // modelBuilder.Ignore<IdentityRoleClaim<Guid>>();
            // modelBuilder.Ignore<IdentityUserToken<Guid>>();
        }
        
        private Guid GetCurrentUserId()
        {
            // var userId =
            //     _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            // if (Guid.TryParse(userId, out var guid))
            //     return guid;
            return Guid.Empty;
        }
    }
}
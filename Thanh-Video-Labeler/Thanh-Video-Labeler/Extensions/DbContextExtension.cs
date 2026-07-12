using Microsoft.EntityFrameworkCore;
using Thanh_Video_Labeler.DataContext;

namespace Thanh_Video_Labeler.Extensions
{
    public static class DbContextExtension
    {
        public static IServiceCollection AddSqliteDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
            
            // services.AddDbContextPool<AppDbContext>(options =>
            //     options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
            
            return services;
        }

        public static void StartMigrationPending(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate(); 
        }
    }
}
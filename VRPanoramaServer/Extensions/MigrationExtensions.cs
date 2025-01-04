
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using VRPanoramaServer.Database;

namespace VRPanoramaServer.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }
    }
}

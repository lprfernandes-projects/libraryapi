
using library.minimalapi.Contracts;
using library.minimalapi.Data;
using library.minimalapi.Services;
using Microsoft.EntityFrameworkCore;

namespace library.minimalapi.Extensions;

public static class BuilderExtensions
{
    public static void InstallServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddValidation();
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            var databasePath = Path.Combine(
                builder.Environment.ContentRootPath,
                "library.db");

            options.UseSqlite($"Data Source={databasePath}");
        });

        builder.Services.AddScoped<IBookService, BookService>();
    }
}
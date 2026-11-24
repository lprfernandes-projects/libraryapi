
using library.minimalapi.Contracts;
using library.minimalapi.Data;
using library.minimalapi.Services;
using Microsoft.EntityFrameworkCore;

namespace library.minimalapi.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder InstallServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
                .UseSeeding((context, _) =>
                {
                    if (context.Set<Book>().Any()) return;

                    context.Set<Book>().AddRange(
                        new Book(
                            Isbn: "9780132350884",
                            Title: "Clean Code",
                            Author: "Robert C. Martin",
                            ShortDescription: "Clean Code",
                            PageCount: 12,
                            ReleaseDate: new DateOnly(2022, 12, 1)
                        ),
                        new Book(
                            Isbn: "9780201616224",
                            Title: "The Pragmatic Programmer",
                            Author: "Andrew Hunt",
                            ShortDescription: "The Pragmatic Programmer",
                            PageCount: 32,
                            ReleaseDate: new DateOnly(2020, 6, 1)
                        ),
                        new Book(
                            Isbn: "9780201633610",
                            Title: "Design Patterns",
                            Author: "Erich Gamma",
                            ShortDescription: "Design Patterns",
                            PageCount: 22,
                            ReleaseDate: new DateOnly(2025, 6, 21)
                        )
                    );
                    context.SaveChanges();
                });
        });

        builder.Services.AddScoped<IBookService, BookService>();
        return builder;
    }
}
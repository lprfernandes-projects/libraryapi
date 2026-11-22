
using library.minimalapi.Contracts;
using library.minimalapi.Services;

namespace library.minimalapi.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder InstallServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddScoped<IBookService, BookService>();
        return builder;
    }
}
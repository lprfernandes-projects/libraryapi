
namespace library.minimalapi.Extensions;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/books", BookHandlers.GetAllBooksAsync)
            .WithName("Get Books");

        app.MapGet("/search", BookHandlers.SearchBooksByTitleAsync)
            .WithName("Search per Title");

        app.MapGet("/book", BookHandlers.GetBookByIsbnAsync)
            .WithName("Get book by Isbn");

        app.MapPut("/book", BookHandlers.UpdateBookAsync)
            .WithName("Update book by Isbn");

        app.MapPost("/book", BookHandlers.CreateBookAsync)
            .WithName("Create book");

        app.MapDelete("/book", BookHandlers.DeleteBookAsync)
            .WithName("Delete book by Isbn");
    }
}
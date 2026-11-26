
namespace library.minimalapi.Extensions;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/books", BookHandlers.GetAllBooksAsync)
            .WithName("Get Books")
            .Produces<IEnumerable<Book>>();

        app.MapGet("/book", BookHandlers.GetBookByIsbnAsync)
            .WithName("Get book by Isbn")
            .Produces<Book>()
            .Produces(404);

        app.MapGet("/search", BookHandlers.SearchBooksByTitleAsync)
            .WithName("Search per Title")
            .Produces<IEnumerable<Book>>()
            .Produces(400);


        app.MapPut("/book", BookHandlers.UpdateBookAsync)
            .WithName("Update book by Isbn")
            .Accepts<Book>("application/json")
            .Produces<Book>()
            .Produces(400);

        app.MapPost("/book", BookHandlers.CreateBookAsync)
            .WithName("Create book")
            .Accepts<Book>("application/json")
            .Produces<Book>()
            .Produces(400);

        app.MapDelete("/book", BookHandlers.DeleteBookAsync)
            .WithName("Delete book by Isbn")
            .Produces(204)
            .Produces(404);
    }
}
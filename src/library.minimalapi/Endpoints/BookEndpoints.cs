
using System.Runtime.CompilerServices;

namespace library.minimalapi.Extensions;

public static class BookEndpoints
{
    private const string ContentType = "application/json";
    private const string Tag = "Books";
    private const string BaseRoute = "/book";

    public static void MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet($"{BaseRoute}s", BookHandlers.GetAllBooksAsync)
            .WithName("Get Books")
            .Produces<IEnumerable<Book>>()
            .WithTags(Tag);

        app.MapGet(BaseRoute, BookHandlers.GetBookByIsbnAsync)
            .WithName("Get book by Isbn")
            .Produces<Book>()
            .Produces(404)
            .WithTags(Tag);

        app.MapGet("/search", BookHandlers.SearchBooksByTitleAsync)
            .WithName("Search per Title")
            .Produces<IEnumerable<Book>>()
            .Produces(400)
            .WithTags(Tag);

        app.MapPut(BaseRoute, BookHandlers.UpdateBookAsync)
            .WithName("Update book by Isbn")
            .Accepts<Book>(ContentType)
            .Produces<Book>()
            .Produces(400)
            .WithTags(Tag);

        app.MapPost(BaseRoute, BookHandlers.CreateBookAsync)
            .WithName("Create book")
            .Accepts<Book>(ContentType)
            .Produces<Book>()
            .Produces(400)
            .WithTags(Tag);

        app.MapDelete(BaseRoute, BookHandlers.DeleteBookAsync)
            .WithName("Delete book by Isbn")
            .Produces(204)
            .Produces(404)
            .WithTags(Tag);
    }
}
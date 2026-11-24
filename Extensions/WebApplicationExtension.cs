using library.minimalapi.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace library.minimalapi.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication MapBookEndpoints(this WebApplication app)
    {
        //Get all Books
        app.MapGet("/books", async ([FromServices] IBookService bookService) =>
        {
            var books = await bookService.GetAllAsync();
            return Results.Ok(books);
        });

        //Get book by searchterm
        app.MapGet("/search", async ([FromQuery] string? searchTerm, [FromServices] IBookService bookService) =>
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Results.Problem("search term missing");
            }

            var books = await bookService.SearchByTitleAsync(searchTerm);

            return Results.Ok(books);
        });

        //Get book by Isbn
        app.MapGet("/book", async ([FromQuery] string? isbn, [FromServices] IBookService bookService) =>
        {
            if (isbn is null)
            {
                return Results.Problem("Isbn missing");
            }

            var book = await bookService.GetByIsbnAsync(isbn);

            if (book is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(book);
        });


        //Update a book by Isbn
        app.MapPut("/book",
            async (
                [FromQuery] string? isbn,
                [FromBody] Book? book,
                [FromServices] IBookService bookService) =>
            {
                if (book is null)
                {
                    return Results.Problem("Updated book's details missing");
                }

                if (isbn is null)
                {
                    return Results.Problem("Isbn missing");
                }

                bool updated = await bookService.UpdateByIsbnAsync(isbn, book);

                if (!updated)
                {
                    return Results.Problem("It wasn't possible to update the book");
                }

                return Results.Ok();
            });


        //Create a book
        app.MapPost("/book",
            async (
                    [FromBody] Book? book,
                    [FromServices] IBookService bookService)
                =>
            {
                if (book is null)
                {
                    Results.NoContent();
                }

                bool created = await bookService.CreateAsync(book);

                if (!created)
                {
                    Results.Problem("It wasn't possible to create the book");
                }

                Results.Created();
            });

        //Delete a book
        app.MapDelete("/book",
            async (
                    [FromQuery] string? isbn,
                    [FromServices] IBookService bookService)
                =>
            {
                if (string.IsNullOrWhiteSpace(isbn))
                {
                    Results.Problem("isbn cannot be blank");
                }

                bool deleted = await bookService.DeleteByIsbnAsync(isbn);

                if (!deleted)
                {
                    Results.Problem("It wasn't possible to delete the book.");
                }

                Results.NoContent();
            }
        );
        return app;
    }
}
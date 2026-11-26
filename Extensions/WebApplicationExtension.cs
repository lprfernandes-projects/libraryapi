using library.minimalapi.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace library.minimalapi.Extensions;

public static class WebApplicationExtension
{
    public static void MapBookEndpoints(this WebApplication app)
    {
        //Get all Books
        app.MapGet("/books", async ([FromServices] IBookService bookService) =>
        {
            var books = await bookService.GetAllAsync();
            return Results.Ok(books);
        }).WithName("Get Books");

        //Get book by searchterm
        app.MapGet("/search", async ([FromQuery] string? searchTerm, [FromServices] IBookService bookService) =>
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Results.BadRequest("search term missing");
            }

            var books = await bookService.SearchByTitleAsync(searchTerm);

            return Results.Ok(books);
        }).WithName("Search per Title");

        //Get book by Isbn
        app.MapGet("/book", async ([FromQuery] string? isbn, [FromServices] IBookService bookService) =>
        {
            if (isbn is null)
            {
                return Results.BadRequest("Isbn missing");
            }

            var book = await bookService.GetByIsbnAsync(isbn);

            if (book is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(book);
        }).WithName("Get book by Isbn");


        //Update a book by Isbn
        app.MapPut("/book",
            async (
                [FromQuery] string? isbn,
                [FromBody] Book? book,
                [FromServices] IBookService bookService) =>
            {
                if (book is null)
                    return Results.BadRequest("Updated book's details missing");

                if (isbn is null)
                    return Results.BadRequest("Isbn missing");

                if (isbn != book.Isbn)
                    return Results.BadRequest("isbn and the book property isbn are not the same");

                bool updated = await bookService.UpdateByIsbnAsync(isbn, book);

                if (!updated)
                    return Results.Problem("It wasn't possible to update the book");

                return Results.Ok();
            }).WithName("Update book by Isbn");


        //Create a book
        app.MapPost("/book",
            async (
                    [FromBody] Book? book,
                    [FromServices] IBookService bookService)
                =>
            {
                if (book is null) return Results.NoContent();

                bool created = await bookService.CreateAsync(book!);

                if (!created) return Results.Problem("It wasn't possible to create the book");

                return Results.Created($"/book?isbn={book!.Isbn}", book);
            }).WithName("Create book");

        //Delete a book
        app.MapDelete("/book",
            async (
                    [FromQuery] string? isbn,
                    [FromServices] IBookService bookService)
                =>
            {
                if (string.IsNullOrWhiteSpace(isbn))
                    return Results.BadRequest("isbn cannot be blank");

                bool deleted = await bookService.DeleteByIsbnAsync(isbn!);

                if (!deleted) return Results.Problem("It wasn't possible to delete the book.");

                return Results.NoContent();
            }
        ).WithName("Delete book by Isbn");
    }
}
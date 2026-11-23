
using library.minimalapi.Contracts;
using library.minimalapi.Extensions;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

builder.InstallServices();

var app = builder.Build();


app.MapGet("/books", ([FromServices] IBookService bookService) =>
{
    var books = bookService.GetAllBooks();
    return Results.Ok(books);
});

app.MapGet("/book", ([FromQuery] string isbn, [FromServices] IBookService bookService) =>
{
    var book = bookService.GetBookByIsbn(isbn);

    if (book is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(book);
});

app.Run();


using System.Runtime.CompilerServices;
using library.minimalapi.Contracts;
using library.minimalapi.Extensions;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

builder.InstallServices();

var app = builder.Build();


app.MapGet("/books", async ([FromServices] IBookService bookService) =>
{
    var books = await bookService.GetAllAsync();
    return Results.Ok(books);
});

app.MapGet("/book", async ([FromQuery] string isbn, [FromServices] IBookService bookService) =>
{
    var book = await bookService.GetByIsbnAsync(isbn);

    if (book is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(book);
});

app.MapPost("/book",
    async ([FromBody] Book? book, [FromServices] IBookService bookService)
        =>
    {
        if (book is null)
        {
            Results.NoContent();
        }

        bool created = await bookService.CreateAsync(book);

        if (!created)
        {
            Results.BadRequest();
        }

        Results.Created();
    });

app.Run();

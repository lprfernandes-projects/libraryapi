using library.minimalapi.Contracts;

namespace library.minimalapi.Extensions;

public static class BookHandlers
{
   public static async Task<IResult> GetAllBooksAsync(IBookService bookService)
   {
      var books = await bookService.GetAllAsync();
      return Results.Ok(books);
   }

   public static async Task<IResult> GetBookByIsbnAsync(string? isbn, IBookService bookService)
   {
      if (isbn is null)
         return Results.BadRequest("Isbn missing");

      var book = await bookService.GetByIsbnAsync(isbn);

      if (book is null)
         return Results.NotFound();

      return Results.Ok(book);
   }

   public static async Task<IResult> SearchBooksByTitleAsync(string? searchTerm, IBookService bookService)
   {
      if (string.IsNullOrWhiteSpace(searchTerm))
      {
         return Results.BadRequest("search term missing");
      }

      var books = await bookService.SearchByTitleAsync(searchTerm);

      return Results.Ok(books);
   }

   public static async Task<IResult> CreateBookAsync(Book? book, IBookService bookService)
   {
      if (book is null) return Results.NoContent();

      bool created = await bookService.CreateAsync(book);

      if (!created)
         return Results.Conflict("A book with this ISBN already exists.");

      return Results.Created($"/book?isbn={book.Isbn}", book);
   }

   public static async Task<IResult> UpdateBookAsync(string? isbn, Book? book, IBookService bookService)
   {
      if (book is null)
         return Results.BadRequest("Updated book's details missing");

      if (isbn is null)
         return Results.BadRequest("Isbn missing");

      if (isbn != book.Isbn)
         return Results.BadRequest("isbn and the book property isbn are not the same");

      bool updated = await bookService.UpdateByIsbnAsync(isbn, book);

      if (!updated)
      {
         return Results.Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Book not found",
            detail: "It wasn't possible to update the book");
      }

      return Results.Ok(book);
   }

   public static async Task<IResult> DeleteBookAsync(string? isbn, IBookService bookService)
   {
      if (string.IsNullOrWhiteSpace(isbn))
         return Results.BadRequest("isbn cannot be blank");

      bool deleted = await bookService.DeleteByIsbnAsync(isbn!);

      if (!deleted) return Results.Problem("It wasn't possible to delete the book.");

      return Results.NoContent();
   }
}
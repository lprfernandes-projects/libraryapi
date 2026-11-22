
using library.minimalapi.Contracts;

namespace library.minimalapi.Services;

public class BookService : IBookService
{
    private readonly IList<Book?> _books =
    [
        new("999-999999999", "The title of the book 1", "A", "A book A", 33,
            new DateOnly(2022, 08, 01)),
        new("888-888888888", "The title of the book 2", "B", "A book B", 32,
            new DateOnly(2023, 07, 04)),
        new("777-777777777", "The title of the book 3", "C", "A book C", 31,
            new DateOnly(2024, 05, 01)),
    ];

    public IEnumerable<Book?> GetAllBooks()
    {
        return _books;
    }

    public Book? GetBookByIsbn(string isbn)
    {
        return _books.SingleOrDefault(x => x?.Isbn == isbn);
    }
}
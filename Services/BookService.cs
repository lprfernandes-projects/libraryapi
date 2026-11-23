
using library.minimalapi.Contracts;
using library.minimalapi.Data;

namespace library.minimalapi.Services;

public class BookService : IBookService
{
    private readonly ApplicationDbContext _dbContext;

    public BookService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<Book?> GetAllBooks()
    {
        return _dbContext.Books.ToList();
    }

    public Book? GetBookByIsbn(string isbn)
    {
        return _dbContext.Books.SingleOrDefault(x => x.Isbn == isbn);
    }
}
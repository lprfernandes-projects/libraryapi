
using library.minimalapi.Contracts;
using library.minimalapi.Data;
using Microsoft.EntityFrameworkCore;

namespace library.minimalapi.Services;

public class BookService : IBookService
{
    private readonly ApplicationDbContext _dbContext;

    public BookService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Book?>> GetAllAsync()
    {
        return await _dbContext.Books.ToListAsync();
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        return await _dbContext.Books.SingleOrDefaultAsync(x => x.Isbn == isbn);
    }

    public async Task<bool> CreateAsync(Book? book)
    {
        if (book is null)
        {
            return false;
        }

        await _dbContext.Books.AddAsync(book);

        int docsChanged = await _dbContext.SaveChangesAsync();

        if (docsChanged < 1)
        {
            return false;
        }

        return true;
    }
}

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

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _dbContext.Books.ToListAsync();
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        return await _dbContext.Books.SingleOrDefaultAsync(x => x.Isbn == isbn);
    }

    public async Task<bool> CreateAsync(Book book)
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

    public async Task<bool> DeleteByIsbnAsync(string isbn)
    {
        var book = await _dbContext.Books.SingleOrDefaultAsync(x => x.Isbn == isbn);

        if (book is null)
        {
            return false;
        }

        _dbContext.Remove(book);
        var nrRowsChanged = await _dbContext.SaveChangesAsync();

        if (nrRowsChanged < 1)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> UpdateByIsbnAsync(string isbn, Book book)
    {
        var existingBook = await _dbContext.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Isbn == isbn);


        if (existingBook is null) return false;

        var updatedBook = new Book(book.Isbn, book.Title, book.Author, book.ShortDescription, book.PageCount,
            book.ReleaseDate);

        _dbContext.Update(updatedBook);
        var nrRowsChanged = await _dbContext.SaveChangesAsync();

        if (nrRowsChanged < 1)
        {
            return false;
        }

        return true;
    }

    public async Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm)
    {
        var books = await _dbContext.Books
            .Where(x => x.Title.ToLower().Contains(searchTerm.ToLower()))
            .ToListAsync();
        return books;
    }
}
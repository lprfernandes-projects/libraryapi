namespace library.minimalapi.Contracts;

public interface IBookService
{
    public Task<IEnumerable<Book>> GetAllAsync();
    public Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm);
    public Task<Book?> GetByIsbnAsync(string isbn);
    public Task<bool> DeleteByIsbnAsync(string isbn);
    public Task<bool> UpdateByIsbnAsync(string isbn, Book book);
    public Task<bool> CreateAsync(Book book);
}
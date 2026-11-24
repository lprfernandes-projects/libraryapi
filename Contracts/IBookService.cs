namespace library.minimalapi.Contracts;

public interface IBookService
{
    public Task<IEnumerable<Book?>> GetAllAsync();

    public Task<Book?> GetByIsbnAsync(string isbn);
    public Task<bool> CreateAsync(Book? book);
}
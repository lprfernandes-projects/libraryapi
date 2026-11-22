namespace library.minimalapi.Contracts;

public interface IBookService
{
    public IEnumerable<Book?> GetAllBooks();

    public Book? GetBookByIsbn(string isbn);
}

public record Book(
    string Isbn,
    string Title,
    string Author,
    string ShortDescription,
    int PageCount,
    DateOnly ReleaseDate);
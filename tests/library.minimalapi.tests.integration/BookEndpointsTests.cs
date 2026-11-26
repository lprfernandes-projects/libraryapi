using System.Net;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace library.minimalapi.tests.integration;

public class BookEndpointsTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly List<string> _createdISBNs = new();

    public BookEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateBook_CreatesBook_WhenDataIsCorrect()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();

        //Act
        var result = await httpClient.PostAsJsonAsync("/book", book);
        _createdISBNs.Add(book.Isbn);
        var createdBook = await result.Content.ReadFromJsonAsync<Book>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        createdBook.Should().NotBeNull();
        createdBook.Should().BeEquivalentTo(book);
        result.Headers.Location.Should().Be($"/book?isbn={book.Isbn}");
    }

    [Fact]
    public async Task CreateBook_Fails_WhenIsbnIsInvalid()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook() with { Isbn = "INVALID" };

        //Act
        var result = await httpClient.PostAsJsonAsync("/book", book);
        var validation = await result.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        validation.Should().NotBeNull();
        validation.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateBook_Fails_WhenBookExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();

        // Act
        var firstResponse = await httpClient.PostAsJsonAsync("/book", book);
        _createdISBNs.Add(book.Isbn);
        var result = await httpClient.PostAsJsonAsync("/book", book);

        // Assert
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var error = await result.Content.ReadFromJsonAsync<string>();

        error.Should().Be("A book with this ISBN already exists.");
    }


    [Fact]
    public async Task GetBook_ReturnsBook_WhenBookExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        await httpClient.PostAsJsonAsync("/book", book);
        _createdISBNs.Add(book.Isbn);

        // Act
        var result = await httpClient.GetAsync($"/book?isbn={book.Isbn}");
        var existingBook = await result.Content.ReadFromJsonAsync<Book>();

        // Assert

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        existingBook.Should().BeEquivalentTo(book);
    }


    [Fact]
    public async Task GetBook_ReturnsNotFound_WhenBookNotExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var isbn = GenerateIsbn();

        // Act
        var result = await httpClient.GetAsync($"/book?isbn={isbn}");

        // Assert

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task GetAllBooks_ReturnsAllBooks_WhenBooksExist()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        await httpClient.PostAsJsonAsync("/book", book);
        _createdISBNs.Add(book.Isbn);
        var books = new List<Book>() { book };

        //Act

        var result = await httpClient.GetAsync("/books");
        var createdBooks = await result.Content.ReadFromJsonAsync<IEnumerable<Book>>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        createdBooks.Should().NotBeNull();
        createdBooks.Should().BeEquivalentTo(books);
    }


    public Task InitializeAsync() => Task.CompletedTask;

    async Task IAsyncLifetime.DisposeAsync()
    {
        var httpClient = _factory.CreateClient();
        foreach (var createdIsbn in _createdISBNs)
        {
            var response = await httpClient.DeleteAsync($"/book?isbn={Uri.EscapeDataString(createdIsbn)}");
            response.EnsureSuccessStatusCode();
        }
    }


    private Book GenerateBook(string title = "ABC")
    {
        return new Book(
            GenerateIsbn(),
            "This is a title of a book",
            "LF",
            "This is a description of a book",
            Random.Shared.Next(1, 1000),
            new DateOnly(2026, 1, 1));
    }

    private string GenerateIsbn()
    {
        return $"000000{Random.Shared.Next(0, 10_000_000):D7}";
    }
}
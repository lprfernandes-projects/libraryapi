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
        var createdBooks = await result.Content.ReadFromJsonAsync<List<Book>>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        createdBooks.Should().NotBeNull();
        createdBooks.Should().BeEquivalentTo(books);
    }

    [Fact]
    public async Task GetAllBooks_ReturnsNoBooks_WhenNoBooksExist()
    {
        //Arrange
        var httpClient = _factory.CreateClient();

        //Act

        var result = await httpClient.GetAsync("/books");
        var createdBooks = await result.Content.ReadFromJsonAsync<List<Book>>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        createdBooks.Should().BeEmpty();
    }


    [Fact]
    public async Task SearchBooks_ReturnsBooks_WhenTitleMatches()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook("Clean code");
        await httpClient.PostAsJsonAsync("/book", book);
        _createdISBNs.Add(book.Isbn);
        var books = new List<Book>() { book };

        //Act

        var result = await httpClient.GetAsync("/search?SearchTerm=code");
        var foundBooks = await result.Content.ReadFromJsonAsync<List<Book>>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        foundBooks.Should().NotBeNull();
        foundBooks.Should().BeEquivalentTo(books);
    }

    [Fact]
    public async Task UpdateBook_UpdatesBook_WhenDataIsCorrect()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook() with { PageCount = 100 };
        await httpClient.PostAsJsonAsync("/book", book);
        _createdISBNs.Add(book.Isbn);

        //Act
        var updatedBook = book with { PageCount = 69 };
        var result = await httpClient.PutAsJsonAsync($"/book?isbn={book.Isbn}", updatedBook);

        var returnedBook = await result.Content.ReadFromJsonAsync<Book>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        returnedBook.Should().NotBeNull();
        returnedBook.Should().BeEquivalentTo(updatedBook);
    }

    [Fact]
    public async Task UpdateBook_DoesNotUpdatesBook_WhenDataIsIncorrect()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        await httpClient.PostAsJsonAsync("/book", book);
        _createdISBNs.Add(book.Isbn);

        //Act
        var updatedBook = book with { Title = string.Empty };
        var result = await httpClient.PutAsJsonAsync($"/book?isbn={book.Isbn}", updatedBook);
        var validation = await result.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        validation.Should().NotBeNull();
        validation.Errors.Should().NotBeEmpty();
    }


    [Fact]
    public async Task UpdateBook_DoesNotUpdateBook_WhenBookIsNotFound()
    {
        // Arrange
        using var httpClient = _factory.CreateClient();
        var updatedBook = GenerateBook() with
        {
            Isbn = "1000000000000"
        };

        // Act
        var result = await httpClient.PutAsJsonAsync(
            $"/book?isbn={updatedBook.Isbn}",
            updatedBook);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await result.Content.ReadFromJsonAsync<ProblemDetails>();

        error.Should().NotBeNull();
        error.Detail.Should().Be("It wasn't possible to update the book");
    }


    [Fact]
    public async Task UpdateBook_DoesNotUpdatesBook_WhenIsbnDontMatch()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var updatedBook = GenerateBook() with { Isbn = "0000000000001" };

        //Act
        var result = await httpClient.PutAsJsonAsync("/book?isbn=0000000000000", updatedBook);
        var error = await result.Content.ReadFromJsonAsync<string>();


        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.Should().Be("isbn and the book property isbn are not the same");
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
            title,
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
using System.Net;
using System.Runtime.CompilerServices;
using AwesomeAssertions;
using library.minimalapi.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace library.minimalapi.tests.integration;

public class BookEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

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
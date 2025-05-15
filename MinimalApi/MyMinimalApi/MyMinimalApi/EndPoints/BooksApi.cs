using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using MyMinimalApi.Filters;
using MyMinimalApi.Models;
using MyMinimalApi.Services;

namespace MyMinimalApi.EndPoints;

public static class BooksApi
{
    public static void MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        
        var books = app.MapGroup("/books");
        // grouping by /books so each endpoint mapped will be an extension of /books
        books.MapGet("/{id}", Results<Ok<Book>, NotFound> (IBookService bookService, int id) =>
                bookService.GetBook(id) is { } book 
                    ? TypedResults.Ok(book)
                    : TypedResults.NotFound()
            )
            .WithName(("GetBookById"))
            .WithOpenApi((x => new OpenApiOperation(x)
            {
                Summary = "Get Library Book By Id",
                Description = "Returns information about selected book from the Amy's library.",
                Tags = new List<OpenApiTag> { new() { Name = "Amy's Library" } }
            }));

        books.MapGet("/", (IBookService bookService) => 
                TypedResults.Ok(bookService.GetBooks()))
            .WithName("GetBooks")
            .WithOpenApi(x => 
                new OpenApiOperation(x)
                {
                    Summary = "Get Library Books",
                    Description = "Returns information about all the available books from Amy's library.",
                    Tags = new List<OpenApiTag> 
                    { 
                        new OpenApiTag { Name = "Amy's Library" } 
                    }
                });

        books.AddEndpointFilter(new TimeStampFilter());
    }
}
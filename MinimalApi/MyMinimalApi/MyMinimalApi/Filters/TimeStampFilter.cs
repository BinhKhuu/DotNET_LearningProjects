using Microsoft.AspNetCore.Http.HttpResults;
using MyMinimalApi.Models;

namespace MyMinimalApi.Filters;

public class TimeStampFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context);
        
        // extracting result
        if (result is Results<Ok<Book>, NotFound> results)
        {
            if (results.Result is Ok<Book> okResult)
            {
                Book book = okResult.Value;
                book.Timestamp = DateTime.UtcNow;
            }
            else
            {
                Console.WriteLine("Book not found.");
            }
        }
        
        // property matching
        // if (result is Results<Ok<Book>, NotFound> { Result: Ok<Book> { Value: { } } book2 })
        // {
        //     book2.Value.Timestamp = DateTime.UtcNow;
        // }

        return result;
    }
}
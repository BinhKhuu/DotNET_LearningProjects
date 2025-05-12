using MyMinimalApi.Models;
namespace MyMinimalApi.Services;

public interface IBookService
{
    List<Book> GetBooks();
        
    Book GetBook(int id);
}
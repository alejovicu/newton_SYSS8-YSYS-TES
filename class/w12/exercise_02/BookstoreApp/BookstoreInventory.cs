
namespace BookstoreApp;

public class BookstoreInventory
{

    private readonly List<Book> _books = new List<Book>();



    public bool AddBook(Book book)
    {

        var existingBook = _books.FirstOrDefault(b => b.ISBN == book.ISBN);

        if (existingBook == null)
        {

            _books.Add(book);

            return true;

        }

        existingBook.Stock += book.Stock; // Restock existing book

        return false; // Indicates existing book was restocked

    }



    public bool RemoveBook(string isbn)
    {

        var bookToRemove = _books.FirstOrDefault(b => b.ISBN == isbn);

        if (bookToRemove != null)
        {

            bookToRemove.Stock--;

            return true;

        }

        return false;

    }



    public Book FindBookByTitle(string title)
    {

        return _books.FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

    }


    public int CheckStock(string isbn)
    {

        var book = _books.FirstOrDefault(b => b.ISBN == isbn);

        return book != null ? book.Stock : 0;

    }

}
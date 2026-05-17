namespace Bookstore;

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

        existingBook.Stock += book.Stock;

        return false;

    }

    public bool RemoveBook(string isbn)
    {

        var bookToRemove = _books.FirstOrDefault(b => b.ISBN == isbn);

        if (bookToRemove != null && bookToRemove.Stock > 0)
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

    public List<Book> GetAllBooks()
    {
        return new List<Book>(_books);
    }
}
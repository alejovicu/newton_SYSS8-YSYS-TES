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
    
    // Changing this method so it correctly removes the book
    public bool RemoveBook(string isbn)
    {
        // Find the book by ISBN
        var bookToRemove = _books.FirstOrDefault(b => b.ISBN == isbn);

        // If the book exists and has stock, reduce the stock by one
        if (bookToRemove != null && bookToRemove.Stock > 0)
        {        
            bookToRemove.Stock--;

            // If the stock reaches zero, remove the book from the inventory
            if (bookToRemove.Stock == 0)
            {
                _books.Remove(bookToRemove);
            }
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
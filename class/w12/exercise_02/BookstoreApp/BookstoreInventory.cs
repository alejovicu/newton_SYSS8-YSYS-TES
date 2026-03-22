namespace BookstoreApp;

public class BookstoreInventory
{
    private readonly List<Book> _books = new();

    public bool AddBook(Book book)
    {
        if (book == null)
            return false;

        var match = _books.FirstOrDefault(b => b.ISBN == book.ISBN);

        if (match == null)
        {
            _books.Add(book);
            return true;
        }

        match.Stock += book.Stock;
        return false;
    }

    public bool RemoveBook(string isbn)
    {
        var match = _books.FirstOrDefault(b => b.ISBN == isbn);

        if (match == null)
            return false;

        if (match.Stock > 0)
        {
            match.Stock--;
            return true;
        }

        return false; // inget att ta bort
    }

    public Book FindBookByTitle(string title)
    {
        return _books.FirstOrDefault(b =>
            b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
    }

    public int CheckStock(string isbn)
    {
        var match = _books.FirstOrDefault(b => b.ISBN == isbn);
        return match?.Stock ?? 0;
    }
}

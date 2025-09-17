namespace Example.Library.BookSchemaTester.Models;

public class Book
{
    public string ISBN13 { get; set; }
    public string Title { get; set; }
    public byte[]? CoverThumbnailWebP { get; set; }
    public List<Author> Authors { get; set; }
    public uint NumberOfTextPosition { get; set; }
    public BookCategory Category { get; set; }
    public ISet<string> Tags { get; set; }
    public decimal? Rating { get; set; }
    public bool HasValidLicense { get; set; }
    public DateTime PublishingDate { get; set; }

    public Book(string isbn13, string title, byte[]? coverThumbnailWebP, List<Author> authors, uint numberOfTextPosition, BookCategory category, ISet<string> tags, decimal? rating, bool hasValidLicense, DateTime publishingDate)
    {
        ISBN13 = isbn13;
        Title = title;
        CoverThumbnailWebP = coverThumbnailWebP;
        Authors = authors;
        NumberOfTextPosition = numberOfTextPosition;
        Category = category;
        Tags = tags;
        Rating = rating;
        HasValidLicense = hasValidLicense;
        PublishingDate = publishingDate;
    }
}

public class Author
{
    public string Name { get; set; }
    public byte[]? PortraitThumbnailWebP { get; set; }

    public Author(string name, byte[]? portraitThumbnailWebP)
    {
        Name = name;
        PortraitThumbnailWebP = portraitThumbnailWebP;
    }
}

public enum BookCategory
{
    NON_FICTION,
    FICTION,
}

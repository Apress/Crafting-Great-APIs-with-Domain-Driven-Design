using System.Globalization;

using Example.Library.BookSchemaTester.Models;

using Google.Protobuf;
using Google.Protobuf.Collections;

namespace Example.Library.BookSchemaTester.Serializers;

public class BookProtobufSerializer : IBookFormatSerializer
{
    public string Format => "Protobuf";

    public void Serialize(Example.Library.BookSchemaTester.Models.Book book, Stream stream)
    {
        var dto = new Book.Dto.Proto.Book
        {
            Isbn13 = book.ISBN13,
            Title = book.Title,
            NumberOfTextPosition = book.NumberOfTextPosition,
            Category = book.Category switch
            {
                BookCategory.NON_FICTION => Book.Dto.Proto.Book.Types.Category.NonFiction,
                BookCategory.FICTION => Book.Dto.Proto.Book.Types.Category.Fiction,
                _ => throw new ArgumentOutOfRangeException(nameof(book.Category))
            },
            HasValidLicense = book.HasValidLicense,
            PublishingDate = book.PublishingDate.ToString("yyyy-MM-dd"),
        };
        dto.Tags.AddRange(book.Tags);
        dto.Authors.Add(MapAuthor(book));

        if (book.CoverThumbnailWebP != null)
        {
            dto.CoverThumbnailWebp = ByteString.CopyFrom(book.CoverThumbnailWebP);
        }

        if (book.Rating.HasValue)
        {
            dto.Rating = (float)book.Rating;
        }

        dto.WriteTo(stream);
    }

    public Example.Library.BookSchemaTester.Models.Book Deserialize(Stream stream)
    {
        var book = Book.Dto.Proto.Book.Parser.ParseFrom(stream);
        return new Example.Library.BookSchemaTester.Models.Book(
            book.Isbn13,
            book.Title,
            book.CoverThumbnailWebp.ToByteArray(),
            book.Authors.Select(a => new Author(a.Name, a.PortraitThumbnailWebp.ToByteArray())).ToList(),
            book.NumberOfTextPosition,
            book.Category switch
            {
                Book.Dto.Proto.Book.Types.Category.NonFiction => BookCategory.NON_FICTION,
                Book.Dto.Proto.Book.Types.Category.Fiction => BookCategory.FICTION,
                _ => throw new ArgumentOutOfRangeException()
            },
            book.Tags.ToHashSet(),
            Convert.ToDecimal(book.Rating),
            book.HasValidLicense,
            DateTime.ParseExact(book.PublishingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture));
    }

    private static RepeatedField<Book.Dto.Proto.Book.Types.Author> MapAuthor(Example.Library.BookSchemaTester.Models.Book dto)
    {
        var authors = new RepeatedField<Example.Library.Book.Dto.Proto.Book.Types.Author>();
        foreach (var dtoAuthor in dto.Authors)
        {
            var author = new Example.Library.Book.Dto.Proto.Book.Types.Author() { Name = dtoAuthor.Name, };
            if (dtoAuthor.PortraitThumbnailWebP != null)
            {
                author.PortraitThumbnailWebp = ByteString.CopyFrom(dtoAuthor.PortraitThumbnailWebP);
            }
            authors.Add(author);
        }

        return authors;
    }
}

using Example.Library.BookSchemaTester.Models;

using BookCategory = Example.Library.Book.Dto.Json.BookCategory;

namespace Example.Library.BookSchemaTester.Serializers;

public class BookJsonSerializer : IBookFormatSerializer
{
    public string Format => "JSON";

    public void Serialize(Models.Book book, Stream stream)
    {
        var dto = new Example.Library.Book.Dto.Json.Book
        {
            Isbn13 = book.ISBN13,
            Title = book.Title,
            CoverThumbnailWebP = book.CoverThumbnailWebP == null ? null : Convert.ToBase64String(book.CoverThumbnailWebP),
            Authors = book.Authors.Select(a => new Example.Library.Book.Dto.Json.Authors(){Name = a.Name, PortraitThumbnailWebP = MapOptionalToBase64(a.PortraitThumbnailWebP)}).ToList(),
            NumberOfTextPosition = Convert.ToInt32(book.NumberOfTextPosition),
            Category = book.Category switch
            {
                Example.Library.BookSchemaTester.Models.BookCategory.NON_FICTION => BookCategory.NON_FICTION,
                Example.Library.BookSchemaTester.Models.BookCategory.FICTION => BookCategory.FICTION,
                _ => throw new ArgumentOutOfRangeException(nameof(book.Category))
            },
            Tags = book.Tags,
            HasValidLicense = book.HasValidLicense,
            PublishingDate = book.PublishingDate,
        };

        if (book.Rating != null)
        {
            dto.Rating = Decimal.ToDouble(book.Rating.Value);
        }

        System.Text.Json.JsonSerializer.Serialize(stream, dto);
    }

    public Models.Book Deserialize(Stream stream)
    {
        var jsonDto = System.Text.Json.JsonSerializer.Deserialize<Example.Library.Book.Dto.Json.Book>(stream);
        if (jsonDto == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }
        return Map(jsonDto);
    }

    public Models.Book Map(Example.Library.Book.Dto.Json.Book book)
    {
        return new Models.Book(
            book.Isbn13,
            book.Title,
            Convert.FromBase64String(book.CoverThumbnailWebP),
            book.Authors.Select(a => new Author(a.Name, MapOptionalFromBase64(a.PortraitThumbnailWebP))).ToList(),
            Convert.ToUInt32(book.NumberOfTextPosition),
            book.Category switch
            {
                BookCategory.NON_FICTION => Example.Library.BookSchemaTester.Models.BookCategory.NON_FICTION,
                BookCategory.FICTION => Example.Library.BookSchemaTester.Models.BookCategory.FICTION,
                _ => throw new ArgumentOutOfRangeException()
            },
            book.Tags.ToHashSet(),
            Convert.ToDecimal(book.Rating),
            book.HasValidLicense,
            book.PublishingDate.Date);
    }

    private string? MapOptionalToBase64(byte[]? bytes)
    {
        if (bytes == null || bytes.Length == 0)
        {
            return null;
        }

        return Convert.ToBase64String(bytes);
    }

    private byte[] MapOptionalFromBase64(string base64)
    {
        return Convert.FromBase64String(base64 ?? "");
    }
}

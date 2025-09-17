using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Example.Library.Book.Dto.Xml;
using Example.Library.BookSchemaTester.Models;

namespace Example.Library.BookSchemaTester.Serializers;

public class BookXmlSerializer : IBookFormatSerializer
{
    public string Format => "XML";

    private XmlSerializer GetXmlSerializer() => new XmlSerializer(typeof(BookType));

    public void Serialize(Models.Book book, Stream stream)
    {
        var dto = new BookType
        {
            isbn13 = book.ISBN13,
            title = book.Title,
            coverThumbnailWebP = book.CoverThumbnailWebP,
            authors = book.Authors.Select(a => new AuthorType { name = a.Name, portraitThumbnailWebP = a.PortraitThumbnailWebP }).ToArray(),
            numberOfTextPosition = book.NumberOfTextPosition.ToString(),
            category = book.Category switch
            {
                BookCategory.NON_FICTION => CategoryType.NON_FICTION,
                BookCategory.FICTION => CategoryType.FICTION,
                _ => throw new ArgumentOutOfRangeException(nameof(book.Category))
            },
            tags = book.Tags.ToArray(),
            rating = book.Rating ?? 0,
            ratingSpecified = book.Rating != null,
            hasValidLicense = book.HasValidLicense,
            publishingDate = book.PublishingDate,
        };
        GetXmlSerializer().Serialize(XmlWriter.Create(stream, new XmlWriterSettings()
        {
            Encoding = new UTF8Encoding(false),
            Indent = false
        }), dto);
    }

    public Models.Book Deserialize(Stream stream)
    {
        var dto = (BookType?)GetXmlSerializer().Deserialize(stream);
        if (dto == null)
        {
            throw new AggregateException("Book is empty");
        }

        return new Models.Book(
            dto.isbn13,
            dto.title,
            dto.coverThumbnailWebP,
            dto.authors.Select(a => new Author(a.name, a.portraitThumbnailWebP)).ToList(),
            uint.Parse(dto.numberOfTextPosition),
            dto.category switch
            {
                CategoryType.NON_FICTION => BookCategory.NON_FICTION,
                CategoryType.FICTION => BookCategory.FICTION,
                _ => throw new ArgumentOutOfRangeException(nameof(dto.category))
            },
            dto.tags.ToHashSet(),
            dto.rating,
            dto.hasValidLicense,
            dto.publishingDate);
    }
}

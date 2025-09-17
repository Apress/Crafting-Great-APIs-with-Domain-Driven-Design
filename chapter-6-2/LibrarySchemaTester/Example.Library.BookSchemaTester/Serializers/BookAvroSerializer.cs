using System.Globalization;

using Avro.IO;
using Avro.Reflect;

using Example.Library.Book.Dto.Avro;
using Example.Library.BookSchemaTester.Models;

using LibrarySchemaTester.Helpers;

using Author = Example.Library.Book.Dto.Avro.Author;

namespace Example.Library.BookSchemaTester.Serializers;

public class BookAvroSerializer : IBookFormatSerializer
{
    public string Format => "Avro";
    private int RatingScale => 2;
    public void Serialize(Models.Book book, Stream stream)
    {
        var dto = new Example.Library.Book.Dto.Avro.Book
        {
            isbn13 = book.ISBN13,
            title = book.Title,
            coverThumbnailWebP = book.CoverThumbnailWebP,
            authors = book.Authors.Select(a => new Author { name = a.Name, portraitThumbnailWebP = a.PortraitThumbnailWebP }).ToList(),
            numberOfTextPosition = book.NumberOfTextPosition,
            category = book.Category switch
            {
                BookCategory.NON_FICTION => Category.NON_FICTION,
                BookCategory.FICTION => Category.FICTION,
                _ => throw new ArgumentOutOfRangeException(nameof(book.Category))
            },
            tags = book.Tags.ToList(),
            rating = book.Rating.HasValue ? AvroDecimalHelper.GetByteArrayFromDecimal(book.Rating.Value, RatingScale) : null,
            hasValidLicense = book.HasValidLicense,
            publishingDate = book.PublishingDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        };
        var avroWriter = new ReflectWriter<Example.Library.Book.Dto.Avro.Book>(Example.Library.Book.Dto.Avro.Book._SCHEMA);
        avroWriter.Write(dto, new BinaryEncoder(stream));
    }

    public Models.Book Deserialize(Stream stream)
    {
        var avroReader = new ReflectReader<Example.Library.Book.Dto.Avro.Book>(Example.Library.Book.Dto.Avro.Book._SCHEMA, Example.Library.Book.Dto.Avro.Book._SCHEMA);
        var dto = avroReader.Read(new Example.Library.Book.Dto.Avro.Book(), new BinaryDecoder(stream));
        return new Models.Book(
            dto.isbn13,
            dto.title,
            dto.coverThumbnailWebP,
            dto.authors.Select(a => new Example.Library.BookSchemaTester.Models.Author(a.name, a.portraitThumbnailWebP)).ToList(),
            (uint)dto.numberOfTextPosition,
            dto.category switch
                {
                    Category.NON_FICTION => BookCategory.NON_FICTION,
                    Category.FICTION => BookCategory.FICTION,
                    _ => throw new ArgumentOutOfRangeException(nameof(dto.category))
                },
            dto.tags.ToHashSet(),
            dto.rating == null ? null : AvroDecimalHelper.GetDecimalFromByteArray(dto.rating, RatingScale),
            dto.hasValidLicense,
            DateTime.ParseExact(dto.publishingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}

namespace Example.Library.BookSchemaTester.Serializers;

public interface IBookFormatSerializer
{
    string Format { get; }
    void Serialize(Models.Book book, Stream stream);
    Models.Book Deserialize(Stream stream);
}

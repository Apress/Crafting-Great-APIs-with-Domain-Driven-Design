using System.Text.Json;

using Example.Library.BookSchemaTester.Models;
using Example.Library.BookSchemaTester.Serializers;

await using var inputJsonStream = File.OpenRead("../../../../../book.json");
var book = new BookJsonSerializer().Deserialize(inputJsonStream);

var serializers = new List<IBookFormatSerializer>
{
    new BookJsonSerializer(),
    new BookXmlSerializer(),
    new BookProtobufSerializer(),
    new BookAvroSerializer(),
};

foreach (var serializer in serializers)
{
    var stream = new MemoryStream();
    serializer.Serialize(book, stream);
    Console.WriteLine($"{serializer.Format} format has length {stream.Length} bytes");
    stream.Position = 0;
    var deserialized = serializer.Deserialize(stream);

    VerifySerializeDeserializeResult(deserialized, inputJsonStream);
}

void VerifySerializeDeserializeResult(Book deserialized, FileStream input)
{
    var jsonSerialized = new MemoryStream();
    new BookJsonSerializer().Serialize(deserialized, jsonSerialized);
    jsonSerialized.Position = 0;
    var outputJsonString = new StreamReader(jsonSerialized).ReadToEnd();;

    input.Position = 0;
    var inputJsonString = new StreamReader(input).ReadToEnd();
    var inputJson = JsonSerializer.Deserialize<Example.Library.Book.Dto.Json.Book>(inputJsonString);
    var outputJson = JsonSerializer.Deserialize<Example.Library.Book.Dto.Json.Book>(outputJsonString);
    var inputStringConvertedAgain = JsonSerializer.Serialize(inputJson);
    var outputStringConvertedAgain = JsonSerializer.Serialize(outputJson);
    if (!inputStringConvertedAgain.Equals(outputStringConvertedAgain))
    {
        throw new Exception($"Result is not the same! \n in: {inputStringConvertedAgain}\n out: {outputStringConvertedAgain}");
    }
}

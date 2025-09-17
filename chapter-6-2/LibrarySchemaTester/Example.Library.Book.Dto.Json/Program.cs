// See https://aka.ms/new-console-template for more information

using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

var jsonSchema = await JsonSchema.FromFileAsync("../../../../../book.schema.json");
var generator = new CSharpGenerator(jsonSchema, new CSharpGeneratorSettings()
{
    Namespace = "Example.Library.Book.Dto.Json",
    JsonLibrary = CSharpJsonLibrary.SystemTextJson
});
var file = generator.GenerateFile();

await using var outputFile = new StreamWriter("../../../Book.cs");
await outputFile.WriteAsync(file);

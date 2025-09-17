using System.Numerics;

using FluentAssertions;

using LibrarySchemaTester.Helpers;

namespace Example.Library.BookSchemaTester.Tests.Helpers;

public class AvroDecimalHelperTests
{
    [Theory]
    [InlineData(1.23456789, 8)]
    [InlineData(1, 0)]
    [InlineData(1.5, 1)]
    public void DecimalToStringAndBack(decimal decimalNumber, int scale)
    {
        var numberAsString = AvroDecimalHelper.GetByteArrayFromDecimal(decimalNumber, scale);
        var numberBack = AvroDecimalHelper.GetDecimalFromByteArray(numberAsString, scale);

        numberBack.Should().Be(decimalNumber);
    }

    [Theory]
    [InlineData(1.5, 0)]
    public void DecimalToStringAndBackWrongScaleShouldNotWokrk(decimal decimalNumber, int scale)
    {
        var numberAsString = AvroDecimalHelper.GetByteArrayFromDecimal(decimalNumber, scale);
        var numberBack = AvroDecimalHelper.GetDecimalFromByteArray(numberAsString, scale);

        numberBack.Should().NotBe(decimalNumber);
    }

    [Theory]
    [InlineData(1.23456789, 8)]
    [InlineData(1, 0)]
    [InlineData(1.5, 1)]
    // check if code is the same as in https://github.com/mjul/avro-dotnet-lab/blob/04688e15e5df014ad067045fc48e4d9c03d0825b/src/csharp/AvroCSharp/LogicalTypesHack.cs#L33
    public void WorksSameAsRepo(decimal decimalNumber, int scale)
    {
        var numberAsString = AvroDecimalHelper.GetByteArrayFromDecimal(decimalNumber, scale);
        var unscaled = new BigInteger(new ReadOnlySpan<byte>(numberAsString), isUnsigned:false, isBigEndian:true);
        var fromRepo = ((decimal)unscaled)/Convert.ToDecimal(Math.Pow(10.0, scale));
        fromRepo.Should().Be(decimalNumber);
    }
}

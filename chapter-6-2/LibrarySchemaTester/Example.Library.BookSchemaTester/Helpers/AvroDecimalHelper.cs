using System.Numerics;

namespace LibrarySchemaTester.Helpers;

public static class AvroDecimalHelper
{
    // from https://github.com/confluentinc/confluent-kafka-dotnet/issues/648#issuecomment-434763819
    // todo verify if it works with another SDK
    public static decimal GetDecimalFromByteArray(byte[] bytes, int scale)
    {
        var result = new BigInteger(bytes.Reverse().ToArray());
        return (decimal) result * (decimal)Math.Pow(10, -scale);
    }

    // reverted from GetDecimalFromByteArray()
    public static byte[] GetByteArrayFromDecimal(decimal value, int scale)
    {
        BigInteger intValue = (BigInteger)(value * (decimal)Math.Pow(10, scale));
        byte[] bytes = intValue.ToByteArray();
        Array.Reverse(bytes);

        return bytes;
    }
}

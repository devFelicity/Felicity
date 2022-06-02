using System.Security.Cryptography;
using System.Text;
// ReSharper disable UnusedMember.Global

namespace FelicityOne.Helpers;

public static class ChecksumHelper
{
    public static string CalculateMD5(string filename)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filename);
        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
    }

    public static string Base64Encode(string input)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }
}
namespace Hestia.Persistence.Utility.Helpers;

public static class StringHelper
{
    public static string Hash(this string inputString)
        => BCrypt.Net.BCrypt.HashPassword(inputString);

    public static bool Verify(this string inputString, string stringToVerifyAgainst)
        => BCrypt.Net.BCrypt.Verify(inputString, stringToVerifyAgainst);
}
namespace Bazza.Extensions;

public static class StringExtensions
{
    public static string NormalizePhoneNumber(this string s)
    {
        s = s.TrimStart('0');
        s = s.Replace("/", "");
        s = s.Replace(" ", "");
        s = s.Replace("(", "");
        s = s.Replace(")", "");
        s = "+43" + s;
        return s;
    }
}

namespace RazorPagesYacht.Services;

public static class Plural
{
    // Uk(1, "відгук", "відгуки", "відгуків") -> "відгук"; 3 -> "відгуки"; 11 -> "відгуків"
    public static string Uk(int n, string one, string few, string many)
    {
        var n100 = Math.Abs(n) % 100;
        var n10 = n100 % 10;
        if (n100 is >= 11 and <= 14) return many;
        return n10 switch
        {
            1 => one,
            >= 2 and <= 4 => few,
            _ => many
        };
    }
}
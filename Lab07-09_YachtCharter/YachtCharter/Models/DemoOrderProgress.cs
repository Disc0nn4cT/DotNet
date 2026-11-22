namespace YachtCharter.Models;

// Навчальна імітація статусу за часом. Не читає БД і не знає про Razor.
public static class DemoOrderProgress
{
    public static string GetStatus(DateTime createdUtc, DateTime nowUtc)
    {
        var age = nowUtc - createdUtc;
        return age.TotalSeconds switch
        {
            < 15 => "Заявку прийнято",
            < 30 => "Яхту готують до виходу",
            _ => "Яхта чекає біля причалу"
        };
    }

    public static bool IsComplete(DateTime createdUtc, DateTime nowUtc)
        => (nowUtc - createdUtc).TotalSeconds >= 30;
}

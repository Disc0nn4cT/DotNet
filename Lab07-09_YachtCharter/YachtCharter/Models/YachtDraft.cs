namespace YachtCharter.Models;

public enum CrewKind { Bareboat, Skipper, FullCrew }

// Чернетка: поточний вибір у діалозі налаштування.
public class YachtDraft
{
    public int Days { get; set; } = 1;                       // параметр 1: тривалість оренди
    public CrewKind Crew { get; set; } = CrewKind.Bareboat;  // параметр 2: екіпаж
    public int Quantity { get; set; } = 1;                   // кількість яхт, 1–10

    // Скільки діб оплачується: при оренді на тиждень сьома доба безкоштовна.
    public int GetBilledDays() => Days switch
    {
        1 => 1,
        3 => 3,
        7 => 6,
        _ => throw new InvalidOperationException("Неприпустима тривалість оренди.")
    };

    // Доплата за екіпаж за одну добу.
    public decimal GetCrewExtra() => Crew switch
    {
        CrewKind.Bareboat => 0m,
        CrewKind.Skipper => 1500m,
        CrewKind.FullCrew => 2500m,
        _ => throw new InvalidOperationException("Неприпустимий варіант екіпажу.")
    };

    // Ціна однієї яхти = (ціна за добу + доплата за екіпаж) × оплачувані доби
    public decimal GetUnitPrice(Yacht yacht) => (yacht.BasePrice + GetCrewExtra()) * GetBilledDays();

    public OrderLine CreateLine(Yacht yacht)
    {
        if (Quantity is < 1 or > 10)
            throw new InvalidOperationException("Кількість яхт має бути від 1 до 10.");

        return new OrderLine
        {
            YachtId = yacht.Id,
            YachtName = yacht.Name,
            Days = Days,
            Crew = CrewName(Crew),
            Quantity = Quantity,
            UnitPrice = GetUnitPrice(yacht)
        };
    }

    public static string CrewName(CrewKind crew) => crew switch
    {
        CrewKind.Bareboat => "без шкіпера",
        CrewKind.Skipper => "зі шкіпером",
        CrewKind.FullCrew => "шкіпер і кок",
        _ => ""
    };

    public static string DaysText(int days) => days switch
    {
        1 => "1 доба",
        3 => "3 доби",
        7 => "7 діб",
        _ => $"{days} діб"
    };

    // Зворотне перетворення тексту з OrderLine на варіант екіпажу (для перевірки на сервері).
    public static CrewKind ParseCrew(string name) => name switch
    {
        "без шкіпера" => CrewKind.Bareboat,
        "зі шкіпером" => CrewKind.Skipper,
        "шкіпер і кок" => CrewKind.FullCrew,
        _ => throw new ArgumentException("Невідомий варіант екіпажу.")
    };
}

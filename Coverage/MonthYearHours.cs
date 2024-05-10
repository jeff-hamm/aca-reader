using Itenso.TimePeriod;

namespace AcaReader.Coverage;

public record MonthYearHours(int Year, YearMonth Month, double Hours) : MonthHours(Month, Hours)
{
    public DateTime DateTime => new DateTime(Year, (int)Month, 1);
}


public record MonthYear(int Year, YearMonth Month)
{
    public DateTime DateTime => new DateTime(Year, (int)Month, 1);
}
public record PeriodOfEmployment(MonthYear Start, MonthYear? End);

using AcaReader;
using Itenso.TimePeriod;
using Truncon.Collections;

public record MonthHours(YearMonth Month, double Hours)
{
    public bool IsFullTime => Hours >= 130;
    public double FteHours => IsFullTime ? 0 : Math.Min(Hours,120);
}

public record EmploymentYear(int Year, OrderedDictionary<YearMonth, MonthHours> Hours)
{
    private MonthHours[]? fullTimeMonths;
    private MonthHours[]? partTimeMonths;
    public DateTime StartDate => new DateTime(Year, 1, 1);

    public MonthHours[] PartTimeMonths =>
        partTimeMonths ??= Hours.Where(kvp => !kvp.Value.IsFullTime).Select(kvp => kvp.Value).ToArray();

    public MonthHours[] FullTimeMonths =>
        fullTimeMonths ??= Hours.Where(kvp => kvp.Value.IsFullTime).Select(kvp => kvp.Value).ToArray();

//    private double? totalHours;
//    public double TotalHours => totalHours ??= Hours.Values.Sum(m => m.FteHours);
    private double? fteHours;
    public double FteHours => fteHours ??= PartTimeMonths.Sum(m => m.FteHours);
    public bool WasFullTimeAllYear => FullTimeMonths.Length == 12;
    public YearMonth? PlanStartMonth { get; set; }

    public ICollection<InsurancePayment> Payments { get; } = new List<InsurancePayment>();
    private double? totalPayments;
    public double TotalPayments => totalPayments ??= Payments.Sum(m => m.Amount ?? 0);

    public double MonthPayment(YearMonth month) =>
        Payments.Where(p => p.Date.Month == (int)month).Sum(p => p.Amount ?? 0) * -1.0;

    public bool? IsSalaried { get; set; }

    public YearMonth? FirstServiceMonth => Hours.Values.FirstOrDefault(h => h.Hours > 0)?.Month;
    public YearMonth? LastServiceMonth => Hours.Values.LastOrDefault(h => h.Hours > 0)?.Month;
    public int NumberOfFullTimeMonths => FullTimeMonths.Length;
    public YearMonth? FirstFullTimeMonth => FullTimeMonths.Length > 0 ? FullTimeMonths.First().Month : null;
    public YearMonth? LastFullTimeMonth => FullTimeMonths.Length > 0 ? FullTimeMonths.Last().Month : null;

}
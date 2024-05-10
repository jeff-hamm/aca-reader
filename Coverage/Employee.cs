using Itenso.TimePeriod;
using Org.BouncyCastle.Crypto.Generators;

namespace AcaReader.Coverage;

public class Employee
{
    public required EmployeeName Name { get; init; }
    public ContactInfo? ContactInfo { get; set; }
    public DateTime HireDate => ContactInfo?.HireDate ?? DateTime.MinValue;
    public Stack<EmploymentYear> PreviousYears { get; } =  new ();
    public IEnumerable<EmploymentYear> Years => PreviousYears.Append(CurrentYear);
    public EmploymentYear? PreviousYear => PreviousYears.LastOrDefault();
    public EmploymentYear CurrentYear { get; set; } = null!;

    public IEnumerable<MonthYearHours> WorkedHours =>
        Years.SelectMany(y =>
            y.Hours
                .Where(kvp => new Month(y.Year,kvp.Value.Month).Start >= HireDate)
                .Select(kvp => new MonthYearHours(y.Year, kvp.Value.Month, kvp.Value.Hours)));

    public ICollection<InsurancePlan> Coverage { get; } = new List<InsurancePlan>();
    public double? Wage { get; set; }
}
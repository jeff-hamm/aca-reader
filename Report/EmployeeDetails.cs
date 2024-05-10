using Itenso.TimePeriod;

namespace AcaReader.Coverage;

public class EmployeeDetails
{
    public required DateTime HireDate { get; init; }
    public required DateTime? ReleaseDate { get; init; }

    public required ReportYear ReportingYear { get; init;  }
    public required EmployeeName Name { get; init; }
//    public required IAcaPeriod AcaPeriod { get; set; }
    public required ContactDetails? ContactInfo { get; init; }
//    public required PeriodMeasurement PreviousPeriod { get; init; }
    public int? AgeOnJan1st => ContactInfo?.Dob.GetAgeOnDate(ReportingYear.FirstDayStart);
    public bool ShouldBeReported => MonthDetails.Any(kvp => kvp.Value.ShouldBeReported);
    public CalendarList<EmployeeMonth> MonthDetails { get; } = new();

    /// <summary>
    /// Plan Start Month. This box is required for the 2023 Form 1095-C and the ALE Member may not leave it blank. To complete the box, enter the 2-digit number (01 through 12) indicating the calendar month during which the plan year begins of the health plan in which the employee is offered coverage (or would be offered coverage if the employee were eligible to participate in the plan). If more than one plan year could apply (for instance, if the ALE Member changes the plan year during the year), enter the earliest applicable month. If there is no health plan under which coverage is offered to the employee, enter “00.”
    /// </summary>
    public int PlanStartMonth => MonthDetails.Where(m => m.Value.Type == EmployeeType.FullTime).Select(s => s.Key)
        .FirstOrDefault() is { } month
        ? (int)month
        : 0;

    public required MeasuredPeriod InitialPeriod { get; init; }
    public required MeasuredPeriod PreviousPeriod { get; set; }
    public required MeasuredPeriod CurrentPeriod { get; set; }
    public double? Wage { get; set; }
    public bool IsPlanAffordable { get; set; }
    public double? PlanWagePercentage { get; set; }
}
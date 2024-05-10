using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcaReader.Coverage;
using AcaReader.Input;
using Itenso.TimePeriod;

namespace AcaReader.Calculations;

public class ReportGenerator(ReportYear reportYear)
{
    public IEnumerable<EmployeeDetails> Generate(EmployeeList employees)
    {
        foreach (var employee in employees.Values)
        {
            if (!employee.WasEmployedDuringYear(reportYear))
            {
                Debug.WriteLine($"Skipping employee {employee.Name}, not active during year");
                continue;
            }
            yield return ToReportRecord(employee);
        }
    }

    private EmployeeDetails ToReportRecord(Employee employee)
    {
        if (employee.Name.FirstName == "Charleslumene")
        {
            int i = 0;
        }
        var hireDate = employee.ContactInfo?.HireDate ?? DateTime.MinValue;
        var details = new EmployeeDetails()
        {
            Name = employee.Name,
            HireDate = hireDate,
            ReleaseDate = employee.ContactInfo?.ReleaseDate,
            ReportingYear = reportYear,
            ContactInfo = employee.ContactInfo != null ? new ContactDetails(employee.ContactInfo):null,
            InitialPeriod = new MeasuredPeriod( 
                new AcaPeriod(
                hireDate, reportYear.StandardPeriod.Months)),
            PreviousPeriod = new MeasuredPeriod(reportYear.PreviousPeriod),
            CurrentPeriod = new MeasuredPeriod(reportYear.StandardPeriod),
            Wage = employee.Wage,
            IsPlanAffordable = employee.Wage > EmployerConfig.AffordabilityPercentage[reportYear.BaseYear].AnnualMinimumSalary,
            PlanWagePercentage = EmployerConfig.AffordabilityPercentage[reportYear.BaseYear].AnnualMinumumRate / employee.Wage,
            //            PreviousPeriod = employee.WorkedHours.MeasurePeriod(reportYear.PreviousPeriod.ForHireDate(hireDate)),
        };
        foreach (var month in reportYear.GetMonths().OfType<Month>())
        {
            var employmentType = GetEmploymentType(employee, details, month, out var periodType);
            var empMonth = new EmployeeMonth()
            {
                MeasurementPeriod = periodType,
                IsAffordable = details.IsPlanAffordable,
                Insurance = employee.Coverage.Where(p => p.ActiveRange.IntersectsWith(month)).ToArray(),
                Type = employmentType,
                Hours = employee.CurrentYear.Hours[month.YearMonth].Hours,
            };

            empMonth.OfferValue = empMonth.IdealOfferCode == OfferCode.CoverageOffered
                ? EmployerConfig.AffordabilityPercentage[reportYear.BaseYear].MonthlyMinimumRate
                : 0;
            details.MonthDetails.Add(month.YearMonth, empMonth);
        }
        return details;
    }

    private EmployeeType GetEmploymentType(Employee employee, EmployeeDetails details, Month month, out MeasurementPeriod? periodType)
    {
        periodType = null;
        if (details.HireDate > month.End)
            return EmployeeType.NotYetHired;
        if (details.ReleaseDate < month.Start)
            return EmployeeType.NotEmployed;
        if (month.HasInside(details.HireDate))
            return EmployeeType.Evaluating;
        if (details.ReleaseDate.HasValue && month.HasInside(details.ReleaseDate.Value))
            return EmployeeType.Terminated;
        if (details.InitialPeriod.Period.GetPeriodType(month) is { } initialType)
        {
            periodType = MeasurementPeriod.Initial;
            details.InitialPeriod.MeasurePeriod(employee.WorkedHours);
            if (initialType is MeasurementPeriodType.Measurement or MeasurementPeriodType.Administrative)
                return EmployeeType.Evaluating;
            if (initialType != MeasurementPeriodType.Stable)
                throw new InvalidOperationException($"Unexpected period type {initialType}");
            if (details.InitialPeriod.Measurement.IsFullTime)
                return EmployeeType.FullTime;

            return EmployeeType.PartTime;
        }

        if (details.CurrentPeriod.Period.GetPeriodType(month) is not { } type)
            throw new InvalidOperationException($"No period type for {month}");
        if (type is MeasurementPeriodType.Measurement or MeasurementPeriodType.Administrative)
        {
            if (details.PreviousPeriod.Period.GetPeriodType(month) != MeasurementPeriodType.Stable)
                throw new InvalidOperationException($"Unexpected period type {type}");
            periodType = MeasurementPeriod.Previous;
            if (details.PreviousPeriod.MeasurePeriod(employee.WorkedHours).IsFullTime)
                return EmployeeType.FullTime;
            return EmployeeType.PartTime;
        }
        periodType = MeasurementPeriod.Current;
        if (type != MeasurementPeriodType.Stable)
            throw new InvalidOperationException($"Unexpected period type {type}");
        if (details.CurrentPeriod.MeasurePeriod(employee.WorkedHours).IsFullTime)
            return EmployeeType.FullTime;
        return EmployeeType.PartTime;
    }
}

public record PeriodMeasurement(double Hours, int Months)
{
    public bool HasHours => Hours > 0;
    public bool IsFullTime => Average >= 130.0;
    public double Average => Hours / Months;
}
public static class EmployeeExtensions
{
    public static bool WasEmployedDuringYear(this Employee employee, ReportYear year) =>
        (employee.ContactInfo?.HireDate ?? DateTime.MinValue) <= year.End &&
        (employee.ContactInfo?.ReleaseDate ?? DateTime.MaxValue) >= year.Start &&
        employee.CurrentYear.Hours.Any();
    public static bool HasWorkedFullTime(this Employee employee) =>
        employee.CurrentYear.FirstFullTimeMonth != null;
    public static bool HadActiveCoverage(this Employee employee, ReportYear year) =>
        employee.Coverage.Any(c => c.ActiveRange.IntersectsWith(year));
}

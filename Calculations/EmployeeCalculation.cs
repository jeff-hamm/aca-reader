using Itenso.TimePeriod;

namespace AcaReader.Coverage;

public abstract class EmployeeCalculation(Employee employee)
{
    public const int NumberOfMonthsForNewPeriod = 3;

    public abstract IEnumerable<(YearMonth, double? payment, EmploymentType)> ShouldBeOfferedInsurance(
        PeriodOfEmployment period);


    public IEnumerable<PeriodOfEmployment> GetPeriodsOfEmployment()
    {
        MonthYear? start = null;
        //if (employee.ContactInfo?.HireDate < previousYear)
        //    start = new MonthYear(previousYear.Year,(Months)previousYear.Month);
        MonthYearHours? previous = null;
        MonthYear? possibleEnd = null;
        int? notWorkingMonths = null;
        foreach (var worked in employee.WorkedHours)
        {
            if (start == null)
            {
                if(worked.Hours == 0)
                    continue;
                start = new MonthYear(worked.Year, worked.Month);
            }
            else
            {
                if (worked.Year < start.Year || (worked.Year == start.Year && worked.Month < start.Month))
                {
                    if(worked.Hours == 0)
                        continue;
                    throw new InvalidOperationException("Worked hours out of order");
                }

                if (worked.Hours == 0)
                {
                    if (possibleEnd == null)
                    {
                        if(previous == null)
                            throw new InvalidOperationException("No previous month, logic error");
                        possibleEnd = new MonthYear(previous.Year, previous.Month);
                        notWorkingMonths = 1;
                    }
                    else
                    {
                        notWorkingMonths++;
                        if (notWorkingMonths >= NumberOfMonthsForNewPeriod)
                        {
                            yield return new PeriodOfEmployment(start, possibleEnd);
                            Reset();
                        }
                    }
                    continue;
                }

            }

            previous = worked;
        }
        if(start != null)
            yield return new PeriodOfEmployment(start, possibleEnd);

        void Reset()
        {
            start = null;
            previous = null;
            possibleEnd = null;
            notWorkingMonths = null;
        }
    }

    public MonthYear? GetPeriodStart()
    {
        if (employee.PreviousYear?.FirstServiceMonth is { } previousStart)
            return new MonthYear(employee.PreviousYear.Year, previousStart);
        if (employee.CurrentYear?.FirstServiceMonth is { } start)
            return new MonthYear(employee.CurrentYear.Year, start);
        return null;
    }

    public MonthYear? GetPeriodEnd()
    {
        if (employee.PreviousYear?.FirstServiceMonth is { } previousStart)
            return new MonthYear(employee.PreviousYear.Year, previousStart);
        if (employee.CurrentYear?.FirstServiceMonth is { } start)
            return new MonthYear(employee.CurrentYear.Year, start);
        return null;
    }
}
using Itenso.TimePeriod;

namespace AcaReader.Coverage;

public class LookbackCalculation(Employee employee) : EmployeeCalculation(employee)
{

    public int StandardMeasurementMonths { get; set; } = 12;
    // Code 2d
    public int InitialMeasurementPeriod { get; set; } = 3;
    public int AdministrativePeriod { get; set; } = 0;
    public int StabilityPeriod { get; set; } = 12;
    public override IEnumerable<(YearMonth, double? payment, EmploymentType)> ShouldBeOfferedInsurance(PeriodOfEmployment period)
    {
        if (period.End?.DateTime < employee.CurrentYear.StartDate)
        {
            //            Console.WriteLine("Not in current year");
            yield break;
        }
        var startMeasurement = new DateTime(employee.CurrentYear.Year - 1, 13 - StandardMeasurementMonths, 1);
        if (period.Start.DateTime <= startMeasurement)
        {
            var sum = 0.0;
            for (int m = 0; m < StandardMeasurementMonths; m++)
            {
                sum += employee.PreviousYear!.Hours[(YearMonth)(12 - m)].Hours;
            }

            var average = sum / StandardMeasurementMonths;
            if (average > 130)
            {

                foreach (var m in Enum.GetValues<YearMonth>())
                {
                    var amount = employee.CurrentYear.MonthPayment(m);
                    if (period.End?.Month == null || period.End.Month >= m)
                    {
                        yield return (m, amount, EmploymentType.Existing);
                    }
                    else
                    {
                        yield return (m, amount, EmploymentType.Inactive);
                    }
                }
            }
        }
        else
        {

            var sum = 0.0;
            var initialCount = 0;
            var average = 0.0;
            foreach (var workedHours in employee.WorkedHours)
            {
                var amount = employee.CurrentYear.MonthPayment(workedHours.Month);
                if (workedHours.DateTime < period.Start.DateTime)
                    continue;
                if (workedHours.DateTime > period.End?.DateTime)
                {
                    if (average >= 130)
                        yield return (workedHours.Month, amount, EmploymentType.Inactive);
                    continue;
                }
                if (initialCount < InitialMeasurementPeriod)
                {
                    if (workedHours.Year == employee.CurrentYear.Year)
                    {
                        if(workedHours.IsFullTime)
                            yield return (workedHours.Month, amount, EmploymentType.Initial);
                        else if (amount > 0)
                            yield return (workedHours.Month, amount, EmploymentType.PartTime);
                    }
                    sum += workedHours.Hours;
                    initialCount++;
                    if (initialCount == InitialMeasurementPeriod)
                        average = sum / InitialMeasurementPeriod;
                    continue;
                }
                if (average >= 130 && workedHours.Year == employee.CurrentYear.Year)
                {
                    yield return (workedHours.Month, amount, EmploymentType.Stable);
                }
                else if (amount > 0)
                {
                    yield return (workedHours.Month, amount, EmploymentType.PartTime);
                }

            }
        }
    }
}
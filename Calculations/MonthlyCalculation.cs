using Itenso.TimePeriod;

namespace AcaReader.Coverage;

public class MonthlyCalculation(Employee employee) : EmployeeCalculation(employee)
{
    public const int InitialMeasurementPeriod = 3;
    public override IEnumerable<(YearMonth, double? payment, EmploymentType)> ShouldBeOfferedInsurance(PeriodOfEmployment period)
    {
        bool qualified =
            employee.PreviousYear?.Payments.Any() == true || employee.PreviousYear?.LastFullTimeMonth <= YearMonth.October;
        var sum = 0.0;
        int sumCount = 0;
        var average = 0.0;

        foreach (var workedHours in employee.WorkedHours)
        {
            var amount = employee.CurrentYear.MonthPayment(workedHours.Month);

            if (workedHours.DateTime < period.Start.DateTime ||
                workedHours.DateTime > period.End?.DateTime) 
                continue;
            if (!qualified)
            {
                if (workedHours.IsFullTime)
                {
                    if(workedHours.Year == employee.CurrentYear.Year)
                        yield return (workedHours.Month, amount, EmploymentType.Initial);
                    sumCount++;
//                    sum += workedHours.Hours;
                    if (sumCount == InitialMeasurementPeriod)
                    {
                        qualified = true;
//                        average = sum / sumCount;
//                        qualified = average >= 130;
                    }
                }
            }
            else if (workedHours.IsFullTime)
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
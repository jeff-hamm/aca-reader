using System.Diagnostics.CodeAnalysis;
using AcaReader.Calculations;

namespace AcaReader.Coverage;

public class MeasuredPeriod(AcaPeriod period)
{
    public AcaPeriod Period { get; } = period;
    public PeriodMeasurement? Measurement { get; private set; }


    [MemberNotNull(nameof(Measurement))]
    public PeriodMeasurement MeasurePeriod(IEnumerable<MonthYearHours> workedHours)
    {
        if(Measurement != null)
            return Measurement;
        var measurementHours = 0.0;
        var measurementMonths = 0;
        bool? measuring = null;
        foreach (var hours in workedHours)
        {
            if (hours.DateTime > Period.End)
                break;
            if (hours.DateTime < Period.Start)
                continue;
            if (Period.GetPeriodType(hours.DateTime) is not { } type)
                continue;
            if (type == MeasurementPeriodType.Measurement)
            {
                if (measuring == false)
                    throw new InvalidOperationException($"Multiple measurement periods are not supported");
                measurementHours += hours.Hours;
                measurementMonths++;
                measuring = true;
                continue;
            }
            if (type is not MeasurementPeriodType.Stable and not MeasurementPeriodType.Administrative)
                throw new NotImplementedException();
            break;
        }

        if (measuring != true)
        {
//            Console.WriteLine($"No hours found for {Period}");
            Measurement = new PeriodMeasurement(0, 0);
        }
        else
            Measurement = new PeriodMeasurement(measurementHours, Period.Months);
        return Measurement;
    }
}
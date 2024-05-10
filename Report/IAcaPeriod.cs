using Itenso.TimePeriod;

namespace AcaReader.Coverage;

public interface IAcaPeriod : ITimePeriod
{
    MeasurementPeriodType? GetPeriodType(DateTime date);
    MeasurementPeriodType? GetPeriodType(ITimePeriod period)
    {
        if (GetPeriodType(period.Start) is { } type)
            return type;
        return GetPeriodType(period.End);
    }
}
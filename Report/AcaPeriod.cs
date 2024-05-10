using Itenso.TimePeriod;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AcaReader.Coverage;

public class AcaPeriod : TimePeriodChain, IAcaPeriod
{
    public AcaPeriod(DateTime startTime, int months)
    {
        Months = months;
        MeasurementPeriod = new TimeInterval(startTime, 
            startTime.AddMonths(months-AcaConfig.AdministrativePeriod), endEdge: IntervalEdge.Open);
        AdministrativePeriod = 
            new TimeInterval(MeasurementPeriod.EndInterval, MeasurementPeriod.EndInterval.AddMonths(AcaConfig.AdministrativePeriod), IntervalEdge.Open);
        StabilityPeriod = new TimeInterval(AdministrativePeriod.EndInterval, AdministrativePeriod.EndInterval.AddMonths(months), IntervalEdge.Open);
        base.Add(MeasurementPeriod);
        base.Add(AdministrativePeriod);
        base.Add(StabilityPeriod);
    }


    public AcaPeriod(int year, int months, TimeCalendar calendar) : this(
        new Year(year, calendar).FirstDayStart.AddMonths(months*-1),months)
    {
        Year = new Year(year, calendar);
    }
    public Year? Year { get; }
    public TimeInterval MeasurementPeriod { get; }
    public TimeInterval AdministrativePeriod { get; set; }
    public TimeInterval StabilityPeriod { get; }
    public int Months { get; }
    public bool IsInitial(DateTime date) => false;
    public IAcaPeriod ForHireDate(DateTime hireDate)
    {
        if (hireDate > this.End)
            throw new ArgumentOutOfRangeException(nameof(hireDate), hireDate, "Hire date is after the end of the year");
        var hirePeriod = new AcaPeriod(hireDate, AcaConfig.InitialPeriod);
        if (hirePeriod.IntersectsWith(this))
            return new EmployeeAcaPeriod(hirePeriod, this);
        return this;
    }

    public MeasurementPeriodType? GetPeriodType(DateTime date)
    {
        if (StabilityPeriod.HasInside(date))
            return MeasurementPeriodType.Stable;
        if (AdministrativePeriod.HasInside(date))
            return MeasurementPeriodType.Stable;
        if (MeasurementPeriod.HasInside(date))
            return MeasurementPeriodType.Measurement;
        return null;
    }
    public MeasurementPeriodType? GetPeriodType(ITimePeriod period)
    {
        if (GetPeriodType(period.Start) is { } type)
            return type;
        return GetPeriodType(period.End);
    }
}
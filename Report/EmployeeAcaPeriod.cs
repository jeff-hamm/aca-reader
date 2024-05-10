using AcaReader.Calculations;
using Itenso.TimePeriod;

namespace AcaReader.Coverage;

public class EmployeeAcaPeriod : TimePeriodCollection, IAcaPeriod
{
    private readonly AcaPeriod initialPeriod;
    private readonly AcaPeriod fallbackPeriod;

    public EmployeeAcaPeriod(AcaPeriod initialPeriod, AcaPeriod fallbackPeriod)
    {
        this.initialPeriod = initialPeriod;
        this.fallbackPeriod = fallbackPeriod;
        base.Add(initialPeriod);
        base.Add(fallbackPeriod);
    }

    public MeasurementPeriodType? GetPeriodType(DateTime date) => 
        initialPeriod.GetPeriodType(date) ?? fallbackPeriod.GetPeriodType(date);
}
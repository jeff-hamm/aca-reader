using System.Collections;
using Itenso.TimePeriod;

namespace AcaReader.Coverage;

public class ReportYear(int year) : YearTimeRange(year, 1, new TimeCalendar())
{
    public AcaPeriod PreviousPeriod { get; } = new(year-1, AcaConfig.OngoingPeriod, AcaConfig.PlanCalendar);
    public AcaPeriod StandardPeriod { get; } = new (year,AcaConfig.OngoingPeriod, AcaConfig.PlanCalendar);

}
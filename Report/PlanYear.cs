using Itenso.TimePeriod;
using System.Globalization;

namespace AcaReader.Coverage;

public enum MeasurementPeriodType
{
    Stable = 0x02,
    Measurement = 0x04,
    Administrative = 0x08,
}

public class PlanYear(int year) : YearTimeRange(year, 1, calendar)
{
    private static readonly TimeCalendar calendar = AcaConfig.PlanCalendar;
    public PlanYear(DateTime moment)
    : this(TimeTool.GetYearOf(calendar.YearBaseMonth, calendar.GetYear(moment), calendar.GetMonth(moment)))
    {
    }

    public int YearValue => this.Calendar.GetYear(this.BaseYear, (int)this.YearBaseMonth);

    public string YearName => this.StartYearName;
//    public int CalendarYear => this.Year

    public bool IsCalendarYear => this.YearBaseMonth == Itenso.TimePeriod.YearMonth.January;

    public Year GetPreviousYear() => this.AddYears(-1);

    public Year GetNextYear() => this.AddYears(1);

    public Year AddYears(int count)
    {
        return new Year(new DateTime(this.BaseYear, (int)this.YearBaseMonth, 1).AddYears(count), this.Calendar);
    }

    protected override string Format(ITimeFormatter formatter)
    {
        return formatter.GetCalendarPeriod(this.YearName, formatter.GetShortDate(this.Start), formatter.GetShortDate(this.End), this.Duration);
    }
}
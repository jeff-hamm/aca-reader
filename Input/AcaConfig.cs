using Itenso.TimePeriod;

namespace AcaReader.Coverage;

public static class AcaConfig
{
    public static readonly TimeCalendar PlanCalendar = new TimeCalendar(new TimeCalendarConfig()
    {
        YearBaseMonth = Itenso.TimePeriod.YearMonth.August,
    });

    public static int OngoingPeriod { get; set; } = 12;
    public static int InitialPeriod { get; set; } = 12;
    public static int AdministrativePeriod { get; set; } = 1;

    public static int? GetAgeOnDate(this DateTime? dob, DateTime compareDate)
    {
        if (dob is not { } d)
            return null;
        DateTime n = compareDate;
        int age = n.Year - d.Year;

        if (n.Month < d.Month || (n.Month == d.Month && n.Day < d.Day))
            age--;

        return age;
    }
}
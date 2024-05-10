using Itenso.TimePeriod;

namespace AcaReader.Coverage;

public class EmployerReport
{

    public int TotalNumberOfForms { get; init; }
    public bool IsAleGroup { get; } = false;
    public bool MinimumEssentialCoverageProvided => MemberInformation
        .Values.All(x => x.MinimumEssentialCoverageProvided);

    public CalendarList<MonthlyEmployerInformation> MemberInformation { get; } = new();
}

public record MonthlyEmployerInformation(
    YearMonth Month,
    bool MinimumEssentialCoverageProvided,
    int FullTimeEmployeeCount,
    int TotalEmployeeCount
)
{
    /*If the ALE Member offered minimum essential coverage, including an individual coverage HRA,
        to at least 95% of its full-time employees and their dependents for the entire calendar year, enter “X” in the “Yes” checkbox on line 23 for “All 12 Months” or for each of the 12 calendar months.
       If the ALE Member offered minimum essential coverage, including an individual coverage HRA, to at least 95% of its full-time employees and their dependents only for certain calendar months, enter “X” in the “Yes” checkbox for each applicable month.
       For the months, if any, for which the ALE Member did not offer minimum essential coverage, including an individual coverage HRA, to at least 95% of its full-time employees and their dependents, enter “X” in the “No” checkbox for each applicable month.
       If the ALE Member did not offer minimum essential coverage, including an individual coverage HRA, to at least 95% of its full-time employees and their dependents for any of the 12 months, enter “X” in the “No” checkbox for “All 12 Months” or for each of the 12 calendar months.
    */
}
using System.Globalization;
using AcaReader.Coverage;
using AcaReader.DetailReport;
using CsvHelper;

public class CountsReport(IEnumerable<EmployeeDetails> employees)
{
    public void Generate(string fileName)
    {
        using var writer = new CsvWriter(new StreamWriter(fileName), CultureInfo.InvariantCulture);
        var monthCounts = SumCounts();
        writer.WriteField("");
        foreach (var month in monthCounts.Keys)
            writer.WriteField(month);
        writer.NextRecord();
        writer.WriteField("Full Time");
        foreach (var month in monthCounts.Values)
            writer.WriteField(month.FullTime);
        writer.NextRecord();
        writer.WriteField("Total Employees");
        foreach (var month in monthCounts.Values)
            writer.WriteField(month.Total);
    }

    private CalendarList<DetailReport.MonthCount> SumCounts()
    {
        var monthCounts = new CalendarList<DetailReport.MonthCount>();
        foreach (var employee in employees)
        {
            foreach (var month in employee.MonthDetails)
            {
                if (!monthCounts.TryGetValue(month.Key, out var count))
                    count = monthCounts[month.Key] = new DetailReport.MonthCount();

                if (month.Value.Type == EmployeeType.FullTime)
                {

                    count.FullTime++;
                    count.Total++;
                }
                else if (month.Value.Type is EmployeeType.PartTime or EmployeeType.Evaluating)
                {
                    count.Total++;
                }
            }
        }
        return monthCounts;
    }

}
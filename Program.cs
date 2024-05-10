using System.Globalization;
using AcaReader;
using AcaReader.Calculations;
using AcaReader.Coverage;
using AcaReader.DetailReport;
using AcaReader.Input;
using CsvHelper;
using Itenso.TimePeriod;

var year = 2021;
var employees = new EmployeeList(year);

var generator = new ReportGenerator(employees.Year);

using var csvWriter = new CsvWriter(new StreamWriter($"{year}-1095c-summary.csv"), CultureInfo.CurrentCulture);
csvWriter.WriteField("Name, SSN, Street1, City, State, Zip, FirstMonth, Jan, Feb, March, April, May, June, July, Aug, Sept, Oct, Nov, Dec, Warning", false);
csvWriter.NextRecord();
var formFiller = new FormFiller($"f1095c--{year}.pdf", csvWriter);

List<EmployeeDetails> employeeDetails = new();
foreach (var employee in generator.Generate(employees))
{
    if (employee.ShouldBeReported)
        formFiller.FillForm(employee);
    employeeDetails.Add(employee);
}
employeeDetails.Sort((a, b) => a.Name.CompareTo(b.Name));

await using var evidenceReport = new DetailReport( employeeDetails, employees.Year);
evidenceReport.Generate($"{year}-fulltime-evidence.csv");
var countsReport = new CountsReport(employeeDetails);
countsReport.Generate($"{year}-1094c-details.csv");
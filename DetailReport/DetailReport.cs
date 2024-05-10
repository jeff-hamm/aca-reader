using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using AcaReader.Coverage;
using CsvHelper;
using Itenso.TimePeriod;

namespace AcaReader.DetailReport;
public class DetailReport(IEnumerable<EmployeeDetails> employees, ReportYear reportYear)
{
    private CsvWriter? writer;
    public void Generate(string fileName)
    {
        writer?.Dispose();
        using var csv = new CsvWriter(new StreamWriter(fileName), CultureInfo.InvariantCulture);
        writer = csv;
        WriteRow("Name", e => e.Name);
        WriteRow("SSN", e => e.ContactInfo?.Ssn[^4..]);
        WriteRow("HireDate", e => e.HireDate == DateTime.MinValue ? "" : e.HireDate.ToString("d"));
        WriteRow("ReleaseDate", e => e.ReleaseDate.HasValue && e.ReleaseDate != DateTime.MaxValue? e.ReleaseDate.Value.ToString("d") : "");
        WriteRow("IsPlanAffordable?", e => e.IsPlanAffordable ? "Yes" : "No");
        WriteRow("PlanWagePercentage", e => e.PlanWagePercentage?.ToString("P"));
        WritePeriod("Initial", e => e.InitialPeriod);
        WritePeriod((reportYear.BaseYear -1).ToString(), e => e.PreviousPeriod);
        WritePeriod((reportYear.BaseYear).ToString(), e => e.CurrentPeriod);
        WriteResult("Initial", e => e.InitialPeriod);
        WriteResult((reportYear.BaseYear - 1).ToString(), e => e.PreviousPeriod);
        WriteResult((reportYear.BaseYear).ToString(), e => e.InitialPeriod);
        foreach (var month in MonthsExtensions.Months)
        {
            WriteRow(month.ToString() + " Result", e =>
            {
                var details = e.MonthDetails[month];
                var result = details.Type.ToString();
                if (details.MeasurementPeriod != null)
                {
                    result += " [" + details.MeasurementPeriod.ToString() + "]";
                }

                return result;
            });
            //WriteRow(month.ToString() + " Measured By", e =>
            //{
            //    var details = e.MonthDetails[month];
            //    return details.MeasurementPeriod?.ToString() ?? "N/A";
            //});
        }
    }

    private void WriteResult(string prefix, Func<EmployeeDetails, MeasuredPeriod> periodFn)
    {
        WriteRow(prefix + " Stability Result", e =>
        {
            var period = periodFn(e);
            if (period.Measurement == null)
                return "N/A";
            return period.Measurement.IsFullTime ? "Full Time" : "Part Time";
        });
    }

    private void WritePeriod(string prefix, Func<EmployeeDetails, MeasuredPeriod> periodFn)
    {
        WriteRow(prefix + " Measurement Dates", e =>
        {
            var period = periodFn(e);
            if (period.Measurement == null)
                return "N/A";
            return FromHire(e,period.Period.MeasurementPeriod.Start).ToString("d") + " - " + 
                   FromRelease(e,period.Period.MeasurementPeriod.End).ToString("d");
        });
        WriteRow(prefix + " Measurement Average", e =>
        {
            var period = periodFn(e);  
            if(period.Period.MeasurementPeriod.End > reportYear.End)
                return "Incomplete";
            return period.Measurement?.Average.ToString("F2") ?? "N/A";
        });
        WriteRow(prefix + " Measurement Total", e =>
        {
            var period = periodFn(e);
            return period.Measurement?.Hours.ToString("F2") ?? "N/A";
        });

        WriteRow(prefix + " Administrative Dates", e =>
        {
            var period = periodFn(e);
            if (period.Measurement == null)
                return "N/A";
            return FromHire(e, period.Period.AdministrativePeriod.Start).ToString("d") + " - " +
                   FromRelease(e, period.Period.AdministrativePeriod.End).ToString("d");
        });
        WriteRow(prefix + " Stability Dates", e =>
        {
            var period = periodFn(e);
            if (period.Measurement == null)
                return "N/A";
            return FromHire(e, period.Period.StabilityPeriod.Start).ToString("d") + " - " + 
                   FromRelease(e,period.Period.StabilityPeriod.End).ToString("d");
        });
    }

    private DateTime FromHire(EmployeeDetails e, DateTime date)
    {
        if(e.HireDate >= date)
            return e.HireDate;
        return date;
    }


    private DateTime FromRelease(EmployeeDetails e, DateTime date)
    {
        if (e.ReleaseDate < date)
            date = e.ReleaseDate.Value;
        //if(reportYear.End < date)
        //    date = reportYear.End;
        return date;
    }

    public void WriteRow<T>(string label, Func<EmployeeDetails,T> value)
    {
        writer.WriteField(label);
        foreach (var employee in employees)
        {
            writer.WriteField(value(employee));
        }
        writer.NextRecord();
    }


    public void Dispose()
    {
        writer?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if(writer != null)
            await writer.DisposeAsync();
    }

    public class MonthCount
    {
        public int FullTime { get; set; }
        public int Total { get; set; }
    }
}
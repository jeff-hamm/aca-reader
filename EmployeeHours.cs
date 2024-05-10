using System.Text;
using System.Text.RegularExpressions;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Itenso.TimePeriod;
using Truncon.Collections;

namespace AcaReader;

public readonly record struct EmployeeHours([Index(1)] EmployeeName Name, int Year, OrderedDictionary<YearMonth, MonthHours> Hours)
{
    public class ClassMap : ClassMap<EmployeeHours>
    {
        public static string[] MonthNames =
            ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        private static readonly Regex MonthsRegex = new Regex(
            @"(?<Month>" +
            MonthNames.Aggregate(new StringBuilder(), (sb, month) => sb.Append(month).Append("|")).ToString().TrimEnd('|') + @")\s*(?<Year>\d+)"
            , RegexOptions.Compiled);
        public ClassMap()
        {
            Map(m => m.Name).Index(1).TypeConverter<EmployeeName.TypeConverter>();
            Map(m => m.Year).Convert(row =>
            {
                
                return 2000 + Int32.Parse(row.Row.HeaderRecord.Select(h => MonthsRegex.Match(h)).First(m => m.Success).Groups["Year"].Value);
            });

            Map(m => m.Hours).Convert(row =>
            {

                OrderedDictionary<YearMonth, MonthHours> c = new();
                foreach (var header in row.Row.HeaderRecord.Select(h => MonthsRegex.Match(h)).Where(m => m.Success))
                {
                    var month =
                        (YearMonth) (Array.IndexOf(MonthNames, header.Groups["Month"].Value) + 1);
                    c.Add(month,  
                        new MonthHours(month, Double.TryParse(row.Row.GetField(header.Value), out var v) ?v: 0));
                }

                return c;
            });
        }
    }
}


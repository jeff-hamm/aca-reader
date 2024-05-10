using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcaReader.Input;
using Itenso.TimePeriod;

namespace AcaReader.Coverage;
internal static class EmployeeCoverage
{

    internal static void ReadWages(this EmployeeList @this, int year, Reader reader)
    {
        var roster = reader.ReadWages($"Gross Wages {year}.csv");
        foreach (var entry in roster)
        {
            if (!@this.TryGetValue(entry.Name, out var employee))
            {
                @this.WarnIfMatchingLastName(@this.Keys, entry.Name);
                continue;
            }
            employee.Wage = entry.Wage;
        }
    }
    internal static void PopulateCoverage(this EmployeeList @this, Reader reader)
    {
        var roster = reader.ReadRoster($"Active Inactive Roster.csv");
        foreach (var entry in roster)
        {
            if (entry.SubscriberName.LastName == "Altieri")
            {
                int i = 0;
            }
            if (!@this.TryGetValue(entry.SubscriberName, out var employee))
            {
                if (entry.ActiveRange.HasInside(@this.Year))
                    throw new Exception($"Active subscriber {entry.SubscriberName} was not in current year, required to report");
                continue;
            }
            employee.Coverage.Add(entry);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcaReader.Coverage;
using Itenso.TimePeriod;

namespace AcaReader.Input;
public class EmployeeList : Dictionary<EmployeeName, Employee>
{
    public ReportYear Year { get; }

    public EmployeeList(int year)
    {
        Year = new ReportYear(year);
        var reader = new Reader();
        ReadCurrentHours(year, reader);
        ReadPreviousHours(year, reader);
        ReadPayments(year, reader);
        ReadContactInfo(reader);
        this.PopulateCoverage(reader);
        this.ReadWages(year, reader);
    }


    private void ReadContactInfo(Reader reader)
    {
        var contacts = reader.ReadContactInfo($"Contact List.csv").ToLookup(c => c.Name);
        foreach (var employee in this.Values)
        {
            if (!contacts.Contains(employee.Name) || contacts[employee.Name] is not { } contact)
            {
                WarnIfMatchingLastName(contacts.Select(s => s.Key), employee.Name);
                Console.WriteLine($"Contact info not found for {employee.Name}");
                continue;
            }
            employee.ContactInfo = contact.First();
        }
    }

    private void ReadPayments(int year, Reader reader)
    {
        var payments = reader.ReadInsurancePayments($"Medical Ins {year}.csv");
        foreach (var payment in payments)
        {
            if (!this.TryGetValue(payment.Name, out var employee))
            {
                if (payment.Date.Year != year)
                {
//                    Console.WriteLine($"Skipping {payment.Name} on {payment.Date} because not found and no current records");
                    continue;
                }

                //        ThrowIfMatchingLastName(employees.Keys, payment.Name);
//                Console.WriteLine($"Payment for {payment.Name} not found in contacts");
                employee = new Employee() { Name = payment.Name };
                this.Add(payment.Name, employee);
            }
            if (payment.Date.Year == employee.CurrentYear.Year)
                employee.CurrentYear.Payments.Add(payment);
            else if (payment.Date.Year == employee.PreviousYear.Year)
                employee.PreviousYear.Payments.Add(payment);
            else
                throw new Exception("Payment year does not match employee year");
        }
    }

    private void ReadPreviousHours(int year, Reader reader)
    {
        for (int y = year - 1; y > 0; y--)
        {
            if(!File.Exists($"ALE {y}.csv"))
                break;
            var previousHours = reader.ReadHours($"ALE {y}.csv");
            foreach (var hour in previousHours)
            {
                if (!this.TryGetValue(hour.Name, out var employee))
                {
                    WarnIfMatchingLastName(this.Keys, hour.Name);
//                    Console.WriteLine($"Employee {hour.Name} not found in currentYear");
                    continue;
                    //employee = new Employee() { Name = hour.Name };
                    //employees.Add(hour.Name, employee);
                }

                employee.PreviousYears.Push(new EmploymentYear(hour.Year, hour.Hours));
            }
        }

    }

    private void ReadCurrentHours(int year, Reader reader)
    {
        var hours = reader.ReadHours($"ALE {year}.csv");
        foreach (var hour in hours)
        {
            if (hour.Hours.Values.Sum(v => v.Hours) == 0)
            {
                Console.WriteLine($"Skipping {hour.Name} because no hours");
                continue;
            }

            if (!this.TryGetValue(hour.Name, out var employee))
            {
                if (hour.Name.LastName == "Nankya")
                {
                    int i = 0;
                }
                employee = new Employee() { Name = hour.Name };
                this.Add(hour.Name, employee);
            }

            if (employee.CurrentYear != null)
                throw new Exception("Employee already has current year hours");
            employee.CurrentYear = new EmploymentYear(hour.Year, hour.Hours);
        }
    }

    public void WarnIfMatchingLastName(IEnumerable<EmployeeName> dictionary, EmployeeName employeeName)
    {
        foreach (var name in dictionary)
        {
            if (employeeName.LastName.Equals(name.LastName, StringComparison.InvariantCulture))
                Console.WriteLine($"Employee not found for previous, but the a matching lastname was found {employeeName} vs {name}");
        }
    }
}

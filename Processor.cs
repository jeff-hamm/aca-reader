using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AcaReader;
using AcaReader.Coverage;
using Itenso.TimePeriod;
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Xfa;
using Truncon.Collections;

namespace AcaReader;

public enum Strategy
{
    Monthly,
}

public record FullTimeCount(YearMonth Month, int FullTime, double Fte, double Sum)
{
}
public class Processor(Employee[] employees) 
{
    public const double FullTimeHours = 130;

    public void Process()
    {
        foreach (var employee in employees)
        {
            var c = new LookbackCalculation(employee);
            foreach (var period in c.GetPeriodsOfEmployment())
            {
                var insuranceMonths = c.ShouldBeOfferedInsurance(period).ToArray();
                if(insuranceMonths.Length == 0)
                    continue;
                var missingMonths = new List<YearMonth>();
                foreach (var m in insuranceMonths)
                {
                    var found = false;
                    foreach (var p in employee.CurrentYear.Payments)
                    {
                        if (p.Date.Month == (int)m.Item1)
                        {
                            found = true;
                            break;
                        }
                    }
                    if(!found)
                        missingMonths.Add(m.Item1);
                }
                Console.Write($"{employee.Name.LastName} should be offered insurance for {String.Join(',',insuranceMonths)}");
                if(missingMonths.Any())
                    Console.Write($", was missing payments for {String.Join(',', missingMonths)}");
                Console.WriteLine();
            }
       }
    }

    public void Process(Strategy strategy)
    {
        var monthlyFte = new OrderedDictionary<YearMonth, FullTimeCount>();
        foreach (var month in Enum.GetValues<YearMonth>())
        {
            var ft = 0;
            double fteHours = 0;
            int fteCount = 0;
            foreach (var employee in employees)
            {
                if (employee.CurrentYear.IsSalaried == true)
                    ft++;
                if (!employee.CurrentYear.Hours.TryGetValue(month, out var hours))
                    continue;
                if (hours.IsFullTime)
                {
                    ft++;
                    continue;
                }
                fteHours += hours.FteHours;
                fteCount++;
            }

            var fte = fteHours / 120.0;
            var count = new FullTimeCount(month, ft, fte,ft + fte);
            monthlyFte.Add(month,count);
            Console.WriteLine($"{month}: {count:F2}");
        }
        Console.WriteLine($"Total: {monthlyFte.Values.Sum(c => c.Sum)/12.0:F2}");
    }

}


public class MonthlyAnalyzer
{
    public void Process(Employee employee) {
    //{
    //    foreach (var month in employee.CurrentYear.)
    //    {
    //        if (month.Value > 130) {
    //            employee.CurrentYear.FullTimeMonths.Add(month.Key);
    //        else

    //    }
    //    }
    //    var totalHours = hours.Values.Sum();
    //    if (totalHours > 130)
    //    {
    //        employee. = true;
    //    }
    }
}

public static class FieldExtensions
{
    public static PdfFormField FieldGroup(this PdfFormField @this, string groupName) =>
        @this.GetChildField($"{groupName}[0]");
    public static PdfFormField FieldGroup(this PdfAcroForm @this, string groupName) =>
        GetPage(@this,1).GetChildField($"{groupName}[0]");

    public static PdfFormField GetPage(this PdfAcroForm @this, int number) => 
        @this.GetField($"topmostSubform[0].Page{number}[0]");

    public static string Field(int number) => $"f1_{number}[0]";

    public static void SetValue(this PdfFormField field,int ix, string value)
    {
        if(String.IsNullOrWhiteSpace(value))
            return;
        field.GetChildField(Field(ix)).SetValue(value);
    }
}
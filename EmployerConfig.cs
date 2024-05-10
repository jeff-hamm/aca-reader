using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcaReader.Coverage;

namespace AcaReader;
// https://sentrichr.com/common-1095c-scenarios/
/*
 * Note. An affordability safe harbor code should not be entered on line 16 for any month that the ALE Member did not offer minimum essential coverage, including an individual coverage HRA, to at least 95% of its full-time employees and their dependents (that is, any month for which the ALE Member checked the “No” box on Form 1094-C, Part III, column (a)). For more information, see the instructions for Form 1094-C, Part III, column (a).
 */
public enum DeclinedCodes
{
    [Display(Name = "2F")]
    W2ButDeclined,
    [Display(Name = "2G")]
    PovertyLineButDeclined,
    [Display(Name = "2H")]
    RateOfPayButDeclined

}
/*
 *Note.  An affordability safe harbor code should not be entered on line 16 for any month that the ALE member did not offer minimum essential coverage to at least 95% of its full-time employees and their dependents (that is, any month for which the ALE member checked the “No” box on Form 1094-C, Part III, column (a)). For more information, see the instructions for Form 1094-C, Part III, column (a).
   
   
 */

public record InsurancePlanInfo(int Year, double AffordabilityPercentage, double BiWeeklyMinimumRate)
{

    public double WeeklyMinimumRate => BiWeeklyMinimumRate / 2;
    public double AnnualMinumumRate => WeeklyMinimumRate * 52; // 3857.88
    public double MonthlyMinimumRate => AnnualMinumumRate / 12; //  321.49
    public double HourlyMinimumRate => MonthlyMinimumRate / 130;
    public double MinumumHourlySalary => HourlyMinimumRate / AffordabilityPercentage; // 25.16
    public double AnnualMinimumSalary => AnnualMinumumRate / AffordabilityPercentage; //39,246
}
public static class EmployerConfig
{
    public static string Name { get; set; } = "Best Home Care";
    public static string Ein { get; set; } = "04-3548553";
    public static string Street1 { get; set; } = "45 Albion Street";
    public static string ContactTel { get; set; } = "781-224-3600";
    public static string City { get; set; } = "Wakefield";
    public static string State { get; set; } = "MA";
    public static string PostalCode { get; set; } = "USA 01880";
    public static int PlanYearStart { get; set; } = 1;
    public static double BiWeeklyMinimumRate { get; set; }



    public static Dictionary<int, InsurancePlanInfo> AffordabilityPercentage { get; } = new()
    {
        {2020,new (2020, .0978, 0)},
        {2021,new (2021, .0983, 148.38)},
        {2022,new (2022, .0961, 0)},
        {2023,new (2023, .0912, 0)}
    };

}

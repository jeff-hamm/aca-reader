using System.ComponentModel.DataAnnotations;

namespace AcaReader;

/*
 *Code Description
   1A
   Qualifying Offer: Minimum essential coverage ("MEC") providing minimum value ("MV")
   offered to full-time employee with employee contribution for self-only coverage equal to or
   less than 9.5 percent mainland single federal poverty line (FPL) (for 2015, 93.18/month), and
   at least MEC offered to spouse and dependent(s). If used, leave line 15 blank. (An employer
   may use 1A only if it checks box 22A or 22B on Form 1094-C.)
   1B MEC providing MV offered to employee only. Not applicable to the State Health Plan (SHP).
   1C MEC providing MV offered to employee and at least MEC to dependent(s) (not spouse). Not
   applicable to the SHP.
   1D MEC providing MV offered to employee and at least MEC to spouse (not dependent(s)). Not
   applicable to the SHP.
   1E MEC providing MV offered to employee and at least MEC to dependent(s) and spouse.
   1F MEC not providing MV offered to employee or employee and spouse and/or dependent(s).
   1G Offer of coverage to employee not a full-time employee for any month of the year and
   enrolled in self-insured coverage for one or more months.
   1H No offer of coverage or offered coverage that is not MEC.
   1I
   Qualified Offer Transition Relief 2015: Employee (and spouse or dependents) received no
   offer of coverage, received an offer that is not a qualifying offer, or received a qualifying
   offer for less than 12 months. If used, leave line 15 blank. (An employer may use 1I only if it
   checks box 22A or 22B on Form 1094-C.)
 */
public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var enumType = enumValue.GetType();
        var enumName = Enum.GetName(enumType, enumValue);
        var memberInfo = enumType.GetMember(enumName);
        var displayAttribute = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
        return displayAttribute?.Name ?? enumName;
    }
}
public enum OfferCode
{
    // Federal minimum -- no way
    [Display(Name = "1A")]
    PovertyLineQualifying,
    // Coverage was offered, including spouse and dependents
    // Requires a safe harbor code for rate of pay or w2 box 1
    [Display(Name = "1E")]
    CoverageOffered,
    // Insurance offered to part time employee (Should never be full year, don't report if full year!)
    [Display(Name = "1G")]
    PartTimeCoverage,
    // No offer made
    // Bad one, needs context
    [Display(Name = "1H")]
    NoCoverageOffered
}
using System.ComponentModel.DataAnnotations;

namespace AcaReader;


/*
 * Line 16 – Code Series 2 (safe harbors and other relief)
   Code Description
   2A Employee not employed during the month. Employee not employed any day of the month.
   Do not use for the month in which an employee terminates.
   2B
   Employee not a full-time employee. Employee is not a full-time employee for the month and
   did not enroll in MEC if offered for the month. Also use 2B if an offer of coverage to a fulltime employee for the month ended before the last day of the month because the employee
   terminated employment during the month.
   2C Employee enrolled in coverage offered. Employee enrolled in coverage for every day of the
   month. This code usually supersedes all other codes.
   2D
   Employee in a section 4980H(b) Limited Non-Assessment Period. Employee in a waiting
   period, initial measurement period, initial administrative period, or other limited nonassessment period.
   2E Multiemployer interim rule relief. Applies only to multiemployer plans, not applicable to
   the SHP.
   2F
   Section 4980H affordability Form W-2 safe harbor. Employer offered coverage affordable
   under the Form W-2 safe harbor, but the employee did not enroll. If used for an employee,
   the employer must use it for all months in which the employer offered the employee
   coverage.
   2G Section 4980H affordability FPL safe harbor. Employer offered coverage affordable under
   the federal poverty line safe harbor, but the employee did not enroll.
   2H Section 4980H affordability rate of pay safe harbor. Employer offered coverage affordable
   under the rate of pay safe harbor, but the employee did not enroll.
   2I Non-calendar year transition relief applies to this employee. Not applicable to the SHP
 */
public enum SafeHarborCode
{
    [Display(Name = "2C")]
    EmployeeEnrolled,
    // Employee not employed 
    [Display(Name ="2A")]
    NotEmployed,
    [Display(Name = "2B")]
    NotFullTimeOrTerminated,
    [Display(Name = "2D")]
    InitialMeasurementPeriod,
    [Display(Name = "2F")]
    W2ButDeclined,
    [Display(Name = "2G")]
    PovertyLineButDeclined,
    [Display(Name = "2H")]
    RateOfPayButDeclined
}
namespace AcaReader.Coverage;

public enum EmployeeType
{
    FullTime,
    Evaluating,
    PartTime,
    NotYetHired,
    NotEmployed,
    Terminated,
}

public enum MeasurementPeriod
{
    Initial,
    Previous,
    Current
}
public class EmployeeMonth
{
//    public bool IsFullTime => Type == EmployeeType.FullTime;
    public required EmployeeType Type { get; init; }
    public MeasurementPeriod? MeasurementPeriod { get; set; }
    public double Hours { get; set; }
    public bool ShouldBeReported => Insurance.Any() || Type == EmployeeType.FullTime;
    public InsurancePlan[] Insurance { get; set; } = Array.Empty<InsurancePlan>();
    //public double OfferValue { get; set; }
    //public double TotalWages { get; set; }
    //public double HourlyWage { get; set; }
    public bool IsAffordable { get; set; }
    public double OfferValue { get; set; }
    public SafeHarborCode? HarborCode => 
        Insurance.Any() ? AcaReader.SafeHarborCode.EmployeeEnrolled:
        Type switch
        {
            EmployeeType.FullTime => IsAffordable ? AcaReader.SafeHarborCode.W2ButDeclined : null,
            EmployeeType.Terminated => SafeHarborCode.NotFullTimeOrTerminated,
            EmployeeType.Evaluating => SafeHarborCode.InitialMeasurementPeriod,
            EmployeeType.NotEmployed =>  SafeHarborCode.NotEmployed,
            EmployeeType.NotYetHired => SafeHarborCode.NotEmployed,
            EmployeeType.PartTime => SafeHarborCode.NotFullTimeOrTerminated,
            _ => throw new InvalidOperationException($"Unexpected employee type {Type}")
        };
    public OfferCode IdealOfferCode => 
        Insurance.Any() ? OfferCode.CoverageOffered :
                Type switch
            {
                EmployeeType.FullTime => OfferCode.CoverageOffered,
                EmployeeType.Terminated => OfferCode.PartTimeCoverage,
                _ => OfferCode.NoCoverageOffered
            };


}
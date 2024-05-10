namespace AcaReader.Coverage;

public enum EmploymentType
{
    Existing,
    Inactive,
    // Don't need to offer insurance to new employees
    Initial,
    Stable,
    PartTime
}
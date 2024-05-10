using CsvHelper.Configuration.Attributes;

namespace AcaReader;

public record ContactInfo([Index(0)]string Ssn, [Index(2)]string FirstName, [Index(4)]string MiddleInitial, 
    [Index(6)] string LastName, [Index(8)] string Street1,
    [Index(10)] string Street2, [Index(12)] string City, [Index(14)] string State, [Index(16)]string Zip, [Index(18)]DateTime? Dob, [Index(20)] DateTime? HireDate, [Index(22)] DateTime? ReleaseDate)
{
    private EmployeeName? name;
    [Ignore]
    public EmployeeName Name => name ??= new EmployeeName(LastName,FirstName,MiddleInitial);
}
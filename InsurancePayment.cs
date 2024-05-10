using CsvHelper.Configuration.Attributes;

namespace AcaReader;

public record InsurancePayment(
    [property: Index(1)][Index(1)] EmployeeName Name,
    [property: Index(2)] DateTime Date,
    [property: Index(3)] string PayrollItem,
    [property: Index(4)] double? Amount,
    [property: Index(9)][Index(9)] string PayrollItem2,
    [property: Index(10)][Index(10)] double? Amount2);
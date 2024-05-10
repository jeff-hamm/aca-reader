using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;
using Itenso.TimePeriod;

namespace AcaReader;
public readonly record struct InsurancePlan(
    EmployeeName SubscriberName,
    string MemberID,
    string MemberLastName,
    string MemberFirstName,
    string MemberMiddleInitial,
    string MemberType,
    DateTime MemberDateOfBirth,
    string MemberGender,
    string AddressLine1,
    string AddressLine2,
    string City,
    string State,
    string ZipCode,
    string Coverage,
    string Policy,
    string PlanName,
    DateTime EffectiveDate,
    DateTime TerminationDate
    )
{
    [Ignore]
    public TimeRange ActiveRange => new(EffectiveDate, TerminationDate);

//    Subscriber Name		Member  ID		Member Last Name		Member First Name		Member Middle Initial		Member Type	Member Date of Birth	Member Gender		Address Line1	Address Line2	City		State		Zip Code	Coverage	Policy 	Plan Name	Effective Date	Termination Date

}

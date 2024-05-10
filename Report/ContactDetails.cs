namespace AcaReader.Coverage;

public record ContactDetails(string Ssn, string Street1, string City, string State, string Zip, DateTime? Dob){
    public ContactDetails(ContactInfo info) : this(info.Ssn, info.Street1, info.City, info.State, info.Zip, info.Dob)
    {
    }
}
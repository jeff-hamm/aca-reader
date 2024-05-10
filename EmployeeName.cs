using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Text.RegularExpressions;

namespace AcaReader;

public record EmployeeName(string LastName, string FirstName, string MiddleInitial) : IComparable<EmployeeName>
{

    public virtual bool Equals(EmployeeName? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return LastName.Equals(other.LastName, StringComparison.InvariantCultureIgnoreCase) && 
               FirstName.Equals(other.FirstName, StringComparison.InvariantCultureIgnoreCase);
    }

    public override int GetHashCode() => HashCode.Combine(LastName.ToLower(), FirstName.ToLower());

    public override string ToString()
    {
        var sb = new StringBuilder(LastName);
        if (!String.IsNullOrEmpty(FirstName))
        {
            sb.Append(", ").Append(FirstName);
            if (!String.IsNullOrEmpty(MiddleInitial))
            {
                sb.Append(' ').Append(MiddleInitial);
            }
        }
        return $"{LastName}, {FirstName} {MiddleInitial}";
    }
    public class TypeConverter : TypeConverter<EmployeeName>
    {
        private static readonly Regex NameRegex = new Regex(@"(Total\s*)?(?<LastName>[^,]+)\s*,(\s(?<FirstName>[\w']{2,}))+(\s(?<MiddleInitial>[a-zA-Z])\.?)?$", RegexOptions.Compiled);
        public override EmployeeName ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (String.IsNullOrEmpty(text))
                return EmployeeName.Empty;
            if (NameRegex.Match(text) is { Success: true } match)
                return new EmployeeName(match.Groups["LastName"].Value.Trim(), String.Join(' ', match.Groups["FirstName"].Captures).Trim(),
                    match.Groups["MiddleInitial"]?.Value ?? "");
            return new EmployeeName(text, "", "");
        }

        public override string ConvertToString(EmployeeName value, IWriterRow row, MemberMapData memberMapData) =>
            value.ToString();
    }

    public static EmployeeName Empty { get; } = new EmployeeName("", "", "");

    public int CompareTo(EmployeeName? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var lastNameComparison = string.Compare(LastName, other.LastName, StringComparison.OrdinalIgnoreCase);
        if (lastNameComparison != 0) return lastNameComparison;
        return string.Compare(FirstName, other.FirstName, StringComparison.OrdinalIgnoreCase);
    }
}
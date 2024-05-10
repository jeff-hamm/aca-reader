using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace AcaReader;

internal class NullableDateTimeConverter : TypeConverter<DateTime?>
{
    private readonly DateTimeConverter converter = new();
    public override DateTime? ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        var formatProvider = (IFormatProvider)memberMapData.TypeConverterOptions.CultureInfo.GetFormat(typeof(DateTimeFormatInfo)) ?? memberMapData.TypeConverterOptions.CultureInfo;
        if (String.IsNullOrEmpty(text) || !DateTime.TryParse(text, formatProvider, out var result))
            return null;
        return result;
    }

    public override string ConvertToString(DateTime? value, IWriterRow row, MemberMapData memberMapData)
    {
        return !value.HasValue ? "" : converter.ConvertToString(value.Value, row, memberMapData);
    }
}
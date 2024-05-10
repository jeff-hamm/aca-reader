using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace AcaReader;

internal class Reader
{
    public static CsvConfiguration ReaderConfig => new CsvConfiguration(CultureInfo.CurrentCulture)
    {
        PrepareHeaderForMatch = PrepareHeader,
        HeaderValidated = null,
        MissingFieldFound = null,
        TrimOptions = TrimOptions.InsideQuotes | TrimOptions.Trim,
        ShouldSkipRecord = SkipEmpty
        
    };

    private static bool SkipEmpty(ShouldSkipRecordArgs args)
    {
        for (var i = 0; i < args.Row.ColumnCount; i++)
        {
            if (!String.IsNullOrEmpty(args.Row[i]))
                return false;
        }

        return true;
    }

    private static readonly Regex WhiteSpace = new Regex(@"\s", RegexOptions.Compiled);

    private static string PrepareHeader(PrepareHeaderForMatchArgs args) =>
        WhiteSpace.Replace(args.Header, "");

    public EmployeeHours[] ReadHours(string fileName)
    {
        using var reader = GetReader(fileName);
        return reader.GetRecords<EmployeeHours>().Where(e => !String.IsNullOrEmpty(e.Name.LastName)).ToArray();
    }
    public InsurancePayment[] ReadInsurancePayments(string fileName)
    {
        using var reader = GetReader(fileName);
        return reader.GetRecords<InsurancePayment>().Where(e => !String.IsNullOrEmpty(e.Name?.LastName)).ToArray();
    }

    public InsurancePlan[] ReadRoster(string fileName)
    {
        using var reader = GetReader(fileName);
        return reader.GetRecords<InsurancePlan>().Where(e => !String.IsNullOrEmpty(e.SubscriberName.LastName)).ToArray();
    }
    public EmployeeWage[] ReadWages(string fileName)
    {
        using var reader = GetReader(fileName);
        return reader.GetRecords<EmployeeWage>().Where(e => !String.IsNullOrEmpty(e.Name.LastName)).ToArray();
    }


    public ContactInfo[] ReadContactInfo(string fileName)
    {
        using var reader = GetReader(fileName);
        return reader.GetRecords<ContactInfo>().Where(e => 
            !String.IsNullOrEmpty(e.Ssn)).ToArray();
    }


    public IEnumerable<TRecord> ReadFile<TRecord>(string fileName)
    {
        using var reader = GetReader(fileName);
        return reader.GetRecords<TRecord>().ToArray();
    }
    /*
     * Hey, I just wanted to be more direct about the conversation that we had about briana. I feel like, actually you came back and
     * acknowledged some really good things. But, I think that I was just not in my clearest form when we talked about her
     * and my relationship the first time. And I just want to say more clearly what I said the second time:
     *
     * I really do love her deeply. I miss her often and it is still very hard. I don't say, or feel these things as things that are threatening
     * to you. I am so often unseen and Vilinized with her, we don't communicate well, too much of my life is full of misunderstandings,
     * conflict, adventures with her are often so contentious ind difficult that I would rather not do them at all.
     *
     * I know these things, I know that I am so unhappy in that kind of relationship with her
     *
     * I feel like, more and more there are moments
     * She and I are not dating,
     */

    private CsvReader GetReader(string fileName)
    {
        var reader = new CsvReader(new StreamReader(fileName), ReaderConfig);
        reader.Context.RegisterClassMap<EmployeeHours.ClassMap>();
        reader.Context.TypeConverterCache.AddConverter(new EmployeeName.TypeConverter());
        reader.Context.TypeConverterCache.AddConverter(new NullableDateTimeConverter());
        return reader;
    }

}

public readonly record struct EmployeeWage(EmployeeName Name, double Wage);
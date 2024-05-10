using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;
using System.Text.RegularExpressions;
using AcaReader.Coverage;
using CsvHelper;
using Itenso.TimePeriod;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Source;
using iText.Kernel.Pdf;

namespace AcaReader;

public class FormFiller(string inputFile, CsvWriter csvWriter)
{
    private readonly byte[] fileData = File.ReadAllBytes(inputFile);
    public void FillForm(EmployeeDetails employee)
    {
        var fileSource = new RandomAccessSourceFactory().CreateSource(fileData);
        using var reader = new PdfReader(fileSource, new ReaderProperties());
        using var writer = new PdfWriter($"{employee.Name.LastName}_{employee.Name.FirstName}_{employee.ContactInfo?.Ssn[^4..]}-1095c.pdf");
            
        using var doc = new PdfDocument(reader, writer 
//            new StampingProperties().UseAppendMode()
        );
        var acroForm = PdfAcroForm.GetAcroForm(doc, false);
        var xfa = acroForm.GetXfaForm();
        SetFormFields(employee, acroForm);
        // xfa.Write(doc);
//        fields["topmostSubform[0].Page1[0].EmployeeName[0].f1_1[0]"].SetValue(employee.Name.FirstName);
//        acroForm.FlattenFields();
//        using var writer = new PdfWriter($);
//        writer.WriteBytes(memoryOutputStream.ToArray());
        doc.Close();
//        xfa.fillXfaForm(new FileInputStream(xml));
//        xfa.write(pdfDoc);
//        pdfDoc.close();
        csvWriter.NextRecord();
    }

    private void SetFormFields(EmployeeDetails employee, PdfAcroForm acroForm)
    {
        var page = acroForm.GetPage(1);
        SetEmployeeFields(employee, page.FieldGroup("EmployeeName"));
        SetEmployerFields(page.FieldGroup("EmployerIssuer"));
        SetPlanDates(employee, page.FieldGroup("PartII"));
        SetMonthlyDetails(employee, page.FieldGroup("Table1"));
    }

    private int box14Start = 17;
    private int box15Start = 30;
    private int box16Start = 43;
    private int box17Start = 56;

    private void SetMonthlyDetails(EmployeeDetails employee, PdfFormField fieldGroup)
    {
        var box14 = fieldGroup.GetChildField("Row1[0]");
        var box15 = fieldGroup.GetChildField("Row2[0]");
        var box16 = fieldGroup.GetChildField("Row3[0]");
        var box17 = fieldGroup.GetChildField("Row4[0]");
        var anyProblems = false;
        //var calc = new MonthlyCalculation(employee);
        if (employee.MonthDetails.Values.GroupBy(m => (m.IdealOfferCode, m.HarborCode)).Count() == 1)
        {
            WriteColumn(employee, box14, 0, employee.MonthDetails.First().Value, box15, box16, box17);
            foreach (var period in employee.MonthDetails)
                anyProblems = WriteCsv(period, anyProblems);
        }
        else
        {
            foreach (var period in employee.MonthDetails)
            {   

                //            foreach (var offer in calc.ShouldBeOfferedInsurance(period))
                //            {
                //if (shouldOffer.Length == 12)
                //{
                //    box14.SetValue(box14Start, "A1");
                //    box15.SetValue(box15Start, year.TotalPayments.ToString("C"));
                //    box17.SetValue(box17Start, employee.ContactInfo?.Zip ?? "");
                //}
                //else
                //{
                //foreach (var month in Enum.GetValues<Months>())
                //{
                //    var total = year.MonthPayment(month);
                //    if (total > 0)
                //    {
                var monthNum = (int)period.Key;
                var result = period.Value;
                anyProblems = WriteCsv(period, anyProblems);
                WriteColumn(employee, box14, monthNum, result, box15, box16, box17);
                //                    }
//                }
//            }
//            }
            }
        }

        if (anyProblems)
        {
            csvWriter.WriteField("WARNING! Insufficient Coverage");
        }

    }

    private void WriteColumn(EmployeeDetails employee, PdfFormField box14, int monthNum, EmployeeMonth result,
        PdfFormField box15, PdfFormField box16, PdfFormField box17)
    {
        box14.SetValue(box14Start + monthNum, result.IdealOfferCode.GetDisplayName());
        if (result.OfferValue > 0)
            box15.SetValue(box15Start + monthNum, result.OfferValue.ToString("F2"));
        if (result.HarborCode is { } code)
            box16.SetValue(box16Start + monthNum, code.GetDisplayName());
        box17.SetValue(box17Start + monthNum, "USA " + employee.ContactInfo?.Zip ?? "");
    }

    private bool WriteCsv(KeyValuePair<YearMonth, EmployeeMonth> period, bool anyProblems)
    {
        var hasProblem = period.Value.IdealOfferCode == OfferCode.CoverageOffered &&
                         !period.Value.Insurance.Any() && !period.Value.IsAffordable;
        anyProblems |= hasProblem;
        string message;
        if (period.Value.IdealOfferCode == OfferCode.CoverageOffered &&
            period.Value.HarborCode == SafeHarborCode.EmployeeEnrolled)
        {
            message = "Enrolled";
            if (period.Value.Type != EmployeeType.FullTime)
                message += $"[{period.Value.Type}]";
        }
        else if (period.Value.IdealOfferCode == OfferCode.CoverageOffered &&
                 period.Value.IsAffordable)
            message = "Affordable";
        else if (period.Value.Type == EmployeeType.Terminated)
            message = "Terminated";
        else if(period.Value.Type == EmployeeType.Evaluating)
            message = "Waiting";
        else if(period.Value.Type is EmployeeType.NotYetHired or EmployeeType.NotEmployed)
            message = "";
        else if(period.Value.Type == EmployeeType.PartTime)
            message = "Part Time";
        else 
            message = "!!!";
        csvWriter.WriteField(message);
        return anyProblems;
    }

    private void SetPlanDates(EmployeeDetails employee, PdfFormField fieldGroup)
    {
        //        fieldGroup.SetValue(17, employee.AgeOnJan1st?.ToString() ?? "");
        csvWriter.WriteField(employee.PlanStartMonth.ToString("00"));
        fieldGroup.SetValue(16, employee.PlanStartMonth.ToString("00"));
    }

    private void SetEmployerFields(PdfFormField employerFields)
    {
        employerFields.SetValue(9, EmployerConfig.Name);
        employerFields.SetValue(10, EmployerConfig.Ein);
        employerFields.SetValue(11, EmployerConfig.Street1);
        employerFields.SetValue(12, EmployerConfig.ContactTel);
        employerFields.SetValue(13, EmployerConfig.City);
        employerFields.SetValue(14, EmployerConfig.State);
        employerFields.SetValue(15, EmployerConfig.PostalCode);
    }

    private static Regex SsnRegex = new Regex(@"^\d{3}-?\d{2}-?\d{4}$", RegexOptions.Compiled);
    private static Regex ZipRegex = new Regex(@"^\d{5}(?:[-\s]\d{4})?$", RegexOptions.Compiled);
    private void SetEmployeeFields(EmployeeDetails employee, PdfFormField employeeFields)
    {
        csvWriter.WriteField(employee.Name);
        employeeFields.SetValue(1, employee.Name.FirstName);
        employeeFields.SetValue(2, employee.Name.MiddleInitial);
        employeeFields.SetValue(3, employee.Name.LastName);
        if (employee.ContactInfo is {} contact)
        {
            if (SsnRegex.IsMatch(contact.Ssn))
                employeeFields.SetValue(4, contact.Ssn.Substring(contact.Ssn.Length-4));
            csvWriter.WriteField(contact.Ssn.Substring(contact.Ssn.Length - 4));
            employeeFields.SetValue(5, contact.Street1);
            csvWriter.WriteField(contact.Street1);
            employeeFields.SetValue(6, contact.City);
            csvWriter.WriteField(contact.City);
            employeeFields.SetValue(7, contact.State);
            csvWriter.WriteField(contact.State);
            if (ZipRegex.IsMatch(contact.Zip)) 
                employeeFields.SetValue(8, contact.Zip);
            csvWriter.WriteField(contact.Zip);
        }
    }
}
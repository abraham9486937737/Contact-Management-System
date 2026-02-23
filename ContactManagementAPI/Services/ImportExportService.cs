using ContactManagementAPI.Models;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Text;

namespace ContactManagementAPI.Services
{
    public class ImportExportService
    {
        public ImportExportService()
        {
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            // Set QuestPDF license
            QuestPDF.Settings.License = LicenseType.Community;
        }

        #region Import Methods

        public async Task<(List<Contact> contacts, List<string> errors)> ImportFromExcel(Stream stream)
        {
            var contacts = new List<Contact>();
            var errors = new List<string>();

            try
            {
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                
                if (worksheet == null)
                {
                    errors.Add("No worksheet found in the Excel file.");
                    return (contacts, errors);
                }

                var rowCount = worksheet.Dimension?.Rows ?? 0;
                
                // Start from row 2 (skip header)
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var contact = new Contact
                        {
                            FirstName = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                            LastName = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                            NickName = worksheet.Cells[row, 3].Value?.ToString(),
                            Gender = worksheet.Cells[row, 4].Value?.ToString(),
                            DateOfBirth = DateTime.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out var dob) ? dob : null,
                            Email = worksheet.Cells[row, 6].Value?.ToString(),
                            Mobile1 = worksheet.Cells[row, 7].Value?.ToString(),
                            Mobile2 = worksheet.Cells[row, 8].Value?.ToString(),
                            Mobile3 = worksheet.Cells[row, 9].Value?.ToString(),
                            WhatsAppNumber = worksheet.Cells[row, 10].Value?.ToString(),
                            PassportNumber = worksheet.Cells[row, 11].Value?.ToString(),
                            PanNumber = worksheet.Cells[row, 12].Value?.ToString(),
                            AadharNumber = worksheet.Cells[row, 13].Value?.ToString(),
                            DrivingLicenseNumber = worksheet.Cells[row, 14].Value?.ToString(),
                            VotersId = worksheet.Cells[row, 15].Value?.ToString(),
                            BankAccountNumber = worksheet.Cells[row, 16].Value?.ToString(),
                            BankName = worksheet.Cells[row, 17].Value?.ToString(),
                            BranchName = worksheet.Cells[row, 18].Value?.ToString(),
                            IfscCode = worksheet.Cells[row, 19].Value?.ToString(),
                            Address = worksheet.Cells[row, 20].Value?.ToString(),
                            City = worksheet.Cells[row, 21].Value?.ToString(),
                            State = worksheet.Cells[row, 22].Value?.ToString(),
                            PostalCode = worksheet.Cells[row, 23].Value?.ToString(),
                            Country = worksheet.Cells[row, 24].Value?.ToString(),
                            OtherDetails = worksheet.Cells[row, 25].Value?.ToString(),
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        if (!string.IsNullOrWhiteSpace(contact.FirstName) || !string.IsNullOrWhiteSpace(contact.LastName))
                        {
                            contacts.Add(contact);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {row}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error reading Excel file: {ex.Message}");
            }

            return (contacts, errors);
        }

        public async Task<(List<Contact> contacts, List<string> errors)> ImportFromCsv(Stream stream)
        {
            var contacts = new List<Contact>();
            var errors = new List<string>();

            try
            {
                using var reader = new StreamReader(stream);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    BadDataFound = null
                };

                using var csv = new CsvReader(reader, config);
                
                csv.Read();
                csv.ReadHeader();
                int rowNumber = 1;

                while (csv.Read())
                {
                    rowNumber++;
                    try
                    {
                        string? ReadField(string name, int index)
                        {
                            if (csv.TryGetField<string>(name, out var namedValue))
                                return namedValue;

                            if (csv.TryGetField<string>(index, out var indexedValue))
                                return indexedValue;

                            return null;
                        }

                        var contact = new Contact
                        {
                            FirstName = ReadField("FirstName", 0) ?? "",
                            LastName = ReadField("LastName", 1) ?? "",
                            NickName = ReadField("NickName", 2),
                            Gender = ReadField("Gender", 3),
                            DateOfBirth = DateTime.TryParse(ReadField("DateOfBirth", 4), out var dob) ? dob : null,
                            Email = ReadField("Email", 5),
                            Mobile1 = ReadField("Mobile1", 6),
                            Mobile2 = ReadField("Mobile2", 7),
                            Mobile3 = ReadField("Mobile3", 8),
                            WhatsAppNumber = ReadField("WhatsAppNumber", 9),
                            PassportNumber = ReadField("PassportNumber", 10),
                            PanNumber = ReadField("PanNumber", 11),
                            AadharNumber = ReadField("AadharNumber", 12),
                            DrivingLicenseNumber = ReadField("DrivingLicenseNumber", 13),
                            VotersId = ReadField("VotersId", 14),
                            BankAccountNumber = ReadField("BankAccountNumber", 15),
                            BankName = ReadField("BankName", 16),
                            BranchName = ReadField("BranchName", 17),
                            IfscCode = ReadField("IfscCode", 18),
                            Address = ReadField("Address", 19),
                            City = ReadField("City", 20),
                            State = ReadField("State", 21),
                            PostalCode = ReadField("PostalCode", 22),
                            Country = ReadField("Country", 23),
                            OtherDetails = ReadField("OtherDetails", 24),
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        if (!string.IsNullOrWhiteSpace(contact.FirstName) || !string.IsNullOrWhiteSpace(contact.LastName))
                        {
                            contacts.Add(contact);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {rowNumber}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error reading CSV file: {ex.Message}");
            }

            return (contacts, errors);
        }

        #endregion

        #region Export Methods

        public async Task<byte[]> ExportToExcel(List<Contact> contacts)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Contacts");

            // Add headers
            worksheet.Cells[1, 1].Value = "First Name";
            worksheet.Cells[1, 2].Value = "Last Name";
            worksheet.Cells[1, 3].Value = "Nick Name";
            worksheet.Cells[1, 4].Value = "Gender";
            worksheet.Cells[1, 5].Value = "Date Of Birth";
            worksheet.Cells[1, 6].Value = "Email";
            worksheet.Cells[1, 7].Value = "Mobile 1";
            worksheet.Cells[1, 8].Value = "Mobile 2";
            worksheet.Cells[1, 9].Value = "Mobile 3";
            worksheet.Cells[1, 10].Value = "WhatsApp";
            worksheet.Cells[1, 11].Value = "Passport Number";
            worksheet.Cells[1, 12].Value = "PAN Number";
            worksheet.Cells[1, 13].Value = "Aadhar Number";
            worksheet.Cells[1, 14].Value = "Driving License Number";
            worksheet.Cells[1, 15].Value = "Voters ID";
            worksheet.Cells[1, 16].Value = "Bank Account Number";
            worksheet.Cells[1, 17].Value = "Bank Name";
            worksheet.Cells[1, 18].Value = "Branch Name";
            worksheet.Cells[1, 19].Value = "IFSC Code";
            worksheet.Cells[1, 20].Value = "Address";
            worksheet.Cells[1, 21].Value = "City";
            worksheet.Cells[1, 22].Value = "State";
            worksheet.Cells[1, 23].Value = "Postal Code";
            worksheet.Cells[1, 24].Value = "Country";
            worksheet.Cells[1, 25].Value = "Other Details";
            worksheet.Cells[1, 26].Value = "Group";
            worksheet.Cells[1, 27].Value = "Created At";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 27])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            // Add data
            for (int i = 0; i < contacts.Count; i++)
            {
                var contact = contacts[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = contact.FirstName;
                worksheet.Cells[row, 2].Value = contact.LastName;
                worksheet.Cells[row, 3].Value = contact.NickName;
                worksheet.Cells[row, 4].Value = contact.Gender;
                worksheet.Cells[row, 5].Value = contact.DateOfBirth?.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 6].Value = contact.Email;
                worksheet.Cells[row, 7].Value = contact.Mobile1;
                worksheet.Cells[row, 8].Value = contact.Mobile2;
                worksheet.Cells[row, 9].Value = contact.Mobile3;
                worksheet.Cells[row, 10].Value = contact.WhatsAppNumber;
                worksheet.Cells[row, 11].Value = contact.PassportNumber;
                worksheet.Cells[row, 12].Value = contact.PanNumber;
                worksheet.Cells[row, 13].Value = contact.AadharNumber;
                worksheet.Cells[row, 14].Value = contact.DrivingLicenseNumber;
                worksheet.Cells[row, 15].Value = contact.VotersId;
                worksheet.Cells[row, 16].Value = contact.BankAccountNumber;
                worksheet.Cells[row, 17].Value = contact.BankName;
                worksheet.Cells[row, 18].Value = contact.BranchName;
                worksheet.Cells[row, 19].Value = contact.IfscCode;
                worksheet.Cells[row, 20].Value = contact.Address;
                worksheet.Cells[row, 21].Value = contact.City;
                worksheet.Cells[row, 22].Value = contact.State;
                worksheet.Cells[row, 23].Value = contact.PostalCode;
                worksheet.Cells[row, 24].Value = contact.Country;
                worksheet.Cells[row, 25].Value = contact.OtherDetails;
                worksheet.Cells[row, 26].Value = contact.Group?.Name;
                worksheet.Cells[row, 27].Value = contact.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return await Task.FromResult(package.GetAsByteArray());
        }

        public async Task<byte[]> ExportToCsv(List<Contact> contacts)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Write headers
            csv.WriteField("FirstName");
            csv.WriteField("LastName");
            csv.WriteField("NickName");
            csv.WriteField("Gender");
            csv.WriteField("DateOfBirth");
            csv.WriteField("Email");
            csv.WriteField("Mobile1");
            csv.WriteField("Mobile2");
            csv.WriteField("Mobile3");
            csv.WriteField("WhatsAppNumber");
            csv.WriteField("PassportNumber");
            csv.WriteField("PanNumber");
            csv.WriteField("AadharNumber");
            csv.WriteField("DrivingLicenseNumber");
            csv.WriteField("VotersId");
            csv.WriteField("BankAccountNumber");
            csv.WriteField("BankName");
            csv.WriteField("BranchName");
            csv.WriteField("IfscCode");
            csv.WriteField("Address");
            csv.WriteField("City");
            csv.WriteField("State");
            csv.WriteField("PostalCode");
            csv.WriteField("Country");
            csv.WriteField("OtherDetails");
            csv.WriteField("Group");
            csv.WriteField("CreatedAt");
            csv.NextRecord();

            // Write data
            foreach (var contact in contacts)
            {
                csv.WriteField(contact.FirstName);
                csv.WriteField(contact.LastName);
                csv.WriteField(contact.NickName);
                csv.WriteField(contact.Gender);
                csv.WriteField(contact.DateOfBirth?.ToString("yyyy-MM-dd"));
                csv.WriteField(contact.Email);
                csv.WriteField(contact.Mobile1);
                csv.WriteField(contact.Mobile2);
                csv.WriteField(contact.Mobile3);
                csv.WriteField(contact.WhatsAppNumber);
                csv.WriteField(contact.PassportNumber);
                csv.WriteField(contact.PanNumber);
                csv.WriteField(contact.AadharNumber);
                csv.WriteField(contact.DrivingLicenseNumber);
                csv.WriteField(contact.VotersId);
                csv.WriteField(contact.BankAccountNumber);
                csv.WriteField(contact.BankName);
                csv.WriteField(contact.BranchName);
                csv.WriteField(contact.IfscCode);
                csv.WriteField(contact.Address);
                csv.WriteField(contact.City);
                csv.WriteField(contact.State);
                csv.WriteField(contact.PostalCode);
                csv.WriteField(contact.Country);
                csv.WriteField(contact.OtherDetails);
                csv.WriteField(contact.Group?.Name);
                csv.WriteField(contact.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                csv.NextRecord();
            }

            await writer.FlushAsync();
            return memoryStream.ToArray();
        }

        public async Task<byte[]> ExportToPdf(List<Contact> contacts)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header()
                        .Text("Contact List")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            // Define columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Name
                                columns.RelativeColumn(2); // Email
                                columns.RelativeColumn(1.5f); // Mobile1
                                columns.RelativeColumn(1.5f); // Mobile2
                                columns.RelativeColumn(2); // City
                                columns.RelativeColumn(1.5f); // State
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Name").Bold();
                                header.Cell().Element(CellStyle).Text("Email").Bold();
                                header.Cell().Element(CellStyle).Text("Mobile 1").Bold();
                                header.Cell().Element(CellStyle).Text("Mobile 2").Bold();
                                header.Cell().Element(CellStyle).Text("City").Bold();
                                header.Cell().Element(CellStyle).Text("State").Bold();

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold())
                                        .PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            // Data rows
                            foreach (var contact in contacts)
                            {
                                table.Cell().Element(CellStyle).Text($"{contact.FirstName} {contact.LastName}");
                                table.Cell().Element(CellStyle).Text(contact.Email ?? "-");
                                table.Cell().Element(CellStyle).Text(contact.Mobile1 ?? "-");
                                table.Cell().Element(CellStyle).Text(contact.Mobile2 ?? "-");
                                table.Cell().Element(CellStyle).Text(contact.City ?? "-");
                                table.Cell().Element(CellStyle).Text(contact.State ?? "-");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .PaddingVertical(5);
                                }
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                            x.Span(" | Generated on ");
                            x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        });
                });
            });

            return await Task.FromResult(document.GeneratePdf());
        }

        #endregion

        #region Template Methods

        public async Task<byte[]> GenerateExcelTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Contacts Template");

            // Add headers
            worksheet.Cells[1, 1].Value = "FirstName";
            worksheet.Cells[1, 2].Value = "LastName";
            worksheet.Cells[1, 3].Value = "NickName";
            worksheet.Cells[1, 4].Value = "Gender";
            worksheet.Cells[1, 5].Value = "DateOfBirth";
            worksheet.Cells[1, 6].Value = "Email";
            worksheet.Cells[1, 7].Value = "Mobile1";
            worksheet.Cells[1, 8].Value = "Mobile2";
            worksheet.Cells[1, 9].Value = "Mobile3";
            worksheet.Cells[1, 10].Value = "WhatsAppNumber";
            worksheet.Cells[1, 11].Value = "PassportNumber";
            worksheet.Cells[1, 12].Value = "PanNumber";
            worksheet.Cells[1, 13].Value = "AadharNumber";
            worksheet.Cells[1, 14].Value = "DrivingLicenseNumber";
            worksheet.Cells[1, 15].Value = "VotersId";
            worksheet.Cells[1, 16].Value = "BankAccountNumber";
            worksheet.Cells[1, 17].Value = "BankName";
            worksheet.Cells[1, 18].Value = "BranchName";
            worksheet.Cells[1, 19].Value = "IfscCode";
            worksheet.Cells[1, 20].Value = "Address";
            worksheet.Cells[1, 21].Value = "City";
            worksheet.Cells[1, 22].Value = "State";
            worksheet.Cells[1, 23].Value = "PostalCode";
            worksheet.Cells[1, 24].Value = "Country";
            worksheet.Cells[1, 25].Value = "OtherDetails";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 25])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
            }

            // Add sample row
            worksheet.Cells[2, 1].Value = "John";
            worksheet.Cells[2, 2].Value = "Doe";
            worksheet.Cells[2, 3].Value = "Johnny";
            worksheet.Cells[2, 4].Value = "Male";
            worksheet.Cells[2, 5].Value = "1990-01-01";
            worksheet.Cells[2, 6].Value = "john.doe@example.com";
            worksheet.Cells[2, 7].Value = "+1234567890";
            worksheet.Cells[2, 8].Value = "+0987654321";
            worksheet.Cells[2, 9].Value = "";
            worksheet.Cells[2, 10].Value = "+1234567890";
            worksheet.Cells[2, 11].Value = "P1234567";
            worksheet.Cells[2, 12].Value = "ABCDE1234F";
            worksheet.Cells[2, 13].Value = "1234-5678-9012";
            worksheet.Cells[2, 14].Value = "DL-12345-2020";
            worksheet.Cells[2, 15].Value = "VOTER12345";
            worksheet.Cells[2, 16].Value = "123456789012";
            worksheet.Cells[2, 17].Value = "State Bank";
            worksheet.Cells[2, 18].Value = "Main Branch";
            worksheet.Cells[2, 19].Value = "SBIN0001234";
            worksheet.Cells[2, 20].Value = "123 Main St";
            worksheet.Cells[2, 21].Value = "New York";
            worksheet.Cells[2, 22].Value = "NY";
            worksheet.Cells[2, 23].Value = "10001";
            worksheet.Cells[2, 24].Value = "USA";
            worksheet.Cells[2, 25].Value = "Sample contact";

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return await Task.FromResult(package.GetAsByteArray());
        }

        public async Task<byte[]> GenerateCsvTemplate()
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Write headers
            csv.WriteField("FirstName");
            csv.WriteField("LastName");
            csv.WriteField("NickName");
            csv.WriteField("Gender");
            csv.WriteField("DateOfBirth");
            csv.WriteField("Email");
            csv.WriteField("Mobile1");
            csv.WriteField("Mobile2");
            csv.WriteField("Mobile3");
            csv.WriteField("WhatsAppNumber");
            csv.WriteField("PassportNumber");
            csv.WriteField("PanNumber");
            csv.WriteField("AadharNumber");
            csv.WriteField("DrivingLicenseNumber");
            csv.WriteField("VotersId");
            csv.WriteField("BankAccountNumber");
            csv.WriteField("BankName");
            csv.WriteField("BranchName");
            csv.WriteField("IfscCode");
            csv.WriteField("Address");
            csv.WriteField("City");
            csv.WriteField("State");
            csv.WriteField("PostalCode");
            csv.WriteField("Country");
            csv.WriteField("OtherDetails");
            csv.NextRecord();

            // Write sample row
            csv.WriteField("John");
            csv.WriteField("Doe");
            csv.WriteField("Johnny");
            csv.WriteField("Male");
            csv.WriteField("1990-01-01");
            csv.WriteField("john.doe@example.com");
            csv.WriteField("+1234567890");
            csv.WriteField("+0987654321");
            csv.WriteField("");
            csv.WriteField("+1234567890");
            csv.WriteField("P1234567");
            csv.WriteField("ABCDE1234F");
            csv.WriteField("1234-5678-9012");
            csv.WriteField("DL-12345-2020");
            csv.WriteField("VOTER12345");
            csv.WriteField("123456789012");
            csv.WriteField("State Bank");
            csv.WriteField("Main Branch");
            csv.WriteField("SBIN0001234");
            csv.WriteField("123 Main St");
            csv.WriteField("New York");
            csv.WriteField("NY");
            csv.WriteField("10001");
            csv.WriteField("USA");
            csv.WriteField("Sample contact");
            csv.NextRecord();

            await writer.FlushAsync();
            return memoryStream.ToArray();
        }

        #endregion
    }
}

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
                            Email = worksheet.Cells[row, 4].Value?.ToString(),
                            Mobile1 = worksheet.Cells[row, 5].Value?.ToString(),
                            Mobile2 = worksheet.Cells[row, 6].Value?.ToString(),
                            Mobile3 = worksheet.Cells[row, 7].Value?.ToString(),
                            WhatsAppNumber = worksheet.Cells[row, 8].Value?.ToString(),
                            Address = worksheet.Cells[row, 9].Value?.ToString(),
                            City = worksheet.Cells[row, 10].Value?.ToString(),
                            State = worksheet.Cells[row, 11].Value?.ToString(),
                            PostalCode = worksheet.Cells[row, 12].Value?.ToString(),
                            Country = worksheet.Cells[row, 13].Value?.ToString(),
                            OtherDetails = worksheet.Cells[row, 14].Value?.ToString(),
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
                        var contact = new Contact
                        {
                            FirstName = csv.GetField<string>("FirstName") ?? csv.GetField<string>(0) ?? "",
                            LastName = csv.GetField<string>("LastName") ?? csv.GetField<string>(1) ?? "",
                            NickName = csv.GetField<string>("NickName") ?? csv.GetField<string>(2),
                            Email = csv.GetField<string>("Email") ?? csv.GetField<string>(3),
                            Mobile1 = csv.GetField<string>("Mobile1") ?? csv.GetField<string>(4),
                            Mobile2 = csv.GetField<string>("Mobile2") ?? csv.GetField<string>(5),
                            Mobile3 = csv.GetField<string>("Mobile3") ?? csv.GetField<string>(6),
                            WhatsAppNumber = csv.GetField<string>("WhatsAppNumber") ?? csv.GetField<string>(7),
                            Address = csv.GetField<string>("Address") ?? csv.GetField<string>(8),
                            City = csv.GetField<string>("City") ?? csv.GetField<string>(9),
                            State = csv.GetField<string>("State") ?? csv.GetField<string>(10),
                            PostalCode = csv.GetField<string>("PostalCode") ?? csv.GetField<string>(11),
                            Country = csv.GetField<string>("Country") ?? csv.GetField<string>(12),
                            OtherDetails = csv.GetField<string>("OtherDetails") ?? csv.GetField<string>(13),
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
            worksheet.Cells[1, 4].Value = "Email";
            worksheet.Cells[1, 5].Value = "Mobile 1";
            worksheet.Cells[1, 6].Value = "Mobile 2";
            worksheet.Cells[1, 7].Value = "Mobile 3";
            worksheet.Cells[1, 8].Value = "WhatsApp";
            worksheet.Cells[1, 9].Value = "Address";
            worksheet.Cells[1, 10].Value = "City";
            worksheet.Cells[1, 11].Value = "State";
            worksheet.Cells[1, 12].Value = "Postal Code";
            worksheet.Cells[1, 13].Value = "Country";
            worksheet.Cells[1, 14].Value = "Other Details";
            worksheet.Cells[1, 15].Value = "Group";
            worksheet.Cells[1, 16].Value = "Created At";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 16])
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
                worksheet.Cells[row, 4].Value = contact.Email;
                worksheet.Cells[row, 5].Value = contact.Mobile1;
                worksheet.Cells[row, 6].Value = contact.Mobile2;
                worksheet.Cells[row, 7].Value = contact.Mobile3;
                worksheet.Cells[row, 8].Value = contact.WhatsAppNumber;
                worksheet.Cells[row, 9].Value = contact.Address;
                worksheet.Cells[row, 10].Value = contact.City;
                worksheet.Cells[row, 11].Value = contact.State;
                worksheet.Cells[row, 12].Value = contact.PostalCode;
                worksheet.Cells[row, 13].Value = contact.Country;
                worksheet.Cells[row, 14].Value = contact.OtherDetails;
                worksheet.Cells[row, 15].Value = contact.Group?.Name;
                worksheet.Cells[row, 16].Value = contact.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
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
            csv.WriteField("Email");
            csv.WriteField("Mobile1");
            csv.WriteField("Mobile2");
            csv.WriteField("Mobile3");
            csv.WriteField("WhatsAppNumber");
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
                csv.WriteField(contact.Email);
                csv.WriteField(contact.Mobile1);
                csv.WriteField(contact.Mobile2);
                csv.WriteField(contact.Mobile3);
                csv.WriteField(contact.WhatsAppNumber);
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
            worksheet.Cells[1, 4].Value = "Email";
            worksheet.Cells[1, 5].Value = "Mobile1";
            worksheet.Cells[1, 6].Value = "Mobile2";
            worksheet.Cells[1, 7].Value = "Mobile3";
            worksheet.Cells[1, 8].Value = "WhatsAppNumber";
            worksheet.Cells[1, 9].Value = "Address";
            worksheet.Cells[1, 10].Value = "City";
            worksheet.Cells[1, 11].Value = "State";
            worksheet.Cells[1, 12].Value = "PostalCode";
            worksheet.Cells[1, 13].Value = "Country";
            worksheet.Cells[1, 14].Value = "OtherDetails";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 14])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
            }

            // Add sample row
            worksheet.Cells[2, 1].Value = "John";
            worksheet.Cells[2, 2].Value = "Doe";
            worksheet.Cells[2, 3].Value = "Johnny";
            worksheet.Cells[2, 4].Value = "john.doe@example.com";
            worksheet.Cells[2, 5].Value = "+1234567890";
            worksheet.Cells[2, 6].Value = "+0987654321";
            worksheet.Cells[2, 7].Value = "";
            worksheet.Cells[2, 8].Value = "+1234567890";
            worksheet.Cells[2, 9].Value = "123 Main St";
            worksheet.Cells[2, 10].Value = "New York";
            worksheet.Cells[2, 11].Value = "NY";
            worksheet.Cells[2, 12].Value = "10001";
            worksheet.Cells[2, 13].Value = "USA";
            worksheet.Cells[2, 14].Value = "Sample contact";

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
            csv.WriteField("Email");
            csv.WriteField("Mobile1");
            csv.WriteField("Mobile2");
            csv.WriteField("Mobile3");
            csv.WriteField("WhatsAppNumber");
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
            csv.WriteField("john.doe@example.com");
            csv.WriteField("+1234567890");
            csv.WriteField("+0987654321");
            csv.WriteField("");
            csv.WriteField("+1234567890");
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

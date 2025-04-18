using ImelAPI.Data;
using ImelAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using System.Text;
using iText.IO.Font.Constants;
using iText.Kernel.Font;


namespace ImelAPI.Services
{
    public class Methods
    {
        private readonly ApplicationDbContext _context;

        public Methods(ApplicationDbContext context)
        {
            _context = context;
            ExcelPackage.License.SetNonCommercialPersonal("Akin");
        }

        public async Task<IActionResult> Download(string fileType)
        {
            List<User> users = await _context.Users
                .Where(u => u.isDeleted == 0 && u.Status == "Active")
                .ToListAsync();

            if (fileType.ToLower() == "csv")
            {
                var csv = "Id,UserSpecificId,Name,Surname,Email,Role,Status,CreatedAt,VersionNum\n";
                foreach (var user in users)
                {
                    csv += $"{user.Id},{user.UserSpecificId},{user.Name},{user.Surname},{user.Email},{user.Role},{user.Status},{user.CreatedAt},{user.VersionNum}\n";
                }

                var bytes = Encoding.UTF8.GetBytes(csv);
                return new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "users.csv"
                };
            }

            else if (fileType.ToLower() == "xlsx")
            {

                var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("Users");

               
                ws.Cells[1, 1].Value = "Id";
                ws.Cells[1, 2].Value = "UserSpecificId";
                ws.Cells[1, 3].Value = "Name";
                ws.Cells[1, 4].Value = "Surname";
                ws.Cells[1, 5].Value = "Email";
                ws.Cells[1, 6].Value = "Role";
                ws.Cells[1, 7].Value = "Status";
                ws.Cells[1, 8].Value = "CreatedAt";
                ws.Cells[1, 9].Value = "VersionNum";

                
                for (int i = 0; i < users.Count; i++)
                {
                    var u = users[i];
                    ws.Cells[i + 2, 1].Value = u.Id;
                    ws.Cells[i + 2, 2].Value = u.UserSpecificId;
                    ws.Cells[i + 2, 3].Value = u.Name;
                    ws.Cells[i + 2, 4].Value = u.Surname;
                    ws.Cells[i + 2, 5].Value = u.Email;
                    ws.Cells[i + 2, 6].Value = u.Role;
                    ws.Cells[i + 2, 7].Value = u.Status;
                    ws.Cells[i + 2, 8].Value = u.CreatedAt.ToString("yyyy-MM-dd");
                    ws.Cells[i + 2, 9].Value = u.VersionNum;
                }

                var content = package.GetAsByteArray();
                return new FileContentResult(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "users.xlsx"
                };
            }

            else if (fileType.ToLower() == "pdf")
            {
                using var ms = new MemoryStream();
                var writer = new PdfWriter(ms);
                var pdf = new PdfDocument(writer);
                var doc = new Document(pdf);

                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                doc.Add(new Paragraph("Lista korisnika").SetFont(boldFont).SetFontSize(16));

                foreach (var u in users)
                {
                    var line = $"{u.Id}, {u.UserSpecificId}, {u.Name} {u.Surname}, {u.Email}, {u.Role}, {u.Status}, {u.CreatedAt:yyyy-MM-dd}, {u.VersionNum}";
                    doc.Add(new Paragraph(line));
                }

                doc.Close();
                return new FileContentResult(ms.ToArray(), "application/pdf")
                {
                    FileDownloadName = "users.pdf"
                };
            }

            return new BadRequestObjectResult("Unsupported file type. Try csv, excel, or pdf.");
        }
    }
}

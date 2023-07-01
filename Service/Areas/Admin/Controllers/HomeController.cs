using ClosedXML.Excel;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using Service.Areas.Admin.Models;
using Service.Domain;
using Service.Domain.Entities;
using Service.Models;
using System.Linq;
using System.Text;
//using System.Web.Mvc;

namespace Service.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext context)
        {
            _db = context;
        }

        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;           
            ViewData["IdSortParam"] = string.IsNullOrEmpty(sortOrder) ? "IdDesc" : "";
            ViewData["NameSortParam"] =  sortOrder == "FullNameAsc" ? "FullNameDesc" : "FullNameAsc";
            ViewData["GroupSortParam"] = sortOrder == "GroupAsc" ? "GroupDesc" : "GroupAsc";
            ViewData["DirSortParam"] = sortOrder == "DirAsc" ? "DirDesc" : "DirAsc";
            ViewData["FirstSortParam"] = sortOrder == "FirstProfileAsc" ? "FirstProfileDesc" : "FirstProfileAsc";
            ViewData["SecondSortParam"] = sortOrder == "SecondProfileAsc" ? "SecondProfileDesc" : "SecondProfileAsc";
            ViewData["ThirdSortParam"] = sortOrder == "ThirdProfileAsc" ? "ThirdProfileDesc" : "ThirdProfileAsc";
            ViewData["AvgSortParam"] = sortOrder == "AvgAsc" ? "AvgDesc" : "AvgAsc";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            
            var statements = from s in _db.Statements select s;

            ViewData["FirstMATH"] = _db.Statements.Where(s => s.FirstProfile == "01.03.01 Математика").Count();
            ViewData["FirstMEH"] = _db.Statements.Where(s => s.FirstProfile == "01.03.03 Механика и математическое моделирование").Count();
            ViewData["FirstMAT"] = _db.Statements.Where(s => s.FirstProfile == "01.03.04 Прикладная математика").Count();
            ViewData["FirstCS"] = _db.Statements.Where(s => s.FirstProfile == "02.03.01 Математика и компьютерные науки").Count();
            ViewData["FirstADM"] = _db.Statements.Where(s => s.FirstProfile == "02.03.03 Математическое обеспечение и администрирование информационных систем").Count();

            ViewData["SecondMATH"] = _db.Statements.Where(s => s.SecondProfile == "01.03.01 Математика").Count();
            ViewData["SecondMEH"] = _db.Statements.Where(s => s.SecondProfile == "01.03.03 Механика и математическое моделирование").Count();
            ViewData["SecondMAT"] = _db.Statements.Where(s => s.SecondProfile == "01.03.04 Прикладная математика").Count();
            ViewData["SecondCS"] = _db.Statements.Where(s => s.SecondProfile == "02.03.01 Математика и компьютерные науки").Count();
            ViewData["SecondADM"] = _db.Statements.Where(s => s.SecondProfile == "02.03.03 Математическое обеспечение и администрирование информационных систем").Count();

            ViewData["ThirdMATH"] = _db.Statements.Where(s => s.ThirdProfile == "01.03.01 Математика").Count();
            ViewData["ThirdMEH"] = _db.Statements.Where(s => s.ThirdProfile == "01.03.03 Механика и математическое моделирование").Count();
            ViewData["ThirdMAT"] = _db.Statements.Where(s => s.ThirdProfile == "01.03.04 Прикладная математика").Count();
            ViewData["ThirdCS"] = _db.Statements.Where(s => s.ThirdProfile == "02.03.01 Математика и компьютерные науки").Count();
            ViewData["ThirdADM"] = _db.Statements.Where(s => s.ThirdProfile == "02.03.03 Математическое обеспечение и администрирование информационных систем").Count();

            if (!string.IsNullOrEmpty(searchString))
            {
                statements = statements.Where(s =>
                                        s.FullName!.Contains(searchString) ||
                                        s.Group!.Contains(searchString) ||
                                        s.Direction!.Contains(searchString) ||
                                        s.FirstProfile!.Contains(searchString) ||
                                        s.SecondProfile!.Contains(searchString) ||
                                        s.ThirdProfile!.Contains(searchString) ||
                                        s.Average.Contains(searchString));
            }

            statements = sortOrder switch
            {
                "FullNameAsc" => statements.OrderBy(s => s.FullName),
                "FullNameDesc" => statements.OrderByDescending(s => s.FullName),
                "IdAsc" => statements.OrderBy(s => s.Id),
                "IdDesc" => statements.OrderByDescending(s => s.Id),
                "GroupAsc" => statements.OrderBy(s => s.Group),
                "GroupDesc" => statements.OrderByDescending(s => s.Group),
                "DirAsc" => statements.OrderBy(s => s.Direction),
                "DirDesc" => statements.OrderByDescending(s => s.Direction),
                "FirstProfileAsc" => statements.OrderBy(s => s.FirstProfile),
                "FirstProfileDesc" => statements.OrderByDescending(s => s.FirstProfile),
                "SecondProfileAsc" => statements.OrderBy(s => s.SecondProfile),
                "SecondProfileDesc" => statements.OrderByDescending(s => s.SecondProfile),
                "ThirdProfileAsc" => statements.OrderBy(s => s.ThirdProfile),
                "ThirdProfileDesc" => statements.OrderByDescending(s => s.ThirdProfile),
                "AvgAsc" => statements.OrderBy(s => s.Average),
                "AvgDesc" => statements.OrderByDescending(s => s.Average),
                _ => statements.OrderBy(s => s.Id),
            };
            int pageSize = 3;
            return View(await PaginatedList<ServicesViewModel>.CreateAsync(statements.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View(new List<PRSystem>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Import(IFormFile excel)
        {
            double sum = 0;
            double num = 0;
            if (ModelState.IsValid)
            {
                if (excel?.Length > 0)
                {
                    var stream = excel.OpenReadStream();
                    List<PRSystem> brsModel = new List<PRSystem>();
                    try
                    {
                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            var worksheet = package.Workbook.Worksheets.First();
                            var rowCount = worksheet.Dimension.Rows;
                            
                            for (var row = 2; row <= rowCount; row++)
                            {
                                try
                                {
                                    var subjects = worksheet.Cells[row, 1].Value?.ToString();
                                    var points = worksheet.Cells[row, 2].Value?.ToString();
                                    var hours = worksheet.Cells[row, 3].Value?.ToString();
                                    var marks = worksheet.Cells[row, 4].Value?.ToString();
                                    var semesters = worksheet.Cells[row, 5].Value?.ToString();

                                    
                                    sum += Convert.ToDouble(worksheet.Cells[row, 2].Value);
                                    num += Convert.ToDouble(worksheet.Cells[row, 2].Count());
                                    
                                    var brs = new PRSystem
                                    {
                                        Subjects = subjects,
                                        Points = points,
                                        Hours = hours,
                                        Marks = marks,
                                        Semesters = semesters,
                                    };

                                    double avg = sum / num;
                                    ViewData["Avg"] = avg;
                                    brsModel.Add(brs);
                                }
                                catch (Exception ex)
                                {
                                    return View("Error");
                                }
                            }
                        }

                        return View(brsModel);

                    }
                    catch (Exception e)
                    {
                        return View("Error");
                    }
                }
            }

            return View("Error");
        }

        public IActionResult Export()
        {
            var stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var xlPackage = new ExcelPackage(stream))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var worksheet = xlPackage.Workbook.Worksheets.Add("Заявления");

                worksheet.Cells["A1"].Value = "№ п\\п";
                worksheet.Cells["B1"].Value = "ФИО";
                worksheet.Cells["C1"].Value = "Текущее направление";
                worksheet.Cells["D1"].Value = "Первое приоритетное направление";
                worksheet.Cells["E1"].Value = "Второе приоритетное направление";
                worksheet.Cells["F1"].Value = "Третье приоритетное направление";
                worksheet.Cells["G1"].Value = "Средний балл";

                var row = 2;

                foreach (var statement in _db.Statements)
                {
                    worksheet.Cells[row, 1].Value = statement.Id;
                    worksheet.Cells[row, 2].Value = statement.FullName;
                    worksheet.Cells[row, 3].Value = statement.Direction;
                    worksheet.Cells[row, 4].Value = statement.FirstProfile;
                    worksheet.Cells[row, 5].Value = statement.SecondProfile;
                    worksheet.Cells[row, 6].Value = statement.ThirdProfile;
                    worksheet.Cells[row, 7].Value = statement.Average;

                    row++;
                }

                xlPackage.Save();
            }
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "заявления.xlsx");
        }
            
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                var service = await _db.Statements.FirstOrDefaultAsync(p => p.Id == id);
                if (service != null)
                    return View(service);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ServicesViewModel service)
        {
            _db.Statements.Update(service);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                var service = await _db.Statements.FirstOrDefaultAsync(p => p.Id == id);
                if (service != null)
                    return View(service);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                ServicesViewModel? service = await _db.Statements.FirstOrDefaultAsync(p => p.Id == id);
                if (service != null)
                {
                    _db.Statements.Remove(service);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using GemBox.Document;
using Service.Domain;
using Service.Models;
using Microsoft.AspNetCore.Authorization;

namespace Service.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _db;

        public ServicesController(IWebHostEnvironment environment, AppDbContext content)
        {
            _db = content;
            _environment = environment;
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            ComponentInfo.FreeLimitReached += (sender, e) => e.FreeLimitReachedAction = FreeLimitReachedAction.Stop;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(new ServicesViewModel());
        }

        public async Task<IActionResult> Download(ServicesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = Path.Combine(_environment.ContentRootPath, "Shablon.docx");
                var document = DocumentModel.Load(path);

                document.MailMerge.Execute(model);
                var stream = new MemoryStream();

                document.Save(stream, model.Options);
                _db.Statements.Add(model);
                await _db.SaveChangesAsync();

                return File(stream, model.Options.ContentType, $"Statement.{model.Format.ToLower()}");
            }
            else
            {
                return View("ErrorService");
            }
        }
    }
}
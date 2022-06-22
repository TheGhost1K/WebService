using Microsoft.AspNetCore.Mvc;
using GemBox.Document;
using Service.Domain;
using Service.Models;
using Microsoft.AspNetCore.Authorization;

namespace Service.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IWebHostEnvironment environment;
        private readonly AppDbContext db;

        public ServicesController(IWebHostEnvironment environment, AppDbContext content)
        {
            db = content;
            this.environment = environment;
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            ComponentInfo.FreeLimitReached += (sender, e) => e.FreeLimitReachedAction = FreeLimitReachedAction.Stop;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(new ServicesViewModel());
        }

        public FileStreamResult Download(ServicesViewModel model)
        {
            var path = Path.Combine(environment.ContentRootPath, "Shablon.docx");
            var document = DocumentModel.Load(path);

            document.MailMerge.Execute(model);
            var stream = new MemoryStream();

            document.Save(stream, model.Options);
            db.Statements.Add(model);
            db.SaveChanges();

            return File(stream, model.Options.ContentType, $"Statement.{model.Format.ToLower()}");
        }
    }
}

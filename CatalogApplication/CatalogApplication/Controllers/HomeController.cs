using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Web;
using CatalogApplication.Data;
using CatalogApplication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CatalogApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogApplication.Controllers;

public class HomeController(CatalogDbContext dbcontext, ICatalogService _catalogService) : Controller
{
    [Route("/{*path}")]
    public IActionResult ShowCatalog(string? path)
    {
        path = HttpUtility.UrlDecode(path);
        if (path == null)
            return StartPage();
        var catalog = dbcontext.Catalogs.Include(cat=>cat.ChildCatalogs).Include(cat=>cat.Parent).ToList().FirstOrDefault(cat => _catalogService.GetPath(cat) == path);
        if (catalog == null)
            return StartPage();
        return View("Index",catalog);
    }
    [Route("/download")]
    public IActionResult UploadCatalogs()
    {
        string fileNameZip = $"Structure_Snapshot_{DateTime.Now}.zip";
        byte[] compressedBytes;
        using (var outStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
            {
                dbcontext.Catalogs.ToList().ForEach(cat=>archive.CreateEntry(_catalogService.GetPath(cat,true)));
            }
            compressedBytes = outStream.ToArray();
        }
        return File(compressedBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileNameZip);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private IActionResult StartPage()
    {
        var initCatalog = dbcontext.Catalogs.FirstOrDefault(cat => cat.ParentId == null);
        return LocalRedirect($"/{initCatalog.Name}");
    }

    [NonAction]
    public static string RollBackPath(string path)
    {
        var folds = path.Split('/');
        string newpath = string.Join('/', folds, 0, folds.Length - 1);
        return newpath;

    }
}
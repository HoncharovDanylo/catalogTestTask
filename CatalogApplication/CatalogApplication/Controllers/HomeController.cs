using System.Diagnostics;
using System.IO.Compression;
using System.Web;
using CatalogApplication.Data;
using CatalogApplication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CatalogApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogApplication.Controllers;

public class HomeController(CatalogDbContext dbcontext, ICatalogService _catalogService) : Controller
{
    [HttpGet]
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
    public IActionResult DownloadCatalogs()
    {
        string fileNameZip = $"Structure_Snapshot_{DateTime.Now}.zip";
        byte[] compressedBytes;
        using (var outStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
            {
                dbcontext.Catalogs.ToList().ForEach(cat=>archive.CreateEntry(_catalogService.GetPath(cat)+"/"));
            }
            compressedBytes = outStream.ToArray();
        }
        return File(compressedBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileNameZip);
    }
    [HttpPost]
    [Route("/{*path}")]
    public async Task<IActionResult> UploadCatalogs(IFormFile file)
    {
       var oldStructure = this.DownloadCatalogs();
       dbcontext.Catalogs.RemoveRange(dbcontext.Catalogs.Where(c=>true));
      var listOfNewCatalogs = new List<Catalog>();
        using (ZipArchive archive = new ZipArchive(file.OpenReadStream()))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                var catalogs = entry.ToString().Split('/',StringSplitOptions.RemoveEmptyEntries);
                var catalog = new Catalog()
                {
                    Name = catalogs[catalogs.Length-1],
                    Parent = listOfNewCatalogs.FirstOrDefault(cat=> _catalogService.GetPath(cat) == string.Join('/',catalogs,0,catalogs.Length-1))
                };
                listOfNewCatalogs.Add(catalog);
            }
            dbcontext.Catalogs.AddRange(listOfNewCatalogs);
            dbcontext.SaveChanges();
        }
        return oldStructure;
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private IActionResult StartPage()
    {
        var startcatalogs = dbcontext.Catalogs.Where(cat => cat.ParentId == null).ToList();
        Catalog init = new Catalog() { Name = String.Empty, ChildCatalogs = startcatalogs, Parent = null };
        return View("Index",init);
    }

    [NonAction]
    public static string RollBackPath(string path)
    {
        var folds = path.Split('/');
        string newpath = string.Join('/', folds, 0, folds.Length - 1);
        return newpath == String.Empty ? "/" : newpath;
    }
}
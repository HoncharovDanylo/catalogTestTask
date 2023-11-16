using System.Diagnostics;
using System.IO.Compression;
using System.Web;
using CatalogApplication.Data;
using CatalogApplication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CatalogApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogApplication.Controllers;

public class HomeController(CatalogDbContext dbcontext, ICatalogService catalogService) : Controller
{
    [HttpGet]
    [Route("/{*path}")]
    public async  Task<IActionResult> ShowCatalog(string? path)
    {
        path = HttpUtility.UrlDecode(path);
        if (path == null)
            return await StartPage();
        var catalogs = await dbcontext.Catalogs.Include(cat => cat.ChildCatalogs).Include(cat => cat.Parent)
            .ToListAsync();
            var catalog = catalogs.FirstOrDefault(cat => catalogService.GetPath(cat) == path);
        if (catalog == null)
            return await StartPage();
        return View("Index",catalog);
    }
    
    [Route("/download")]
    public async Task<IActionResult> DownloadCatalogs()
    {
        string fileNameZip = $"Structure_Snapshot_{DateTime.Now}.zip";
        byte[] compressedBytes;
        using (var outStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
            {
                var catalogs = await dbcontext.Catalogs.ToListAsync();
                  catalogs.ForEach(cat=>archive.CreateEntry(catalogService.GetPath(cat)+"/"));
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
                    Name = catalogs[^1],
                    Parent = listOfNewCatalogs.FirstOrDefault(cat=> catalogService.GetPath(cat) == string.Join('/',catalogs,0,catalogs.Length-1))
                };
                listOfNewCatalogs.Add(catalog);
            }
            await dbcontext.Catalogs.AddRangeAsync(listOfNewCatalogs);
            await dbcontext.SaveChangesAsync();
        }
        return await oldStructure;
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<IActionResult> StartPage()
    {
        var startcatalogs = await dbcontext.Catalogs.Where(cat => cat.ParentId == null).ToListAsync();
        Catalog init = new Catalog() { Name = String.Empty, ChildCatalogs = startcatalogs, Parent = null };
        return View("Index",init);
    }
    
}
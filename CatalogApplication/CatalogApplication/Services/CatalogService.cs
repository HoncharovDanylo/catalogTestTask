using System.Text;
using CatalogApplication.Interfaces;
using CatalogApplication.Models;

namespace CatalogApplication.Services;

public class  CatalogService : ICatalogService
{
    public string GetPath(Catalog? catalog)
    {
        var path = new StringBuilder(catalog.Name);
        if (catalog.Parent != null)
        {
            Catalog? current = catalog.Parent;
            while (current != null)
            {
                path.Insert(0, current.Name + "/");
                current = current.Parent;
            }
        }
            return path.ToString();
    }
    public  string RollBackPath(string path)
    {
        var folds = path.Split('/');
        string newpath = string.Join('/', folds, 0, folds.Length - 1);
        return newpath == String.Empty ? "/" : newpath;
    }
}
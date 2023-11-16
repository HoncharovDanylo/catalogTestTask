using System.Text;
using CatalogApplication.Interfaces;
using CatalogApplication.Models;

namespace CatalogApplication.Services;

public class  CatalogService : ICatalogService
{
    public string GetPath(Catalog? catalog, bool finalslash = false)
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

        if (!finalslash)
            return path.ToString();
        return path.ToString() + "/";
    }
}
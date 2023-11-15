using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogApplication.Models;

public class Catalog
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("ParentCatalog")]
    public int? ParentId { get; set; }
    public virtual Catalog Parent { get; set; }
    public virtual List<Catalog> ChildCatalogs { get; set; } = new List<Catalog>();
    [Required]
    public string Name { get; set; }
    
}
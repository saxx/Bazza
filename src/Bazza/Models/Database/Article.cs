using System;

namespace Bazza.Models.Database;

public class Article
{
    public int PersonId { get; set; }
    public int ArticleId { get; set; }
    public string? Name { get; set; }
    public string? Size { get; set; }
    public double Price { get; set; }
    public int? SaleId { get; set; }
    public DateTime? SaleUtc { get; set; }
    public string? SaleUsername { get; set; }
    public DateTime? BlockedUtc { get; set; }
    public string? BlockedUsername { get; set; }
    public Sale? Sale { get; set; }
    
}
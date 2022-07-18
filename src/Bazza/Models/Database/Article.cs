namespace Bazza.Models.Database;

public class Article
{
    public int PersonId { get; set; }
    public int ArticleId { get; set; }
    public string? Name { get; set; }
    public string? Size { get; set; }
    public double Price { get; set; }
}
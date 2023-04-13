using System;
using System.Collections.Generic;

namespace Bazza.Models.Database;

public class Sale
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public DateTime CreatedUtc { get; set; }
    public ICollection<Article> Articles { get; set; } = null!;
}
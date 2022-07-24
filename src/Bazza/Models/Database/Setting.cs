using System;

namespace Bazza.Models.Database;

public class Setting
{
    public string Key { get; set; } = "";
    public string? StringValue { get; set; }
    public int? NumberValue { get; set; }
    public DateTime? DateTimeValue { get; set; }

    public DateTime UpdatedUtc { get; set; }
}
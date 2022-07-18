using System.ComponentModel.DataAnnotations;
using Bazza.ViewModels.Home;

namespace Bazza.Services;

public class DivisableBy50Attribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value,
        ValidationContext validationContext)
    {
        var article = (RegisterViewModel.Article) validationContext.ObjectInstance;


        if ((article.Price ?? 0) % 0.5 > 0)
        {
            return new ValidationResult("Bitte den Preis auf 50 Cent genau angeben.");
        }

        return ValidationResult.Success;
    }
}
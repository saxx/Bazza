using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bazza.ViewModels.Home
{
    public class IndexViewModel
    {
        [Required(ErrorMessage = "Bitte gib deinen Namen an.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Bitte gib deine Adresse an.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Bitte gib deine E-Mail-Adresse an."), EmailAddress(ErrorMessage = "Bitte gib deine korrekte E-Mail-Adresse an.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bitte gib deine Telefonnummer an.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Bitte stimm unserer Datenschutzerklärung zu."), RegularExpression("true", ErrorMessage = "Bitte stimme unserer Datenschutzerklärung zu.")]
        public string Privacy { get; set; }

        public IList<Article> Articles { get; set; } = new List<Article>();

        [BindNever] public bool DisplaySuccess { get; set; }
        
        public class Article
        {
            [Required(ErrorMessage = "Bitte gib eine aussagekräftige Artikelbeschreibung an.")]
            public string Name { get; set; }

            public string Size { get; set; }

            [Required(ErrorMessage = "Bitte gib den Preis an.")]
            public double? Price { get; set; }
        }
    }
}
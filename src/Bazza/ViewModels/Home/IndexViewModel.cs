using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public IList<Article> Articles { get; set; } = new List<Article>();
        
        public class Article
        {
            public string Name { get; set; }
            public string Size { get; set; }
            public double Price { get; set; }
        }
    }
}
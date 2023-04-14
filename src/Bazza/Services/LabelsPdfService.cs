using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Pdf.V2;
using Adliance.AspNetCore.Buddy.Template.Razor;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using ZXing.QrCode;

namespace Bazza.Services;

public class LabelsPdfService
{
    private readonly ITemplater _templater;
    private readonly IPdfer _pdfer;
    private readonly Db _db;

    public LabelsPdfService(ITemplater templater, IPdfer pdfer, Db db)
    {
        _templater = templater;
        _pdfer = pdfer;
        _db = db;
    }

    public async Task<byte[]> BuildPdf(int personId)
    {
        var html = await _templater.Render("Pdf", "Labels", await BuildViewModel(personId));
        return await _pdfer.HtmlToPdf(html, new PdfOptions());
    }

    private async Task<LabelsPdfViewModel> BuildViewModel(int personId)
    {
        var person = await _db.Persons.AsNoTracking().SingleOrDefaultAsync(x => x.PersonId == personId) ?? throw new EntityNotFoundException();
        var articles = await _db.Articles.AsNoTracking().Where(x => x.PersonId == person.PersonId).OrderBy(x => x.ArticleId).ToListAsync();
        
        var result = new LabelsPdfViewModel
        {
            PersonEmail = person.Email ?? "",
            PersonId = person.PersonId,
            PersonName = person.Name ?? "",
            Articles = articles.Select(x => new LabelsPdfViewModel.Article
            {
                Id = x.ArticleId,
                Price = x.Price,
                Size = x.Size,
                Name = x.Name
            }).ToList()
        };

        foreach (var a in result.Articles)
        {
            var barcodeWriter = new ZXing.ImageSharp.BarcodeWriter<SixLabors.ImageSharp.PixelFormats.La32>
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 500,
                    Width = 500,
                    Margin = 0
                }
            };

            var image = barcodeWriter.Write($"{result.PersonId}-{a.Id}");
            a.QrCodeBase64 = image.ToBase64String(PngFormat.Instance);
        }

        return result;
    }

    public class LabelsPdfViewModel
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; } = "";
        public string PersonEmail { get; set; } = "";
        public IList<Article> Articles { get; set; } = new List<Article>();

        public class Article
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Size { get; set; }
            public double Price { get; set; }
            public string? QrCodeBase64 { get; set; }
        }
    }
}
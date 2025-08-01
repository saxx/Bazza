﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Bazza.Models;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Bazza.Services;

public class ExcelExportService(Db db)
{
    public async Task<byte[]> CreateExcelFile()
    {
        var allPersons = await db.Persons.OrderBy(x => x.PersonId).ToListAsync();
        var allArticles = await db.Articles.OrderBy(x => x.PersonId).ThenBy(x => x.ArticleId).ToListAsync();

        ExcelPackage.License.SetNonCommercialPersonal("Bazza");
        var excel = new ExcelPackage();
        AddPersons(excel, allPersons);
        AddArticles(excel, allArticles);
        foreach (var p in allPersons)
        {
            AddPerson(excel, p, allArticles.Where(x => x.PersonId == p.PersonId).ToList());
        }

        return await excel.GetAsByteArrayAsync();
    }

    private static void AddPersons(ExcelPackage excel, IList<Person> persons)
    {
        var worksheet = excel.Workbook.Worksheets.Add("Alle Personen");
        worksheet.Column(1).Width = 10;
        worksheet.Column(2).Width = 30;
        worksheet.Column(3).Width = 30;
        worksheet.Column(4).Width = 25;
        worksheet.Column(5).Width = 20;
        worksheet.Column(6).Width = 20;
        worksheet.Column(7).Width = 20;

        worksheet.Cells[1, 1].Value = "Nummer";
        worksheet.Cells[1, 2].Value = "Name";
        worksheet.Cells[1, 3].Value = "Adresse";
        worksheet.Cells[1, 4].Value = "E-Mail-Adresse";
        worksheet.Cells[1, 5].Value = "Telefonnummer";
        worksheet.Cells[1, 6].Value = "Erstellt (UTC)";
        worksheet.Cells[1, 7].Value = "Geändert (UTC)";
        worksheet.Cells[1, 1, 1, 7].Style.Font.Bold = true;

        var row = 2;
        foreach (var p in persons)
        {
            worksheet.Cells[row, 1].Value = p.PersonId;
            worksheet.Cells[row, 2].Value = p.Name;
            worksheet.Cells[row, 3].Value = p.Address;
            worksheet.Cells[row, 3].Style.WrapText = true;
            worksheet.Cells[row, 4].Value = p.Email;
            worksheet.Cells[row, 5].Value = p.Phone;
            worksheet.Cells[row, 6].Value = p.CreatedUtc;
            worksheet.Cells[row, 6].Style.Numberformat.Format = "dd.MM.yyyy HH:mm";
            worksheet.Cells[row, 7].Value = p.UpdatedUtc;
            worksheet.Cells[row, 7].Style.Numberformat.Format = "dd.MM.yyyy HH:mm";
            row++;
        }

        worksheet.Cells[1, 1, row - 1, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
    }

    private static void AddArticles(ExcelPackage excel, IList<Article> articles)
    {
        var worksheet = excel.Workbook.Worksheets.Add("Alle Artikel");
        worksheet.Column(1).Width = 10;
        worksheet.Column(2).Width = 10;
        worksheet.Column(3).Width = 30;
        worksheet.Column(4).Width = 10;
        worksheet.Column(5).Width = 10;
        worksheet.Column(6).Width = 10;

        worksheet.Cells[1, 1].Value = "Person";
        worksheet.Cells[1, 2].Value = "Artikel";
        worksheet.Cells[1, 3].Value = "Name";
        worksheet.Cells[1, 4].Value = "Größe";
        worksheet.Cells[1, 5].Value = "Preis";
        worksheet.Cells[1, 6].Value = "Letzte Zeile";
        worksheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;

        var row = 2;
        var lastPersonId = 0;
        foreach (var a in articles)
        {
            worksheet.Cells[row, 1].Value = a.PersonId;
            worksheet.Cells[row, 2].Value = a.ArticleId;
            worksheet.Cells[row, 3].Value = a.Name;
            worksheet.Cells[row, 4].Value = string.IsNullOrWhiteSpace(a.Size) ? "" : "Gr. " + a.Size;
            worksheet.Cells[row, 5].Value = a.Price.ToString("N2", new CultureInfo("de-DE")) + " €";

            if (lastPersonId != 0 && lastPersonId != a.PersonId)
            {
                worksheet.Cells[row - 1, 6].Value = 1;
            }
            else if (lastPersonId != 0)
            {
                worksheet.Cells[row - 1, 6].Value = 0;
            }

            lastPersonId = a.PersonId;
            row++;
        }

        worksheet.Cells[row - 1, 6].Value = 1; // so that the last row also is counted as "Letzte Zeile"
    }

    private static void AddPerson(ExcelPackage excel, Person p, IList<Article> articles)
    {
        var worksheet = excel.Workbook.Worksheets.Add($"Kunde {p.PersonId}");
        worksheet.PrinterSettings.RepeatRows = new ExcelAddress("1:2");

        worksheet.Cells[1, 1].Value = "Nummer:";
        worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        worksheet.Cells[1, 1, 1, 2].Merge = true;
        worksheet.Cells[2, 1].Value = "Name:";
        worksheet.Cells[2, 1, 2, 2].Merge = true;
        worksheet.Cells[3, 1].Value = "Adresse:";
        worksheet.Cells[3, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        worksheet.Cells[3, 1, 3, 2].Merge = true;
        worksheet.Cells[4, 1].Value = "E-Mail-Adresse:";
        worksheet.Cells[4, 1, 4, 2].Merge = true;
        worksheet.Cells[5, 1].Value = "Telefonnummer:";
        worksheet.Cells[5, 1, 5, 2].Merge = true;
        worksheet.Cells[7, 1].Value = "Anzahl verkaufte Artikel:";
        worksheet.Cells[7, 1, 7, 2].Merge = true;
        worksheet.Cells[8, 1].Value = "Erlös:";
        worksheet.Cells[8, 1, 8, 2].Merge = true;
        worksheet.Cells[9, 1].Value = "- 10 % vom Umsatz:";
        worksheet.Cells[9, 1, 9, 2].Merge = true;
        worksheet.Cells[1, 1, 9, 1].Style.Font.Bold = true;
        worksheet.Cells[10, 1].Value = "- Einschreibgebühr:";
        worksheet.Cells[10, 1, 10, 2].Merge = true;
        worksheet.Cells[1, 1, 10, 1].Style.Font.Bold = true;
        worksheet.Cells[11, 1].Value = "Auszahlungsbetrag:";
        worksheet.Cells[11, 1, 11, 2].Merge = true;
        worksheet.Cells[11, 1, 11, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        worksheet.Cells[11, 1, 11, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
        worksheet.Cells[1, 1, 11, 1].Style.Font.Bold = true;

        worksheet.Cells[1, 3].Value = p.PersonId.ToString(CultureInfo.InvariantCulture);
        worksheet.Cells[1, 3].Style.Font.Size = 18;
        worksheet.Cells[1, 3, 1, 5].Merge = true;
        worksheet.Cells[2, 3].Value = p.Name;
        worksheet.Cells[2, 3, 2, 5].Merge = true;
        worksheet.Cells[3, 3].Value = p.Address;
        worksheet.Cells[3, 3, 3, 5].Style.WrapText = true;
        worksheet.Cells[3, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        worksheet.Row(3).Height = 30;
        worksheet.Cells[3, 3, 3, 5].Merge = true;
        worksheet.Cells[4, 3].Value = p.Email;
        worksheet.Cells[4, 3, 4, 5].Merge = true;
        worksheet.Cells[5, 3].Value = p.Phone;
        worksheet.Cells[5, 3, 5, 5].Merge = true;
        worksheet.Cells[7, 3].Formula = "=COUNTIF(E14:E999,\"=v\")";
        worksheet.Cells[8, 3].Formula = "=SUMIF(E14:E999,\"=v\",D14:D999)";
        worksheet.Cells[8, 3].Style.Numberformat.Format = "#0.00 €";
        worksheet.Cells[9, 3].Formula = $"=C8*-{Settings.PercentageProvision.ToString(CultureInfo.InvariantCulture)}";
        worksheet.Cells[9, 3].Style.Numberformat.Format = "#0.00 €";
        worksheet.Cells[10, 3].Formula = $"=COUNTIF(D14:D999,\">=25\")*-{Settings.CostsPerArticleAbove25.ToString(CultureInfo.InvariantCulture)}+COUNTIF(D14:D999,\"<25\")*-{Settings.CostsPerArticleBelow25.ToString(CultureInfo.InvariantCulture)}";
        worksheet.Cells[10, 3].Style.Numberformat.Format = "#0.00 €";
        worksheet.Cells[11, 3].Formula = "=MAX(C8+C9+C10,0)";
        worksheet.Cells[11, 3].Style.Numberformat.Format = "#0.00 €";

        worksheet.Column(1).Width = 10;
        worksheet.Column(2).Width = 30;
        worksheet.Column(3).Width = 10;
        worksheet.Column(4).Width = 10;
        worksheet.Column(5).Width = 10;

        var row = 13;
        worksheet.Cells[row, 1].Value = "Artikel";
        worksheet.Cells[row, 2].Value = "Name";
        worksheet.Cells[row, 3].Value = "Größe";
        worksheet.Cells[row, 4].Value = "Preis";
        worksheet.Cells[row, 5].Value = "Verkauft";
        worksheet.Cells[row, 1, row, 6].Style.Font.Bold = true;
        row++;

        foreach (var a in articles)
        {
            worksheet.Cells[row, 1].Value = a.ArticleId;
            worksheet.Cells[row, 2].Value = a.Name;
            worksheet.Cells[row, 2].Style.WrapText = true;
            worksheet.Cells[row, 3].Value = a.Size;
            worksheet.Cells[row, 4].Value = a.Price;
            worksheet.Cells[row, 4].Style.Numberformat.Format = "#0.00 €";
            worksheet.Cells[row, 5].Value = "";
            worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[row, 1, row, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            worksheet.Cells[row, 1, row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Row(row).Height = 25.8; // 43px
            row++;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.Models;

public class Settings
{
    private readonly Db _db;
    private readonly IDictionary<string, Setting> _settings;

    public Settings(Db db)
    {
        _db = db;
        _settings = db.Settings.AsNoTracking().ToDictionary(x => x.Key, x => x);
    }


    public async Task UpdateDatabase()
    {
        foreach (var (key, setting) in _settings)
        {
            var existing = await _db.Settings.SingleOrDefaultAsync(x => x.Key == key);
            if (existing == null)
            {
                existing = new Setting
                {
                    Key = key
                };
                await _db.Settings.AddAsync(existing);
            }

            if (existing.StringValue != setting.StringValue || existing.NumberValue != setting.NumberValue || existing.DateTimeValue != setting.DateTimeValue)
            {
                existing.StringValue = setting.StringValue;
                existing.NumberValue = setting.NumberValue;
                existing.DateTimeValue = setting.DateTimeValue;
                await _db.SaveChangesAsync();
            }
        }
    }

    public DateTime EventStartDate
    {
        get => Get("event_start_date", new DateTime(2022, 09, 09));
        set => Set("event_start_date", value);
    }

    public DateTime EventEndDate
    {
        get => Get("event_end_date", new DateTime(2022, 09, 10));
        set => Set("event_end_date", value);
    }

    public DateTime RegistrationStartDate
    {
        get => Get("registration_start_date", new DateTime(2022, 08, 23));
        set => Set("registration_start_date", value);
    }

    public DateTime RegistrationEndDate
    {
        get => Get("registration_end_date", new DateTime(2022, 09, 05));
        set => Set("registration_end_date", value);
    }

    public bool RegistrationHasStarted => DateTime.UtcNow.Date >= RegistrationStartDate.Date;
    public bool RegistrationHasEnded => DateTime.UtcNow.Date > RegistrationEndDate;
    public bool RegistrationIsActive => RegistrationHasStarted && !RegistrationHasEnded;

    public DateTime ArticlesDropOffDate
    {
        get => Get("articles_drop_off_date", new DateTime(2022, 09, 08));
        set => Set("articles_drop_off_date", value);
    }

    public int MaxNumberOfArticles
    {
        get => Get("max_number_of_articles", 6000);
        set => Set("max_number_of_articles", value);
    }

    public string CoronaText
    {
        get => Get("corona_text", "Es gelten die aktuellen Coronaregeln.");
        set => Set("corona_text", value);
    }

    public string AllowedArticleTypes
    {
        get => Get("allowed_article_types", @"<ul>
            <li>&#x2713; Alles rund ums Baby</li>
            <li>&#x2713; Spielsachen</li>
            <li>&#x2713; Kinderbekleidung bis Größe 164</li>
            <li>&#x2713; Schuhe</li>
            <li>&#x2713; Bücher</li>
            <li>&#x2713; Kinderwägen</li>
            <li>&#x2713; Umstandsmode</li>
            <li>&#x2713; Kinderfahrzeuge</li>
            <li>&#x2713; Autositze</li>
            <li>&#x2713; Schaukelpferde</li>
            <li>&#x2713; Hochstühle</li>
            <li>&#x2713; Wintersportartikel</li>
            <li>&#x2713; Faschingsbekleidung</li>
        </ul>");
        set => Set("allowed_article_types", value);
    }

    private DateTime Get(string key, DateTime defaultValue)
    {
        if (_settings.ContainsKey(key)) return _settings[key].DateTimeValue ?? defaultValue;
        return defaultValue;
    }

    private void Set(string key, DateTime value)
    {
        if (!_settings.ContainsKey(key)) _settings[key] = new Setting();
        _settings[key].DateTimeValue = value;
    }

    private int Get(string key, int defaultValue)
    {
        if (_settings.ContainsKey(key)) return _settings[key].NumberValue ?? defaultValue;
        return defaultValue;
    }

    private void Set(string key, int value)
    {
        if (!_settings.ContainsKey(key)) _settings[key] = new Setting();
        _settings[key].NumberValue = value;
    }

    private string Get(string key, string defaultValue)
    {
        if (_settings.ContainsKey(key)) return _settings[key].StringValue ?? defaultValue;
        return defaultValue;
    }

    private void Set(string key, string value)
    {
        if (!_settings.ContainsKey(key)) _settings[key] = new Setting();
        _settings[key].StringValue = value;
    }
}
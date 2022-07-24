using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Bazza.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bazza.ViewModels.Admin;

public class SettingsViewModelFactory
{
    private readonly Settings _settings;

    public SettingsViewModelFactory(Settings settings)
    {
        _settings = settings;
    }

    public async Task<SettingsViewModel> Build()
    {
        var result = new SettingsViewModel
        {
            CoronaText = _settings.CoronaText,
            AllowedArticleTypes = _settings.AllowedArticleTypes,
            EventEndDate = _settings.EventEndDate,
            EventStartDate = _settings.EventStartDate,
            RegistrationEndDate = _settings.RegistrationEndDate,
            RegistrationStartDate = _settings.RegistrationStartDate,
            ArticlesDropOffDate = _settings.ArticlesDropOffDate,
            MaxNumberOfArticles = _settings.MaxNumberOfArticles
        };
        return await Task.FromResult(result);
    }

    public async Task UpdateDatebase(SettingsViewModel viewModel)
    {
        _settings.CoronaText = viewModel.CoronaText;
        _settings.AllowedArticleTypes = viewModel.AllowedArticleTypes;
        _settings.EventEndDate = viewModel.EventEndDate;
        _settings.EventStartDate = viewModel.EventStartDate;
        _settings.RegistrationEndDate = viewModel.RegistrationEndDate;
        _settings.RegistrationStartDate = viewModel.RegistrationStartDate;
        _settings.ArticlesDropOffDate = viewModel.ArticlesDropOffDate;
        _settings.MaxNumberOfArticles = viewModel.MaxNumberOfArticles;
        await _settings.UpdateDatabase();
        viewModel.Success = true;
    }
}

public class SettingsViewModel
{
    [Required(ErrorMessage = "Bitte angeben.")] public DateTime EventStartDate { get; init; }
    [Required(ErrorMessage = "Bitte angeben.")] public DateTime EventEndDate { get; init; }
    [Required(ErrorMessage = "Bitte angeben.")] public DateTime RegistrationStartDate { get; init; }
    [Required(ErrorMessage = "Bitte angeben.")] public DateTime RegistrationEndDate { get; init; }
    [Required(ErrorMessage = "Bitte angeben.")] public DateTime ArticlesDropOffDate { get; init; }
    [Required(ErrorMessage = "Bitte angeben.")] public int MaxNumberOfArticles { get; init; }
    [Required(ErrorMessage = "Bitte angeben.")] public string CoronaText { get; init; } = "";
    [Required(ErrorMessage = "Bitte angeben.")] public string AllowedArticleTypes { get; init; } = "";

    [BindNever] public bool Success { get; set; }
}
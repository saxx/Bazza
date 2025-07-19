using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Bazza.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bazza.ViewModels.Admin;

public class SettingsViewModelFactory(Settings settings)
{
    public async Task<SettingsViewModel> Build()
    {
        var result = new SettingsViewModel
        {
            CoronaText = settings.CoronaText,
            AllowedArticleTypes = settings.AllowedArticleTypes,
            EventEndDate = settings.EventEndDate,
            EventStartDate = settings.EventStartDate,
            RegistrationEndDate = settings.RegistrationEndDate,
            RegistrationStartDate = settings.RegistrationStartDate,
            ArticlesDropOffDate = settings.ArticlesDropOffDate,
            MaxNumberOfArticles = settings.MaxNumberOfArticles,
            PrimaryResponsibleName = settings.PrimaryResponsibleName,
            PrimaryResponsibleEmail = settings.PrimaryResponsibleEmail,
            PrimaryResponsiblePhone = settings.PrimaryResponsiblePhone,
            SecondaryResponsibleName = settings.SecondaryResponsibleName,
            SecondaryResponsiblePhone = settings.SecondaryResponsiblePhone
        };
        return await Task.FromResult(result);
    }

    public async Task UpdateDatebase(SettingsViewModel viewModel)
    {
        settings.CoronaText = viewModel.CoronaText;
        settings.AllowedArticleTypes = viewModel.AllowedArticleTypes;
        settings.EventEndDate = viewModel.EventEndDate;
        settings.EventStartDate = viewModel.EventStartDate;
        settings.RegistrationEndDate = viewModel.RegistrationEndDate;
        settings.RegistrationStartDate = viewModel.RegistrationStartDate;
        settings.ArticlesDropOffDate = viewModel.ArticlesDropOffDate;
        settings.MaxNumberOfArticles = viewModel.MaxNumberOfArticles;
        settings.PrimaryResponsibleName = viewModel.PrimaryResponsibleName;
        settings.PrimaryResponsibleEmail = viewModel.PrimaryResponsibleEmail;
        settings.PrimaryResponsiblePhone = viewModel.PrimaryResponsiblePhone;
        settings.SecondaryResponsibleName = viewModel.SecondaryResponsibleName;
        settings.SecondaryResponsiblePhone = viewModel.SecondaryResponsiblePhone;
        await settings.UpdateDatabase();
        viewModel.Success = true;
    }
}

public class SettingsViewModel
{
    [Required(ErrorMessage = "Bitte angeben.")]
    public DateTime EventStartDate { get; init; }

    [Required(ErrorMessage = "Bitte angeben.")]
    public DateTime EventEndDate { get; init; }

    [Required(ErrorMessage = "Bitte angeben.")]
    public DateTime RegistrationStartDate { get; init; }

    [Required(ErrorMessage = "Bitte angeben.")]
    public DateTime RegistrationEndDate { get; init; }

    [Required(ErrorMessage = "Bitte angeben.")]
    public DateTime ArticlesDropOffDate { get; init; }

    [Required(ErrorMessage = "Bitte angeben.")]
    public int MaxNumberOfArticles { get; init; }

    [Required(ErrorMessage = "Bitte angeben.")]
    public string CoronaText { get; init; } = "";

    [Required(ErrorMessage = "Bitte angeben.")]
    public string AllowedArticleTypes { get; init; } = "";

    [Required(ErrorMessage = "Bitte angeben.")]
    public string SecondaryResponsibleName { get; init; } = "";

    [Required(ErrorMessage = "Bitte angeben.")]
    public string SecondaryResponsiblePhone { get; init; } = "";

    [Required(ErrorMessage = "Bitte angeben.")]
    public string PrimaryResponsibleEmail { get; init; } = "";

    [Required(ErrorMessage = "Bitte angeben.")]
    public string PrimaryResponsiblePhone { get; init; } = "";

    [Required(ErrorMessage = "Bitte angeben.")]
    public string PrimaryResponsibleName { get; init; } = "";


    [BindNever] public bool Success { get; set; }
}

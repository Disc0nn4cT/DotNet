using System.Globalization;
using YachtCatalog.Models;

namespace YachtCatalog;

public partial class MainPage : ContentPage
{
    private static readonly CultureInfo UkrainianCulture =
        CultureInfo.GetCultureInfo("uk-UA");

    // Дані товару й опції власної теми.
    private readonly Product _product = new()
    {
        Name = "Яхта «Альбатрос»",
        Description = "Простора вітрильна яхта для сімейних походів Чорним морем.",
        ImageName = "albatross.png",
        UnitPrice = 6500m,
        OptionName = "Шкіпер",
        OptionPrice = 1500m
    };

    // Події можуть виникнути ще під час InitializeComponent().
    private bool _isReady;

    public MainPage()
    {
        InitializeComponent();
        ProductNameLabel.Text = _product.Name;
        DescriptionLabel.Text = _product.Description;
        ProductImage.Source = _product.ImageName;
        PriceLabel.Text = $"{Money(_product.UnitPrice)} за добу";
        OptionLabel.Text = $"{_product.OptionName} (+{Money(_product.OptionPrice)}/доба)";
        _isReady = true;
        ClearFeedback();
    }

    private static string Money(decimal value) =>
        value.ToString("N2", UkrainianCulture) + " грн";

    private void OnCalculateClicked(object? sender, EventArgs e)
    {
        string input = QuantityEntry.Text?.Trim() ?? "";
        if (!int.TryParse(input, out int quantity) ||
            quantity < 1 || quantity > Product.MaxQuantity)
        {
            ErrorLabel.Text = $"Введіть ціле число діб від 1 до {Product.MaxQuantity}.";
            ErrorLabel.IsVisible = true;
            ResultLabel.Text = "Вартість не розраховано.";
            QuantityEntry.Focus();
            return;
        }

        decimal total = _product.CalculateTotal(quantity, OptionCheckBox.IsChecked);
        ErrorLabel.IsVisible = false;
        ResultLabel.Text = $"Разом: {Money(total)}";
    }

    private void OnResetClicked(object? sender, EventArgs e)
    {
        QuantityEntry.Text = "1";
        OptionCheckBox.IsChecked = false;
        ClearFeedback();
    }

    // Обидві події успадковують аргументи від EventArgs.
    private void OnInputsChanged(object? sender, EventArgs e)
    {
        if (_isReady)
            ClearFeedback();
    }

    private void ClearFeedback()
    {
        ErrorLabel.Text = "";
        ErrorLabel.IsVisible = false;
        ResultLabel.Text = "Вартість ще не розраховано.";
    }
}

using System.Collections.ObjectModel;
using YachtCatalog.Infrastructure;
using YachtCatalog.Models;

namespace YachtCatalog.ViewModels;

public sealed class CatalogViewModel : ObservableObject
{
    public ObservableCollection<Product> Products { get; } = new();

    public DelegateCommand CalculateCommand { get; }
    public DelegateCommand ResetCommand { get; }
    public DelegateCommand AddDemoProductCommand { get; }
    public DelegateCommand RemoveSelectedCommand { get; }

    private Product? _selectedProduct;
    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (!SetProperty(ref _selectedProduct, value))
                return;

            OnPropertyChanged(nameof(HasSelection));
            OnPropertyChanged(nameof(SelectedName));
            OnPropertyChanged(nameof(SelectedDescription));
            OnPropertyChanged(nameof(SelectedImageName));
            OnPropertyChanged(nameof(SelectedImageDescription));
            OnPropertyChanged(nameof(SelectedPriceText));
            OnPropertyChanged(nameof(SelectedOptionText));
            Reset();
            RefreshCommands();
        }
    }

    private string _quantityText = "1";
    public string QuantityText
    {
        get => _quantityText;
        set
        {
            if (SetProperty(ref _quantityText, value ?? ""))
                ClearFeedback();
        }
    }

    private bool _includeOption;
    public bool IncludeOption
    {
        get => _includeOption;
        set
        {
            if (SetProperty(ref _includeOption, value))
                ClearFeedback();
        }
    }

    private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    private string _resultText = "Вартість ще не розраховано.";
    public string ResultText
    {
        get => _resultText;
        private set => SetProperty(ref _resultText, value);
    }

    public bool HasSelection => SelectedProduct is not null;
    public string SelectedName => SelectedProduct?.Name ?? "Виберіть яхту в каталозі";
    public string SelectedDescription => SelectedProduct?.Description ?? "Додайте яхту, якщо список порожній.";
    public string SelectedImageName => SelectedProduct?.ImageName ?? "";
    public string SelectedImageDescription => SelectedProduct?.ImageDescription ?? "";
    public string SelectedPriceText => SelectedProduct?.PriceText ?? "";
    public string SelectedOptionText => SelectedProduct?.OptionText ?? "Додаткова опція";
    public string QuantityHint => $"Кількість діб (1–{Product.MaxQuantity})";
    public string ProductCountText => $"Яхт у каталозі: {Products.Count}";

    public CatalogViewModel()
    {
        CalculateCommand = new DelegateCommand(Calculate, () => HasSelection);
        ResetCommand = new DelegateCommand(Reset, () => HasSelection);
        RemoveSelectedCommand = new DelegateCommand(RemoveSelected, () => HasSelection);
        AddDemoProductCommand = new DelegateCommand(AddDemoProduct,
            () => Products.All(product => product.Id != 4));

        Products.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(ProductCountText));
            RefreshCommands();
        };

        Products.Add(new Product
        {
            Id = 1, Name = "Яхта «Альбатрос»",
            Description = "Простора вітрильна яхта для сімейних походів Чорним морем.",
            ImageName = "albatross.png", ImageDescription = "Вітрильна яхта з білим і жовтим вітрилами",
            UnitPrice = 6500m, OptionName = "Шкіпер", OptionPrice = 1500m
        });
        Products.Add(new Product
        {
            Id = 2, Name = "Яхта «Бриз»",
            Description = "Компактна вітрильна яхта для першого чартеру.",
            ImageName = "breeze.png", ImageDescription = "Невелика біла вітрильна яхта",
            UnitPrice = 4500m, OptionName = "Шкіпер", OptionPrice = 1500m
        });
        Products.Add(new Product
        {
            Id = 3, Name = "Катамаран «Дельфін»",
            Description = "Стійкий катамаран із великим салоном для компанії.",
            ImageName = "dolphin.png", ImageDescription = "Білий катамаран із двома корпусами",
            UnitPrice = 9000m, OptionName = "Шкіпер і кок", OptionPrice = 2500m
        });
        SelectedProduct = Products[0];
    }

    private void Calculate()
    {
        Product? product = SelectedProduct;
        if (product is null)
            return;

        if (!int.TryParse(QuantityText.Trim(), out int quantity) ||
            quantity < 1 || quantity > Product.MaxQuantity)
        {
            ErrorMessage = $"Введіть ціле число діб від 1 до {Product.MaxQuantity}.";
            ResultText = "Вартість не розраховано.";
            return;
        }

        ErrorMessage = "";
        ResultText = $"Разом: {Product.Money(product.CalculateTotal(quantity, IncludeOption))}";
    }

    private void Reset()
    {
        QuantityText = "1";
        IncludeOption = false;
        ClearFeedback();
    }

    private void ClearFeedback()
    {
        ErrorMessage = "";
        ResultText = "Вартість ще не розраховано.";
    }

    private void AddDemoProduct()
    {
        var product = new Product
        {
            Id = 4, Name = "Моторна яхта «Хвиля»",
            Description = "Швидка моторна яхта для коротких прогулянок узбережжям.",
            ImageName = "wave.png", ImageDescription = "Біла моторна яхта з темними вікнами",
            UnitPrice = 11000m, OptionName = "Капітан", OptionPrice = 2000m
        };
        Products.Add(product);
        SelectedProduct = product;
    }

    private void RemoveSelected()
    {
        Product? product = SelectedProduct;
        if (product is null)
            return;

        int index = Products.IndexOf(product);
        SelectedProduct = null;
        Products.Remove(product);
        if (Products.Count > 0)
            SelectedProduct = Products[Math.Min(index, Products.Count - 1)];
    }

    private void RefreshCommands()
    {
        CalculateCommand.NotifyCanExecuteChanged();
        ResetCommand.NotifyCanExecuteChanged();
        RemoveSelectedCommand.NotifyCanExecuteChanged();
        AddDemoProductCommand.NotifyCanExecuteChanged();
    }
}

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.Controls.Sample.Issues
{
    [Issue(IssueTracker.Github, 24369, "Picker's SelectedItem in CollectionView DataTemplate not working", PlatformAffected.All)]
    public class Issue24369 : ContentPage
    {
        public Issue24369()
        {
            BindingContext = new Issue24369ViewModel();
            var label = new Label
            {
                Text = "This test passes if the the Picker's SelectedItem, within a CollectionView DataTemplate is visible.",
                AutomationId = "Issue24369Label"
            };

            Content = new StackLayout
            {
                Spacing = 25,
                Margin = new Thickness(10),
                Children =
                {
                    label,
                    new Label { Text = "Single Picker", TextColor = Colors.Red},
                    CreateSinglePicker(),
                    new Label { Text = "Picker in CollectionView", TextColor = Colors.Red },
                    CreateCollectionViewWithPickers(),
                    new Label { Text = "Force refresh picker value", TextColor = Colors.Red },
                    CreateRefreshButton()
                }
            };
        }

        private Picker CreateSinglePicker()
        {
            var picker = new Picker();
            picker.SetBinding(Picker.ItemsSourceProperty, nameof(Issue24369ViewModel.Numbers));
            picker.SetBinding(Picker.SelectedItemProperty, nameof(Issue24369ViewModel.SelectedNumber));
            return picker;
        }

        private CollectionView CreateCollectionViewWithPickers()
        {
            var collectionView = new CollectionView
            {
                AutomationId = "ProductsCollectionView",
                ItemTemplate = new DataTemplate(() =>
                {
                    var stackLayout = new VerticalStackLayout();

                    var nameLabel = new Label();
                    nameLabel.SetBinding(Label.TextProperty, nameof(Product.Name));
                    stackLayout.Children.Add(nameLabel);

                    var picker = new Picker();
                    picker.SetBinding(Picker.ItemsSourceProperty,
                        new Binding("BindingContext.Numbers",
                        source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestor, typeof(Issue24369))));
                    picker.SetBinding(Picker.SelectedItemProperty, nameof(Product.Quantity));

                    stackLayout.Children.Add(picker);

                    return stackLayout;
                })
            };

            collectionView.SetBinding(CollectionView.ItemsSourceProperty, nameof(Issue24369ViewModel.Products));
            return collectionView;
        }

        private Button CreateRefreshButton()
        {
            var button = new Button
            {
                Text = "Refresh",
            };
            button.SetBinding(Button.CommandProperty, nameof(Issue24369ViewModel.ButtonCommand));
            return button;
        }
    }

    public class Issue24369ViewModel : INotifyPropertyChanged
    {
        public Issue24369ViewModel()
        {
            ButtonCommand = new Command(OnButtonClicked);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private int _selectedNumber = 5;
        public int SelectedNumber
        {
            get => _selectedNumber;
            set
            {
                _selectedNumber = value;
                OnPropertyChanged();
            }
        }

        public List<int> Numbers { get; set; } = Enumerable.Range(1, 8).ToList();

        private ObservableCollection<Product> _products = new ObservableCollection<Product>
        {
            new Product { Name = "Product 1", Quantity = 2 },
            new Product { Name = "Product 2", Quantity = 5 }
        };

        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
            }
        }

        public Command ButtonCommand { get; }

        private void OnButtonClicked()
        {
            foreach (var product in Products)
            {
                product.Quantity = Random.Shared.Next(1, 8);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Product : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

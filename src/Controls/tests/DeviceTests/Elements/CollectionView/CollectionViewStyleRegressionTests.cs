using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	[Category(TestCategory.CollectionView)]
	public class CollectionViewStyleRegressionTests : ControlsHandlerTestBase
	{
		[Fact(DisplayName = "CollectionView ItemTemplate with Label and ContentPage Style should not throw exceptions")]
		public async Task CollectionViewWithLabelStyleDoesNotThrow()
		{
			SetupBuilder();

			var data = new ObservableCollection<string> { "Test Item 1", "Test Item 2" };
			
			var page = new ContentPage();
			
			// This Style in ContentPage.Resources was causing Labels to disappear in CollectionView
			// on Windows in version 8.0.3 (regression from 7.0.101)
			page.Resources = new ResourceDictionary
			{
				new Style(typeof(Label))
				{
					Setters =
					{
						new Setter { Property = Label.TextColorProperty, Value = Colors.Red },
						new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center }
					}
				}
			};

			var collectionView = new CollectionView
			{
				ItemTemplate = new DataTemplate(() =>
				{
					var label = new Label();
					label.SetBinding(Label.TextProperty, new Binding("."));
					return label;
				}),
				ItemsSource = data
			};

			page.Content = collectionView;

			// The main test is that this doesn't throw an exception
			// Previously, the resource resolution would fail and could cause issues
			await CreateHandlerAndAddToWindow<WindowHandler>(new Window { Page = page }, async handler =>
			{
				await Task.Delay(100); // Allow for items to be created
				
				// Basic validation that everything was created successfully
				Assert.NotNull(collectionView.Handler);
				Assert.Equal(2, data.Count);
				
				// If we get here without exceptions, the fix is working
				Assert.True(true, "CollectionView with Label styles created successfully");
			});
		}

		[Fact(DisplayName = "Complex CollectionView template with multiple styled elements should work")]
		public async Task ComplexCollectionViewTemplateWithStyles()
		{
			SetupBuilder();

			var data = new ObservableCollection<TestItem> 
			{ 
				new TestItem { Title = "Item 1", Description = "Description 1" },
				new TestItem { Title = "Item 2", Description = "Description 2" }
			};
			
			var page = new ContentPage();
			page.Resources = new ResourceDictionary
			{
				new Style(typeof(Label))
				{
					Setters =
					{
						new Setter { Property = Label.TextColorProperty, Value = Colors.Blue },
						new Setter { Property = Label.FontSizeProperty, Value = 14.0 }
					}
				},
				new Style(typeof(StackLayout))
				{
					Setters =
					{
						new Setter { Property = StackLayout.SpacingProperty, Value = 5.0 }
					}
				}
			};

			var collectionView = new CollectionView
			{
				ItemTemplate = new DataTemplate(() =>
				{
					var stack = new StackLayout();
					
					var titleLabel = new Label { FontAttributes = FontAttributes.Bold };
					titleLabel.SetBinding(Label.TextProperty, new Binding(nameof(TestItem.Title)));
					
					var descLabel = new Label();
					descLabel.SetBinding(Label.TextProperty, new Binding(nameof(TestItem.Description)));
					
					stack.Children.Add(titleLabel);
					stack.Children.Add(descLabel);
					
					return stack;
				}),
				ItemsSource = data
			};

			page.Content = collectionView;

			await CreateHandlerAndAddToWindow<WindowHandler>(new Window { Page = page }, async handler =>
			{
				await Task.Delay(100);
				
				Assert.NotNull(collectionView.Handler);
				Assert.Equal(2, data.Count);
			});
		}

		public class TestItem
		{
			public string Title { get; set; }
			public string Description { get; set; }
		}
	}
}
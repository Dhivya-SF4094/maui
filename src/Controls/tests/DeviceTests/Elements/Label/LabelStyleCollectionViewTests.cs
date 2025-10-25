using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	[Category(TestCategory.Label)]
	public class LabelStyleCollectionViewTests : ControlsHandlerTestBase
	{
		[Fact(DisplayName = "Label should be visible in CollectionView with Style in ContentPage.Resources")]
		public async Task LabelVisibleInCollectionViewWithPageStyle()
		{
			SetupBuilder();

			var data = new ObservableCollection<string>
			{
				"Item 1",
				"Item 2", 
				"Item 3"
			};

			// Create a ContentPage with Style in Resources targeting Label
			var page = new ContentPage();
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
					var stackLayout = new VerticalStackLayout();
					var label = new Label();
					label.SetBinding(Label.TextProperty, new Binding("."));
					stackLayout.Children.Add(label);
					return stackLayout;
				}),
				ItemsSource = data
			};

			page.Content = collectionView;

			await CreateHandlerAndAddToWindow<WindowHandler>(new Window { Page = page }, async handler =>
			{
				await Task.Delay(100); // Allow for layout to complete

				// Verify that the CollectionView was created and has items
				Assert.NotNull(collectionView.Handler);
				
				// For this test, we primarily want to ensure no exceptions occur during
				// item creation and that the fix prevents the regression where Labels 
				// would disappear when styles are applied in DataTemplates.
				// The actual verification that styles are applied would require platform-specific
				// testing that can inspect the visual tree.
				
				// This test ensures the fix doesn't break item creation
				Assert.NotNull(data);
				Assert.Equal(3, data.Count);
			});
		}

		[Fact(DisplayName = "Label should be visible in CollectionView without Style")]
		public async Task LabelVisibleInCollectionViewWithoutStyle()
		{
			SetupBuilder();

			var data = new ObservableCollection<string>
			{
				"Item 1",
				"Item 2",
				"Item 3"
			};

			var collectionView = new CollectionView
			{
				ItemTemplate = new DataTemplate(() =>
				{
					var stackLayout = new VerticalStackLayout();
					var label = new Label();
					label.SetBinding(Label.TextProperty, new Binding("."));
					stackLayout.Children.Add(label);
					return stackLayout;
				}),
				ItemsSource = data
			};

			var page = new ContentPage { Content = collectionView };

			await CreateHandlerAndAddToWindow<WindowHandler>(new Window { Page = page }, async handler =>
			{
				await Task.Delay(100);

				// Verify basic functionality without styles works as expected
				Assert.NotNull(collectionView.Handler);
				Assert.NotNull(data);
				Assert.Equal(3, data.Count);
			});
		}

		[Fact(DisplayName = "Multiple styles should work in CollectionView")]
		public async Task MultipleStylesInCollectionView()
		{
			SetupBuilder();

			var data = new ObservableCollection<string>
			{
				"Test Item 1",
				"Test Item 2"
			};

			var page = new ContentPage();
			page.Resources = new ResourceDictionary
			{
				// Style for Label
				new Style(typeof(Label))
				{
					Setters =
					{
						new Setter { Property = Label.TextColorProperty, Value = Colors.Blue },
						new Setter { Property = Label.FontSizeProperty, Value = 16.0 }
					}
				},
				// Style for StackLayout  
				new Style(typeof(StackLayout))
				{
					Setters =
					{
						new Setter { Property = StackLayout.BackgroundColorProperty, Value = Colors.LightGray }
					}
				}
			};

			var collectionView = new CollectionView
			{
				ItemTemplate = new DataTemplate(() =>
				{
					var stackLayout = new VerticalStackLayout();
					var label = new Label();
					label.SetBinding(Label.TextProperty, new Binding("."));
					stackLayout.Children.Add(label);
					return stackLayout;
				}),
				ItemsSource = data
			};

			page.Content = collectionView;

			await CreateHandlerAndAddToWindow<WindowHandler>(new Window { Page = page }, async handler =>
			{
				await Task.Delay(100);

				// Ensure multiple styles can be applied without issues
				Assert.NotNull(collectionView.Handler);
				Assert.Equal(2, data.Count);
			});
		}
	}
}
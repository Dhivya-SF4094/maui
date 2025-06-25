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

			// Create a ContentPage with Style in Resources
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
				await Task.Delay(100);

				// Get the first item's label
				var firstItemLabel = GetFirstLabelFromCollectionView(collectionView);
				
				// Verify the label exists and is visible
				Assert.NotNull(firstItemLabel);
				Assert.True(firstItemLabel.IsVisible);
				Assert.Equal("Item 1", firstItemLabel.Text);
				
				// Verify style was applied
				Assert.Equal(Colors.Red, firstItemLabel.TextColor);
				Assert.Equal(TextAlignment.Center, firstItemLabel.HorizontalTextAlignment);
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

				// Get the first item's label
				var firstItemLabel = GetFirstLabelFromCollectionView(collectionView);

				// Verify the label exists and is visible
				Assert.NotNull(firstItemLabel);
				Assert.True(firstItemLabel.IsVisible);
				Assert.Equal("Item 1", firstItemLabel.Text);
			});
		}

		private Label GetFirstLabelFromCollectionView(CollectionView collectionView)
		{
			// Try to find the first label in the collection view's visual tree
			var children = collectionView.LogicalChildrenInternal;
			if (children.Count > 0)
			{
				// Look for labels in the first item
				return FindLabelInElement(children[0] as Element);
			}
			return null;
		}

		private Label FindLabelInElement(Element element)
		{
			if (element is Label label)
				return label;

			if (element is IViewContainer viewContainer)
			{
				foreach (var child in viewContainer.Children)
				{
					if (child is Element childElement)
					{
						var foundLabel = FindLabelInElement(childElement);
						if (foundLabel != null)
							return foundLabel;
					}
				}
			}

			foreach (var child in element?.LogicalChildrenInternal ?? new List<Element>())
			{
				var foundLabel = FindLabelInElement(child);
				if (foundLabel != null)
					return foundLabel;
			}

			return null;
		}
	}
}
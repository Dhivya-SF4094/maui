using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 7184, "Search Handler visual and functional bug in subtabs", PlatformAffected.Android)]
public class SearchHandlerSubtabsBug : TestShell
{
	protected override void Init()
	{
		CreateBottomTabWithSubtabs("Boys", new[] { "Group1", "Group2" });
		CreateBottomTabWithSubtabs("Girls", new[] { "Group1", "Group2" });
	}

	void CreateBottomTabWithSubtabs(string bottomTabTitle, string[] subTabTitles)
	{
		var bottomTab = new ShellItem()
		{
			Title = bottomTabTitle,
			AutomationId = $"BottomTab{bottomTabTitle}"
		};

		var shellSection = new ShellSection()
		{
			Title = bottomTabTitle,
			AutomationId = $"Section{bottomTabTitle}"
		};

		foreach (string subTabTitle in subTabTitles)
		{
			var page = new ContentPage()
			{
				Title = subTabTitle,
				AutomationId = $"Page{bottomTabTitle}{subTabTitle}",
				Content = new StackLayout()
				{
					new Label()
					{
						Text = $"Page: {bottomTabTitle} - {subTabTitle}",
						AutomationId = $"Label{bottomTabTitle}{subTabTitle}"
					},
					new Label()
					{
						Text = "Switch between tabs and observe search handlers stacking",
						AutomationId = "Instructions"
					}
				}
			};

			// Create a unique SearchHandler for each subtab page
			var searchHandler = new TestSearchHandler(bottomTabTitle, subTabTitle)
			{
				AutomationId = $"SearchHandler{bottomTabTitle}{subTabTitle}",
				Placeholder = $"Search {bottomTabTitle} {subTabTitle}",
				ShowsResults = true
			};

			Shell.SetSearchHandler(page, searchHandler);

			var shellContent = new ShellContent()
			{
				Title = subTabTitle,
				Content = page,
				AutomationId = $"ShellContent{bottomTabTitle}{subTabTitle}"
			};

			shellSection.Items.Add(shellContent);
		}

		bottomTab.Items.Add(shellSection);
		Items.Add(bottomTab);
	}

	public class TestSearchHandler : SearchHandler
	{
		private readonly string _bottomTab;
		private readonly string _subTab;
		
		public TestSearchHandler(string bottomTab, string subTab)
		{
			_bottomTab = bottomTab;
			_subTab = subTab;
			
			// Create different items for each search handler to verify functionality
			ItemsSource = new ObservableCollection<SearchResult>()
			{
				new SearchResult { Name = $"{bottomTab} {subTab} Result 1", Description = "Test result 1" },
				new SearchResult { Name = $"{bottomTab} {subTab} Result 2", Description = "Test result 2" },
				new SearchResult { Name = $"{bottomTab} {subTab} Result 3", Description = "Test result 3" }
			};
			
			ItemTemplate = new DataTemplate(() =>
			{
				var grid = new Grid()
				{
					ColumnDefinitions = { new ColumnDefinition(), new ColumnDefinition() }
				};
				
				var nameLabel = new Label();
				nameLabel.SetBinding(Label.TextProperty, nameof(SearchResult.Name));
				Grid.SetColumn(nameLabel, 0);
				
				var descLabel = new Label()
				{
					FontSize = 12,
					TextColor = Colors.Gray
				};
				descLabel.SetBinding(Label.TextProperty, nameof(SearchResult.Description));
				Grid.SetColumn(descLabel, 1);
				
				grid.Children.Add(nameLabel);
				grid.Children.Add(descLabel);
				
				return grid;
			});
		}
		
		protected override void OnQueryChanged(string oldValue, string newValue)
		{
			base.OnQueryChanged(oldValue, newValue);
			
			// This should search items specific to this tab
			var items = (ObservableCollection<SearchResult>)ItemsSource;
			
			if (string.IsNullOrWhiteSpace(newValue))
			{
				// Show all items when query is empty
				return;
			}
			
			// Filter items based on query - this demonstrates the functional bug
			// Each SearchHandler should search its own items, not the first tab's items
			var filteredItems = items.Where(item => 
				item.Name.ToLower().Contains(newValue.ToLower()) ||
				item.Description.ToLower().Contains(newValue.ToLower())
			).ToList();
			
			// For demonstration, we could update ItemsSource here
			// but the core issue is that the wrong SearchHandler is used
		}
	}
	
	public class SearchResult
	{
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
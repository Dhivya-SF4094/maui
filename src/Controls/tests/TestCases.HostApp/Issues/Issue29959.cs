using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 29959, "[Windows] ImageButton Aspect Property Not Working for Various ImageSource Types", PlatformAffected.UWP)]
public class Issue29959 : ContentPage
{
	readonly ImageButton _imageButton;

	public Issue29959()
	{
		_imageButton = new ImageButton
		{
			AutomationId = "MyImageButton",
			Source = "dotnet_bot.png",
			Aspect = Aspect.AspectFit,
			BorderWidth = 1,
			HeightRequest = 450,
			WidthRequest = 350,
			BorderColor = Colors.Black,
		};

		var aspectRow = new HorizontalStackLayout();
		aspectRow.Add(MakeAspectRadio("AspectFit", "ImageAspectFit", true));
		aspectRow.Add(MakeAspectRadio("AspectFill", "ImageAspectFill", false));
		aspectRow.Add(MakeAspectRadio("Fill", "ImageFill", false));
		aspectRow.Add(MakeAspectRadio("Center", "ImageCenter", false));

		var sourceRow = new HorizontalStackLayout();
		sourceRow.Add(MakeSourceRadio("File", "SourceTypeFile", true));
		sourceRow.Add(MakeSourceRadio("Uri", "SourceTypeUri", false));
		sourceRow.Add(MakeSourceRadio("Stream", "SourceTypeStream", false));
		sourceRow.Add(MakeSourceRadio("FontImage", "SourceTypeFontImage", false));

		Content = new VerticalStackLayout
		{
			Children = { aspectRow, sourceRow, _imageButton }
		};
	}

	RadioButton MakeAspectRadio(string content, string automationId, bool isChecked)
	{
		var rb = new RadioButton
		{
			Content = content,
			IsChecked = isChecked,
			GroupName = "AspectGroup",
			AutomationId = automationId,
		};
		rb.CheckedChanged += OnAspectChanged;
		return rb;
	}

	RadioButton MakeSourceRadio(string content, string automationId, bool isChecked)
	{
		var rb = new RadioButton
		{
			Content = content,
			IsChecked = isChecked,
			GroupName = "SourceTypeGroup",
			AutomationId = automationId,
		};
		rb.CheckedChanged += OnSourceTypeChanged;
		return rb;
	}

	void OnAspectChanged(object sender, CheckedChangedEventArgs e)
	{
		if (sender is not RadioButton rb || !rb.IsChecked)
			return;

		_imageButton.Aspect = rb.Content?.ToString() switch
		{
			"AspectFit" => Aspect.AspectFit,
			"AspectFill" => Aspect.AspectFill,
			"Fill" => Aspect.Fill,
			"Center" => Aspect.Center,
			_ => _imageButton.Aspect,
		};
	}

	void OnSourceTypeChanged(object sender, CheckedChangedEventArgs e)
	{
		if (sender is not RadioButton rb || !rb.IsChecked)
			return;

		_imageButton.Source = rb.Content?.ToString() switch
		{
			"File" => new FileImageSource { File = "dotnet_bot.png" },
			"Uri" => new UriImageSource
			{
				Uri = new Uri("https://aka.ms/campus.jpg"),
				CachingEnabled = true,
				CacheValidity = TimeSpan.MaxValue,
			},
			"Stream" => ImageSource.FromStream(() =>
				FileSystem.Current.OpenAppPackageFileAsync("dotnet_bot.png").Result is Stream s
					? s
					: Stream.Null),
			"FontImage" => new FontImageSource
			{
				Glyph = "\u2665",
				Color = Colors.Red,
				Size = 300,
			},
			_ => _imageButton.Source,
		};
	}
}

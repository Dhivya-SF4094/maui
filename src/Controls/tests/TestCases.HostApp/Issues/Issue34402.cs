using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 34402, "FlowDirection property not working on BoxView Control", PlatformAffected.All)]
public class Issue34402 : ContentPage
{
	public Issue34402()
	{
		var boxView = new BoxView
		{
			CornerRadius = new CornerRadius(30, 60, 10, 20),
			Color = Colors.CornflowerBlue,
			WidthRequest = 200,
			HeightRequest = 200,
			AutomationId = "MyBoxView"
		};

		var graphicsView = new GraphicsView
		{
			Drawable = new TriangleDrawable(),
			WidthRequest = 200,
			HeightRequest = 100,
			AutomationId = "MyGraphicsView"
		};

		var checkBox = new CheckBox
		{
			AutomationId = "RtlCheckBox"
		};

		checkBox.CheckedChanged += (s, e) =>
		{
			var direction = e.Value ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
			boxView.FlowDirection = direction;
			graphicsView.FlowDirection = direction;
		};

		Content = new VerticalStackLayout
		{
			Padding = new Thickness(20),
			Spacing = 20,
			Children =
			{
				new Label { Text = "Toggle RTL to see corners/shape mirror:" },
				checkBox,
				boxView,
				graphicsView
			}
		};
	}

	class TriangleDrawable : IDrawable
	{
		public void Draw(ICanvas canvas, RectF dirtyRect)
		{
			canvas.StrokeColor = Colors.Black;
			canvas.StrokeSize = 2;

			// Right angle triangle points
			float x1 = 20;
			float y1 = 180;

			float x2 = 180;
			float y2 = 180;

			float x3 = 20;
			float y3 = 20;

			PathF path = new PathF();
			path.MoveTo(x1, y1);
			path.LineTo(x2, y2);
			path.LineTo(x3, y3);
			path.Close();

			canvas.DrawPath(path);
		}
	}
}

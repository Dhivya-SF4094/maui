using Maui.Controls.Sample;
using MauiClient.Pages;
using MauiClient.Services;
using MauiClient.ViewModels;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

#if __ANDROID__ || __IOS__
		builder.UseMauiMaps();
#endif

		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("Dokdo-Regular.ttf", "Dokdo");
				fonts.AddFont("LobsterTwo-Regular.ttf", "Lobster Two");
				fonts.AddFont("LobsterTwo-Bold.ttf", "Lobster Two Bold");
				fonts.AddFont("LobsterTwo-Italic.ttf", "Lobster Two Italic");
				fonts.AddFont("LobsterTwo-BoldItalic.ttf", "Lobster Two BoldItalic");
				fonts.AddFont("ionicons.ttf", "Ionicons");
				fonts.AddFont("SegoeUI.ttf", "Segoe UI");
				fonts.AddFont("SegoeUI-Bold.ttf", "Segoe UI Bold");
				fonts.AddFont("SegoeUI-Italic.ttf", "Segoe UI Italic");
				fonts.AddFont("SegoeUI-Bold-Italic.ttf", "Segoe UI Bold Italic");
			});

		builder.Services.AddHttpClient<MonkeyService>();
		builder.Services.AddSingleton<MasterViewModel>();
		builder.Services.AddSingleton<MasterPage>();
		builder.Services.AddTransient<DetailViewModel>();
		builder.Services.AddTransient<DetailPage>();

		return builder.Build();
	}
}

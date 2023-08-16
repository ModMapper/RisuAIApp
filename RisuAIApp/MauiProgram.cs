using Microsoft.Extensions.Logging;

namespace RisuAIApp;

public static class MauiProgram
{
	public static RisuAIServer Server { get; } = new();

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
		Server.Start();

        return builder.Build();
	}
}

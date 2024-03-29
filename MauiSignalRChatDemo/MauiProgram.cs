using CommunityToolkit.Maui;
using MauiSignalRChatDemo.Pages;
using MauiSignalRChatDemo.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace MauiSignalRChatDemo;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		Preferences.Set("arg0", 2);
       // Preferences.Set("arg0", 1);
     //   Preferences.Set("arg0", 2);
        // Preferences.Set("arg0", 1);
        // Preferences.Set("arg0", 2);
        //Preferences.Set("Url", "");
        //Preferences.Set("Url", "");

        //var config = new ConfigurationBuilder()
        //       .AddJsonPlatformBundle("appsettings.json", false)
        //       .AddPlatformPreferences()
        //       .Build();

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<EqutiesList>();
        builder.Services.AddSingleton<EquitiesViewModel>();


     //   using var stream = Assembly.GetExecutingAssembly()
     //.GetManifestResourceStream("YourAppName.appsettings.json");
     //   var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
     //   builder.Configuration.AddConfiguration(config);




#if DEBUG
       // builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

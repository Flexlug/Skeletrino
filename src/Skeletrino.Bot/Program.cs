
using System.Reflection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Skeletrino.Bot;
using Skeletrino.Bot.Settings;

DateTime StartTime = DateTime.Now;
DateTime? LastFailure = null;
int Failures = 0;
string BuildString = string.Empty;

var settingsService = new SettingsLoader();

Log.Logger = new LoggerConfiguration()
    //.WriteTo.Console(new ExpressionTemplate ("{@t:HH:mm:ss} [{@l:u3}] [{Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}] {@m}\n{@x}"),
    //                 theme: AnsiConsoleTheme.Literate)
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{SourceContext}] {Message}{NewLine}{Exception}",
        theme: SystemConsoleTheme.Colored)
    .WriteTo.File($"logs/log-{DateTime.Now.Ticks}-", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .CreateLogger();

BuildString = $"{Assembly.GetEntryAssembly().GetName().Version} {File.GetCreationTime(Assembly.GetCallingAssembly().Location)} .NET {System.Environment.Version}";
Log.Logger.Information($"WAV-Bot-DSharp: {Assembly.GetEntryAssembly().GetName().Version} (builded {File.GetCreationTime(Assembly.GetCallingAssembly().Location)}");

while (true)
{
    try
    {
        using (var bot = new Bot(settingsService.LoadFromFile()))
            await bot.RunAsync();
    }
    catch(Exception ex)
    {
        Log.Logger.Fatal(ex, "Bot failed");
        LastFailure = DateTime.Now;
        Failures++;
    }
}
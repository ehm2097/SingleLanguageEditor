using Microsoft.Extensions.Configuration;

namespace Avat.SingleLanguageEditor;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddCommandLine(args)
            .AddJsonFile("profile.json")
            .Build();

        // Console.WriteLine(configuration["TestKey"]);

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.EnableVisualStyles();
        Application.Run(new Explorer(configuration));
    }    
}
namespace DeleteLogFiles.Configurator;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new ConfigForm(args.FirstOrDefault()));
    }
}

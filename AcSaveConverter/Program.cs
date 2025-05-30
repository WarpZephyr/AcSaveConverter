using AcSaveConverter.Graphics;
using AcSaveConverter.GUI.Windows;
using AcSaveConverter.Logging;

namespace AcSaveConverter
{
    internal class Program
    {
        internal const string AppName = "AcSaveConverter";
        internal static readonly AppInfo AppInfo;
        internal static readonly string AppDataFolder;

        static Program()
        {
            AppInfo = new AppInfo();
            AppDataFolder = AppInfo.AppDirectory;
        }

        static void Main(string[] args)
        {
            Log.DirectWriteLine($"Starting {AppName}.");

            Log.DirectWriteLine("Starting graphics.");
            using var graphics = new ImGuiGraphicsContext(100, 100, 800, 500, "AcSaveConverter");

            Log.DirectWriteLine("Starting converter.");
            var converter = new ConverterWindow(graphics, graphics.Window);

            Log.DirectWriteLine($"Running {AppName}.");
            graphics.Run();

            Log.DirectWriteLine($"Closing {AppName}.");
            Dispose();
        }

        static void Dispose()
        {
            AppLog.Instance.Dispose();
        }
    }
}

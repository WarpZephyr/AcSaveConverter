using AcSaveConverter.Logging;

namespace AcSaveConverter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Log.DirectWriteLine("Initializing app...");
            using var app = new App();

            Log.DirectWriteLine("Starting app run.");
            app.Run();

            Log.DirectWriteLine("Saving app config...");
            AppConfig.Current.Save();

            Log.Flush();
            Log.DirectWriteLine("Finished app run.");
            Log.Dispose();
        }
    }
}

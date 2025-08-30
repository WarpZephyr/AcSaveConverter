using AcSaveConverter.Logging;
using System;
using System.Diagnostics;
using System.IO;

namespace AcSaveConverter.Interface
{
    internal static class Explorer
    {
        public static void OpenFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Log.WriteLine($"Folder to open in explorer doesn't exist: \"{folder}\"");
                return;
            }

            try
            {
                var startInfo = new ProcessStartInfo(folder)
                {
                    UseShellExecute = true,
                    Verb = "explore"
                };

                using var process = Process.Start(startInfo);
                Log.WriteLine($"Opened folder in explorer: \"{folder}\"");
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Failed to open folder \"{folder}\" in explorer: {ex}");
            }
        }
    }
}

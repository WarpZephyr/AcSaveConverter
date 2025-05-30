using AcSaveConverter.Logging;
using NativeFileDialogSharp;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace AcSaveConverter.IO
{
    public static class FileDialog
    {
        public static string? OpenFile(string? filterList = null, string? defaultPath = null)
        {
            var result = Dialog.FileOpen(filterList, defaultPath);
            if (result != null && result.IsOk)
            {
                Log.WriteLine($"Opening file: \"{result.Path}\"");
                return result.Path;
            }

            Log.WriteLine("Canceled or failed opening file.");
            return null;
        }

        public static string[] OpenFiles(string? filterList = null, string? defaultPath = null)
        {
            var result = Dialog.FileOpenMultiple(filterList, defaultPath);
            if (result != null && result.IsOk)
            {
                Log.WriteLine($"Opening files: [{string.Join(',', result.Paths)}]");
                return [.. result.Paths];
            }

            Log.WriteLine("Canceled or failed opening files.");
            return [];
        }

        public static string? OpenFolder(string? defaultPath = null)
        {
            var result = Dialog.FolderPicker(defaultPath);
            if (result != null && result.IsOk)
            {
                Log.WriteLine($"Opening folder: \"{result.Path}\"");
                return result.Path;
            }

            Log.WriteLine("Canceled or failed opening folder.");
            return null;
        }

        public static string? GetSaveFilePath(string? filterList = null, string? defaultPath = null)
        {
            var result = Dialog.FileSave(filterList, defaultPath);
            if (result != null && result.IsOk)
            {
                Log.WriteLine($"Getting save path: \"{result.Path}\"");
                return result.Path;
            }

            Log.WriteLine("Canceled or failed getting save path.");
            return null;
        }

        public static bool ValidFile([NotNullWhen(true)] string? file)
        {
            Log.WriteLine($"Validating file path: \"{file}\"");
            return !string.IsNullOrWhiteSpace(file) && File.Exists(file);
        }

        public static bool ValidSavePath([NotNullWhen(true)] string? file)
        {
            Log.WriteLine($"Validating save path: \"{file}\"");
            return !string.IsNullOrWhiteSpace(file);
        }

        public static bool ValidFolder([NotNullWhen(true)] string? folder)
        {
            Log.WriteLine($"Validating folder path: \"{folder}\"");
            return !string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder);
        }
    }
}

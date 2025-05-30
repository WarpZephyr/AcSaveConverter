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
                return result.Path;
            }

            return null;
        }

        public static string[] OpenFiles(string? filterList = null, string? defaultPath = null)
        {
            var result = Dialog.FileOpenMultiple(filterList, defaultPath);
            if (result != null && result.IsOk)
            {
                return [.. result.Paths];
            }

            return [];
        }

        public static string? OpenFolder(string? defaultPath = null)
        {
            var result = Dialog.FolderPicker(defaultPath);
            if (result != null && result.IsOk)
            {
                return result.Path;
            }

            return null;
        }

        public static string? GetSaveFilePath(string? filterList = null, string? defaultPath = null)
        {
            var result = Dialog.FileSave(filterList, defaultPath);
            if (result != null && result.IsOk)
            {
                return result.Path;
            }

            return null;
        }

        public static bool ValidFile([NotNullWhen(true)] string? file)
        {
            return !string.IsNullOrWhiteSpace(file) && File.Exists(file);
        }

        public static bool ValidSavePath([NotNullWhen(true)] string? file)
        {
            return !string.IsNullOrWhiteSpace(file);
        }

        public static bool ValidFolder([NotNullWhen(true)] string? folder)
        {
            return !string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder);
        }
    }
}

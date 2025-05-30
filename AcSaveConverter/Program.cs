using AcSaveConverter.Graphics;
using AcSaveConverter.GUI.Windows;
using System;

namespace AcSaveConverter
{
    internal class Program
    {
        internal static readonly string AppFolder = AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        { 
            using var graphics = new ImGuiGraphicsContext(100, 100, 800, 500, "AcSaveConverter");
            var converter = new ConverterWindow(graphics, graphics.Window);
            graphics.Run();
        }
    }
}

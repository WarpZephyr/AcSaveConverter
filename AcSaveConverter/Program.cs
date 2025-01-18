using AcSaveConverterImGui.Graphics;
using AcSaveConverterImGui.GUI.Windows;
using System;

namespace AcSaveConverterImGui
{
    internal class Program
    {
        internal static readonly string AppFolder = AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        { 
            var graphics = new ImGuiGraphicsContext(100, 100, 800, 500, "AcSaveConverter");
            var converter = new ConverterWindow(graphics, graphics.Window);
            graphics.Run();
        }
    }
}

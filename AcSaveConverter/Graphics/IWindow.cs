using System.Numerics;

namespace AcSaveConverterImGui.Graphics
{
    public interface IWindow
    {
        public string Title { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Position { get; set; }
        public bool Exists { get; }
        public float GetDisplayDPI();
    }
}

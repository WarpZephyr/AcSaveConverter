using AcSaveConverterImGui.Native;
using System.Numerics;
using System.Runtime.CompilerServices;
using Veldrid;
using Veldrid.Sdl2;

namespace AcSaveConverterImGui.Graphics
{
    public class Window : IWindow
    {
        private readonly Sdl2Window InternalWindow;

        public string Title
        {
            get => InternalWindow.Title;
            set => InternalWindow.Title = value;
        }

        public int X
        {
            get => InternalWindow.X;
            set => InternalWindow.X = value;
        }

        public int Y
        {
            get => InternalWindow.Y;
            set => InternalWindow.Y = value;
        }

        public int Width
        {
            get => InternalWindow.Width;
            set => InternalWindow.Width = value;
        }

        public int Height
        {
            get => InternalWindow.Height;
            set => InternalWindow.Height = value;
        }

        public Vector2 Position
        {
            get => new Vector2(X, Y);
            set
            {
                X = (int)value.X;
                Y = (int)value.Y;
            }
        }

        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set
            {
                Width = (int)value.X;
                Height = (int)value.Y;
            }
        }

        public bool Exists
            => InternalWindow.Exists;

        internal RgbaFloat BgColorInternal;
        public Vector4 BackgroundColor
        {
            get => new Vector4(BgColorInternal.R, BgColorInternal.G, BgColorInternal.B, BgColorInternal.A);
            set => BgColorInternal = new RgbaFloat(value);
        }

        internal Window(Sdl2Window sdl2Window)
        {
            InternalWindow = sdl2Window;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InputSnapshot PumpEvents()
            => InternalWindow.PumpEvents();

        public float GetDisplayDPI()
        {
            float ddpi = 96f;
            float _ = 0f;
            Sdl2Ex.GetDisplayDPI(InternalWindow, ref ddpi, ref _, ref _);
            return ddpi;
        }
    }
}

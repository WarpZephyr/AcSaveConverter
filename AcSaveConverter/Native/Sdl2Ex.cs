using Veldrid.Sdl2;

namespace AcSaveConverter.Native
{
    public static class Sdl2Ex
    {
        public static int GetDisplayDPI(int displayIndex, ref float ddpi, ref float hdpi, ref float vdpi)
        {
            unsafe
            {
                fixed (float* ddpiPtr = &ddpi)
                fixed (float* hdpiPtr = &hdpi)
                fixed (float* vdpiPtr = &vdpi)
                {
                    return Sdl2NativeEx.SDL_GetDisplayDPI(displayIndex, ddpiPtr, hdpiPtr, vdpiPtr);
                }
            }
        }

        public static int GetDisplayIndex(Sdl2Window window)
        {
            return Sdl2Native.SDL_GetWindowDisplayIndex(window.SdlWindowHandle);
        }

        public static int GetDisplayDPI(Sdl2Window window, ref float ddpi, ref float hdpi, ref float vdpi)
        {
            return GetDisplayDPI(GetDisplayIndex(window), ref ddpi, ref hdpi, ref vdpi);
        }
    }
}

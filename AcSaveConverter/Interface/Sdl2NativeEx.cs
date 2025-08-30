using System.Diagnostics;
using System.Runtime.InteropServices;
using NativeLibrary = NativeLibraryLoader.NativeLibrary;

namespace AcSaveConverter.Interface
{
    public static unsafe class Sdl2NativeEx
    {
        private static readonly NativeLibrary s_sdl2Lib = LoadSdl2();
        private static NativeLibrary LoadSdl2()
        {
            string name;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                name = "SDL2.dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                name = "libSDL2-2.0.so";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                name = "libsdl2.dylib";
            }
            else
            {
                Debug.WriteLine("Unknown SDL platform. Attempting to load \"SDL2\"");
                name = "SDL2";
            }

            NativeLibrary lib = new NativeLibrary(name);
            return lib;
        }

        private static T LoadFunction<T>(string name)
        {
            return s_sdl2Lib.LoadFunction<T>(name);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SDL_GetDisplayDPI_t(int displayIndex, float* ddpi, float* hdpi, float* vdpi);
        private static readonly SDL_GetDisplayDPI_t s_sdl_getWindowDisplayDPI = LoadFunction<SDL_GetDisplayDPI_t>("SDL_GetDisplayDPI");
        public static int SDL_GetDisplayDPI(int displayIndex, float* ddpi, float* hdpi, float* vdpi) => s_sdl_getWindowDisplayDPI(displayIndex, ddpi, hdpi, vdpi);
    }
}

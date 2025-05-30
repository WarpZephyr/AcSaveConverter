using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AcSaveConverter
{
    internal class AppInfo
    {
#if DEBUG
        public const bool IsDebug = true;
#else
        public const bool IsDebug = false;
#endif

        public readonly string Platform;
        public readonly string Version;
        public readonly string AppDirectory;

        public AppInfo()
        {
            Version = GetVersion();
            Platform = GetPlatform();
            AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        private static string GetVersion()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            object[] attributes = executingAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            if (attributes.Length > 0)
            {
                return ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion;
            }
            else
            {
                return "0.0.0.0";
            }
        }

        private static string GetPlatform()
        {
            string platform;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                platform = "Linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                platform = "FreeBSD";
            }
            else
            {
                return Environment.OSVersion.ToString();
            }

            var osVersion = Environment.OSVersion;
            string servicePack = osVersion.ServicePack;
            return string.IsNullOrEmpty(servicePack) ?
               $"{platform} {osVersion.Version}" :
               $"{platform} {osVersion.Version.ToString(3)} {servicePack}";
        }
    }
}

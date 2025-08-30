using AcSaveConverter.Configuration;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace AcSaveConverter
{
    [JsonSourceGenerationOptions(WriteIndented = true,
        GenerationMode = JsonSourceGenerationMode.Metadata,
        IncludeFields = true,
        UseStringEnumConverter = true)]
    [JsonSerializable(typeof(AppConfig))]
    internal partial class AppConfigSerializerContext : JsonSerializerContext
    {
    }

    internal class AppConfig : IConfig
    {
        #region Helper Properties

        [JsonIgnore]
        private const string FileName = "appconfig.json";

        [JsonIgnore]
        private const int CurrentConfigVersion = 2;

        [JsonIgnore]
        internal static readonly AppConfig Current;

        #endregion

        #region Settings

        public int ConfigVersion;
        public int WindowX;
        public int WindowY;
        public int WindowWidth;
        public int WindowHeight;
        public PlatformType Platform;
        public RegionType Region;
        public EncodingType Encoding;
        public bool AutoDetectPlatform;
        public bool AutoDetectRegion;
        public bool AutoDetectEncoding;

        #endregion

        #region Settings Helpers

        [JsonIgnore]
        internal bool IsXbox360
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Platform == PlatformType.Xbox360;
            set
            {
                if (value)
                {
                    Platform = PlatformType.Xbox360;
                }
                else if (Platform == PlatformType.Xbox360)
                {
                    Platform = PlatformType.PlayStation3;
                }
            }
        }

        [JsonIgnore]
        internal bool IsUtf16
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Encoding == EncodingType.Utf16;
            set
            {
                if (value)
                {
                    Encoding = EncodingType.Utf16;
                }
                else if (Encoding == EncodingType.Utf16)
                {
                    Encoding = EncodingType.ShiftJIS;
                }
            }
        }

        #endregion

        #region Static Constructor

        static AppConfig()
        {
            Current = Load();
        }

        #endregion

        public AppConfig()
        {
            ConfigVersion = CurrentConfigVersion;
            WindowX = 80;
            WindowY = 80;
            WindowWidth = 800;
            WindowHeight = 500;
            Platform = PlatformType.PlayStation3;
            Region = RegionType.US;
            Encoding = EncodingType.Utf16;
        }

        #region IO

        public static AppConfig Load()
            => ConfigLoader.Load(FileName, AppConfigSerializerContext.Default.AppConfig);

        public void Save()
            => ConfigLoader.Save(this, FileName, AppConfigSerializerContext.Default.AppConfig);

        #endregion

        #region IConfig

        public int GetConfigVersion()
            => ConfigVersion;

        public int GetCurrentVersion()
            => CurrentConfigVersion;

        public void SetConfigVersion(int version)
            => ConfigVersion = version;

        #endregion
    }
}

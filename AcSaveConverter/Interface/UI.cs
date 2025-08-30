using AcSaveConverter.Configuration;
using System.Text.Json.Serialization;

namespace AcSaveConverter.Interface
{
    [JsonSourceGenerationOptions(WriteIndented = true,
        GenerationMode = JsonSourceGenerationMode.Metadata,
        IncludeFields = true,
        UseStringEnumConverter = true)]
    [JsonSerializable(typeof(UI))]
    internal partial class UISerializerContext : JsonSerializerContext
    {
    }

    internal class UI : IConfig
    {
        #region Helper Properties

        [JsonIgnore]
        private const string FileName = "uiconfig.json";

        [JsonIgnore]
        private const int CurrentConfigVersion = 1;

        [JsonIgnore]
        internal static readonly UI Current;

        #endregion

        #region Settings

        public int ConfigVersion;
        public string FontEnglish;
        public string FontOther;
        public bool UseFontChinese;
        public bool UseFontKorean;
        public bool UseFontThai;
        public bool UseFontVietnamese;
        public bool UseFontCyrillic;
        public bool ScaleByDPI;
        public float InterfaceFontSize;
        public float UIScale;

        #endregion

        #region Static Constructor

        static UI()
        {
            Current = Load();
        }

        #endregion

        public UI()
        {
            FontEnglish = "RobotoMono-Light.ttf";
            FontOther = "NotoSansCJKtc-Light.otf";
            InterfaceFontSize = 13.0f;
            UIScale = 1.0f;
        }

        #region IO

        public static UI Load()
            => ConfigLoader.Load(FileName, UISerializerContext.Default.UI);

        public void Save()
            => ConfigLoader.Save(this, FileName, UISerializerContext.Default.UI);

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

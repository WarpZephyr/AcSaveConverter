using AcSaveConverter.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AcSaveConverter.Configuration
{
    [JsonSourceGenerationOptions(WriteIndented = true,
        GenerationMode = JsonSourceGenerationMode.Metadata,
        IncludeFields = true,
        UseStringEnumConverter = true)]
    [JsonSerializable(typeof(AppConfig))]
    internal partial class AppConfigSerializerContext : JsonSerializerContext
    {
    }

    internal class AppConfig
    {
        private const string FileName = "config.json";
        private static readonly string FolderPath = Program.AppDataFolder;
        private static readonly string DataPath = Path.Combine(FolderPath, FileName);

        public bool Xbox360;
        public bool UTF16;
        public bool AutoDetectEncoding;
        public bool AutoDetectPlatform;

        internal static AppConfig Instance { get; private set; } = Load();

        public AppConfig()
        {
            Xbox360 = false;
            UTF16 = true;
            AutoDetectEncoding = true;
            AutoDetectPlatform = true;
        }

        public static AppConfig Load()
        {
            AppConfig config;
            if (!File.Exists(DataPath))
            {
                Log.WriteLine($"Making default app config due to it being missing from expected path: \"{DataPath}\"");
                config = new AppConfig();

                Log.WriteLine("Saving default app config to expected path.");
                config.Save();
            }
            else
            {
                try
                {
                    var options = new JsonSerializerOptions();
                    config = JsonSerializer.Deserialize(File.ReadAllText(DataPath),
                        AppConfigSerializerContext.Default.AppConfig) ?? throw new Exception("JsonConvert returned null when loading config.");
                }
                catch (Exception ex)
                {
                    Log.WriteLine($"Failed to load app config from expected path \"{DataPath}\": {ex}");
                    Log.WriteLine("Making default app config due to failure to load it from expected path.");
                    config = new AppConfig();

                    Log.WriteLine("Saving default app config to expected path.");
                    config.Save();
                }
            }

            return config;
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(this, AppConfigSerializerContext.Default.AppConfig);

            try
            {
                Directory.CreateDirectory(FolderPath);
                File.WriteAllText(DataPath, json);
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Failed to save app config to path \"{DataPath}\": {ex}");
            }
        }
    }
}

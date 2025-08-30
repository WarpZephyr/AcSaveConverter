using AcSaveConverter.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace AcSaveConverter.Configuration
{
    internal static class ConfigLoader
    {
        internal static TValue Load<TValue>(string name, JsonTypeInfo<TValue> jsonTypeInfo) where TValue : IConfig, new()
        {
            string folder = AppInfo.AppDirectory;
            string dataPath = Path.Combine(folder, name);

            TValue config;
            if (!File.Exists(dataPath))
            {
                Log.WriteLine($"Making default app config due to it being missing from expected path: \"{dataPath}\"");
                config = new TValue();

                Log.WriteLine("Saving default app config to expected path.");
                config.Save();
            }
            else
            {
                try
                {
                    var options = new JsonSerializerOptions();
                    config = JsonSerializer.Deserialize(File.ReadAllText(dataPath),
                        jsonTypeInfo) ?? throw new Exception("JsonConvert returned null when loading config.");

                    int currentVersion = config.GetCurrentVersion();
                    int configVersion = config.GetConfigVersion();

                    // Update config file
                    if (configVersion < currentVersion)
                    {
                        Log.WriteLine("Replacing outdated config file.");
                        config.SetConfigVersion(currentVersion);
                        config.Save();
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine($"Failed to load app config from expected path \"{dataPath}\": {ex}");
                    Log.WriteLine("Making default app config due to failure to load it from expected path.");
                    config = new TValue();

                    Log.WriteLine("Saving default app config to expected path.");
                    config.Save();
                }
            }

            return config;
        }

        internal static void Save(object? config, string name, JsonTypeInfo jsonTypeInfo)
        {
            var json = JsonSerializer.Serialize(config, jsonTypeInfo);
            string folder = AppInfo.AppDirectory;
            string dataPath = Path.Combine(folder, name);

            try
            {
                Directory.CreateDirectory(folder);
                File.WriteAllText(dataPath, json);
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Failed to save config to path \"{dataPath}\": {ex}");
            }
        }
    }
}

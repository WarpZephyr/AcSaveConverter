using AcSaveConverter.Editors.AcfaEditor.Popups;
using AcSaveConverter.Editors.AcfaEditor.Utilities;
using AcSaveConverter.Editors.Framework;
using AcSaveConverter.Graphics;
using AcSaveConverter.Logging;
using AcSaveConverter.Resources;
using AcSaveFormats.ArmoredCoreForAnswer;
using AcSaveFormats.ArmoredCoreForAnswer.Designs;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;

namespace AcSaveConverter.Editors.AcfaEditor.Views
{
    public class DesignDocumentView : IDisposable
    {
        private readonly ResourceHandler ResourceHandler;
        private readonly AcColorSetPopup ColorPopup;
        private readonly List<TextureHandle> ThumbnailsCache;
        private DesignDocument Data;
        private bool disposedValue;

        public DesignDocumentView(ResourceHandler resourceHandler, AcColorSetPopup colorPopup)
        {
            ResourceHandler = resourceHandler;
            ColorPopup = colorPopup;
            ThumbnailsCache = [];

            Data = new DesignDocument
            {
                IsUtf16 = AppConfig.Current.IsUtf16,
                IsXbox360 = AppConfig.Current.IsXbox360
            };

            ReloadThumbnails();
        }

        public void Display()
        {
            EditorDecorator.SetupWindow();
            if (ImGui.Begin("Design Document"))
            {
                for (int i = 0; i < Data.Designs.Count; i++)
                {
                    var design = Data.Designs[i];
                    var thumbnailCache = ThumbnailsCache[i];
                    DesignView.DisplayDesign($"Design[{i}]", design, thumbnailCache, ResourceHandler.GetDefaultThumbnail(), ColorPopup, out bool thumbnailUpdate);
                    if (thumbnailUpdate)
                    {
                        ReloadThumbnail(i);
                    }
                }
            }

            ImGui.End();
        }

        public void Load(string path)
        {
            try
            {
                bool utf16;
                if (AppConfig.Current.AutoDetectEncoding)
                {
                    utf16 = DetectUTF16(path);
                    Log.WriteLine($"Detected design document encoding: {(utf16 ? "UTF16" : "ShiftJIS")}");
                }
                else
                {
                    utf16 = AppConfig.Current.IsUtf16;
                }

                bool xbox360;
                if (AppConfig.Current.AutoDetectPlatform)
                {
                    xbox360 = DetectXbox360(path);
                    Log.WriteLine($"Detected design document platform: {(xbox360 ? "Xbox 360" : "PS3")}");
                }
                else
                {
                    xbox360 = AppConfig.Current.IsXbox360;
                }

                var data = DesignDocument.Read(path, utf16, xbox360);
                Load(data);
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Error: Design Document load failed: {ex.Message}");
            }
        }

        public void Load(DesignDocument data)
        {
            DesignValidator.Validate(data);
            Data = data;
            ReloadThumbnails();
        }

        public DesignDocument GetData()
            => Data;

        #region Detection

        private static bool DetectUTF16(string file)
        {
            if (AppConfig.Current.AutoDetectEncoding)
            {
                var fileInfo = new FileInfo(file);
                long length = fileInfo.Length;
                if (length == 4856328)
                {
                    // A 2-byte encoding is being used
                    // Which should be UTF16
                    return true;
                }
                else if (length == 4837128)
                {
                    // A 1-byte encoding is being used
                    // Which should be ShiftJIS
                    return false;
                }
            }

            // Encoding state could not be determined automatically or we choose to manually override it
            return AppConfig.Current.IsUtf16;
        }

        private static bool DetectXbox360(string file)
        {
            if (AppConfig.Current.AutoDetectPlatform)
            {
                string? directoryPath = Path.GetDirectoryName(file);
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    bool xboxThumbnailExists = File.Exists(Path.Combine(directoryPath, "__thumbnail.png"));
                    bool ps3ParamSfoExists = File.Exists(Path.Combine(directoryPath, "PARAM.SFO"));
                    bool ps3IconExists = File.Exists(Path.Combine(directoryPath, "ICON0.PNG"));
                    bool ps3PictureExists = File.Exists(Path.Combine(directoryPath, "PIC1.PNG"));

                    bool hasPs3 = ps3ParamSfoExists || ps3IconExists || ps3PictureExists;
                    bool hasXbox360 = xboxThumbnailExists;

                    bool purePs3 = hasPs3 && !hasXbox360;
                    bool pureXbox360 = hasXbox360 && !hasPs3;

                    if (purePs3)
                    {
                        // Only PS3 specific save files are present
                        return false;
                    }
                    else if (pureXbox360)
                    {
                        // Only Xbox 360 specific save files are present
                        return true;
                    }

                    string directoryName = Path.GetFileName(directoryPath);
                    if (directoryName.StartsWith("ASSMBLY"))
                    {
                        // The folder name starts the way an Xbox 360 folder would
                        return true;
                    }
                    else if (directoryName.Length == 19 && directoryName.EndsWith("ASSMBLY064"))
                    {
                        // The folder name ends the way a PS3 folder would
                        // It has the same length as a PS3 folder name
                        return false;
                    }
                }
            }

            // Platform state could not be determined automatically or we choose to manually override it
            return AppConfig.Current.IsXbox360;
        }

        #endregion

        #region Thumbnails

        private void DestroyThumbnails()
        {
            foreach (var thumbnailCache in ThumbnailsCache)
            {
                thumbnailCache.Dispose();
            }

            ThumbnailsCache.Clear();
        }

        private void ReloadThumbnails()
        {
            DestroyThumbnails();
            foreach (var design in Data.Designs)
            {
                LoadThumbnail(design.Thumbnail);
            }
        }

        private void ReloadThumbnail(int index)
        {
            ThumbnailsCache[index].Dispose();
            ThumbnailsCache[index] = ResourceHandler.LoadDDS(Data.Designs[index].Thumbnail.GetDdsBytes());
        }

        private void LoadThumbnail(Thumbnail thumbnail)
        {
            ThumbnailsCache.Add(ResourceHandler.LoadDDS(thumbnail.GetDdsBytes()));
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DestroyThumbnails();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

using AcSaveConverter.Configuration;
using AcSaveConverter.Graphics;
using AcSaveConverter.Graphics.Textures;
using AcSaveConverter.GUI.Dialogs.Popups.ACFA;
using AcSaveConverter.GUI.Dialogs.Tabs;
using AcSaveConverter.IO;
using AcSaveConverter.Logging;
using AcSaveFormats.ACFA;
using AcSaveFormats.ACFA.Designs;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
namespace AcSaveConverter.GUI.Dialogs.ACFA
{
    internal class DesignDocumentFaDialog : IDataTab
    {
        private readonly ImGuiGraphicsContext Graphics;
        private bool disposedValue;

        public string Name { get; set; }
        public string DataType
            => "Design Document";

        public bool IsDisposed
            => disposedValue;

        public DesignDocument DesignDocument { get; private set; }
        private readonly List<ImGuiTexture> ThumbnailCache;

        private readonly AcColorSetPopup ColorsPopup;

        public DesignDocumentFaDialog(string name, ImGuiGraphicsContext graphics, DesignDocument data)
        {
            Graphics = graphics;
            Name = name;

            Validate_Designs(data);
            DesignDocument = data;
            ThumbnailCache = [];
            RebuildThumbnailCache();

            ColorsPopup = new AcColorSetPopup("AC Colors", new AcSaveFormats.ACFA.Colors.AcColorSet());
        }

        #region Render

        public void Render()
        {
            ImGui.PushID(nameof(DesignDocumentFaDialog));
            Render_MenuBar();
            Render_DesignDocument();
            ColorsPopup.Render();
            ImGui.PopID();
        }

        void Render_MenuBar()
        {
            if (ImGui.BeginMenuBar())
            {
                Render_FileMenu();
                ImGui.EndMenuBar();
            }
        }

        void Render_FileMenu()
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Open"))
                {
                    string? file = FileDialog.OpenFile();
                    if (FileDialog.ValidFile(file))
                    {
                        Load_Data(file);
                    }
                }

                ImGui.EndMenu();
            }
        }

        void Render_DesignDocument()
        {
            if (ImGui.BeginTable("DesignDocumentTable", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 256);
                for (int i = 0; i < DesignDocument.Designs.Count; i++)
                {
                    ImGui.PushID(i);

                    var design = DesignDocument.Designs[i];
                    var thumbnail = ThumbnailCache[i];

                    ImGui.TableNextColumn();
                    thumbnail.Image(thumbnail.Size);
                    if (ImGui.BeginPopupContextItem("ThumbnailContextMenu"))
                    {
                        Render_ThumbnailContextMenu(design, i);

                        ImGui.EndPopup();
                    }

                    ImGui.AlignTextToFramePadding();

                    ImGui.TableNextColumn();
                    DesignFaDialog.Render_Design(design, ColorsPopup);
                    ImGui.TableNextRow();
                    ImGui.PopID();
                }

                ImGui.EndTable();
            }
        }

        void Render_ThumbnailContextMenu(Design design, int index)
        {
            if (ImGui.Button("Export"))
            {
                try
                {
                    const StringComparison comp = StringComparison.InvariantCultureIgnoreCase;
                    string? path = FileDialog.GetSaveFilePath("png;jpg;bmp;dds");
                    if (FileDialog.ValidSavePath(path))
                    {
                        Log.WriteLine($"Exporting thumbnail to: {path}");
                        if (path.EndsWith(".png", comp))
                        {
                            TextureSave.ExportPng(path, design.Thumbnail.GetDDSBytes());
                        }
                        else if (path.EndsWith(".jpg", comp) || path.EndsWith(".jpeg", comp))
                        {
                            TextureSave.ExportJpeg(path, design.Thumbnail.GetDDSBytes());
                        }
                        else if (path.EndsWith(".bmp", comp))
                        {
                            TextureSave.ExportBmp(path, design.Thumbnail.GetDDSBytes());
                        }
                        else if (path.EndsWith(".dds", comp))
                        {
                            TextureSave.ExportDDS(path, design.Thumbnail.GetDDSBytes());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine($"Failed to export: {ex}");
                }
            }

            if (ImGui.Button("Import"))
            {
                try
                {
                    string? path = FileDialog.OpenFile("dds;bin");
                    if (FileDialog.ValidFile(path))
                    {
                        Log.WriteLine($"Importing thumbnail from: {path}");
                        if (DesignFaDialog.ImportThumbnail(path, AppConfig.Instance.Xbox360, design.Thumbnail, out Thumbnail? output))
                        {
                            design.Thumbnail = output;
                            ReloadThumbnailCache(index);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine($"Failed to import: {ex}");
                }
            }
        }

        #endregion

        #region Data

        public void Load_Data(DesignDocument data)
        {
            Validate_Designs(data);
            DesignDocument = data;
            RebuildThumbnailCache();
        }

        public void Load_Data(string path)
        {
            try
            {
                Log.WriteLine($"Loading {DataType} from path: \"{path}\"");

                bool utf16 = DetectUTF16(path);
                bool xbox360 = DetectXbox360(path);

                Log.WriteLine($"Detected design document encoding: {(utf16 ? "UTF16" : "ShiftJIS")}");
                Log.WriteLine($"Detected design document platform: {(xbox360 ? "Xbox 360" : "PS3")}");
                Load_Data(DesignDocument.Read(path, utf16, xbox360));
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Failed to load {DataType} from path \"{path}\": {ex}");
            }
        }

        public bool IsData(string path)
        {
            return path.EndsWith("DESDOC.DAT", StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Detection

        private static bool DetectUTF16(string file)
        {
            if (AppConfig.Instance.AutoDetectEncoding)
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
            return AppConfig.Instance.UTF16;
        }

        private static bool DetectXbox360(string file)
        {
            if (AppConfig.Instance.AutoDetectPlatform)
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
            return AppConfig.Instance.Xbox360;
        }

        #endregion

        #region Validation

        internal static void Validate_Designs(DesignDocument designDocument)
        {
            foreach (var design in designDocument.Designs)
            {
                DesignFaDialog.Validate_Design(design);
            }
        }

        #endregion

        #region Cache

        void RebuildThumbnailCache()
        {
            InvalidateThumbnailCache();
            foreach (var design in DesignDocument.Designs)
            {
                LoadThumbnail(design.Thumbnail);
            }
        }

        void ReloadThumbnailCache(int index)
        {
            ThumbnailCache[index].Dispose();
            ThumbnailCache[index] = Graphics.TexturePool.LoadDDS(DesignDocument.Designs[index].Thumbnail.GetDDSBytes());
        }

        void LoadThumbnail(Thumbnail thumbnail)
        {
            ThumbnailCache.Add(Graphics.TexturePool.LoadDDS(thumbnail.GetDDSBytes()));
        }

        void InvalidateThumbnailCache()
        {
            foreach (var thumbnail in ThumbnailCache)
            {
                thumbnail.Dispose();
            }

            ThumbnailCache.Clear();
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    InvalidateThumbnailCache();
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

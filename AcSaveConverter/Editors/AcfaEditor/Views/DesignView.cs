using AcSaveConverter.Editors.AcfaEditor.Popups;
using AcSaveConverter.Editors.AcfaEditor.Utilities;
using AcSaveConverter.Editors.Framework;
using AcSaveConverter.Fonts;
using AcSaveConverter.Graphics;
using AcSaveConverter.Interface;
using AcSaveConverter.Logging;
using AcSaveConverter.Resources;
using AcSaveFormats.ArmoredCoreForAnswer;
using AcSaveFormats.ArmoredCoreForAnswer.Designs;
using ImGuiNET;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AcSaveConverter.Editors.AcfaEditor.Views
{
    public class DesignView : IDisposable
    {
        private readonly ResourceHandler ResourceHandler;
        private readonly AcColorSetPopup ColorPopup;

        private Design Data;
        private TextureHandle ThumbnailCache;
        private bool disposedValue;

        public DesignView(ResourceHandler resourceHandler, AcColorSetPopup colorPopup)
        {
            ResourceHandler = resourceHandler;
            ColorPopup = colorPopup;
            Data = new Design()
            {
                IsUtf16 = AppConfig.Current.IsUtf16,
                IsXbox360 = AppConfig.Current.IsXbox360,
                Thumbnail = GetDefaultThumbnail()
            };

            ThumbnailCache = ResourceHandler.LoadDDS(Data.Thumbnail.GetDdsBytes());
        }

        public void Display()
        {
            EditorDecorator.SetupWindow();
            if (ImGui.Begin("Design"))
            {
                DisplayDesign("CurrentDesign", Data, ThumbnailCache, ResourceHandler.GetDefaultThumbnail(), ColorPopup, out bool thumbnailUpdate);
                if (thumbnailUpdate)
                {
                    ReloadThumbnail();
                }
            }

            ImGui.End();
        }

        #region Inner Gui

        internal static void DisplayDesign(string id, Design data, TextureHandle thumbnail, TextureHandle defaultThumbnail, AcColorSetPopup colorPopup, out bool thumbnailUpdate)
        {
            thumbnailUpdate = false;

            var pos = ImGui.GetCursorScreenPos();
            ImGui.Image(defaultThumbnail.GetHandle(), defaultThumbnail.Size);
            ImGuiEx.DrawOver(thumbnail.GetHandle(), pos, defaultThumbnail.Size);
            ImGui.PushID(id);
            if (ImGui.BeginPopupContextItem("ThumbnailContextMenu"))
            {
                ThumbnailContextMenu(data, out thumbnailUpdate);
                ImGui.EndPopup();
            }
            ImGui.PopID();

            ImGui.SameLine();

            ImGui.BeginGroup();
            if (ImGui.BeginTable("##DesignTable", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.ScrollY | ImGuiTableFlags.BordersOuter, new Vector2(0, 128)))
            {
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthStretch, 2.0f); // Default twice larger

                ShowProperties(data, colorPopup);
                ImGui.EndTable();
            }
            ImGui.EndGroup();
        }

        private static void ShowProperties(Design data, AcColorSetPopup colorPopup)
        {
            // Variables
            string designName = data.DesignName;
            string designerName = data.DesignerName;
            DateTime creationTimeStamp = data.CreationTimeStamp;
            string creationTimeStampStr = creationTimeStamp.ToString();
            bool protect = data.Protect;
            byte category = data.Category;

            // Name Column
            ImGui.TableNextColumn();
            ImGui.AlignTextToFramePadding();
            ImGuiEx.RightAlignedColumnText("Design Name");
            ImGuiEx.ShowHoverTooltip("The design name.");

            ImGui.AlignTextToFramePadding();
            ImGuiEx.RightAlignedColumnText("Designer Name");
            ImGuiEx.ShowHoverTooltip("The designer name.");

            ImGui.AlignTextToFramePadding();
            ImGuiEx.RightAlignedColumnText("Creation Time");
            ImGuiEx.ShowHoverTooltip("The date and time this design was created.");

            ImGui.AlignTextToFramePadding();
            ImGuiEx.RightAlignedColumnText("Category");
            ImGuiEx.ShowHoverTooltip("The category this design is under in the designs menu.");

            ImGui.AlignTextToFramePadding();
            ImGuiEx.RightAlignedColumnText("Colors");
            ImGuiEx.ShowHoverTooltip("The AcColorSet for this design.");

            // Value Column
            ImGui.TableNextColumn();

            var colWidth = ImGui.GetColumnWidth();
            ImGui.AlignTextToFramePadding();
            ImGui.SetNextItemWidth(colWidth);
            if (ImGui.InputText("##DesignName", ref designName, 48))
            {
                if (data.DesignName != designName)
                {
                    data.DesignName = designName;
                }
            }

            ImGui.AlignTextToFramePadding();
            ImGui.SetNextItemWidth(colWidth);
            if (ImGui.InputText("##DesignerName", ref designerName, 48))
            {
                if (data.DesignerName != designerName)
                {
                    data.DesignerName = designerName;
                }
            }

            ImGui.AlignTextToFramePadding();
            if (ImGui.InputText("##CreationTimeStamp", ref creationTimeStampStr, 255))
            {
                if (DateTime.TryParse(creationTimeStampStr, out DateTime result))
                {
                    if (data.CreationTimeStamp != result)
                    {
                        data.CreationTimeStamp = result;
                    }
                }
            }

            ImGui.SameLine();
            if (ImGui.Button("Now"))
                data.CreationTimeStamp = DateTime.Now;
            ImGuiEx.ShowHoverTooltip("Set the creation timestamp to the current time.");

            ImGui.AlignTextToFramePadding();
            if (ImGuiEx.InputNumeric("##Category", ref category))
            {
                category = Math.Clamp(category, byte.MinValue, (byte)sbyte.MaxValue);
                if (data.Category != category)
                {
                    data.Category = category;
                }
            }
            ImGui.SameLine();
            ImGuiEx.PushStyleHideButtonBg();
            if (ImGui.Button(protect ? EditorFont.LockedIcon : EditorFont.UnlockedIcon, new Vector2(20, 18)))
            {
                protect = !protect;
                if (data.Protect != protect)
                {
                    data.Protect = protect;
                }
            }
            ImGuiEx.PopStyleHideButtonBg();
            ImGuiEx.ShowHoverTooltip("Whether or not this design is locked to prevent editing (Depreciated).");

            ImGui.AlignTextToFramePadding();
            if (ImGui.Button("Edit"))
            {
                colorPopup.Load(data.Colors);
                colorPopup.OpenPopup = true;
            }
        }

        private static void ThumbnailContextMenu(Design data, out bool thumbnailUpdate)
        {
            thumbnailUpdate = false;
            if (ImGui.Button("Export"))
            {
                ExportThumbnail(data);
            }

            if (ImGui.Button("Import"))
            {
                ImportThumbnail(data, out thumbnailUpdate);
            }
        }

        #endregion

        #region IO

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

                var data = Design.Read(path, utf16, xbox360);
                Load(data);
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Error: Design load failed: {ex.Message}");
            }
        }

        public void Load(Design data)
        {
            DesignValidator.Validate(data);
            Data = data;
        }

        public Design GetData()
            => Data;

        #endregion

        #region Detection

        private static bool DetectUTF16(string file)
        {
            if (AppConfig.Current.AutoDetectEncoding)
            {
                var fileInfo = new FileInfo(file);
                long length = fileInfo.Length;
                if (length == 24280)
                {
                    // A 2-byte encoding is being used
                    // Which should be UTF16
                    return true;
                }
                else if (length == 24184)
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
            // Design data won't be purely alone on Xbox 360 like it is on PS3
            return false;
        }

        #endregion

        #region Thumbnail IO

        private static void ExportThumbnail(Design data)
        {
            const StringComparison comp = StringComparison.InvariantCultureIgnoreCase;
            string? path = FileDialog.GetSaveFilePath("png;jpg;bmp;dds");
            if (!string.IsNullOrWhiteSpace(path) && !Directory.Exists(path))
            {
                if (path.EndsWith(".png", comp))
                {
                    TextureExporter.ExportPng(path, data.Thumbnail.GetDdsBytes());
                }
                else if (path.EndsWith(".jpg", comp) || path.EndsWith(".jpeg", comp))
                {
                    TextureExporter.ExportJpeg(path, data.Thumbnail.GetDdsBytes());
                }
                else if (path.EndsWith(".bmp", comp))
                {
                    TextureExporter.ExportBmp(path, data.Thumbnail.GetDdsBytes());
                }
                else if (path.EndsWith(".dds", comp))
                {
                    TextureExporter.ExportDds(path, data.Thumbnail.GetDdsBytes());
                }
                else
                {
                    // TODO: Figure out how to determine the selected filter from the filter list
                    // Fallback on forcing png for now
                    // Otherwise nothing is saved
                    path += ".png";
                    TextureExporter.ExportPng(path, data.Thumbnail.GetDdsBytes());
                }
            }
        }

        private static void ImportThumbnail(Design data, out bool thumbnailUpdate)
        {
            thumbnailUpdate = false;
#if !DEBUG
            try
            {
#endif
            string? path = FileDialog.OpenFile("png;jpg;jpeg;dds;bin");
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                if (DesignThumbnailImporter.ImportThumbnail(path, AppConfig.Current.IsXbox360, data.Thumbnail, out Thumbnail? output))
                {
                    data.Thumbnail = output;
                    thumbnailUpdate = true;
                }
            }
#if !DEBUG
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Error: Thumbnail import failed: {ex.Message}");
            }
#endif
        }

        #endregion

        #region Thumbnail

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DestroyThumbnail()
        {
            ThumbnailCache.Dispose();
        }

        private void ReloadThumbnail()
        {
            DestroyThumbnail();
            ThumbnailCache = ResourceHandler.LoadDDS(Data.Thumbnail.GetDdsBytes());
        }

        private static Thumbnail GetDefaultThumbnail()
        {
            var defaultThumbnailPath = AssetPath.GetImagePath(Path.Combine("ArmoredCoreForAnswer", "thumb4026.bin"));
            var defaultThumbnail = Thumbnail.Read(defaultThumbnailPath, false);
            return defaultThumbnail;
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DestroyThumbnail();
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

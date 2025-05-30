using AcSaveConverter.Graphics.Textures;
using AcSaveConverter.Graphics;
using AcSaveConverter.GUI.Dialogs.Popups.ACFA;
using AcSaveConverter.GUI.Dialogs.Tabs;
using AcSaveConverter.IO;
using AcSaveFormats.ACFA;
using AcSaveFormats.ACFA.Designs;
using ImGuiNET;
using System;
using System.Collections.Generic;
namespace AcSaveConverter.GUI.Dialogs.ACFA
{
    internal class DesignDocumentFaDialog : IDataTab
    {
        private readonly ImGuiGraphicsContext Graphics;
        private bool disposedValue;

        public string Name { get; set; }
        public bool IsDisposed
            => disposedValue;

        public DesignDocument DesignDocument { get; private set; }
        private readonly List<ImGuiTexture> ThumbnailCache;
        private bool UTF16;
        private bool Xbox;

        private readonly AcColorSetPopup ColorsPopup;

        public DesignDocumentFaDialog(string name, ImGuiGraphicsContext graphics, DesignDocument data)
        {
            Graphics = graphics;
            Name = name;

            Validate_Designs(data);
            DesignDocument = data;
            ThumbnailCache = [];
            RebuildThumbnailCache();

            UTF16 = true;
            Xbox = false;

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
                Render_OptionsMenu();
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

        void Render_OptionsMenu()
        {
            if (ImGui.BeginMenu("Options"))
            {
                ImGui.MenuItem("UTF16", "", ref UTF16);
                ImGui.MenuItem("Xbox", "", ref Xbox);
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
                const StringComparison comp = StringComparison.InvariantCultureIgnoreCase;
                string? path = FileDialog.GetSaveFilePath("png;jpg;bmp;dds");
                if (FileDialog.ValidSavePath(path))
                {
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

            if (ImGui.Button("Import"))
            {
                try
                {
                    string? path = FileDialog.OpenFile("dds;bin");
                    if (FileDialog.ValidFile(path))
                    {
                        if (DesignFaDialog.ImportThumbnail(path, Xbox, design.Thumbnail, out Thumbnail? output))
                        {
                            design.Thumbnail = output;
                            ReloadThumbnailCache(index);
                        }
                    }
                }
                catch
                {

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

        public void Load_Data(string file)
        {
            Load_Data(DesignDocument.Read(file, UTF16, Xbox));
        }

        public bool IsData(string file)
        {
            return file.EndsWith("DESDOC.DAT", StringComparison.InvariantCultureIgnoreCase);
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

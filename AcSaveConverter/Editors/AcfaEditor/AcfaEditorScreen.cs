using AcSaveConverter.Configuration;
using AcSaveConverter.Editors.AcfaEditor.Data;
using AcSaveConverter.Editors.AcfaEditor.Popups;
using AcSaveConverter.Editors.AcfaEditor.Utilities;
using AcSaveConverter.Editors.AcfaEditor.Views;
using AcSaveConverter.Graphics;
using AcSaveConverter.Input;
using AcSaveConverter.Interface;
using AcSaveConverter.Logging;
using AcSaveFormats.ArmoredCoreForAnswer;
using AcSaveFormats.ArmoredCoreForAnswer.Xbox360;
using ImGuiNET;
using System;
using System.IO;
using System.Numerics;

namespace AcSaveConverter.Editors.AcfaEditor
{
    public class AcfaEditorScreen : IEditorScreen
    {
        public string EditorName
            => "Acfa Save Editor";

        private readonly DesignView DesignView;
        private readonly GameProgressView GameProgressView;
        private readonly OptionsSettingsView OptionsSettingsView;
        private readonly PlayerDataView PlayerDataView;
        private readonly DesignDocumentView DesignDocumentView;
        private readonly PaintView PaintView;
        private readonly ExportPopup ExportPopup;
        private readonly AcColorSetPopup ColorPopup;
        private bool disposedValue;

        public AcfaEditorScreen(GuiTexturePool texturePool)
        {
            ExportPopup = new ExportPopup();
            ExportPopup.ExportPressed += ExportPressed;

            ColorPopup = new AcColorSetPopup();

            DesignView = new DesignView(texturePool, ColorPopup);
            GameProgressView = new GameProgressView();
            OptionsSettingsView = new OptionsSettingsView();
            PlayerDataView = new PlayerDataView();
            DesignDocumentView = new DesignDocumentView(texturePool, ColorPopup);
            PaintView = new PaintView();
        }

        public void Update(float deltaTime)
        {
            if (InputTracker.HasDragDrop())
            {
                string path = InputTracker.GetDragDrop();
                if (Directory.Exists(path))
                {
                    LoadFolder(path);
                }
                else if (File.Exists(path))
                {
                    LoadFile(path);
                }
            }
        }

        #region IO

        private void LoadFile(string path)
        {
            const StringComparison strComp = StringComparison.InvariantCultureIgnoreCase;
            if (path.EndsWith("APGD.DAT", strComp))
            {
                try
                {
                    var apgd = APGD.Read(path);
                    DesignView.Load(apgd.Design);
                    GameProgressView.Load(apgd.GameProgress);
                    OptionsSettingsView.Load(apgd.OptionsSettings);
                    PlayerDataView.Load(apgd.PlayerData);
                    Log.WriteLine("Loaded Xbox 360 APGD Save.");
                }
                catch (Exception ex)
                {
                    Log.WriteLine($"Error: Xbox 360 APGD Save load failed: {ex.Message}");
                }
            }
            else if (path.EndsWith("GPROG.DAT", strComp))
            {
                GameProgressView.Load(path);
                Log.WriteLine("Loaded Game Progress Save.");
            }
            else if (path.EndsWith("PDATA.DAT", strComp))
            {
                PlayerDataView.Load(path);
                Log.WriteLine("Loaded Player Data Save.");
            }
            else if (path.EndsWith("OSET.DAT", strComp))
            {
                OptionsSettingsView.Load(OptionsSettings.Read(path));
                Log.WriteLine("Loaded Options/Settings Save.");
            }
            else if (path.EndsWith("ACDES.DAT", strComp))
            {
                DesignView.Load(path);
                Log.WriteLine("Loaded Current Design Save.");
            }
            else if (path.EndsWith("DESDOC.DAT", strComp))
            {
                DesignDocumentView.Load(path);
                Log.WriteLine("Loaded Design Document Save.");
            }
            else if (path.EndsWith("PAINT.DAT", strComp))
            {
                PaintView.Load(path);
                Log.WriteLine("Loaded Paint Save.");
            }
            else if (path.EndsWith(".PNG", strComp) ||
                path.EndsWith("PARAM.SFO", strComp) ||
                path.EndsWith("GAMEDAT0000_CONTENT", strComp) ||
                path.EndsWith("GAMEDAT0000_CONTENTINFO", strComp))
            {
                Log.WriteLine($"Skipping: \"{path}\"");
            }
            else
            {
                Log.WriteLine($"Warning: File unrecognized: \"{path}\"");
            }
        }

        private void LoadFolder(string folder)
        {
            foreach (string file in Directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly))
            {
                LoadFile(file);
            }
        }

        private void OpenFiles()
        {
            string[] paths = FileDialog.OpenFiles();
            for (int i = 0; i < paths.Length; i++)
            {
                LoadFile(paths[i]);
            }
        }

        private void OpenFolder()
        {
            string? folder = FileDialog.OpenFolder();
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
            {
                return;
            }

            LoadFolder(folder);
        }

        #endregion

        #region Export

        private void ExportPressed(object? sender, ExportEventArgs exportArgs)
        {
            string? path = FileDialog.OpenFolder();
            if (!FileDialog.ValidFolder(path))
            {
                return;
            }

            var exportName = exportArgs.ExportName;
            var game = exportArgs.Game;
            var platform = exportArgs.Platform;
            var region = exportArgs.Region;
            var kind = exportArgs.Kind;

            string folder = Path.Combine(path, exportName);
            Directory.CreateDirectory(folder);

            bool jp = region == RegionType.JP;
            bool xbox = platform == PlatformType.Xbox360;

            try
            {
                switch (kind)
                {
                    case ExportKind.GameData:
                        var gameData = new ExportGameData(DesignView.GetData(), GameProgressView.GetData(), OptionsSettingsView.GetData(), PlayerDataView.GetData());
                        GameDataExporter.Export(folder, gameData, jp, xbox);
                        Log.WriteLine($"Exported {region} {(xbox ? "Xbox 360" : "PS3")} game data save to folder: {folder}");
                        break;
                    case ExportKind.DesignDocument:
                        DesignDocumentExporter.Export(folder, DesignDocumentView.GetData(), xbox, jp);
                        Log.WriteLine($"Exported {region} {(xbox ? "Xbox 360" : "PS3")} design document save to folder: {folder}");
                        break;
                    case ExportKind.Paint:
                        PaintExporter.Export(folder, PaintView.GetData(), xbox, jp);
                        Log.WriteLine($"Exported {region} {(xbox ? "Xbox 360" : "PS3")} paint save to folder: {folder}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Export failed: {ex}");
            }
        }

        #endregion

        #region Menu Gui

        public void OnMenuGui()
        {
            FileDropdown();
            OptionsDropdown();
        }

        private void FileDropdown()
        {
            if (ImGui.BeginMenu($"File##{nameof(AcfaEditorScreen)}"))
            {
                if (ImGui.MenuItem("Open Files"))
                {
                    OpenFiles();
                }

                if (ImGui.MenuItem("Open Folder"))
                {
                    OpenFolder();
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Export"))
                {
                    ExportPopup.OpenPopup = true;
                }

                ImGui.EndMenu();
            }
        }

        private void OptionsDropdown()
        {
            if (ImGui.BeginMenu($"Options##{nameof(AcfaEditorScreen)}"))
            {
                ImGuiEx.ComboEnum("Platform", ref AppConfig.Current.Platform);
                ImGuiEx.ComboEnum("Region", ref AppConfig.Current.Region);
                ImGuiEx.ComboEnum("Encoding", ref AppConfig.Current.Encoding);
                ImGui.Checkbox("Auto Detect Platform", ref AppConfig.Current.AutoDetectPlatform);
                ImGui.Checkbox("Auto Detect Region", ref AppConfig.Current.AutoDetectRegion);
                ImGui.Checkbox("Auto Detect Encoding", ref AppConfig.Current.AutoDetectEncoding);

                ImGui.EndMenu();
            }
        }

        #endregion

        #region Gui

        public void OnGui()
        {
            var scale = DPI.Current.GetUIScale();

            // Docking setup
            Vector2 wins = ImGui.GetWindowSize();
            Vector2 winp = ImGui.GetWindowPos();
            winp.Y += 20.0f * scale;
            wins.Y -= 20.0f * scale;
            ImGui.SetNextWindowPos(winp);
            ImGui.SetNextWindowSize(wins);
            var dsid = ImGui.GetID($"DockSpace_{nameof(AcfaEditorScreen)}");
            ImGui.DockSpace(dsid, Vector2.Zero);

            DesignView.Display();
            GameProgressView.Display();
            OptionsSettingsView.Display();
            PlayerDataView.Display();
            DesignDocumentView.Display();
            PaintView.Display();
            ExportPopup.Display();
            ColorPopup.Display();
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DesignView.Dispose();
                    DesignDocumentView.Dispose();
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

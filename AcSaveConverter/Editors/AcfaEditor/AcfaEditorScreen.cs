using AcSaveConverter.Configuration;
using AcSaveConverter.Editors.AcfaEditor.Data;
using AcSaveConverter.Editors.AcfaEditor.Popups;
using AcSaveConverter.Editors.AcfaEditor.Views;
using AcSaveConverter.Graphics;
using AcSaveConverter.Interface;
using AcSaveConverter.Logging;
using AcSaveConverter.Resources;
using AcSaveFormats.ArmoredCoreForAnswer;
using AcSaveFormats.ArmoredCoreForAnswer.Xbox360;
using AcSaveFormats.PlayStation3;
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

            foreach (string file in Directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly))
            {
                LoadFile(file);
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

        #region Export

        private void ExportGameDataPs3(string folder, bool jp)
        {
            var design = DesignView.GetData();
            var gameProgress = GameProgressView.GetData();
            var optionsSettings = OptionsSettingsView.GetData();
            var playerData = PlayerDataView.GetData();

            bool utf16 = !jp;
            design.Write(Path.Combine(folder, "ACDES.DAT"), utf16, false);
            gameProgress.Write(Path.Combine(folder, "GPROG.DAT"));
            optionsSettings.Write(Path.Combine(folder, "OSET.DAT"));
            playerData.Write(Path.Combine(folder, "PDATA.DAT"));

            string iconName = Path.Combine("ArmoredCoreForAnswer", "icon1.png");
            AssetPath.CopyImageTo(iconName, "ICON0.PNG", folder);

            string picName = Path.Combine("ArmoredCoreForAnswer", "pic1.png");
            AssetPath.CopyImageTo(picName, "PIC1.PNG", folder);

            string nameFormat;
            string subTitle;
            if (jp)
            {
                nameFormat = "LINKS";
                subTitle = "ゲーム進行データ";
            }
            else
            {
                nameFormat = "LYNX NAME";
                subTitle = "Game Progress Data";
            }

            string totalRankStr = GetTotalRankString(playerData.Rank);
            string detail = GetDetailString(playerData.PlayTimeSeconds, playerData.LynxName, design.DesignName, totalRankStr, nameFormat);
            var sfo = Build_GameData_PARAMSFO(folder, detail, subTitle);
            sfo.Write(Path.Combine(folder, "PARAM.SFO"));
        }

        private void ExportGameDataXbox360(string folder)
        {
            var design = DesignView.GetData();
            var gameProgress = GameProgressView.GetData();
            var optionsSettings = OptionsSettingsView.GetData();
            var playerData = PlayerDataView.GetData();
            var apgd = new APGD(design, gameProgress, optionsSettings, playerData);

            apgd.Write(Path.Combine(folder, "APGD.DAT"));

            string thumbnailName = Path.Combine("ArmoredCoreForAnswer", "icon360.png");
            AssetPath.CopyImageTo(thumbnailName, "__thumbnail.png", folder);

            var content = Build_GameData_Content(folder, design, playerData);
            var contentinfo = new ContentInfo
            {
                Unk00 = 0,
                ContentSize = 31832,
                InfoSize = 5926,
                Unk0C = 2,
                Unk0E = 96,
                Unk0F = 0
            };

            string folderName = Path.GetFileName(folder);
            content.Write(Path.Combine(folder, $"{folderName}_CONTENT"));
            contentinfo.Write(Path.Combine(folder, $"{folderName}_CONTENTINFO"));
        }

        private void ExportDesignDocument(string folder, bool xbox, bool jp)
        {
            var desdoc = DesignDocumentView.GetData();

            bool utf16 = xbox || !jp;
            desdoc.Write(Path.Combine(folder, "DESDOC.DAT"), utf16, xbox);
            if (xbox)
            {
                string thumbnailName = Path.Combine("ArmoredCoreForAnswer", "icon360.png");
                AssetPath.CopyImageTo(thumbnailName, "__thumbnail.png", folder);
            }
            else
            {
                string iconName = Path.Combine("ArmoredCoreForAnswer", "icon0.png");
                AssetPath.CopyImageTo(iconName, "ICON0.PNG", folder);

                string picName = Path.Combine("ArmoredCoreForAnswer", "pic1_design.png");
                AssetPath.CopyImageTo(picName, "PIC1.PNG", folder);

                string subTitle;
                if (jp)
                {
                    subTitle = "機体図面データ";
                }
                else
                {
                    subTitle = "Schematic Data";
                }

                var sfo = Build_DesignDocument_PARAMSFO(folder, subTitle);
                sfo.Write(Path.Combine(folder, "PARAM.SFO"));
            }
        }

        private void ExportPaint(string folder, bool xbox, bool jp)
        {
            var paint = PaintView.GetData();
            paint.Write(Path.Combine(folder, "PAINT.DAT"));

            if (xbox)
            {
                string thumbnailName = Path.Combine("ArmoredCoreForAnswer", "icon360.png");
                AssetPath.CopyImageTo(thumbnailName, "__thumbnail.png", folder);
            }
            else
            {
                string iconName = Path.Combine("ArmoredCoreForAnswer", "iconpnt.png");
                AssetPath.CopyImageTo(iconName, "ICON0.PNG", folder);

                string picName = Path.Combine("ArmoredCoreForAnswer", "pic1_paint.png");
                AssetPath.CopyImageTo(picName, "PIC1.PNG", folder);

                string subTitle;
                if (jp)
                {
                    subTitle = "ペイントデータ";
                }
                else
                {
                    subTitle = "Paint Data";
                }

                var sfo = Build_Paint_PARAMSFO(folder, subTitle);
                sfo.Write(Path.Combine(folder, "PARAM.SFO"));
            }
        }

        #endregion

        #region Export Build

        static string GetTotalRankString(PlayerData.TotalRank rank)
        {
            if (rank == PlayerData.TotalRank.None)
            {
                return PlayerData.TotalRank.E.ToString();
            }

            return rank.ToString();
        }

        static string GetDetailString(float playTimeSeconds, string name, string acName, string totalRank, string nameFormat)
        {
            float hour = playTimeSeconds / 3600;
            float minute = playTimeSeconds / 60 % 60;
            float second = playTimeSeconds % 60;
            return $"PLAY TIME: {hour:00}：{minute:00}：{second:00}\r\n{nameFormat}: {name}\r\nAC: {acName}\r\nTOTAL RANK: {totalRank}";
        }

        static ParamSfo Build_GameData_PARAMSFO(string folder, string detail, string subTitle)
        {
            var sfo = new ParamSfo();
            var builder = new ParamSfoBuilder(sfo);
            builder.AddFile("ACDES.DAT", 1);
            builder.AddFile("GPROG.DAT", 1);
            builder.AddFile("ICON0.PNG", 0);
            builder.AddFile("OSET.DAT", 1);
            builder.AddFile("PDATA.DAT", 1);
            builder.AddFile("PDATA.DAT", 1);
            builder.AddFile("PIC1.PNG", 0);
            builder.SetDefaultsRPCS3();
            builder.BuildRPCS3BLIST();

            builder.SetSaveDataDirectory(Path.GetFileName(folder));
            builder.SetTitle("ARMORED CORE for Answer");
            builder.SetDetail(detail);
            builder.SetSubTitle(subTitle);
            return sfo;
        }

        static ParamSfo Build_DesignDocument_PARAMSFO(string folder, string subTitle)
        {
            var sfo = new ParamSfo();
            var builder = new ParamSfoBuilder(sfo);
            builder.AddFile("DESDOC.DAT", 1);
            builder.AddFile("ICON0.PNG", 0);
            builder.AddFile("PIC1.PNG", 0);
            builder.SetDefaultsRPCS3();
            builder.BuildRPCS3BLIST();

            builder.SetSaveDataDirectory(Path.GetFileName(folder));
            builder.SetTitle("ARMORED CORE for Answer");
            builder.SetSubTitle(subTitle);
            return sfo;
        }

        static ParamSfo Build_Paint_PARAMSFO(string folder, string subTitle)
        {
            var sfo = new ParamSfo();
            var builder = new ParamSfoBuilder(sfo);
            builder.AddFile("PAINT.DAT", 1);
            builder.AddFile("ICON0.PNG", 0);
            builder.AddFile("PIC1.PNG", 0);
            builder.SetDefaultsRPCS3();
            builder.BuildRPCS3BLIST();

            builder.SetSaveDataDirectory(Path.GetFileName(folder));
            builder.SetTitle("ARMORED CORE for Answer");
            builder.SetSubTitle(subTitle);
            return sfo;
        }

        static GameDataContent Build_GameData_Content(string folder, Design design, PlayerData playerData)
        {
            // Try to parse save index
            // TODO: Might need to make a better way of doing this
            int saveIndex = 0;
            const string DefaultFolderNameStart = "GAMEDAT000";
            string folderName = Path.GetFileName(folder);
            if (folderName.StartsWith(DefaultFolderNameStart, StringComparison.InvariantCultureIgnoreCase)
                && folderName.Length >= DefaultFolderNameStart.Length + 1
                && int.TryParse($"{folderName[DefaultFolderNameStart.Length]}", out int digit))
            {
                saveIndex = digit;
            }

            var content = new GameDataContent
            {
                Index = saveIndex,
                LynxName = playerData.LynxName,
                AcName = design.DesignName,
                Rank = playerData.Rank,
                Complete = playerData.Completed,
                CollaredRank = playerData.CollaredRank,
                OrcaRank = playerData.OrcaRank,
                Coam = playerData.Coam,
                PlayTimeSeconds = playerData.PlayTimeSeconds
            };

            return content;
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
                        if (xbox)
                        {
                            ExportGameDataXbox360(folder);
                            Log.WriteLine($"Exported {region} Xbox 360 game data save to folder: {folder}");
                        }
                        else
                        {
                            ExportGameDataPs3(folder, jp);
                            Log.WriteLine($"Exported {region} PS3 game data save to folder: {folder}");
                        }
                        break;
                    case ExportKind.DesignDocument:
                        ExportDesignDocument(folder, xbox, jp);
                        Log.WriteLine($"Exported {region} {(xbox ? "Xbox 360" : "PS3")} design document save to folder: {folder}");
                        break;
                    case ExportKind.Paint:
                        ExportPaint(folder, xbox, jp);
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

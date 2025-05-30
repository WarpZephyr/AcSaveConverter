using AcSaveConverter.Graphics;
using AcSaveConverter.GUI.Dialogs.ACFA;
using AcSaveConverter.GUI.Dialogs.Popups;
using AcSaveConverter.IO;
using AcSaveConverter.IO.Assets;
using AcSaveConverter.Logging;
using AcSaveConverter.Saves;
using AcSaveFormats.ACFA;
using AcSaveFormats.ACFA.PS3;
using AcSaveFormats.ACFA.Xbox360;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;

namespace AcSaveConverter.GUI.Dialogs.Tabs
{
    public class SaveTabFa : ISaveTab
    {
        private static readonly Dictionary<GameType, Dictionary<RegionType, string>> ExportHintCache = BuildExportHintCache();

        private readonly ImGuiGraphicsContext Graphics;
        public string Name { get; set; }
        public bool Open { get; set; }
        private bool disposedValue;
        public bool IsDisposed
            => disposedValue;

        private readonly DesignFaDialog DesignDialog;
        private readonly GameProgressFaDialog GameProgressDialog;
        private readonly OptionsSettingsFaDialog OptionsSettingsDialog;
        private readonly PlayerDataFaDialog PlayerDataDialog;
        private readonly DesignDocumentFaDialog DesignDocumentDialog;
        private readonly PaintFaDialog PaintDialog;
        private readonly ExportPopup ExportPopup;

        private int SelectedTabIndex;
        private readonly List<IDataTab> TabDialogs;

        private ExportKind CurrentExportKind;

        public SaveTabFa(string name, ImGuiGraphicsContext graphics)
        {
            Graphics = graphics;
            Name = name;
            Open = true;

            DesignDialog = new DesignFaDialog("Design", Graphics, new Design());
            GameProgressDialog = new GameProgressFaDialog("Game Progress", Graphics, new GameProgress());
            OptionsSettingsDialog = new OptionsSettingsFaDialog("Options Settings", Graphics, new OptionsSettings());
            PlayerDataDialog = new PlayerDataFaDialog("Player Data", Graphics, new PlayerData());
            DesignDocumentDialog = new DesignDocumentFaDialog("Design Document", Graphics, new DesignDocument());
            PaintDialog = new PaintFaDialog("Paint", graphics, new Paint());

            ExportPopup = new ExportPopup();
            ExportPopup.PopupOpened += ExportSettingsChanged;
            ExportPopup.SettingsChanged += ExportSettingsChanged;
            ExportPopup.ExportPressed += ExportPressed;

            SelectedTabIndex = 0;
            TabDialogs =
            [
                DesignDialog,
                GameProgressDialog,
                OptionsSettingsDialog,
                PlayerDataDialog,
                DesignDocumentDialog,
                PaintDialog
            ];
        }

        #region Render

        public void Render()
        {
            ImGui.PushID(nameof(SaveTabFa));

            bool open = Open;
            if (ImGui.BeginTabItem(Name, ref open, ImGuiTabItemFlags.NoCloseWithMiddleMouseButton))
            {
                Render_Tabs();
                ImGui.SameLine();
                Render_Content();
                ImGui.EndTabItem();
            }

            Render_Popups();
            Open = open;

            ImGui.PopID();
        }

        void Render_Tabs()
        {
            if (ImGui.BeginChild("Tabs", default, ImGuiChildFlags.AutoResizeX, ImGuiWindowFlags.MenuBar))
            {
                Render_TabsMenuBar();

                for (int i = 0; i < TabDialogs.Count; i++)
                {
                    ImGuiEx.SimpleVerticalTab(TabDialogs[i].Name, i, ref SelectedTabIndex);
                }

                ImGui.EndChild();
            }
        }

        void Render_TabsMenuBar()
        {
            if (ImGui.BeginMenuBar())
            {
                Render_TabsFileMenu();
                ImGui.EndMenuBar();
            }
        }

        void Render_TabsFileMenu()
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Open Folder"))
                {
                    string? path = FileDialog.OpenFolder();
                    if (FileDialog.ValidFolder(path))
                    {
                        Load_Folder(path);
                    }
                }

                Render_ExportMenu();
                ImGui.EndMenu();
            }
        }

        void Render_ExportMenu()
        {
            if (ImGui.BeginMenu("Export"))
            {
                if (ImGui.MenuItem("Game Data"))
                {
                    CurrentExportKind = ExportKind.GameData;
                    ExportPopup.OpenPopup = true;
                }

                if (ImGui.MenuItem("Design Document"))
                {
                    CurrentExportKind = ExportKind.DesignDocument;
                    ExportPopup.OpenPopup = true;
                }

                if (ImGui.MenuItem("Paint"))
                {
                    CurrentExportKind = ExportKind.Paint;
                    ExportPopup.OpenPopup = true;
                }

                ImGui.EndMenu();
            }
        }

        void Render_Content()
        {
            if (ImGui.BeginChild("Content", default, ImGuiChildFlags.Borders, ImGuiWindowFlags.MenuBar))
            {
                TabDialogs[SelectedTabIndex].Render();
                ImGui.EndChild();
            }
        }

        void Render_Popups()
        {
            ExportPopup.Render();
        }

        #endregion

        #region Load

        void Load_Folder(string folder)
        {
            Log.WriteLine($"Loading ACFA save files from folder: \"{folder}\"");

            foreach (string file in Directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly))
            {
                // Load first if it exists
                if (file.EndsWith("APGD.DAT", StringComparison.InvariantCultureIgnoreCase))
                {
                    var apgd = APGD.Read(file);
                    DesignDialog.Load_Data(apgd.Design);
                    GameProgressDialog.Load_Data(apgd.GameProgress);
                    OptionsSettingsDialog.Load_Data(apgd.OptionsSettings);
                    PlayerDataDialog.Load_Data(apgd.PlayerData);
                }

                // Override APGD loaded data if necessary
                foreach (var dialog in TabDialogs)
                {
                    if (dialog.IsData(file))
                    {
                        dialog.Load_Data(file);
                    }
                }
            }
        }

        #endregion

        #region Export

        void Export_GameData_PS3(string folder, bool jp)
        {
            var design = DesignDialog.Design;
            var gameProgress = GameProgressDialog.GameProgress;
            var optionsSettings = OptionsSettingsDialog.OptionsSettings;
            var playerData = PlayerDataDialog.PlayerData;

            bool utf16 = !jp;
            design.Write(Path.Combine(folder, "ACDES.DAT"), utf16, false);
            gameProgress.Write(Path.Combine(folder, "GPROG.DAT"));
            optionsSettings.Write(Path.Combine(folder, "OSET.DAT"));
            playerData.Write(Path.Combine(folder, "PDATA.DAT"));

            string iconName = Path.Combine("ACFA", "icon1.png");
            ImagesPath.CopyImageTo(iconName, "ICON0.PNG", folder);

            string picName = Path.Combine("ACFA", "pic1.png");
            ImagesPath.CopyImageTo(picName, "PIC1.PNG", folder);

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

        void Export_GameData_Xbox360(string folder)
        {
            var design = DesignDialog.Design;
            var gameProgress = GameProgressDialog.GameProgress;
            var optionsSettings = OptionsSettingsDialog.OptionsSettings;
            var playerData = PlayerDataDialog.PlayerData;
            var apgd = new APGD(design, gameProgress, optionsSettings, playerData);

            apgd.Write(Path.Combine(folder, "APGD.DAT"));

            string thumbnailName = Path.Combine("ACFA", "icon360.png");
            ImagesPath.CopyImageTo(thumbnailName, "__thumbnail.png", folder);

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

        void Export_DesignDocument(string folder, bool xbox, bool jp)
        {
            var desdoc = DesignDocumentDialog.DesignDocument;

            bool utf16 = xbox || !jp;
            desdoc.Write(Path.Combine(folder, "DESDOC.DAT"), utf16, xbox);
            if (xbox)
            {
                string thumbnailName = Path.Combine("ACFA", "icon360.png");
                ImagesPath.CopyImageTo(thumbnailName, "__thumbnail.png", folder);
            }
            else
            {
                string iconName = Path.Combine("ACFA", "icon0.png");
                ImagesPath.CopyImageTo(iconName, "ICON0.PNG", folder);

                string picName = Path.Combine("ACFA", "pic1_design.png");
                ImagesPath.CopyImageTo(picName, "PIC1.PNG", folder);

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

        void Export_Paint(string folder, bool xbox, bool jp)
        {
            var paint = PaintDialog.Paint;
            paint.Write(Path.Combine(folder, "PAINT.DAT"));

            if (xbox)
            {
                string thumbnailName = Path.Combine("ACFA", "icon360.png");
                ImagesPath.CopyImageTo(thumbnailName, "__thumbnail.png", folder);
            }
            else
            {
                string iconName = Path.Combine("ACFA", "iconpnt.png");
                ImagesPath.CopyImageTo(iconName, "ICON0.PNG", folder);

                string picName = Path.Combine("ACFA", "pic1_paint.png");
                ImagesPath.CopyImageTo(picName, "PIC1.PNG", folder);

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

        static PARAMSFO Build_GameData_PARAMSFO(string folder, string detail, string subTitle)
        {
            var sfo = new PARAMSFO();
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

        static PARAMSFO Build_DesignDocument_PARAMSFO(string folder, string subTitle)
        {
            var sfo = new PARAMSFO();
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

        static PARAMSFO Build_Paint_PARAMSFO(string folder, string subTitle)
        {
            var sfo = new PARAMSFO();
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

        #region Export Hint

        private static Dictionary<GameType, Dictionary<RegionType, string>> BuildExportHintCache()
        {
            var acfaHintCache = new Dictionary<RegionType, string>
            {
                { RegionType.US, "BLUS30187" },
                { RegionType.JP, "BLJM60066" },
                { RegionType.EU, "BLES00370" }
            };

            var hintCache = new Dictionary<GameType, Dictionary<RegionType, string>>
            {
                {GameType.ACFA, acfaHintCache }
            };

            return hintCache;
        }

        #endregion

        #region Export Update

        void ExportSettingsChanged(object? sender, ExportEventArgs exportArgs)
        {
            var game = exportArgs.Game;
            var platform = exportArgs.Platform;
            var region = exportArgs.Region;

            string kind = string.Empty;
            if (CurrentExportKind == ExportKind.GameData)
            {
                if (platform == PlatformType.PlayStation3)
                {
                    kind = "GAMEDAT000000F5K7M4000";
                }
                else if (platform == PlatformType.Xbox360)
                {
                    kind = "GAMEDAT0000";
                }
            }
            else if (CurrentExportKind == ExportKind.DesignDocument)
            {
                if (platform == PlatformType.PlayStation3)
                {
                    kind = "ASSMBLY064";
                }
                else if (platform == PlatformType.Xbox360)
                {
                    kind = "ASSMBLY0000";
                }
            }
            else if (CurrentExportKind == ExportKind.Paint)
            {
                if (platform == PlatformType.PlayStation3)
                {
                    kind = "PAINT014";
                }
                else if (platform == PlatformType.Xbox360)
                {
                    kind = "PAINT0000";
                }
            }

            string hint = string.Empty;
            switch (platform)
            {
                case PlatformType.PlayStation3:
                    hint = $"{ExportHintCache[game][region]}{kind}";
                    break;
                case PlatformType.Xbox360:
                    hint = kind;
                    break;
            }

            ExportPopup.ExportHint = hint;
        }

        void ExportPressed(object? sender, ExportEventArgs exportArgs)
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

            string folder = Path.Combine(path, exportName);
            Directory.CreateDirectory(folder);

            bool jp = region == RegionType.JP;
            bool xbox = platform == PlatformType.Xbox360;
            switch (CurrentExportKind)
            {
                case ExportKind.GameData:
                    if (xbox)
                    {
                        Log.WriteLine($"Exporting {region} Xbox 360 game data save to folder: {folder}");
                        Export_GameData_Xbox360(folder);
                    }
                    else
                    {
                        Log.WriteLine($"Exporting {region} PS3 game data save to folder: {folder}");
                        Export_GameData_PS3(folder, jp);
                    }
                    break;
                case ExportKind.DesignDocument:
                    Log.WriteLine($"Exporting {region} {(xbox ? "Xbox 360" : "PS3")} design document save to folder: {folder}");
                    Export_DesignDocument(folder, xbox, jp);
                    break;
                case ExportKind.Paint:
                    Log.WriteLine($"Exporting {region} {(xbox ? "Xbox 360" : "PS3")} paint save to folder: {folder}");
                    Export_Paint(folder, xbox, jp);
                    break;
            }

            Log.WriteLine("Finished export.");
        }

        #endregion

        #region Export Enum

        enum ExportKind
        {
            GameData,
            DesignDocument,
            Paint
        }

        #endregion

        #region Util

        private static string GetSecondsTimeString(float seconds)
        {
            float hour = seconds / 3600;
            float minute = seconds / 60 % 60;
            float second = seconds % 60;
            return $"{hour:00}:{minute:00}:{second:00}";
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
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

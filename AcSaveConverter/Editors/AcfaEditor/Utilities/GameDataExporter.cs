using AcSaveConverter.Editors.AcfaEditor.Data;
using AcSaveConverter.Resources;
using AcSaveFormats.ArmoredCoreForAnswer;
using AcSaveFormats.ArmoredCoreForAnswer.Xbox360;
using AcSaveFormats.PlayStation3;
using System;
using System.IO;

namespace AcSaveConverter.Editors.AcfaEditor.Utilities
{
    internal static class GameDataExporter
    {
        public static void Export(string folder, ExportGameData gameData, bool jp, bool xbox)
        {
            if (xbox)
            {
                ExportPs3(folder, gameData, jp);
            }
            else
            {
                ExportXbox360(folder, gameData);
            }
        }

        private static void ExportPs3(string folder, ExportGameData gameData, bool jp)
        {
            var design = gameData.Design;
            var gameProgress = gameData.GameProgress;
            var optionsSettings = gameData.OptionsSettings;
            var playerData = gameData.PlayerData;

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
            var sfo = BuildParamSfo(folder, detail, subTitle);
            sfo.Write(Path.Combine(folder, "PARAM.SFO"));
        }

        private static void ExportXbox360(string folder, ExportGameData gameData)
        {
            var design = gameData.Design;
            var gameProgress = gameData.GameProgress;
            var optionsSettings = gameData.OptionsSettings;
            var playerData = gameData.PlayerData;
            var apgd = new APGD(design, gameProgress, optionsSettings, playerData);

            apgd.Write(Path.Combine(folder, "APGD.DAT"));

            string thumbnailName = Path.Combine("ArmoredCoreForAnswer", "icon360.png");
            AssetPath.CopyImageTo(thumbnailName, "__thumbnail.png", folder);

            var content = BuildContent(folder, design, playerData);
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

        private static string GetTotalRankString(PlayerData.TotalRank rank)
        {
            if (rank == PlayerData.TotalRank.None)
            {
                return PlayerData.TotalRank.E.ToString();
            }

            return rank.ToString();
        }

        private static string GetDetailString(float playTimeSeconds, string name, string acName, string totalRank, string nameFormat)
        {
            float hour = playTimeSeconds / 3600;
            float minute = playTimeSeconds / 60 % 60;
            float second = playTimeSeconds % 60;
            return $"PLAY TIME: {hour:00}：{minute:00}：{second:00}\r\n{nameFormat}: {name}\r\nAC: {acName}\r\nTOTAL RANK: {totalRank}";
        }

        private static ParamSfo BuildParamSfo(string folder, string detail, string subTitle)
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

        private static GameDataContent BuildContent(string folder, Design design, PlayerData playerData)
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
    }
}

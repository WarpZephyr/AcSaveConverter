using AcSaveConverter.Editors.Framework;
using AcSaveConverter.Interface;
using AcSaveConverter.Logging;
using AcSaveFormats.ArmoredCoreForAnswer;
using ImGuiNET;
using System;
using System.Text;

namespace AcSaveConverter.Editors.AcfaEditor.Views
{
    public class PlayerDataView
    {
        private PlayerData Data;

        public PlayerDataView()
        {
            Data = new PlayerData();
        }

        public void Display()
        {
            EditorDecorator.SetupWindow();
            if (ImGui.Begin("Player Data"))
            {
                ShowProperties(Data);
            }

            ImGui.End();
        }

        private void ShowProperties(PlayerData data)
        {
            if (ImGui.BeginTable("PlayerDataTable", 2, ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthStretch, 2.0f); // Default twice larger

                // Variables
                var lynxName = data.LynxName;
                var totalRank = data.Rank;
                var completed = data.Completed;
                var collaredRank = data.CollaredRank;
                var orcaRank = data.OrcaRank;
                var coam = data.Coam;
                var playTime = data.PlayTimeSeconds;
                var playTimeStr = TimeToString(playTime);
                var oldPlayTimeStr = playTimeStr;

                // Name Column
                ImGui.TableNextColumn();
                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Lynx Name");
                ImGuiEx.ShowHoverTooltip("The lynx pilot name.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Total Rank");
                ImGuiEx.ShowHoverTooltip("The total rank of the save.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Completed");
                ImGuiEx.ShowHoverTooltip("The total number of completed missions and arenas.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Collared Rank");
                ImGuiEx.ShowHoverTooltip("The collared rank of the save.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Orca Rank");
                ImGuiEx.ShowHoverTooltip("The orca rank of the save.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Coam");
                ImGuiEx.ShowHoverTooltip("The amount of money in the save.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Play Time");
                ImGuiEx.ShowHoverTooltip("The play time of the save stored as seconds.");

                // Value Column
                ImGui.TableNextColumn();
                ImGui.AlignTextToFramePadding();
                if (ImGui.InputTextWithHint("##LynxName", PlayerData.DefaultLynxName, ref lynxName, 31))
                {
                    if (data.LynxName != lynxName)
                    {
                        data.LynxName = lynxName;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.ComboEnum("##TotalRank", ref totalRank))
                {
                    if (data.Rank != totalRank)
                    {
                        data.Rank = totalRank;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.InputNumeric("##Completed", ref completed))
                {
                    if (data.Completed != completed)
                    {
                        data.Completed = completed;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.InputNumeric("##CollaredRank", ref collaredRank))
                {
                    collaredRank = Math.Clamp(collaredRank, byte.MinValue, PlayerData.CollaredStartingRank);
                    if (data.CollaredRank != collaredRank)
                    {
                        data.CollaredRank = collaredRank;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.InputNumeric("##OrcaRank", ref orcaRank))
                {
                    orcaRank = Math.Clamp(orcaRank, byte.MinValue, PlayerData.OrcaStartingRank);
                    if (data.OrcaRank != orcaRank)
                    {
                        data.OrcaRank = orcaRank;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.InputNumeric("##Coam", ref coam))
                {
                    if (data.Coam != coam)
                    {
                        data.Coam = coam;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGui.InputTextWithHint("##PlayTime", "00:00:00", ref playTimeStr, 255))
                {
                    if (oldPlayTimeStr != playTimeStr)
                    {
                        float newPlayTimeSeconds = ParseTimeString(playTimeStr, playTime);
                        if (data.PlayTimeSeconds != newPlayTimeSeconds)
                        {
                            data.PlayTimeSeconds = newPlayTimeSeconds;
                        }
                    }
                }

                ImGui.EndTable();
            }
        }

        private static string TimeToString(float seconds)
        {
            float hour = seconds / 3600;
            float minute = seconds / 60 % 60;
            float second = seconds % 60;
            return $"{hour:00}:{minute:00}:{second:00}";
        }

        private static float ParseTimeString(string time, float defaultValue)
        {
            var sb = new StringBuilder(time);
            for (int i = sb.Length - 1; i >= 0; i--)
            {
                if (sb[i] != ':' && !char.IsNumber(sb[i]))
                {
                    sb.Remove(i, 1);
                }
            }

            string cleaned = sb.ToString();
            string[] numbers = cleaned.Split(':');
            if (numbers.Length != 3)
            {
                return defaultValue;
            }

            if (!float.TryParse(numbers[0], out float hour))
            {
                return defaultValue;
            }

            if (!float.TryParse(numbers[1], out float minute))
            {
                return defaultValue;
            }

            if (!float.TryParse(numbers[2], out float second))
            {
                return defaultValue;
            }

            return (hour * 3600) + (minute * 60) + second;
        }

        public void Load(string path)
        {
            try
            {
                var data = PlayerData.Read(path);
                Load(data);
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Error: Player Data load failed: {ex.Message}");
            }
        }

        public void Load(PlayerData data)
        {
            Data = data;
        }

        public PlayerData GetData()
            => Data;
    }
}

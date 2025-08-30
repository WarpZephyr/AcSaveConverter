using AcSaveConverter.Editors.AcfaEditor.Utilities;
using AcSaveConverter.Editors.Framework;
using AcSaveConverter.Interface;
using AcSaveConverter.Logging;
using AcSaveFormats.ArmoredCoreForAnswer;
using ImGuiNET;
using System;

namespace AcSaveConverter.Editors.AcfaEditor.Views
{
    public class GameProgressView
    {
        private GameProgress Data;

        public GameProgressView()
        {
            Data = new GameProgress();
        }

        public void Display()
        {
            EditorDecorator.SetupWindow();
            if (ImGui.Begin("Game Progress"))
            {
                ShowProperties(Data);
            }

            ImGui.End();
        }

        private void ShowProperties(GameProgress data)
        {
            if (ImGui.BeginTable("GameProgressTable", 2, ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthStretch, 2.0f); // Default twice larger

                // Variables
                var gameCompletions = data.GameCompletions;
                var showIntro = data.ShowIntro;
                var dataPacksUnlocked = data.DataPacksUnlocked;
                bool collaredArenaUnlocked = dataPacksUnlocked[0];
                bool orcaArenaUnlocked = dataPacksUnlocked[1];
                var frsAmount = data.FrsAmount;

                // Name Column
                ImGui.TableNextColumn();
                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Game Completions");
                ImGuiEx.ShowHoverTooltip("The recorded number of times the game has been completed.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Show Intro");
                ImGuiEx.ShowHoverTooltip("Whether or not to show the intro upon loading the save.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Collared Arena Unlocked");
                ImGuiEx.ShowHoverTooltip("Whether or not the collared arena is unlocked.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Orca Arena Unlocked");
                ImGuiEx.ShowHoverTooltip("Whether or not the orca arena is unlocked.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Frs Amount");
                ImGuiEx.ShowHoverTooltip("The amount of FRS points the save has unlocked.");

                // Value Column
                ImGui.TableNextColumn();
                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.InputUInt16("##GameCompletions", ref gameCompletions))
                {
                    if (data.GameCompletions != gameCompletions)
                    {
                        data.GameCompletions = gameCompletions;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGui.Checkbox("##ShowIntro", ref showIntro))
                {
                    if (data.ShowIntro != showIntro)
                    {
                        data.ShowIntro = showIntro;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGui.Checkbox("##CollaredArenaUnlocked", ref collaredArenaUnlocked))
                {
                    if (data.DataPacksUnlocked[0] != collaredArenaUnlocked)
                    {
                        data.DataPacksUnlocked[0] = collaredArenaUnlocked;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGui.Checkbox("##OrcaArenaUnlocked", ref orcaArenaUnlocked))
                {
                    if (data.DataPacksUnlocked[1] != orcaArenaUnlocked)
                    {
                        data.DataPacksUnlocked[1] = orcaArenaUnlocked;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGui.InputInt("##FrsAmount", ref frsAmount))
                {
                    frsAmount = Math.Clamp(frsAmount, 0, GameProgress.MaxFrsCount);
                    if (data.FrsAmount != frsAmount)
                    {
                        data.FrsAmount = frsAmount;
                    }
                }

                ImGui.EndTable();
            }
        }

        public void Load(string path)
        {
            try
            {
                var data = GameProgress.Read(path);
                Load(data);
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Error: Game Progress load failed: {ex.Message}");
            }
        }

        public void Load(GameProgress data)
        {
            GameProgressValidator.Validate(data);
            Data = data;
        }

        public GameProgress GetData()
            => Data;
    }
}

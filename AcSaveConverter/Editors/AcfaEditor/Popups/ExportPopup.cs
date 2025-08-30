using AcSaveConverter.Configuration;
using AcSaveConverter.Editors.AcfaEditor.Data;
using AcSaveConverter.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;

namespace AcSaveConverter.Editors.AcfaEditor.Popups
{
    public class ExportPopup
    {
        private static readonly Dictionary<GameType, Dictionary<RegionType, string>> ExportHintCache;

        private GameType SelectedGame;
        private PlatformType SelectedPlatform;
        private RegionType SelectedRegion;
        private ExportKind SelectedExportKind;
        private string ExportName;
        private string ExportHint;
        private bool SetInputFocus;

        public bool IsOpen { get; set; }
        public bool OpenPopup { get; set; }

        public event EventHandler<ExportEventArgs>? ExportPressed;

        static ExportPopup()
        {
            var acfaHintCache = new Dictionary<RegionType, string>
            {
                { RegionType.US, "BLUS30187" },
                { RegionType.JP, "BLJM60066" },
                { RegionType.EU, "BLES00370" }
            };

            var hintCache = new Dictionary<GameType, Dictionary<RegionType, string>>
            {
                {GameType.ArmoredCoreForAnswer, acfaHintCache }
            };

            ExportHintCache = hintCache;
        }

        public ExportPopup()
        {
            ExportName = string.Empty;
            ExportHint = "GAMEDATXXXXXXXXXXXXXXX";
            RebuildExportHint();
        }

        public void Display()
        {
            ImGui.PushID(nameof(ExportPopup));

            if (OpenPopup)
            {
                ImGui.OpenPopup("Export Save");
                IsOpen = true;
                OpenPopup = false;
                SetInputFocus = true;
            }

            bool open = IsOpen;
            if (ImGui.BeginPopupModal("Export Save", ref open))
            {
                ShowPopup(ref open);
                ImGui.EndPopup();
            }

            IsOpen = open;
            ImGui.PopID();
        }

        private void ShowPopup(ref bool open)
        {
            if (ImGui.BeginTable("ExportTable", 2, ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthStretch, 2.0f); // Default twice larger

                // Variables
                string exportName = ExportName;
                var selectedGame = SelectedGame;
                var selectedPlatform = SelectedPlatform;
                var selectedRegion = SelectedRegion;
                var selectedExportKind = SelectedExportKind;

                // Name Column
                ImGui.TableNextColumn();
                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Name");
                ImGuiEx.ShowHoverTooltip("The save name.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Game");
                ImGuiEx.ShowHoverTooltip("The export game.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Platform");
                ImGuiEx.ShowHoverTooltip("The export platform.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Region");
                ImGuiEx.ShowHoverTooltip("The export region.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Export Kind");
                ImGuiEx.ShowHoverTooltip("The kind of export to perform.");

                // Value Column
                ImGui.TableNextColumn();

                var colWidth = ImGui.GetColumnWidth();
                ImGui.AlignTextToFramePadding();
                ImGui.SetNextItemWidth(colWidth);
                if (ImGui.InputTextWithHint("##ExportName", ExportHint, ref exportName, 48))
                {
                    if (ExportName != exportName)
                    {
                        ExportName = exportName;
                    }
                }

                if (SetInputFocus)
                {
                    ImGui.SetKeyboardFocusHere(0);
                    SetInputFocus = false;
                }

                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.ComboEnum("##Game", ref selectedGame))
                {
                    if (SelectedGame != selectedGame)
                    {
                        SelectedGame = selectedGame;
                        RebuildExportHint();
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.ComboEnum("##Platform", ref selectedPlatform))
                {
                    if (SelectedPlatform != selectedPlatform)
                    {
                        SelectedPlatform = selectedPlatform;
                        RebuildExportHint();
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.ComboEnum("##Region", ref selectedRegion))
                {
                    if (SelectedRegion != selectedRegion)
                    {
                        SelectedRegion = selectedRegion;
                        RebuildExportHint();
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.ComboEnum("##ExportKind", ref selectedExportKind))
                {
                    if (SelectedExportKind != selectedExportKind)
                    {
                        SelectedExportKind = selectedExportKind;
                        RebuildExportHint();
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGui.Button("Export"))
                {
                    open = false;
                    OnExportPressed();
                }
                ImGuiEx.ShowHoverTooltip("Exports the save.");

                ImGui.EndTable();
            }
        }

        private void RebuildExportHint()
        {
            var game = SelectedGame;
            var platform = SelectedPlatform;
            var region = SelectedRegion;

            string kind = string.Empty;
            if (SelectedExportKind == ExportKind.GameData)
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
            else if (SelectedExportKind == ExportKind.DesignDocument)
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
            else if (SelectedExportKind == ExportKind.Paint)
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

            ExportHint = hint;
        }

        private void OnExportPressed()
        {
            bool reset = false;
            if (ExportName == string.Empty)
            {
                reset = true;
                ExportName = ExportHint;
            }

            RebuildExportHint();
            ExportPressed?.Invoke(this, new ExportEventArgs(ExportName, SelectedGame, SelectedPlatform, SelectedRegion, SelectedExportKind));

            if (reset)
            {
                ExportName = string.Empty;
                RebuildExportHint();
            }
        }
    }
}

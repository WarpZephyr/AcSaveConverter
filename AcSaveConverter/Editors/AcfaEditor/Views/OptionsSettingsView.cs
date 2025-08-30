using AcSaveConverter.Editors.Framework;
using AcSaveConverter.Interface;
using AcSaveFormats.ArmoredCoreForAnswer;
using ImGuiNET;
using System;

namespace AcSaveConverter.Editors.AcfaEditor.Views
{
    public class OptionsSettingsView
    {
        private OptionsSettings Data;

        public OptionsSettingsView()
        {
            Data = new OptionsSettings();
        }

        public void Display()
        {
            EditorDecorator.SetupWindow();
            if (ImGui.Begin("Options/Settings"))
            {
                ShowProperties(Data);
            }

            ImGui.End();
        }

        private void ShowProperties(OptionsSettings data)
        {
            if (ImGui.BeginTable("OptionsSettingsTable", 2, ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthStretch, 2.0f); // Default twice larger

                // Variables
                var vibration = data.Vibration;
                var brightness = data.Brightness;
                var volumeMusic = data.VolumeMusic;
                var volumeEffects = data.VolumeEffects;
                var volumeVoice = data.VolumeVoice;
                var autoFlags = data.AutoFlags;
                var autoSighting = (data.AutoFlags & OptionsSettings.AutoOptionFlags.AutoSighting) != 0;
                var autoBoost = (data.AutoFlags & OptionsSettings.AutoOptionFlags.AutoBoost) != 0;
                var autoSwitch = (data.AutoFlags & OptionsSettings.AutoOptionFlags.AutoSwitch) != 0;
                var radarType = data.RadarType;
                var cockpitColorId = data.CockpitColorId;
                var regulation = data.Regulation;

                // Name Column
                ImGui.TableNextColumn();
                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Vibration");
                ImGuiEx.ShowHoverTooltip("The vibration setting.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Brightness");
                ImGuiEx.ShowHoverTooltip("The brightness setting.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Music Volume");
                ImGuiEx.ShowHoverTooltip("The music volume setting.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Effects Volume");
                ImGuiEx.ShowHoverTooltip("The effects volume setting.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Voice Volume");
                ImGuiEx.ShowHoverTooltip("The voice volume setting.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Auto Sighting");
                ImGuiEx.ShowHoverTooltip("The auto sighting setting.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Auto Boost");
                ImGuiEx.ShowHoverTooltip("The auto boost setting.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Auto Switch");
                ImGuiEx.ShowHoverTooltip("The auto switch setting.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Radar Type");
                ImGuiEx.ShowHoverTooltip("The radar type setting.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Cockpit Color Id");
                ImGuiEx.ShowHoverTooltip("The cockpit color setting.");

                ImGui.AlignTextToFramePadding();
                ImGuiEx.RightAlignedColumnText("Regulation");
                ImGuiEx.ShowHoverTooltip("The regulation setting.");

                // Value Column
                ImGui.TableNextColumn();
                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.InputNumeric("##Vibration", ref vibration))
                {
                    vibration = Math.Clamp(vibration, byte.MinValue, OptionsSettings.MaxVibration);
                    if (data.Vibration != vibration)
                    {
                        data.Vibration = vibration;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.InputNumeric("##Brightness", ref brightness))
                {
                    brightness = Math.Clamp(brightness, byte.MinValue, OptionsSettings.MaxBrightness);
                    if (data.Brightness != brightness)
                    {
                        data.Brightness = brightness;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.InputNumeric("##VolumeMusic", ref volumeMusic))
                {
                    volumeMusic = Math.Clamp(volumeMusic, byte.MinValue, OptionsSettings.MaxVolume);
                    if (data.VolumeMusic != volumeMusic)
                    {
                        data.VolumeMusic = volumeMusic;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.InputNumeric("##VolumeEffects", ref volumeEffects))
                {
                    volumeEffects = Math.Clamp(volumeEffects, byte.MinValue, OptionsSettings.MaxVolume);
                    if (data.VolumeEffects != volumeEffects)
                    {
                        data.VolumeEffects = volumeEffects;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.InputNumeric("##VolumeVoice", ref volumeVoice))
                {
                    volumeVoice = Math.Clamp(volumeVoice, byte.MinValue, OptionsSettings.MaxVolume);
                    if (data.VolumeVoice != volumeVoice)
                    {
                        data.VolumeVoice = volumeVoice;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGui.Checkbox("##AutoSighting", ref autoSighting))
                {
                    var current = (data.AutoFlags & OptionsSettings.AutoOptionFlags.AutoSighting) != 0;
                    if (current != autoSighting)
                    {
                        if (autoSighting)
                        {
                            data.AutoFlags |= OptionsSettings.AutoOptionFlags.AutoSighting;
                        }
                        else
                        {
                            data.AutoFlags &= ~OptionsSettings.AutoOptionFlags.AutoSighting;
                        }

                        autoFlags = data.AutoFlags;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGui.Checkbox("##AutoBoost", ref autoBoost))
                {
                    var current = (data.AutoFlags & OptionsSettings.AutoOptionFlags.AutoBoost) != 0;
                    if (current != autoBoost)
                    {
                        if (autoBoost)
                        {
                            data.AutoFlags |= OptionsSettings.AutoOptionFlags.AutoBoost;
                        }
                        else
                        {
                            data.AutoFlags &= ~OptionsSettings.AutoOptionFlags.AutoBoost;
                        }

                        autoFlags = data.AutoFlags;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGui.Checkbox("##AutoSwitch", ref autoSwitch))
                {
                    var current = (data.AutoFlags & OptionsSettings.AutoOptionFlags.AutoSwitch) != 0;
                    if (current != autoSwitch)
                    {
                        if (autoSwitch)
                        {
                            data.AutoFlags |= OptionsSettings.AutoOptionFlags.AutoSwitch;
                        }
                        else
                        {
                            data.AutoFlags &= ~OptionsSettings.AutoOptionFlags.AutoSwitch;
                        }

                        autoFlags = data.AutoFlags;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGuiEx.ComboEnum("##RadarType", ref radarType))
                {
                    if (data.RadarType != radarType)
                    {
                        data.RadarType = radarType;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGui.InputInt("##CockpitColorId", ref cockpitColorId))
                {
                    if (data.CockpitColorId != cockpitColorId)
                    {
                        data.CockpitColorId = cockpitColorId;
                    }
                }

                ImGui.AlignTextToFramePadding();
                if (ImGui.InputTextWithHint("##Regulation", "1.20", ref regulation, 15))
                {
                    if (data.Regulation != regulation)
                    {
                        data.Regulation = regulation;
                    }
                }

                ImGui.EndTable();
            }
        }

        public void Load(OptionsSettings data)
        {
            Data = data;
        }

        public OptionsSettings GetData()
            => Data;
    }
}

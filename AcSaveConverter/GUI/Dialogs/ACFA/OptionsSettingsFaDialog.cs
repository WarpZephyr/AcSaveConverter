using AcSaveConverter.Graphics;
using AcSaveConverter.GUI.Dialogs.Tabs;
using AcSaveConverter.IO;
using AcSaveFormats.ACFA;
using ImGuiNET;
using System;

namespace AcSaveConverter.GUI.Dialogs.ACFA
{
    public class OptionsSettingsFaDialog : IDataTab
    {
        private readonly ImGuiGraphicsContext Graphics;
        public string Name { get; set; }

        public OptionsSettings OptionsSettings { get; private set; }

        public OptionsSettingsFaDialog(string name, ImGuiGraphicsContext graphics, OptionsSettings data)
        {
            Graphics = graphics;
            Name = name;

            OptionsSettings = data;
        }

        #region Render

        public void Render()
        {
            ImGui.PushID(nameof(GameProgressFaDialog));
            Render_MenuBar();
            Render_OptionsSettings();
            ImGui.PopID();
        }

        void Render_MenuBar()
        {
            if (ImGui.BeginMenuBar())
            {
                Render_FileMenu();
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

        void Render_OptionsSettings()
        {
            if (ImGui.BeginTable("OptionsSettingsTable", 2, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.BordersInnerV))
            {
                ImGui.TableNextColumn();
                ImGui.Text("Vibration");
                ImGui.TableNextColumn();
                ImGui.Text(OptionsSettings.Vibration.ToString());
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Brightness");
                ImGui.TableNextColumn();
                ImGui.Text(OptionsSettings.Brightness.ToString());
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Music Volume");
                ImGui.TableNextColumn();
                ImGui.Text(OptionsSettings.VolumeMusic.ToString());
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Effects Volume");
                ImGui.TableNextColumn();
                ImGui.Text(OptionsSettings.VolumeEffects.ToString());
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Voice Volume");
                ImGui.TableNextColumn();
                ImGui.Text(OptionsSettings.VolumeVoice.ToString());
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Auto Sighting");
                ImGui.TableNextColumn();
                ImGui.Text(((OptionsSettings.AutoFlags & OptionsSettings.AutoOptionFlags.AutoSighting) != 0) ? "ON" : "OFF");
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Auto Boost");
                ImGui.TableNextColumn();
                ImGui.Text(((OptionsSettings.AutoFlags & OptionsSettings.AutoOptionFlags.AutoBoost) != 0) ? "ON" : "OFF");
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Auto Switch");
                ImGui.TableNextColumn();
                ImGui.Text(((OptionsSettings.AutoFlags & OptionsSettings.AutoOptionFlags.AutoSwitch) != 0) ? "ON" : "OFF");
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Radar Type");
                ImGui.TableNextColumn();
                ImGui.Text(OptionsSettings.RadarType.ToString());
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Regulation");
                ImGui.TableNextColumn();
                ImGui.Text(OptionsSettings.Regulation);
                ImGui.EndTable();
            }
        }

        #endregion

        #region Data

        public void Load_Data(OptionsSettings data)
        {
            OptionsSettings = data;
        }

        public void Load_Data(string path)
        {
            OptionsSettings = OptionsSettings.Read(path);
        }

        public bool IsData(string file)
        {
            return file.EndsWith("OSET.DAT", StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

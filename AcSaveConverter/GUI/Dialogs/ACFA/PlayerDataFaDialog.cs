using AcSaveConverter.Graphics;
using AcSaveConverter.GUI.Dialogs.Tabs;
using AcSaveConverter.IO;
using AcSaveFormats.ACFA;
using ImGuiNET;
using System;

namespace AcSaveConverter.GUI.Dialogs.ACFA
{
    internal class PlayerDataFaDialog : IDataTab
    {
        private readonly ImGuiGraphicsContext Graphics;
        public string Name { get; set; }

        public PlayerData PlayerData { get; private set; }
        private string PlayTimeStringCache;

        public PlayerDataFaDialog(string name, ImGuiGraphicsContext graphics, PlayerData data)
        {
            Graphics = graphics;
            Name = name;

            PlayerData = data;
            PlayTimeStringCache = GetSecondsTimeString(PlayerData.PlayTimeSeconds);
        }

        #region Render

        public void Render()
        {
            ImGui.PushID(nameof(PlayerDataFaDialog));
            Render_MenuBar();
            Render_PlayerData();
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

        void Render_PlayerData()
        {
            if (ImGui.BeginTable("PlayerDataTable", 2, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.BordersInnerV))
            {
                ImGui.TableNextColumn();
                ImGui.Text("Lynx Name");
                ImGui.TableNextColumn();
                ImGui.Text(PlayerData.LynxName);
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Total Rank");
                ImGui.TableNextColumn();
                ImGui.Text(PlayerData.Rank.ToString());
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Completed");
                ImGui.TableNextColumn();
                ImGui.Text(PlayerData.Completed.ToString());
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Collared Rank");
                ImGui.TableNextColumn();
                ImGui.Text(PlayerData.CollaredRank.ToString());
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Orca Rank");
                ImGui.TableNextColumn();
                ImGui.Text(PlayerData.OrcaRank.ToString());
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Coam");
                ImGui.TableNextColumn();
                ImGui.Text(PlayerData.Coam.ToString());
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Play Time");
                ImGui.TableNextColumn();
                ImGui.Text(PlayTimeStringCache);
                ImGui.EndTable();
            }
        }

        #endregion

        #region Data

        public void Load_Data(PlayerData data)
        {
            PlayerData = data;
            PlayTimeStringCache = GetSecondsTimeString(PlayerData.PlayTimeSeconds);
        }

        public void Load_Data(string path)
        {
            PlayerData = PlayerData.Read(path);
            PlayTimeStringCache = GetSecondsTimeString(PlayerData.PlayTimeSeconds);
        }

        public bool IsData(string file)
        {
            return file.EndsWith("PDATA.DAT", StringComparison.InvariantCultureIgnoreCase);
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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

using AcSaveConverter.Graphics;
using AcSaveConverter.GUI.Dialogs.Tabs;
using AcSaveConverter.IO;
using AcSaveConverter.Logging;
using AcSaveFormats.ACFA;
using ImGuiNET;
using System;

namespace AcSaveConverter.GUI.Dialogs.ACFA
{
    public class GameProgressFaDialog : IDataTab
    {
        private readonly ImGuiGraphicsContext Graphics;
        public string Name { get; set; }
        public string DataType
            => "Game Progress";

        public GameProgress GameProgress { get; private set; }

        public GameProgressFaDialog(string name, ImGuiGraphicsContext graphics, GameProgress data)
        {
            Graphics = graphics;
            Name = name;

            GameProgress = data;
            Validate_GameProgress();
        }

        #region Render

        public void Render()
        {
            ImGui.PushID(nameof(GameProgressFaDialog));
            Render_MenuBar();
            Render_GameProgress();
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

        void Render_GameProgress()
        {
            if (ImGui.BeginTable("GameProgressTable", 2, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.BordersInnerV))
            {
                ImGui.TableNextColumn();
                ImGui.Text("Game Completions");
                ImGui.TableNextColumn();
                ImGui.Text(GameProgress.GameCompletions.ToString());
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Collared Arena Unlocked");
                ImGui.TableNextColumn();
                ImGui.Text(GameProgress.DataPacksUnlocked[0].ToString());
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Orca Arena Unlocked");
                ImGui.TableNextColumn();
                ImGui.Text(GameProgress.DataPacksUnlocked[1].ToString());
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("FRS Amount");
                ImGui.TableNextColumn();
                ImGui.Text(GameProgress.FrsAmount.ToString());
                ImGui.EndTable();
            }
        }

        #endregion

        #region Data

        public void Load_Data(GameProgress data)
        {
            GameProgress = data;
            Validate_GameProgress();
        }

        public void Load_Data(string path)
        {
            try
            {
                Log.WriteLine($"Loading {DataType} from path: \"{path}\"");
                Load_Data(GameProgress.Read(path));
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Failed to load {DataType} from path \"{path}\": {ex}");
            }
        }

        public bool IsData(string path)
        {
            return path.EndsWith("GPROG.DAT", StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Validation

        void Validate_GameProgress()
        {
            var frsAmount = GameProgress.FrsAmount;
            var clamped = Math.Clamp(GameProgress.FrsAmount, 0, 442);
            if (frsAmount != clamped)
            {
                Log.WriteLine($"Detected invalid FRS amount: {frsAmount}");
                Log.WriteLine($"Clamping FRS amount to: {clamped}");
            }

            GameProgress.FrsAmount = clamped;
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

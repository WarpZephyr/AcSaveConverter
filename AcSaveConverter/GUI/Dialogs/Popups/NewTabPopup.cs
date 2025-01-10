using AcSaveConverterImGui.Graphics;
using AcSaveConverterImGui.GUI.Dialogs.Tabs;
using ImGuiNET;
using System.Numerics;

namespace AcSaveConverterImGui.GUI.Dialogs.Popups
{
    public class NewTabPopup : IPopup
    {
        private string NewTabName;
        private readonly List<ISaveTab> FileTabs;
        private readonly ImGuiGraphicsContext Graphics;
        private GameType SelectedGame;
        private bool SetInputFocus;

        public bool Open { get; set; }
        public bool OpenPopup { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        public NewTabPopup(List<ISaveTab> tabs, ImGuiGraphicsContext graphics)
        {
            NewTabName = string.Empty;
            FileTabs = tabs;
            Graphics = graphics;
            Position = new Vector2(20, 20);
            Size = new Vector2(210, 110);
        }

        #region Render

        public void Render()
        {
            ImGui.PushID(nameof(NewTabPopup));
            ImGui.SetNextWindowPos(Position, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSize(Size);
            if (OpenPopup)
            {
                ImGui.OpenPopup("New Tab");
                OpenPopup = false;
                Open = true;
                SetInputFocus = true;
            }

            bool open = Open;
            if (ImGui.BeginPopupModal("New Tab", ref open, ImGuiWindowFlags.NoResize))
            {
                Render_Table(ref open);
                ImGui.EndPopup();
            }

            Open = open;
            ImGui.PopID();
        }

        void Render_Table(ref bool open)
        {
            if (ImGui.BeginTable("NewTabTable", 2, ImGuiTableFlags.BordersInnerV))
            {
                ImGui.TableSetupColumn("##LabelColumn", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("##ValueColumn", ImGuiTableColumnFlags.WidthStretch);

                ImGui.TableNextColumn();
                ImGui.Text("Name");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1);
                if (SetInputFocus)
                {
                    ImGui.SetKeyboardFocusHere(0);
                    SetInputFocus = false;
                }

                ImGui.InputTextWithHint("##Name", "New Tab", ref NewTabName, 32);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Game");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1);
                ImGuiEx.ComboEnum("##Game", ref SelectedGame);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.TableNextColumn();
                if (ImGui.Button("Create"))
                {
                    Press_Create();
                    open = false;
                }

                if (ImGui.IsKeyPressed(ImGuiKey.Enter) || ImGui.IsKeyPressed(ImGuiKey.KeypadEnter))
                {
                    Press_Create();
                    open = false;
                }

                ImGui.EndTable();
            }
        }

        #endregion

        #region Press

        public void Press_Create()
        {
            bool reset = false;
            if (NewTabName == string.Empty)
            {
                reset = true;
                NewTabName = "New Tab";
            }

            switch (SelectedGame)
            {
                case GameType.ACFA:
                    FileTabs.Add(new SaveTabFa(NewTabName, Graphics));
                    break;
            }

            if (reset)
            {
                NewTabName = string.Empty;
            }
        }

        #endregion
    }
}

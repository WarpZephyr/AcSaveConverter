using ImGuiNET;
using System.Numerics;

namespace AcSaveConverterImGui.GUI.Dialogs.Popups
{
    public class ExportPopup : IPopup
    {
        private GameType SelectedGame;
        private PlatformType SelectedPlatform;
        private RegionType SelectedRegion;
        private string ExportName;
        private readonly ExportEventArgs ExportEventArgs;
        private bool SetInputFocus;

        public string ExportHint { get; set; }
        public bool Open { get; set; }
        public bool OpenPopup { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        public event EventHandler<ExportEventArgs>? PopupOpened;
        public event EventHandler<ExportEventArgs>? SettingsChanged;
        public event EventHandler<ExportEventArgs>? ExportPressed;

        public ExportPopup()
        {
            Position = new Vector2(20, 20);
            Size = new Vector2(330, 150);
            ExportName = string.Empty;
            ExportHint = "GAMEDATXXXXXXXXXXXXXXX";
            ExportEventArgs = new ExportEventArgs(ExportName, SelectedGame, SelectedPlatform, SelectedRegion);
        }

        #region Render

        public void Render()
        {
            ImGui.PushID(nameof(ExportPopup));
            ImGui.SetNextWindowPos(Position, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSize(Size);

            if (OpenPopup)
            {
                ImGui.OpenPopup("Export Save");
                Open = true;
                OpenPopup = false;
                SetInputFocus = true;
                OnPopupOpened();
            }

            bool open = Open;
            if (ImGui.BeginPopupModal("Export Save", ref open, ImGuiWindowFlags.NoResize))
            {
                Render_Export(ref open);
                ImGui.EndPopup();
            }

            Open = open;
            ImGui.PopID();
        }

        void Render_Export(ref bool open)
        {
            if (ImGui.BeginTable("ExportTable", 2, ImGuiTableFlags.BordersInnerV))
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

                ImGui.InputTextWithHint("##Name", ExportHint, ref ExportName, 32);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Game");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1);
                if (ImGuiEx.ComboEnum("##Game", ref SelectedGame))
                {
                    OnSettingsChanged();
                }

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Platform");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1);
                if (ImGuiEx.ComboEnum("##Platform", ref SelectedPlatform))
                {
                    OnSettingsChanged();
                }

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Region");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1);
                if (ImGuiEx.ComboEnum("##Region", ref SelectedRegion))
                {
                    OnSettingsChanged();
                }

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.TableNextColumn();
                if (ImGui.Button("Export"))
                {
                    open = false;
                    OnExportPressed();
                }

                ImGui.EndTable();
            }
        }

        #endregion

        #region Events

        void UpdateExportEventArgs()
        {
            ExportEventArgs.ExportName = ExportName;
            ExportEventArgs.Game = SelectedGame;
            ExportEventArgs.Platform = SelectedPlatform;
            ExportEventArgs.Region = SelectedRegion;
        }

        void OnPopupOpened()
        {
            UpdateExportEventArgs();
            PopupOpened?.Invoke(this, ExportEventArgs);
        }

        void OnSettingsChanged()
        {
            UpdateExportEventArgs();
            SettingsChanged?.Invoke(this, ExportEventArgs);
        }

        void OnExportPressed()
        {
            bool reset = false;
            if (ExportName == string.Empty)
            {
                reset = true;
                ExportName = ExportHint;
            }

            UpdateExportEventArgs();
            ExportPressed?.Invoke(this, ExportEventArgs);

            if (reset)
            {
                ExportName = string.Empty;
                UpdateExportEventArgs();
            }
        }

        #endregion
    }
}

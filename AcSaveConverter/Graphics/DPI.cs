using System;

namespace AcSaveConverter.Graphics
{
    public class DPI
    {
        private readonly UI UI;

        private const float DefaultDpi = 96f;
        private float _dpi = DefaultDpi;

        public EventHandler? UIScaleChanged;

        public float Dpi
        {
            get => _dpi;
            set
            {
                if (Math.Abs(_dpi - value) < 0.0001f) return; // Skip doing anything if no difference

                _dpi = value;
                if (UI.ScaleByDPI)
                    UIScaleChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        internal DPI(UI ui)
        {
            UI = ui;
        }

        public void UpdateDPI(float dpi)
        {
            Dpi = dpi;
        }

        public float GetUIScale()
        {
            var scale = UI.UIScale;
            if (UI.ScaleByDPI)
                scale = scale / DefaultDpi * Dpi;
            return scale;
        }
    }
}

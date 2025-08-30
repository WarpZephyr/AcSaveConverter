using System;

namespace AcSaveConverter.Interface
{
    internal class DPI
    {
        private const float DefaultDpi = 96f;
        internal static DPI Current;
        private float _dpi;
        public EventHandler? UIScaleChanged;
        public float Dpi
        {
            get => _dpi;
            set
            {
                if (Math.Abs(_dpi - value) < 0.0001f)
                    return; // Skip doing anything if no difference

                _dpi = value;
                if (UI.Current.ScaleByDPI)
                    UIScaleChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        static DPI()
        {
            Current = new DPI();
        }

        public DPI()
        {
            _dpi = DefaultDpi;
        }

        public void UpdateDPI(float dpi)
        {
            Dpi = dpi;
        }

        public float GetUIScale()
        {
            var scale = UI.Current.UIScale;
            if (UI.Current.ScaleByDPI)
                scale = scale / DefaultDpi * Dpi;
            return scale;
        }
    }
}

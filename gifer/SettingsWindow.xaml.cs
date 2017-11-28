using System;
using System.Windows;
using System.Windows.Media;

namespace gifer {
    public partial class SettingsWindow : Window {
        private readonly Action<BitmapScalingMode> _onRenderingModeChanged;
        private readonly Action<Language> _onLanguagehanged;

        public SettingsWindow(Action<BitmapScalingMode> onRenderingModeChanged, Action<Language> onLanguagehanged) {
            _onRenderingModeChanged = onRenderingModeChanged;
            _onLanguagehanged = onLanguagehanged;

            InitializeComponent();
        }
    }
}

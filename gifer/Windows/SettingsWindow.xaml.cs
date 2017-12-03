using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using gifer.Utils;

namespace gifer {
    public partial class SettingsWindow : Window {
        private readonly Action<BitmapScalingMode> _onRenderingModeChanged;
        private readonly Action<Language> _onLanguagehanged;
        private Language _language;

        public SettingsWindow(Action<BitmapScalingMode> onRenderingModeChanged, Action<Language> onLanguagehanged, Language language) {
            _onRenderingModeChanged = onRenderingModeChanged;
            _onLanguagehanged = onLanguagehanged;
            _language = language;

            InitializeComponent();

            OnLanguageChanged(_language);

            this.Settings_LanguageComboBox.SelectionChanged += ComboBox_SelectionChanged;
        }

        public void OnLanguageChanged(Language language) {

            this.Settings_RenderingModeLabel.Content = LanguageDictionary.GetString(language, nameof(Settings_RenderingModeLabel));
            this.Settings_RenderingModeComboBox.Items.Clear();
            this.Settings_RenderingModeComboBox.Items.Add(LanguageDictionary.GetString(language, nameof(Settings_RenderingModeLinear)));
            this.Settings_RenderingModeComboBox.Items.Add(LanguageDictionary.GetString(language, nameof(Settings_RenderingModeFant)));
            this.Settings_RenderingModeComboBox.Items.Add(LanguageDictionary.GetString(language, nameof(Settings_RenderingModeNearestNeighbor)));
            this.Settings_RenderingModeComboBox.SelectedIndex = 1;
            this.Settings_LanguageLabel.Content = LanguageDictionary.GetString(language, nameof(Settings_LanguageLabel));
            this.Settings_LanguageComboBox.Items.Clear();
            this.Settings_LanguageComboBox.Items.Add(LanguageDictionary.GetString(nameof(Settings_Language_EN)));
            this.Settings_LanguageComboBox.Items.Add(LanguageDictionary.GetString(nameof(Settings_Language_RU)));
            this.Settings_LanguageComboBox.SelectedIndex = (int)_language;

            _onLanguagehanged(language);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.Settings_LanguageComboBox.SelectionChanged -= ComboBox_SelectionChanged;
            _language = (Language)((ComboBox)sender).SelectedIndex;
            OnLanguageChanged(_language);
            this.Settings_LanguageComboBox.SelectionChanged += ComboBox_SelectionChanged;
        }
    }
}

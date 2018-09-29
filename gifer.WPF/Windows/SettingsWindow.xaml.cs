using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using gifer.Languages;

namespace gifer {
    public partial class SettingsWindow : Window {
        private static readonly BitmapScalingMode[] ScalingModes = new[] { BitmapScalingMode.Linear, BitmapScalingMode.NearestNeighbor };

        private readonly Action<BitmapScalingMode> _onRenderingModeChanged;
        private readonly Action<Language> _onLanguageChanged;
		private readonly Action<bool> _onCheckForUpdateChanged;
		private readonly Action<bool> _onCenterOpenedImageChanged;
		private BitmapScalingMode _scalingMode;
        private Language _language;

        public SettingsWindow(
				Action<BitmapScalingMode> onRenderingModeChanged,
				Action<Language> onLanguageChanged,
				bool checkForUpdate, Action<bool> onCheckForUpdateChanged,
				bool centerOpenedImage, Action<bool> onCenterOpenedImageChanged,
				BitmapScalingMode scalingMode, Language language) {
            _onRenderingModeChanged = onRenderingModeChanged;
            _onLanguageChanged = onLanguageChanged;
			_onCheckForUpdateChanged = onCheckForUpdateChanged;
			_onCenterOpenedImageChanged = onCenterOpenedImageChanged;
			_scalingMode = scalingMode;
            _language = language;

            InitializeComponent();
            OnLanguageChanged(_language);
			this.Settings_CheckForUpdate.IsChecked = checkForUpdate;
			this.Settings_CenterOpenedImage.IsChecked = centerOpenedImage;


			this.Settings_RenderingModeComboBox.SelectionChanged += RenderingModeComboBox_SelectionChanged;
            this.Settings_LanguageComboBox.SelectionChanged += LanguageComboBox_SelectionChanged;
        }

        public void OnLanguageChanged(Language language) {
            this.Title = LanguageDictionary.GetString("Settings_Title", language);
            this.Settings_RenderingModeLabel.Content = LanguageDictionary.GetString(nameof(Settings_RenderingModeLabel), language);
            this.Settings_RenderingModeComboBox.Items.Clear();
            this.Settings_RenderingModeComboBox.Items.Add(LanguageDictionary.GetString(nameof(Settings_RenderingModeLinear), language));
            this.Settings_RenderingModeComboBox.Items.Add(LanguageDictionary.GetString(nameof(Settings_RenderingModeNearestNeighbor), language));
            this.Settings_RenderingModeComboBox.SelectedIndex = Array.IndexOf(ScalingModes, _scalingMode);
            this.Settings_LanguageLabel.Content = LanguageDictionary.GetString(nameof(Settings_LanguageLabel), language);
            this.Settings_LanguageComboBox.Items.Clear();
            this.Settings_LanguageComboBox.Items.Add(LanguageDictionary.GetString(nameof(Settings_Language_EN)));
            this.Settings_LanguageComboBox.Items.Add(LanguageDictionary.GetString(nameof(Settings_Language_RU)));
            this.Settings_LanguageComboBox.SelectedIndex = (int)_language;
            this.Settings_CheckForUpdate.Content = LanguageDictionary.GetString(nameof(Settings_CheckForUpdate), language);
            this.Settings_CenterOpenedImage.Content = LanguageDictionary.GetString(nameof(Settings_CenterOpenedImage), language);

            _onLanguageChanged(language);
        }

        private void RenderingModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.RemovedItems.Count != 1 || e.AddedItems.Count != 1) { // Item_Added/Removed by Items.Clear(), not Selection_Changed directly
                return;
            }
            _scalingMode = ScalingModes[this.Settings_RenderingModeComboBox.SelectedIndex];
            _onRenderingModeChanged(_scalingMode);
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.Settings_LanguageComboBox.SelectionChanged -= LanguageComboBox_SelectionChanged;
            _language = (Language)((ComboBox)sender).SelectedIndex;
            OnLanguageChanged(_language);
            this.Settings_LanguageComboBox.SelectionChanged += LanguageComboBox_SelectionChanged;
        }

        private void Settings_RenderingModeComboBox_MouseEnter(object sender, MouseEventArgs e) {
            this.Settings_RenderingModeComboBox.Focus();
        }

        private void Settings_LanguageComboBox_MouseEnter(object sender, MouseEventArgs e) {
            this.Settings_LanguageComboBox.Focus();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                this.DragMove();
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Right) {
                this.Close();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape || e.Key == Key.S) {
                this.Close();
            }
        }

		private void Settings_CheckForUpdate_Click(object sender, RoutedEventArgs e) {
			_onCheckForUpdateChanged((bool)this.Settings_CheckForUpdate.IsChecked);
		}

		private void Settings_CenterOpenedImage_Click(object sender, RoutedEventArgs e) {
			_onCenterOpenedImageChanged((bool)this.Settings_CenterOpenedImage.IsChecked);
		}
	}
}

﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using gifer.Utils;

namespace gifer {
    public partial class SettingsWindow : Window {
        private readonly Action<BitmapScalingMode> _onRenderingModeChanged;
        private readonly Action<Language> _onLanguagehanged;
        private BitmapScalingMode _scalingMode;
        private Language _language;

        public SettingsWindow(Action<BitmapScalingMode> onRenderingModeChanged, Action<Language> onLanguagehanged, BitmapScalingMode scalingMode, Language language) {
            _onRenderingModeChanged = onRenderingModeChanged;
            _onLanguagehanged = onLanguagehanged;
            _scalingMode = scalingMode;
            _language = language;

            InitializeComponent();

            OnLanguageChanged(_language);

            this.Settings_RenderingModeComboBox.SelectionChanged += RenderingModeComboBox_SelectionChanged;
            this.Settings_LanguageComboBox.SelectionChanged += LanguageComboBox_SelectionChanged;
        }

        public void OnLanguageChanged(Language language) {
            this.Settings_RenderingModeLabel.Content = LanguageDictionary.GetString(language, nameof(Settings_RenderingModeLabel));
            this.Settings_RenderingModeComboBox.Items.Clear();
            this.Settings_RenderingModeComboBox.Items.Add(LanguageDictionary.GetString(language, nameof(Settings_RenderingModeLinear)));
            this.Settings_RenderingModeComboBox.Items.Add(LanguageDictionary.GetString(language, nameof(Settings_RenderingModeNearestNeighbor)));
            this.Settings_RenderingModeComboBox.SelectedIndex = _scalingMode == BitmapScalingMode.NearestNeighbor ? 1 : 0;
            this.Settings_LanguageLabel.Content = LanguageDictionary.GetString(language, nameof(Settings_LanguageLabel));
            this.Settings_LanguageComboBox.Items.Clear();
            this.Settings_LanguageComboBox.Items.Add(LanguageDictionary.GetString(nameof(Settings_Language_EN)));
            this.Settings_LanguageComboBox.Items.Add(LanguageDictionary.GetString(nameof(Settings_Language_RU)));
            this.Settings_LanguageComboBox.SelectedIndex = (int)_language;

            _onLanguagehanged(language);
        }

        private void RenderingModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.RemovedItems.Count != 1 || e.AddedItems.Count != 1) { // Item_Added/Removed by Items.Clear(), not Selection_Changed directly
                return;
            }
            _scalingMode = (BitmapScalingMode)(this.Settings_RenderingModeComboBox.SelectedIndex + 1);
            _onRenderingModeChanged(_scalingMode);
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.Settings_LanguageComboBox.SelectionChanged -= LanguageComboBox_SelectionChanged;
            _language = (Language)((ComboBox)sender).SelectedIndex;
            OnLanguageChanged(_language);
            this.Settings_LanguageComboBox.SelectionChanged += LanguageComboBox_SelectionChanged;
        }

        private void Settings_RenderingModeComboBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
            this.Settings_RenderingModeComboBox.Focus();
        }

        private void Settings_LanguageComboBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
            this.Settings_LanguageComboBox.Focus();
        }
    }
}
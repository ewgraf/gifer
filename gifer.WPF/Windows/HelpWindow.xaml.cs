using System;
using System.Windows;
using System.Windows.Input;
using gifer.Languages;

namespace giferWpf {
    public partial class HelpWindow : Window {
        private Language _language;
        public bool ShowHelpAtStartup { get; private set; }

        public HelpWindow(bool showHelpAtStartUp, Language language) {
            _language = language;

            InitializeComponent();

            this.Title = LanguageDictionary.GetString("Help_Title", _language);
            this.checkBox1.IsChecked = showHelpAtStartUp;			
        }

        private void Window_Closing(object s, EventArgs e) {
            ShowHelpAtStartup = (bool)this.checkBox1.IsChecked;
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

        private void HelpForm_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape || e.Key == Key.H) {
                this.Close();
            }
        }
    }
}

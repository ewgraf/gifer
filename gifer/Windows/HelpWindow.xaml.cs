using System;
using System.Windows;
using System.Windows.Input;

namespace giferWpf {
    public partial class HelpWindow : Window {
        public bool ShowHelpAtStartup { get; private set; }

        public HelpWindow(bool showHelpAtStartUp) {
            InitializeComponent();

            this.checkBox1.IsChecked = showHelpAtStartUp;
        }

        private void Window_KeyUp(object s, KeyEventArgs e) {
            if (e.Key == Key.Escape) {
                this.Close();
            }
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
    }
}

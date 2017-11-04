using System;
using System.Linq;
using System.Text;
using System.Windows;

namespace giferWpf {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            if (e.Args != null && e.Args.Count() > 0) {
                this.Properties["Args"] = e.Args[0];
            }

            base.OnStartup(e);
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Windows;
using gifer.Utils;

namespace giferWpf {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override async void OnStartup(StartupEventArgs e) {
            // due to some mistery reason this should be at start
            // if move this Args setum to end - Args would be == null
            if (e.Args != null && e.Args.Count() > 0) {
                this.Properties["Args"] = e.Args[0];
            }

            string giferUpdateScrips = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gifer", "update.bat");
            if (!File.Exists(giferUpdateScrips)) {
                File.WriteAllText(giferUpdateScrips, "@echo off\necho updating gifer...\nset giferPath=%1\n@\"%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe\" -NoProfile -InputFormat None -ExecutionPolicy Bypass \"Stop-Process -processname gifer; ((New-Object System.Net.WebClient).DownloadFile('https://github.com/ewgraf/Gifer/releases/download/v1/gifer.exe', '%giferPath%'))");
            }

            if (ConfigHelper.CheckForUpdate()) {
				try {
					using (var client = new HttpClient()) {
						string version = await client.GetStringAsync("https://raw.githubusercontent.com/ewgraf/Gifer/master/version.txt");
						if (Gifer.MyVersion != version) {
							Process.Start(new ProcessStartInfo(giferUpdateScrips, Assembly.GetEntryAssembly().Location) {
								Verb = "runas"
							});
							Thread.Sleep(1000);
							Application.Current.Shutdown(); // in case script does not stop process in time
						}
					}
				} catch { // in case no internet
				}

			}

            base.OnStartup(e);
        }
    }
}

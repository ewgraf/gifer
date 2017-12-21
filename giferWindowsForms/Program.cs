using System;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace gifer
{
    static class Program
    {
        private static Mutex mutex = new Mutex(true, "gefaa2cb-1f90-470e-b3a0-beb6c3e4fdc4");

        [STAThread]
        static void Main(string[] args)
        {
            //FileAssotiationHelper.CheckGiferIsRegistered(); it would require admin
            if (args.Length == 0) {
                //args = new[] { @"E:\img\gif\Shinobu\Kiss-Shot part I\KizuI-Acerola1.gif" };
            }

			Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath).Setup();
            
            if (config.AppSettings.Settings["openInStandalone"].Value == true.ToString()) {
                if (mutex.WaitOne(TimeSpan.Zero, true)) {
                    GiferForm form = GetForm(config, args);
                    Application.Run(form);
                    //mutex.ReleaseMutex();
                } else {
                    // Gifer opened
                    try {
                        using (var client = new TcpClient()) {
                            client.Connect(Gifer.EndPoint);
                            byte[] mesasge = Encoding.UTF8.GetBytes(args[0]);
                            NetworkStream networkStream = client.GetStream();
                            networkStream.Write(mesasge, 0, mesasge.Length);
                            networkStream.Close();
                            networkStream.Dispose();
                        }
                    } catch (Exception ex) {
                        MessageBox.Show(ex.ToString());
                    } finally {
                    }
                }
            } else {
                GiferForm form = GetForm(config, args);
                Application.Run(form);
            }
        }

        private static GiferForm GetForm(Configuration config, string[] args) {
            return args.Length == 0 ? new GiferForm(config) 
                                    : new GiferForm(config, args[0]);
        }

        private static Configuration Setup(this Configuration config)
        {
            if (!config.AppSettings.Settings.AllKeys.Contains("showHelpAtStartup")) {
                config.AppSettings.Settings.Add("showHelpAtStartup", "true");
                config.Save(ConfigurationSaveMode.Minimal);
            }
            if (!config.AppSettings.Settings.AllKeys.Contains("openInStandalone")) {
                config.AppSettings.Settings.Add("openInStandalone", "false");
                config.Save(ConfigurationSaveMode.Minimal);
            }
            return config;
        }
    }
}

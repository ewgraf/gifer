using System;
using System.Windows.Forms;

namespace gifer {
    static class Program {
        [STAThread]
        static void Main(string[] args) {
			Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GiferForm form = GetForm(args);
            Application.Run(form);
        }

        private static GiferForm GetForm(string[] args) {
            return args.Length == 0 ? new GiferForm() 
                                    : new GiferForm(args[0]);
        }
    }
}

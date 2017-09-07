using System;
using System.Windows.Forms;

namespace gifer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
			Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(args.Length == 0 ? new GiferForm(string.Empty) : new GiferForm(args[0]));
        }
    }
}

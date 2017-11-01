using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace gifer.Utils {
    public class FileAssotiationHelper {
        public static void CheckGiferIsRegistered() {
            string k = @"Applications\gifer.exe";
            var q = Registry.ClassesRoot.OpenSubKey(k);
            if(q == null) {
                var d = Registry.ClassesRoot.CreateSubKey($@"{k}\shell\open\command", RegistryKeyPermissionCheck.ReadWriteSubTree);
                d.SetValue("(default)", @"""C:\Program Files(x86)\Gifer\gifer.exe"" ""%1""");
                d.Close();
            }
        }

        public static void SetAssociation(string extension) {
            // The stuff that was above here is basically the same

            // Delete the key instead of trying to change it
            var CurrentUser = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\" + extension, true);
            CurrentUser.DeleteSubKey("UserChoice", false);
            CurrentUser.Close();

            // Tell explorer the file association has been changed
            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}

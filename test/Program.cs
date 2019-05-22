using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace test {
	class Program {
        public static void Main(string[] args) {
			FileAssociations.SetAssociation(".jpg", "gifer", $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\gifer\gifer.exe", "gifer - .gif & images viewer");
		}

		public class FileAssociations {
			public static void SetAssociation(string extension, string appName, string appPath, string appDescription) {
				RegistryKey baseKey = Registry.ClassesRoot.CreateSubKey(extension);
				baseKey.SetValue("", appName);
                baseKey.Close();

                RegistryKey openMethod = Registry.ClassesRoot.CreateSubKey(appName);
				openMethod.SetValue("", appDescription);

                //OpenMethod.CreateSubKey("DefaultIcon").SetValue("", "\"" + OpenWith + "\",0");
                RegistryKey shell = openMethod.CreateSubKey("Shell");
				//Shell.CreateSubKey("edit").CreateSubKey("command").SetValue("", "\"" + OpenWith + "\"" + " \"%1\"");
				shell.CreateSubKey("open").CreateSubKey("command").SetValue("", "\"" + appPath + "\"" + " \"%1\"");
				shell.Close();
                openMethod.Close();

                // Delete the key instead of trying to change it
                RegistryKey currentUser = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\" + extension, true);
				currentUser.DeleteSubKey("UserChoice", false);
				currentUser.Close();

				// Tell explorer the file association has been changed
				SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
			}
		}

		[DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
	}
}

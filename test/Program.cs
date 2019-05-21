using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace test {
	class Program {
		static void Main(string[] args) {

			FileAssociations.SetAssociation(".png", "gifer", $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\gifer\gifer.exe", ".gif image");
		}



		public class FileAssociations {
			public static void SetAssociation(string Extension, string KeyName, string OpenWith, string FileDescription) {
				RegistryKey baseKey = Registry.ClassesRoot.CreateSubKey(Extension);
				baseKey.SetValue("", KeyName);

				RegistryKey openMethod = Registry.ClassesRoot.CreateSubKey(KeyName);
				openMethod.SetValue("", FileDescription);
				//OpenMethod.CreateSubKey("DefaultIcon").SetValue("", "\"" + OpenWith + "\",0");
				RegistryKey shell = openMethod.CreateSubKey("Shell");
				//Shell.CreateSubKey("edit").CreateSubKey("command").SetValue("", "\"" + OpenWith + "\"" + " \"%1\"");
				shell.CreateSubKey("open").CreateSubKey("command").SetValue("", "\"" + OpenWith + "\"" + " \"%1\"");
				baseKey.Close();
				openMethod.Close();
				shell.Close();

				//RegistryKey currentUser = Registry.CurrentUser.CreateSubKey(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + Extension);
				//currentUser = currentUser.OpenSubKey("UserChoice", RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl);
				//currentUser.SetValue("Progid", KeyName, RegistryValueKind.String);
				//currentUser.Close();

				// Delete the key instead of trying to change it
				RegistryKey currentUser = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\" + Extension, true);
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

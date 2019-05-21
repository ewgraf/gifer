using System;
using WixSharp;

namespace Gifer.MSI {
	class Program {
		static void Main(string[] args) {
			// Because of the excessive size of the WiX Toolset the WixSharp.wix.bin NuGet package
			//  isn't a direct dependency of the WixSharp package and it needs to be added to the project explicitly:
			Compiler.WixLocation = @"..\packages\WixSharp.wix.bin.<version>\tools\bin";

			string prefix = @"..\gifer\bin\Debug";
			var project = new Project("Gifer",
				new Dir(@"%AppData%\Gifer",
					new File($@"{prefix}\gifer.exe",
						new FileAssociation(".gif", "image/gif", "open", "%1"),
						new FileAssociation(".jpg", "image/jpeg", "open", "%1"),
						new FileAssociation(".jpeg", "image/jpeg", "open", "%1"),
						new FileAssociation(".jpe", "image/jpeg", "open", "%1"),
						new FileAssociation(".jfif", "image/jpeg", "open", "%1"),
						new FileAssociation(".bmp", "image/bmp", "open", "%1"),
						new FileAssociation(".png", "image/png", "open", "%1"),
						new FileShortcut("Sharer", @"%AppData%\Microsoft\Windows\Start Menu\Programs")
					)
					//,new File($@"{prefix}\gifer.exe.config")
				)
				//,
				//// http://www.reza-aghaei.com/how-to-add-item-to-windows-shell-context-menu-to-open-your-application/
				//new RegValue(RegistryHive.ClassesRoot, @"*\shell\Share", "", "Share"),
				//new RegValue(RegistryHive.ClassesRoot, @"*\shell\Share", "Icon", "[INSTALLDIR]sharer.exe"),
				//new RegValue(RegistryHive.ClassesRoot, @"*\shell\Share\command", "", "\"[INSTALLDIR]sharer.exe\" \"%1\"")
			);

			project.BackgroundImage = "gifer-background.jpg";
			project.ControlPanelInfo.Comments = "Gifer is a program for viewing .gif and other images.";
			project.ControlPanelInfo.HelpLink = "https://github.com/ewgraf/gifer";
			project.ControlPanelInfo.Manufacturer = "https://github.com/ewgraf";
			project.ControlPanelInfo.ProductIcon = @"..\gifer\Resources\gifer logo 42x42 exe icon.ico";
			project.GUID = new Guid("56628ce2-91ad-464c-a005-a19e09a5c9a3");
			project.LicenceFile = "Licence.rtf";
			project.Version = new Version(3, 0);

			Compiler.BuildMsi(project);
		}
	}
}

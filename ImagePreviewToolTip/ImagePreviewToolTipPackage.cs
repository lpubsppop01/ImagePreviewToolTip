using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.Win32;

namespace lpubsppop01.ImagePreviewToolTip
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidImagePreviewToolTipPkgString)]
    public sealed class ImagePreviewToolTipPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs == null) return;

            {
                CommandID menuCommandID = new CommandID(GuidList.guidImagePreviewToolTipCmdSet, (int)PkgCmdIDList.cmdidToggleImagePreviewToolTip);
                MenuCommand menuItem = new MenuCommand((sender, e) =>
                {
                    ImagePreviewToolTipSettings.Current.IsEnabled = !ImagePreviewToolTipSettings.Current.IsEnabled;
                    ImagePreviewToolTipSettings.SaveCurrent();
                    MessageBox.Show(string.Format("ImagePreviewToolTip is {0}.", (ImagePreviewToolTipSettings.Current.IsEnabled ? "enabled" : "disabled")));
                }, menuCommandID);
                mcs.AddCommand(menuItem);
            }
        }
    }
}

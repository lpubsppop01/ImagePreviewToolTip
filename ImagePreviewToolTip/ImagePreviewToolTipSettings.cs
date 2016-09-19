using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

namespace lpubsppop01.ImagePreviewToolTip
{
    // ref. https://github.com/omsharp/BetterComments/blob/master/BetterComments/Options/FontSettingsManager.cs

    class ImagePreviewToolTipSettings
    {
        public ImagePreviewToolTipSettings()
        {
            IsEnabled = true;
        }

        public bool IsEnabled { get; set; }

        #region Static Members

        const string CollectionPath = "ImagePreviewToolTip";

        static readonly WritableSettingsStore settingsStore;

        public static ImagePreviewToolTipSettings Current { get; private set; }

        static ImagePreviewToolTipSettings()
        {
            var settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            settingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            Current = new ImagePreviewToolTipSettings();
            LoadCurrent();
        }

        public static void SaveCurrent()
        {
            if (!settingsStore.CollectionExists(CollectionPath))
            {
                settingsStore.CreateCollection(CollectionPath);
            }

            settingsStore.SetBoolean(CollectionPath, "IsEnabled", Current.IsEnabled);
        }

        public static void LoadCurrent()
        {
            if (!settingsStore.CollectionExists(CollectionPath)) return;

            Current.IsEnabled = settingsStore.GetBoolean(CollectionPath, "IsEnabled", true);
        }

        #endregion
    }
}

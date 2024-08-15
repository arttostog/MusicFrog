using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MusicFrog
{
    internal class AppStartup
    {
        public readonly ToolStripMenuItem AddToStartupButton;

        private const string SubKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

        private readonly string _appName = Application.ProductName;
        private readonly string _pathToApp = Process.GetCurrentProcess().MainModule.FileName;

        public AppStartup() =>
            AddToStartupButton = new ToolStripMenuItem("Add to startup", null, AddToStartup)
            {
                Checked = IsStartupEnabled()
            };

        private void AddToStartup(object sender, EventArgs eventArgs)
        {
            AddToStartupButton.Checked = !AddToStartupButton.Checked;
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(SubKey, true))
                SetValue(registryKey);
        }

        private void SetValue(RegistryKey registryKey)
        {
            if (AddToStartupButton.Checked)
            {
                registryKey.SetValue(_appName, _pathToApp);
                return;
            }
            registryKey.DeleteValue(_appName, false);
        }

        private bool IsStartupEnabled()
        {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(SubKey))
            {
                object value = registryKey.GetValue(_appName);
                
                if (value == null)
                    return false;

                return value.Equals(_pathToApp);
            }
        }
    }
}

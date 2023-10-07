using Microsoft.Win32;
using MusicFrog.Properties;
using NAudio.CoreAudioApi;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace MusicFrog
{
    public class MusicFrogApp : ApplicationContext
    {
        private NotifyIcon Icon;

        public AudioMeterInformation Device { set; private get; }

        private readonly Thread TickThread;

        private float MinVolume;

        private Icon FrogIdle;
        private Icon FrogTalking;

        private ContextMenuStrip Menu;
        private ToolStripMenuItem MinVolumeMenu;
        private ToolStripMenuItem AddToStartupButton;

        public MusicFrogApp()
        {
            MinVolume = User.Default.minVolume;

            SetContextMenuStrip();
            SetIconSettings();

            MMDeviceEnumerator MMDeviceEnum = new MMDeviceEnumerator();
            MMDeviceEnum.RegisterEndpointNotificationCallback(new NotificationClient(this));
            Device = MMDeviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).AudioMeterInformation;

            Application.ApplicationExit += new EventHandler(Exit);
            
            TickThread = new Thread(Tick);
            TickThread.Start();
        }

        private void SetContextMenuStrip()
        {
            Menu = new ContextMenuStrip();

            MinVolumeMenu = new ToolStripMenuItem("Min volume", null, GetMinVolumeMenuItems());
            AddToStartupButton = new ToolStripMenuItem("Add to startup", null, AddToStartup)
            {
                Checked = IsStartupEnabled()
            };
            ToolStripMenuItem ExitButton = new ToolStripMenuItem("Exit", null, Exit);

            Menu.Items.AddRange(new ToolStripItem[]
            {
                MinVolumeMenu,
                AddToStartupButton,
                ExitButton
            });
        }

        private void SetIconSettings()
        {
            FrogIdle = BitmapToIcon(Resources.frog_1);
            FrogTalking = BitmapToIcon(Resources.frog_2);
            Icon = new NotifyIcon
            {
                Icon = FrogIdle,
                Text = "MusicFrog",
                ContextMenuStrip = Menu,
                Visible = true
            };
        }

        private Icon BitmapToIcon(Bitmap bitmap)
        {
            return System.Drawing.Icon.FromHandle(bitmap.GetHicon());
        }

        private ToolStripMenuItem[] GetMinVolumeMenuItems()
        {
            return new ToolStripMenuItem[]
            {
                new ToolStripMenuItem("100%", null, SetMinVolume) { Checked = MinVolume == 1f },
                new ToolStripMenuItem("90%", null, SetMinVolume) { Checked = MinVolume == 0.9f },
                new ToolStripMenuItem("80%", null, SetMinVolume) { Checked = MinVolume == 0.8f },
                new ToolStripMenuItem("70%", null, SetMinVolume) { Checked = MinVolume == 0.7f },
                new ToolStripMenuItem("60%", null, SetMinVolume) { Checked = MinVolume == 0.6f },
                new ToolStripMenuItem("50%", null, SetMinVolume) { Checked = MinVolume == 0.5f },
                new ToolStripMenuItem("40%", null, SetMinVolume) { Checked = MinVolume == 0.4f },
                new ToolStripMenuItem("30%", null, SetMinVolume) { Checked = MinVolume == 0.3f },
                new ToolStripMenuItem("20%", null, SetMinVolume) { Checked = MinVolume == 0.2f },
                new ToolStripMenuItem("10%", null, SetMinVolume) { Checked = MinVolume == 0.1f }
            };
        }

        private bool IsStartupEnabled()
        {
            using (RegistryKey rKey = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run"))
            {
                return rKey.GetValue(Application.ProductName) != null;
            }
        }

        private void Tick()
        {
            while (true)
            {
                SetIcon(Device.MasterPeakValue < MinVolume);
                Thread.Sleep(100);
            }
        }

        private void SetIcon(bool IsIdle)
        {
            if (IsIdle)
            {
                if (Icon.Icon.Equals(FrogTalking)) Icon.Icon = FrogIdle;
                return;
            }
            if (Icon.Icon.Equals(FrogIdle))
            {
                Icon.Icon = FrogTalking;
            }
        }

        private void SetMinVolume(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            UpdateCheckedState(item, MinVolumeMenu);
            MinVolume = int.Parse(item.Text.Replace("%", "")) / 100f;
        }

        private void UpdateCheckedState(ToolStripMenuItem item, ToolStripMenuItem menu)
        {
            foreach (ToolStripMenuItem i in menu.DropDownItems)
            {
                i.Checked = false;
            }
            item.Checked = true;
        }

        private void AddToStartup(object sender, EventArgs e)
        {
            AddToStartupButton.Checked = !AddToStartupButton.Checked;
            using (RegistryKey RegKey = Registry.CurrentUser.OpenSubKey
                (@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                SetValue(RegKey);
                RegKey.Close();
            }
        }

        private void SetValue(RegistryKey RegKey)
        {
            if (AddToStartupButton.Checked)
            {
                RegKey.SetValue(Application.ProductName, Process.GetCurrentProcess().MainModule.FileName);
                return;
            }
            RegKey.DeleteValue(Application.ProductName, false);
        }

        private void Exit(object sender, EventArgs e)
        {
            Icon.Visible = false;
            User.Default.minVolume = MinVolume;
            User.Default.Save();
            TickThread.Abort();
            Environment.Exit(0);
        }
    }
}

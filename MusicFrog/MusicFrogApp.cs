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
        private NotifyIcon icon;

        private readonly MMDeviceEnumerator devices;

        private readonly Thread tickThread;

        private float minVolume;

        private Icon FrogIdle;
        private Icon FrogTalking;

        private ToolStripMenuItem minVolumeMenu;
        private ToolStripMenuItem addToStartupButton;

        public MusicFrogApp()
        {
            minVolume = User.Default.minVolume;
            SetIconSettings();
            devices = new MMDeviceEnumerator();
            devices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            Application.ApplicationExit += new EventHandler(Exit);
            tickThread = new Thread(Tick);
            tickThread.Start();
        }

        private void SetIconSettings()
        {
            FrogIdle = BitmapToIcon(Resources.frog_1);
            FrogTalking = BitmapToIcon(Resources.frog_2);
            icon = new NotifyIcon
            {
                Icon = FrogIdle,
                Text = "MusicFrog",
                ContextMenuStrip = GetContextMenuStrip(),
                Visible = true
            };
        }

        private Icon BitmapToIcon(Bitmap bitmap)
        {
            return Icon.FromHandle(bitmap.GetHicon());
        }

        private ContextMenuStrip GetContextMenuStrip()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            minVolumeMenu = new ToolStripMenuItem("Min volume", null, GetMinVolumeMenuItems());

            addToStartupButton = new ToolStripMenuItem("Add to startup", null, AddToStartup)
            {
                Checked = IsStartupEnabled()
            };

            ToolStripMenuItem exitButton = new ToolStripMenuItem("Exit", null, Exit);

            menu.Items.AddRange(new ToolStripItem[]
            {
                minVolumeMenu,
                addToStartupButton,
                exitButton
            });

            return menu;
        }

        private bool IsStartupEnabled()
        {
            string keyName = @"Software\Microsoft\Windows\CurrentVersion\Run";
            using (RegistryKey rKey = Registry.CurrentUser.OpenSubKey(keyName))
            {
                return (rKey.GetValue(Application.ProductName) != null);
            }
        }

        private ToolStripMenuItem[] GetMinVolumeMenuItems()
        {
            return new ToolStripMenuItem[]
            {
                new ToolStripMenuItem("100%", null, SetMinVolume)
                {
                    Checked = minVolume == 1f
                },
                new ToolStripMenuItem("90%", null, SetMinVolume)
                {
                    Checked = minVolume == 0.9f
                },
                new ToolStripMenuItem("80%", null, SetMinVolume)
                {
                    Checked = minVolume == 0.8f
                },
                new ToolStripMenuItem("70%", null, SetMinVolume)
                {
                    Checked = minVolume == 0.7f
                },
                new ToolStripMenuItem("60%", null, SetMinVolume)
                {
                    Checked = minVolume == 0.6f
                },
                new ToolStripMenuItem("50%", null, SetMinVolume)
                {
                    Checked = minVolume == 0.5f
                },
                new ToolStripMenuItem("40%", null, SetMinVolume)
                {
                    Checked = minVolume == 0.4f
                },
                new ToolStripMenuItem("30%", null, SetMinVolume)
                {
                    Checked = minVolume == 0.3f
                },
                new ToolStripMenuItem("20%", null, SetMinVolume)
                {
                    Checked = minVolume == 0.2f
                },
                new ToolStripMenuItem("10%", null, SetMinVolume)
                {
                    Checked = minVolume == 0.1f
                }
            };
        }

        private void Tick()
        {
            while (true)
            {
                OnTick();
                Thread.Sleep(100);
            }
        }

        private void OnTick()
        {
            SetIcon(devices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).AudioMeterInformation.MasterPeakValue < minVolume);
        }

        private void SetIcon(bool isIdle)
        {
            if (isIdle)
            {
                icon.Icon = FrogIdle;
                return;
            }
            icon.Icon = FrogTalking;
        }

        private void SetMinVolume(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            UpdateCheckedState(item, minVolumeMenu);
            minVolume = int.Parse(item.Text.Replace("%", "")) / 100f;
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
            addToStartupButton.Checked = !addToStartupButton.Checked;
            string keyName = @"Software\Microsoft\Windows\CurrentVersion\Run";
            using (RegistryKey rKey = Registry.CurrentUser.OpenSubKey(keyName, true))
            {
                if (addToStartupButton.Checked)
                {
                    rKey.SetValue(Application.ProductName, Process.GetCurrentProcess().MainModule.FileName);
                    rKey.Close();
                    return;
                }
                rKey.DeleteValue(Application.ProductName, false);
                rKey.Close();
            }
        }

        private void Exit(object sender, EventArgs e)
        {
            User.Default.minVolume = minVolume;
            User.Default.Save();
            tickThread.Abort();
            Environment.Exit(0);
        }
    }
}

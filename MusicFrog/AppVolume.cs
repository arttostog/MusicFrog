using MusicFrog.Properties;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MusicFrog
{
    internal class AppVolume
    {
        public ToolStripMenuItem VolumeMenu { get; private set; }
        public AudioMeterInformation Device { private get; set; }

        private float _minVolume = User.Default.MinVolume;

        public AppVolume()
        {
            VolumeMenu = new ToolStripMenuItem("Min volume", null, GetMinVolumeMenuItems());

            MMDeviceEnumerator MMDeviceEnum = new MMDeviceEnumerator();
            MMDeviceEnum.RegisterEndpointNotificationCallback(new AppNotificationClient(this));
            Device = MMDeviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).AudioMeterInformation;

            Application.ApplicationExit += new EventHandler(OnExit);
        }

        public ToolStripMenuItem[] GetMinVolumeMenuItems()
        {
            List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();

            for (int i = 100; i > 0; i -= 10)
                items.Add(new ToolStripMenuItem(i.ToString() + "%", null, SetMinVolume)
                {
                    Checked = _minVolume.Equals(i / 100f)
                });

            return items.ToArray();
        }

        public void SetMinVolume(object sender, EventArgs eventArgs)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            UpdateCheckedState(item);
            _minVolume = int.Parse(item.Text.Replace("%", "")) / 100f;
        }

        public void UpdateCheckedState(ToolStripMenuItem item)
        {
            foreach (ToolStripMenuItem i in VolumeMenu.DropDownItems)
                i.Checked = false;
            item.Checked = true;
        }

        private void OnExit(object sender, EventArgs eventArgs)
        {
            User.Default.MinVolume = _minVolume;
            User.Default.Save();
        }

        public bool FrogIsIdle() =>
            Device.MasterPeakValue < _minVolume;
    }
}

using NAudio.CoreAudioApi;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace MusicFrog
{
    public class MusicFrogApp : ApplicationContext
    {
        private NotifyIcon icon;

        private MMDeviceEnumerator devices;

        private Thread tickThread;

        public MusicFrogApp()
        {
            setIconSettings();
            devices = new MMDeviceEnumerator();
            devices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            tickThread = new Thread(tick);
            tickThread.Start();
            Application.ApplicationExit += new EventHandler(Exit);
        }

        private void setIconSettings()
        {
            icon = new NotifyIcon();
            //icon.Icon = new Icon("icon.png");
            icon.Text = "MusicFrog";
            icon.Visible = true;
        }

        private void setIcon(bool isIdle)
        {
            if(isIdle)
            {
                Console.WriteLine("idle");
                return;
            }
            Console.WriteLine("not idle");
        }

        private void tick()
        {
            while (true)
            {
                onTick();
                Thread.Sleep(100);
            }
        }

        public void onTick()
        {
            setIcon(devices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).AudioMeterInformation.MasterPeakValue < 0.4f);
        }

        private void Exit(object sender, EventArgs e)
        {
            tickThread.Abort();
        }
    }
}

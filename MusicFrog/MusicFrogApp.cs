using System;
using System.Threading;
using System.Windows.Forms;

namespace MusicFrog
{
    internal class MusicFrogApp : ApplicationContext
    {
        private readonly AppVolume _appVolume;
        private readonly AppIcon _appIcon;
        private readonly Thread _tickThread;

        public MusicFrogApp()
        {
            _appVolume = new AppVolume();
            _appIcon = new AppIcon(
                new AppIconMenu(_appVolume.VolumeMenu, new AppStartup().AddToStartupButton).MainMenu
            );

            Application.ApplicationExit += new EventHandler(OnExit);
            
            _tickThread = new Thread(Tick);
            _tickThread.Start();
        }

        private void OnExit(object sender, EventArgs eventArgs) =>
            _tickThread.Abort();

        private void Tick()
        {
            while (true)
            {
                _appIcon.UpdateIcon(_appVolume.FrogIsIdle());
                Thread.Sleep(100);
            }
        }
    }
}

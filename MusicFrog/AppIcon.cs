using MusicFrog.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MusicFrog
{
    internal class AppIcon
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly Icon _frogIdle, _frogTalking;

        public AppIcon(ContextMenuStrip contextMenuStrip)
        {
            _frogIdle = BitmapToIcon(Resources.frog_1);
            _frogTalking = BitmapToIcon(Resources.frog_2);
            _notifyIcon = new NotifyIcon
            {
                Icon = _frogIdle,
                Text = "MusicFrog",
                ContextMenuStrip = contextMenuStrip,
                Visible = true
            };

            Application.ApplicationExit += new EventHandler(OnExit);
        }

        private Icon BitmapToIcon(Bitmap bitmap) =>
            Icon.FromHandle(bitmap.GetHicon());

        public void UpdateIcon(bool frogIsIdle)
        {
            if (frogIsIdle)
            {
                if (_notifyIcon.Icon.Equals(_frogTalking))
                    _notifyIcon.Icon = _frogIdle;
                return;
            }
            if (_notifyIcon.Icon.Equals(_frogIdle))
                _notifyIcon.Icon = _frogTalking;
        }

        private void OnExit(object sender, EventArgs eventArgs) =>
            _notifyIcon.Visible = false;
    }
}

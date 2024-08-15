using System;
using System.Windows.Forms;

namespace MusicFrog
{
    internal class AppIconMenu
    {
        public ContextMenuStrip MainMenu { get; private set; }

        private readonly ToolStripMenuItem _volumeMenu, _startupButton;

        public AppIconMenu(ToolStripMenuItem volumeMenu, ToolStripMenuItem startupButton)
        {
            _volumeMenu = volumeMenu;
            _startupButton = startupButton;
            SetContextMenuStrip();
        }

        private void SetContextMenuStrip()
        {
            ToolStripMenuItem exitButton = new ToolStripMenuItem(
                "Exit",
                null,
                (object sender, EventArgs eventArgs) => Application.Exit()
            );

            (MainMenu = new ContextMenuStrip()).Items.AddRange(new ToolStripItem[]
            {
                _volumeMenu,
                _startupButton,
                exitButton
            });
        }
    }
}

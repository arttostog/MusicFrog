using System;
using System.Windows.Forms;

namespace MusicFrog
{
    internal static class Program
    {
        [STAThread]
        static void Main() =>
            Application.Run(new MusicFrogApp());
    }
}

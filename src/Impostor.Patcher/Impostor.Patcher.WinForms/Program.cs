using System;
using System.Windows.Forms;
using Impostor.Patcher.WinForms.Forms;

namespace Impostor.Patcher.WinForms
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}

using System;
using System.Windows.Forms;
using Impostor.Client.WinForms.Forms;

namespace Impostor.Client.WinForms
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
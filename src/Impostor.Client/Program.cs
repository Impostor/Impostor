using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Impostor.Client.Core;
using Impostor.Client.Forms;

namespace Impostor.Client
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            var firstArg = Environment.GetCommandLineArgs().FirstOrDefault();

            if (firstArg != null && Uri.TryCreate(firstArg, UriKind.Absolute, out var uri) && uri.Scheme.ToLower() == "impostor")
            {
                HandleUri(uri).GetAwaiter().GetResult();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }

        private static async Task HandleUri(Uri uri)
        {
            var modifier = new AmongUsModifier();
            await modifier.SaveIp(uri.Host);
            Process.Start("steam://launch/945360");
        }
    }
}
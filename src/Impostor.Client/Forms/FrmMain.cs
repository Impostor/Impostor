using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Impostor.Shared.Innersloth;

namespace Impostor.Client.Forms
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            // TODO: Load old IP.
        }

        private void buttonLaunch_Click(object sender, EventArgs e)
        {
            var ipText = textIp.Text;
            
            if (!IPAddress.TryParse(ipText, out var ipAddress))
            {
                MessageBox.Show("Invalid IP Address entered", "Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                
                textIp.Text = string.Empty;
                textIp.Focus();
                return;
            }

            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                MessageBox.Show("Invalid IP Address entered, only IPv4 is allowed.", "Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                
                textIp.Text = string.Empty;
                textIp.Focus();
                return;
            }
            
            // TODO: Clean up, move to somewhere else & error handling.
            
            var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..\\LocalLow");
            var regionFile = Path.Combine(appData, "Innersloth", "Among Us", "regionInfo.dat");
            var region = new RegionInfo("Private", ipText, new []
            {
                new ServerInfo("Private-Master-1", ipText, 22023)
            });
            
            using (var file = File.Open(regionFile, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(file))
            {
                region.Serialize(writer);
            }
            
            MessageBox.Show("The IP Address was saved, please (re)start Among Us.", "Success", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Information);
        }
    }
}
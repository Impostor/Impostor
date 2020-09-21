using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Impostor.Client.Core;
using Impostor.Client.Core.Events;

namespace Impostor.Client.Forms
{
    public partial class FrmMain : Form
    {
        private readonly AmongUsModifier _modifier;
        
        public FrmMain()
        {
            InitializeComponent();

            AcceptButton = buttonLaunch;
            
            _modifier = new AmongUsModifier();
            _modifier.Error += ModifierOnError;
            _modifier.Saved += ModifierOnSaved;
        }

        private void ModifierOnError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.Message, "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            textIp.Text = string.Empty;
            textIp.Focus();
            
            textIp.Enabled = true;
            buttonLaunch.Enabled = true;
        }

        private void ModifierOnSaved(object sender, SavedEventArgs e)
        {
            MessageBox.Show("The IP Address was saved, please (re)start Among Us.", "Success", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Information);

            textIp.Text = e.IpAddress;
            textIp.Enabled = true;
            buttonLaunch.Enabled = true;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (_modifier.TryLoadIp(out var ipAddress))
            {
                textIp.Text = ipAddress;
            }
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            textIp.Focus();
        }

        private void textIp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                
                buttonLaunch_Click(this, EventArgs.Empty);
            }
        }

        private async void buttonLaunch_Click(object sender, EventArgs e)
        {
            textIp.Enabled = false;
            buttonLaunch.Enabled = false;

            await _modifier.SaveIp(textIp.Text);
        }

        private void lblUrl_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/AeonLucid/Impostor");
        }
    }
}
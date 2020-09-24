using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Impostor.Client.Core;
using Impostor.Client.Core.Events;
using ErrorEventArgs = Impostor.Client.Core.Events.ErrorEventArgs;

namespace Impostor.Client.Forms
{
    public partial class FrmMain : Form
    {
        private readonly AmongUsModifier _modifier;
        private const string IpListConfigPath = @"iplist.cfg";

        public FrmMain()
        {
            InitializeComponent();

            AcceptButton = buttonLaunch;

            _modifier = new AmongUsModifier();
            _modifier.Error += ModifierOnError;
            _modifier.Saved += ModifierOnSaved;
        }

        private void LoadSavedIpsFromCfg()
        {
            if (File.Exists(IpListConfigPath))
            {
                comboIp.Items.AddRange(File.ReadAllLines(IpListConfigPath).Reverse().ToArray());
            }
        }

        private void SaveIpToCfg(string ip)
        {
            File.AppendAllLines(IpListConfigPath, new[] { ip });
        }

        private void ModifierOnError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.Message, "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            comboIp.Text = string.Empty;
            comboIp.Focus();

            comboIp.Enabled = true;
            buttonLaunch.Enabled = true;
        }

        private void ModifierOnSaved(object sender, SavedEventArgs e)
        {
            MessageBox.Show("The IP Address was saved, please (re)start Among Us.", "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            comboIp.Text = e.IpAddress;
            comboIp.Enabled = true;
            buttonLaunch.Enabled = true;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            LoadSavedIpsFromCfg();
            if (_modifier.TryLoadIp(out var ipAddress))
            {
                comboIp.Text = ipAddress;
            }
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            comboIp.Focus();
        }

        private void textIp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.Handled = true;

            buttonLaunch_Click(this, EventArgs.Empty);
        }

        private async void buttonLaunch_Click(object sender, EventArgs e)
        {
            comboIp.Enabled = false;
            buttonLaunch.Enabled = false;

            await _modifier.SaveIp(comboIp.Text);
            SaveIpToCfg(comboIp.Text);
            LoadSavedIpsFromCfg();
        }

        private void clearSaved_Click(object sender, EventArgs e)
        {
            File.WriteAllText(IpListConfigPath, string.Empty);
            comboIp.Items.Clear();
            LoadSavedIpsFromCfg();
        }

        private void lblUrl_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/AeonLucid/Impostor");
        }
    }
}
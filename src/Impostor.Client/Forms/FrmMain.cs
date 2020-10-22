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
        private readonly Configuration _config;
        private readonly AmongUsModifier _modifier;

        public FrmMain()
        {
            InitializeComponent();

            AcceptButton = buttonLaunch;

            var amongUsDir = Path.GetFullPath(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow"), "Innersloth", "Among Us"));

            _config = new Configuration(amongUsDir);
            _modifier = new AmongUsModifier(this);
            _modifier.Error += ModifierOnError;
            _modifier.Saved += ModifierOnSaved;
        }

        public Configuration Configuration => _config;

        private void ModifierOnError(object sender, ErrorEventArgs e)
        {
            DialogResult dr = MessageBox.Show(e.Message, "Error",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Error);

            comboIp.Text = string.Empty;
            comboIp.Focus();

            comboIp.Enabled = true;
            buttonLaunch.Enabled = true;

            if (e.OpenLocalLow)
            {
                if (dr == DialogResult.No)
                {
                    Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow"));
                }
                else if (dr == DialogResult.Yes)
                {
                    var dialog = new FolderBrowserDialog();
                    DialogResult folderDialogResult = dialog.ShowDialog();
                    if (folderDialogResult == DialogResult.OK)
                    {
                        if (File.Exists(Path.Combine(dialog.SelectedPath, "regionInfo.dat")))
                        {
                            _config.SetAmongUsFolder(dialog.SelectedPath);
                            _config.Save();
                        }
                        else
                        {
                            MessageBox.Show($"Finished with code -1 - Invalid Among Us installation given.", "Operation cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Finished with code {folderDialogResult.GetHashCode().ToString()} - No folder given", "Operation cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void ModifierOnSaved(object sender, SavedEventArgs e)
        {
            MessageBox.Show("The IP Address was saved, please (re)start Among Us.", "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            var ipText = e.Port == AmongUsModifier.DefaultPort
                ? e.IpAddress
                : $"{e.IpAddress}:{e.Port}";

            comboIp.Text = ipText;
            comboIp.Enabled = true;
            buttonLaunch.Enabled = true;

            _config.AddIp(ipText);
            _config.Save();

            RefreshComboIps();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            _config.Load();

            RefreshComboIps();

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
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.Handled = true;

            buttonLaunch_Click(this, EventArgs.Empty);
        }

        private async void buttonLaunch_Click(object sender, EventArgs e)
        {
            comboIp.Enabled = false;
            buttonLaunch.Enabled = false;

            await _modifier.SaveIp(comboIp.Text);
        }

        private void lblUrl_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/AeonLucid/Impostor");
        }

        private void RefreshComboIps()
        {
            comboIp.Items.Clear();

            if (_config.RecentIps.Count > 0)
            {
                foreach (var ip in _config.RecentIps)
                {
                    comboIp.Items.Add(ip);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RandoEditor
{
	public partial class SettingsForm : Form
	{
		public SettingsForm()
		{
			InitializeComponent();
		}
		
		private void chkSimpleNodes_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default["SimpleNodeGraphics"] = chkSimpleNodes.Checked;
			Properties.Settings.Default.Save();
		}

		private void trkMapQuality_Scroll(object sender, EventArgs e)
		{
			Properties.Settings.Default["MapQuality"] = trkMapQuality.Value;
			Properties.Settings.Default.Save();
		}

		private void SettingsForm_Shown(object sender, EventArgs e)
		{
			chkSimpleNodes.Checked = (bool)Properties.Settings.Default["SimpleNodeGraphics"];
			trkMapQuality.Value = Math.Max(Math.Min((int)Properties.Settings.Default["MapQuality"], trkMapQuality.Maximum), trkMapQuality.Minimum);
		}
	}
}

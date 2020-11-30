using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Common.Utils;
using Common.Key;

namespace RandoEditor
{
	public partial class KeySelector : Form
	{
		public KeySelector()
		{
			InitializeComponent();

			listView1.Items.Clear();

			listView1.Items.AddRange(Utility.GenerateKeyList(listView1).ToArray());

			listView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		public List<BaseKey> SelectedKeys { get; set; } = new List<BaseKey>();
		public bool MultiSelect
		{
			get { return listView1.MultiSelect; }
			set { listView1.MultiSelect = value; }
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			SelectAndClose();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void listView1_MouseDoubleclick(object sender, MouseEventArgs e)
		{
			SelectAndClose();
		}

		private void SelectAndClose()
		{
			SelectedKeys.Clear();
			foreach (ListViewItem item in listView1.SelectedItems)
			{
				SelectedKeys.Add((BaseKey)item.Tag);
			}

			this.DialogResult = DialogResult.OK;

			this.Close();
		}
	}
}

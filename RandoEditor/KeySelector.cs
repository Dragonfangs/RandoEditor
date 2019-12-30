using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using RandoEditor.Key;

namespace RandoEditor
{
	public partial class KeySelector : Form
	{
		public KeySelector()
		{
			InitializeComponent();

			GenerateList();
		}

		public List<BaseKey> SelectedKeys { get; set; } = new List<BaseKey>();

		private void GenerateList()
		{
			listView1.Items.Clear();
	
			foreach (var key in KeyManager.GetRandomKeys())
			{
				var item = new ListViewItem(key.Name);
				item.Group = GetGroup("listViewGroupRandom");
				item.Tag = key;
				listView1.Items.Add(item);
			}

			foreach (var key in KeyManager.GetEventKeys())
			{
				var item = new ListViewItem(key.Name);
				item.Group = GetGroup("listViewGroupEvents");
				item.Tag = key;
				listView1.Items.Add(item);
			}

			foreach (var key in KeyManager.GetSettingKeys())
			{
				var item = new ListViewItem(key.Name);
				item.Group = GetGroup("listViewGroupSettings");
				item.Tag = key;
				listView1.Items.Add(item);
			}

			foreach (var key in KeyManager.GetCustomKeys())
			{
				var item = new ListViewItem(key.Name);
				item.Group = GetGroup("listViewGroupCustom");
				item.Tag = key;
				listView1.Items.Add(item);
			}

			listView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		private ListViewGroup GetGroup(string name)
		{
			foreach(ListViewGroup group in listView1.Groups)
			{
				if(group.Name == name)
				{
					return group;
				}
			}

			return null;
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			SelectAndClose();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
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

			this.Close();
		}
	}
}

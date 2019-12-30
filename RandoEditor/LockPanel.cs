using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using RandoEditor.Node;
using RandoEditor.Key;

namespace RandoEditor
{
	public partial class LockPanel : UserControl
	{
		private PathNode myNode = null;

		public LockPanel()
		{
			InitializeComponent();
		}

		public void SetNode(PathNode node)
		{
			myNode = node;
			RefreshKeyList();
		}

		private void RefreshKeyList()
		{
			listView1.Items.Clear();
			/*foreach (var key in myNode.myKeys)
			{
				var item = new ListViewItem(key.Name);
				item.Tag = key;
				listView1.Items.Add(item);
			}*/

			listView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		private void btnAddKey_Click(object sender, EventArgs e)
		{
			var keySelector = new KeySelector();
			keySelector.ShowDialog();
			
			foreach(var key in keySelector.SelectedKeys.Distinct())
			{
				//myNode.AddKey(key);
			}

			RefreshKeyList();
		}

		private void listView1_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Delete)
			{
				foreach(ListViewItem item in listView1.SelectedItems)
				{
					//myNode.myKeys.Remove((BaseKey)item.Tag);
				}

				RefreshKeyList();
			}
		}
	}
}

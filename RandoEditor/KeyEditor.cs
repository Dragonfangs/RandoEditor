using RandoEditor.Key;
using System;
using System.Windows.Forms;

namespace RandoEditor
{
	public partial class KeyEditor : Form
	{
		public KeyEditor()
		{
			InitializeComponent();

			lockPanelLogic1.Enabled = false;
			lockPanelLogic1.Visible = false;

			GenerateList();
		}

		private void GenerateList()
		{
			listView1.Items.Clear();

			foreach (var key in KeyManager.GetCustomKeys())
			{
				var item = new ListViewItem(key.Name);
				item.Tag = key;
				listView1.Items.Add(item);
			}

			listView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				lockPanelLogic1.Enabled = true;
				lockPanelLogic1.Visible = true;

				var selectedKey = (ComplexKey)listView1.SelectedItems[0].Tag;
				lockPanelLogic1.SetNode(selectedKey.myRequirement);
			}
			else
			{
				lockPanelLogic1.Enabled = false;
				lockPanelLogic1.Visible = false;
			}
		}

		private void newKeyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var newId = Guid.NewGuid();
			KeyManager.SaveCustomKey(newId, new ComplexKey(newId, GenerateUniqueName()));

			GenerateList();
		}

		private string GenerateUniqueName()
		{
			string newName = "New key";
			int index = 0;
			bool unique = true;

			do
			{
				newName = index == 0 ? "New Key" : $"New Key({index})";
				unique = true;
				index++;
				
				foreach (ListViewItem item in listView1.Items)
				{
					if(item.Text == newName)
					{
						unique = false;
						break;
					}
				}
			} while (unique == false);

			return newName;
		}

		private void listView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				if (listView1.SelectedItems.Count > 0)
				{
					var keyToRemove = (ComplexKey)listView1.SelectedItems[0].Tag;

					KeyManager.DeleteCustomKey(keyToRemove.Id);

					GenerateList();
				}
			}

			if (e.KeyCode == Keys.F2)
			{
				if (listView1.SelectedItems.Count > 0)
				{
					listView1.SelectedItems[0].BeginEdit();
				}
			}
		}

		private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			if (e.Label is null)
				return;

			var item = listView1.Items[e.Item];

			if (item.Tag is ComplexKey key)
			{ 
				key.Name = e.Label;
			}

			listView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

			GenerateList();
		}
	}
}

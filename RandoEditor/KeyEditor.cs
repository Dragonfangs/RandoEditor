using Common.Key;
using Common.Key.Requirement;
using Common.Node;
using RandoEditor.SaveData;
using System;
using System.Linq;
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

			GenerateLists();
		}

		private void GenerateLists()
		{
			listViewCustoms.Items.Clear();
			listViewEvents.Items.Clear();
			listViewSettings.Items.Clear();

			foreach (var key in KeyManager.GetCustomKeys())
			{
				var item = new ListViewItem(key.Name);
				item.Tag = key;
				listViewCustoms.Items.Add(item);
			}

			foreach (var key in KeyManager.GetEventKeys())
			{
				var item = new ListViewItem(key.Name);
				item.Tag = key;
				if(key.Static)
				{
					item.ForeColor = System.Drawing.Color.LightGray;
				}
				listViewEvents.Items.Add(item);
			}

			foreach (var key in KeyManager.GetSettingKeys())
			{
				var item = new ListViewItem(key.Name);
				item.Tag = key;
				if (key.Static)
				{
					item.ForeColor = System.Drawing.Color.LightGray;
				}
				listViewSettings.Items.Add(item);
			}

			listViewCustoms.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
			listViewEvents.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
			listViewSettings.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

			lockPanelLogic1.SetKeys();
		}

		private void listViewCustoms_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewCustoms.SelectedItems.Count > 0)
			{
				lockPanelLogic1.Enabled = true;
				lockPanelLogic1.Visible = true;

				var selectedKey = (ComplexKey)listViewCustoms.SelectedItems[0].Tag;
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
			if (tabControl1.SelectedTab == tabComplex)
			{
				KeyManager.SaveCustomKey(newId, new ComplexKey(newId, GenerateUniqueName(listViewCustoms.Items)));
			}
			else if (tabControl1.SelectedTab == tabEvents)
			{
				KeyManager.SaveEventKey(newId, new BaseKey(newId, GenerateUniqueName(listViewCustoms.Items)));
			}
			else if (tabControl1.SelectedTab == tabSettings)
			{
				KeyManager.SaveSettingKey(newId, new BaseKey(newId, GenerateUniqueName(listViewCustoms.Items)));
			}
			SaveManager.Dirty = true;

			GenerateLists();
		}

		private string GenerateUniqueName(ListView.ListViewItemCollection items)
		{
			string newName = "New key";
			int index = 0;
			bool unique = true;

			do
			{
				newName = index == 0 ? "New Key" : $"New Key({index})";
				unique = true;
				index++;
				
				foreach (ListViewItem item in items)
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

		private void DeleteKey(BaseKey key)
		{
			var affectedLockNodes = SaveManager.Data.Nodes.Where(node => node is LockNode).Select(node => node as LockNode).Where(node => node.myRequirement.ContainsKey(key.Id));
			var affectedEventNodes = SaveManager.Data.Nodes.Where(node => node is EventKeyNode).Select(node => node as EventKeyNode).Where(node => node.myKeyId == key.Id);

			if (affectedLockNodes.Any() ||
				affectedEventNodes.Any())
			{
				var deleteActions = new DeleteReplaceForm();
				deleteActions.StartPosition = FormStartPosition.CenterParent;
				deleteActions.ShowDialog(key.Name);

				if (deleteActions.Result == DeleteResult.Cancel)
					return;

				if (deleteActions.Result == DeleteResult.Delete)
				{
					foreach(var lockNode in affectedLockNodes)
					{
						lockNode.myRequirement.RemoveKey(key.Id);
					}

					foreach (var node in affectedEventNodes)
					{
						node.myKeyId = null;
					}
				}

				if (deleteActions.Result == DeleteResult.Replace)
				{
					foreach (var lockNode in affectedLockNodes)
					{
						lockNode.myRequirement.ReplaceKey(key.Id, deleteActions.ReplaceId);
					}

					foreach (var node in affectedEventNodes.Where(node => node.myKeyId == key.Id))
					{
						if(KeyManager.GetEventKeys().Any(eventKey => eventKey.Id == deleteActions.ReplaceId))
						{
							node.myKeyId = deleteActions.ReplaceId;
						}
						else // New Key is not an event
						{
							node.myKeyId = Guid.Empty;
						}
					}
				}
			}

			KeyManager.DeleteKey(key.Id);

			SaveManager.Dirty = true;

			GenerateLists();
		}

		private void listViewCustoms_KeyDown(object sender, KeyEventArgs e)
		{
			if (listViewCustoms.SelectedItems.Count > 0)
			{
				var selectedItem = listViewCustoms.SelectedItems[0];
				var selectedKey = (BaseKey)selectedItem.Tag;

				if (selectedKey.Static)
					return;

				if (e.KeyCode == Keys.Delete)
				{
					DeleteKey(selectedKey);
				}
				else if (e.KeyCode == Keys.F2)
				{
					selectedItem.BeginEdit();
				}
			}
		}

		private void listViewCustoms_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			if (e.Label is null)
				return;

			var item = listViewCustoms.Items[e.Item];

			if (item.Tag is BaseKey key)
			{ 
				key.Name = e.Label;
			}

			listViewCustoms.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

			SaveManager.Dirty = true;

			GenerateLists();
		}

		private void listViewEvents_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			if (e.Label is null)
				return;

			var item = listViewEvents.Items[e.Item];

			if (item.Tag is BaseKey key)
			{
				key.Name = e.Label;
			}

			listViewEvents.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

			SaveManager.Dirty = true;

			GenerateLists();
		}

		private void listViewEvents_KeyDown(object sender, KeyEventArgs e)
		{
			if (listViewEvents.SelectedItems.Count > 0)
			{
				var selectedItem = listViewEvents.SelectedItems[0];
				var selectedKey = (BaseKey)selectedItem.Tag;

				if (selectedKey.Static)
					return;

				if (e.KeyCode == Keys.Delete)
				{
					DeleteKey(selectedKey);
				}
				else if (e.KeyCode == Keys.F2)
				{
					selectedItem.BeginEdit();
				}
			}
		}

		private void listViewSettings_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			if (e.Label is null)
				return;

			var item = listViewSettings.Items[e.Item];

			if (item.Tag is BaseKey key)
			{
				key.Name = e.Label;
			}

			listViewSettings.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

			SaveManager.Dirty = true;

			GenerateLists();
		}

		private void listViewSettings_KeyDown(object sender, KeyEventArgs e)
		{
			if (listViewSettings.SelectedItems.Count > 0)
			{
				var selectedItem = listViewSettings.SelectedItems[0];
				var selectedKey = (BaseKey)selectedItem.Tag;

				if (selectedKey.Static)
					return;

				if (e.KeyCode == Keys.Delete)
				{
					DeleteKey(selectedKey);
				}
				else if (e.KeyCode == Keys.F2)
				{
					selectedItem.BeginEdit();
				}
			}
		}
	}
}

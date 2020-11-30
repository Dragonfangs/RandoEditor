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
	public enum DeleteResult
	{
		Delete,
		Replace,
		Cancel
	}

	public partial class DeleteReplaceForm : Form
	{
		private string keyName;

		public DeleteReplaceForm()
		{
			InitializeComponent();
		}

		public DeleteResult Result { get; set; }
		public Guid ReplaceId { get; set; }

		public void ShowDialog(string keyName)
		{
			this.keyName = keyName;
			labelWarning.Text = $"The key \"{keyName}\" is in use in one or more requirements, how would you like to update the affected requirements?";
			ShowDialog();
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show($"This will delete the key \"{keyName}\" and all references to it. This cannot be undone without reloading the file, are you sure?", "Delete confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				Result = DeleteResult.Delete;
				Close();
			}
		}

		private void buttonReplace_Click(object sender, EventArgs e)
		{
			var keySelector = new KeySelector();
			keySelector.StartPosition = FormStartPosition.CenterParent;
			keySelector.MultiSelect = false;
			if (keySelector.ShowDialog() != DialogResult.OK)
				return;
			
			var replaceKey = keySelector.SelectedKeys.FirstOrDefault();
			if(replaceKey == null)
				return;

			ReplaceId = replaceKey.Id;
			if (MessageBox.Show($"This will replace all references to the key \"{keyName}\" with \"{replaceKey.Name}\", and \"{keyName}\" will then be removed. This cannot be undone without reloading the file, are you sure?", "Replace confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				ReplaceId = keySelector.SelectedKeys.FirstOrDefault()?.Id ?? Guid.Empty;
				Result = DeleteResult.Replace;
				Close();
			}
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			Result = DeleteResult.Cancel;
			Close();
		}
	}
}

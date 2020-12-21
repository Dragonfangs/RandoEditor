using RandoEditor.CustomControl;
using System.Windows.Forms;

namespace RandoEditor
{
	partial class LockPanelLogic
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.ListViewGroup listViewGroup25 = new System.Windows.Forms.ListViewGroup("Randomized keys", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup26 = new System.Windows.Forms.ListViewGroup("Event Keys", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup27 = new System.Windows.Forms.ListViewGroup("Setting Keys", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup28 = new System.Windows.Forms.ListViewGroup("Custom Keys", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewItem listViewItem49 = new System.Windows.Forms.ListViewItem("Morph");
			System.Windows.Forms.ListViewItem listViewItem50 = new System.Windows.Forms.ListViewItem("Bombs");
			System.Windows.Forms.ListViewItem listViewItem51 = new System.Windows.Forms.ListViewItem("Missiles");
			System.Windows.Forms.ListViewItem listViewItem52 = new System.Windows.Forms.ListViewItem("Hi-Jump");
			System.Windows.Forms.ListViewItem listViewItem53 = new System.Windows.Forms.ListViewItem("Ziplines");
			System.Windows.Forms.ListViewItem listViewItem54 = new System.Windows.Forms.ListViewItem("Unknown Item 1 trigger");
			System.Windows.Forms.ListViewItem listViewItem55 = new System.Windows.Forms.ListViewItem("Can Walljump");
			System.Windows.Forms.ListViewItem listViewItem56 = new System.Windows.Forms.ListViewItem("Morph Jump");
			this.listView1 = new System.Windows.Forms.ListView();
			this.KeyName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.treeView1 = new RandoEditor.CustomControl.TreeViewScroll();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listView1
			// 
			this.listView1.Alignment = System.Windows.Forms.ListViewAlignment.Left;
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.KeyName});
			listViewGroup25.Header = "Randomized keys";
			listViewGroup25.Name = "listViewGroupRandom";
			listViewGroup26.Header = "Event Keys";
			listViewGroup26.Name = "listViewGroupEvents";
			listViewGroup27.Header = "Setting Keys";
			listViewGroup27.Name = "listViewGroupSettings";
			listViewGroup28.Header = "Custom Keys";
			listViewGroup28.Name = "listViewGroupCustom";
			this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup25,
            listViewGroup26,
            listViewGroup27,
            listViewGroup28});
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listView1.HideSelection = false;
			listViewItem49.Group = listViewGroup25;
			listViewItem50.Group = listViewGroup25;
			listViewItem51.Group = listViewGroup25;
			listViewItem52.Group = listViewGroup25;
			listViewItem53.Group = listViewGroup26;
			listViewItem54.Group = listViewGroup26;
			listViewItem55.Group = listViewGroup27;
			listViewItem56.Group = listViewGroup28;
			this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem49,
            listViewItem50,
            listViewItem51,
            listViewItem52,
            listViewItem53,
            listViewItem54,
            listViewItem55,
            listViewItem56});
			this.listView1.Location = new System.Drawing.Point(-2, -2);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(260, 220);
			this.listView1.TabIndex = 3;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listView1_ItemDrag);
			// 
			// KeyName
			// 
			this.KeyName.Width = 120;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.treeView1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.listView1);
			this.splitContainer1.Size = new System.Drawing.Size(263, 445);
			this.splitContainer1.SplitterDistance = 221;
			this.splitContainer1.TabIndex = 4;
			// 
			// treeView1
			// 
			this.treeView1.AllowDrop = true;
			this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.treeView1.CausesValidation = false;
			this.treeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
			this.treeView1.ItemHeight = 22;
			this.treeView1.Location = new System.Drawing.Point(-2, -2);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(263, 224);
			this.treeView1.TabIndex = 0;
			this.treeView1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.treeView1_Scroll);
			this.treeView1.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeCollapse);
			this.treeView1.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCollapse);
			this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
			this.treeView1.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterExpand);
			this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
			this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
			this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
			this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
			this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
			this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView1_DragOver);
			this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
			this.treeView1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.treeView1_KeyPress);
			this.treeView1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseWheel);
			// 
			// LockPanelLogic
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "LockPanelLogic";
			this.Size = new System.Drawing.Size(263, 445);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private TreeViewScroll treeView1;
        private ListView listView1;
        private ColumnHeader KeyName;
		private SplitContainer splitContainer1;
	}
}

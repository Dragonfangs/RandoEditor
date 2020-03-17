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
            System.Windows.Forms.ListViewGroup listViewGroup17 = new System.Windows.Forms.ListViewGroup("Randomized keys", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup18 = new System.Windows.Forms.ListViewGroup("Event Keys", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup19 = new System.Windows.Forms.ListViewGroup("Setting Keys", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup20 = new System.Windows.Forms.ListViewGroup("Custom Keys", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem33 = new System.Windows.Forms.ListViewItem("Morph");
            System.Windows.Forms.ListViewItem listViewItem34 = new System.Windows.Forms.ListViewItem("Bombs");
            System.Windows.Forms.ListViewItem listViewItem35 = new System.Windows.Forms.ListViewItem("Missiles");
            System.Windows.Forms.ListViewItem listViewItem36 = new System.Windows.Forms.ListViewItem("Hi-Jump");
            System.Windows.Forms.ListViewItem listViewItem37 = new System.Windows.Forms.ListViewItem("Ziplines");
            System.Windows.Forms.ListViewItem listViewItem38 = new System.Windows.Forms.ListViewItem("Unknown Item 1 trigger");
            System.Windows.Forms.ListViewItem listViewItem39 = new System.Windows.Forms.ListViewItem("Can Walljump");
            System.Windows.Forms.ListViewItem listViewItem40 = new System.Windows.Forms.ListViewItem("Morph Jump");
            this.listView1 = new System.Windows.Forms.ListView();
            this.KeyName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.treeView1 = new RandoEditor.CustomControl.TreeViewScroll();
            this.tableLayoutPanel1.SuspendLayout();
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
            listViewGroup17.Header = "Randomized keys";
            listViewGroup17.Name = "listViewGroupRandom";
            listViewGroup18.Header = "Event Keys";
            listViewGroup18.Name = "listViewGroupEvents";
            listViewGroup19.Header = "Setting Keys";
            listViewGroup19.Name = "listViewGroupSettings";
            listViewGroup20.Header = "Custom Keys";
            listViewGroup20.Name = "listViewGroupCustom";
            this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup17,
            listViewGroup18,
            listViewGroup19,
            listViewGroup20});
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.HideSelection = false;
            listViewItem33.Group = listViewGroup17;
            listViewItem34.Group = listViewGroup17;
            listViewItem35.Group = listViewGroup17;
            listViewItem36.Group = listViewGroup17;
            listViewItem37.Group = listViewGroup18;
            listViewItem38.Group = listViewGroup18;
            listViewItem39.Group = listViewGroup19;
            listViewItem40.Group = listViewGroup20;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem33,
            listViewItem34,
            listViewItem35,
            listViewItem36,
            listViewItem37,
            listViewItem38,
            listViewItem39,
            listViewItem40});
            this.listView1.Location = new System.Drawing.Point(3, 164);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(197, 155);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listView1_ItemDrag);
            // 
            // KeyName
            // 
            this.KeyName.Width = 120;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.listView1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.treeView1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(-3, -3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(203, 322);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.CausesValidation = false;
            this.treeView1.ItemHeight = 22;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(197, 155);
            this.treeView1.TabIndex = 0;
            this.treeView1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.treeView1_Scroll);
            this.treeView1.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeCollapse);
            this.treeView1.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCollapse);
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.treeView1.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterExpand);
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
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LockPanelLogic";
            this.Size = new System.Drawing.Size(200, 319);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private TreeViewScroll treeView1;
        private ListView listView1;
        private ColumnHeader KeyName;
        private TableLayoutPanel tableLayoutPanel1;
    }
}

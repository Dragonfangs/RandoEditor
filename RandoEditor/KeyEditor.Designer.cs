namespace RandoEditor
{
	partial class KeyEditor
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Active Space Jump");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Morph Jump");
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Ballspark");
			System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Active Space Jump");
			System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Morph Jump");
			System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Ballspark");
			System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Active Space Jump");
			System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Morph Jump");
			System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("Ballspark");
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.lockPanelLogic1 = new RandoEditor.LockPanelLogic();
			this.listViewCustoms = new System.Windows.Forms.ListView();
			this.KeyName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.newKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabComplex = new System.Windows.Forms.TabPage();
			this.tabEvents = new System.Windows.Forms.TabPage();
			this.listViewEvents = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tabSettings = new System.Windows.Forms.TabPage();
			this.listViewSettings = new System.Windows.Forms.ListView();
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tableLayoutPanel1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabComplex.SuspendLayout();
			this.tabEvents.SuspendLayout();
			this.tabSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.lockPanelLogic1, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.listViewCustoms, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(586, 466);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// lockPanelLogic1
			// 
			this.lockPanelLogic1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lockPanelLogic1.Location = new System.Drawing.Point(296, 3);
			this.lockPanelLogic1.Name = "lockPanelLogic1";
			this.lockPanelLogic1.Size = new System.Drawing.Size(287, 460);
			this.lockPanelLogic1.TabIndex = 0;
			// 
			// listViewCustoms
			// 
			this.listViewCustoms.Alignment = System.Windows.Forms.ListViewAlignment.Left;
			this.listViewCustoms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewCustoms.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.KeyName});
			this.listViewCustoms.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewCustoms.HideSelection = false;
			this.listViewCustoms.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3});
			this.listViewCustoms.LabelEdit = true;
			this.listViewCustoms.Location = new System.Drawing.Point(3, 3);
			this.listViewCustoms.MultiSelect = false;
			this.listViewCustoms.Name = "listViewCustoms";
			this.listViewCustoms.Size = new System.Drawing.Size(287, 460);
			this.listViewCustoms.TabIndex = 3;
			this.listViewCustoms.UseCompatibleStateImageBehavior = false;
			this.listViewCustoms.View = System.Windows.Forms.View.Details;
			this.listViewCustoms.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listViewCustoms_AfterLabelEdit);
			this.listViewCustoms.SelectedIndexChanged += new System.EventHandler(this.listViewCustoms_SelectedIndexChanged);
			this.listViewCustoms.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewCustoms_KeyDown);
			// 
			// KeyName
			// 
			this.KeyName.Width = 200;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newKeyToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(594, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// newKeyToolStripMenuItem
			// 
			this.newKeyToolStripMenuItem.Name = "newKeyToolStripMenuItem";
			this.newKeyToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
			this.newKeyToolStripMenuItem.Text = "New key";
			this.newKeyToolStripMenuItem.Click += new System.EventHandler(this.newKeyToolStripMenuItem_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabComplex);
			this.tabControl1.Controls.Add(this.tabEvents);
			this.tabControl1.Controls.Add(this.tabSettings);
			this.tabControl1.Location = new System.Drawing.Point(0, 27);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(594, 492);
			this.tabControl1.TabIndex = 2;
			// 
			// tabComplex
			// 
			this.tabComplex.Controls.Add(this.tableLayoutPanel1);
			this.tabComplex.Location = new System.Drawing.Point(4, 22);
			this.tabComplex.Name = "tabComplex";
			this.tabComplex.Padding = new System.Windows.Forms.Padding(3);
			this.tabComplex.Size = new System.Drawing.Size(586, 466);
			this.tabComplex.TabIndex = 0;
			this.tabComplex.Text = "Complex Keys";
			this.tabComplex.UseVisualStyleBackColor = true;
			// 
			// tabEvents
			// 
			this.tabEvents.Controls.Add(this.listViewEvents);
			this.tabEvents.Location = new System.Drawing.Point(4, 22);
			this.tabEvents.Name = "tabEvents";
			this.tabEvents.Padding = new System.Windows.Forms.Padding(3);
			this.tabEvents.Size = new System.Drawing.Size(586, 466);
			this.tabEvents.TabIndex = 1;
			this.tabEvents.Text = "Event keys";
			this.tabEvents.UseVisualStyleBackColor = true;
			// 
			// listViewEvents
			// 
			this.listViewEvents.Alignment = System.Windows.Forms.ListViewAlignment.Left;
			this.listViewEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listViewEvents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewEvents.HideSelection = false;
			this.listViewEvents.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem4,
            listViewItem5,
            listViewItem6});
			this.listViewEvents.LabelEdit = true;
			this.listViewEvents.Location = new System.Drawing.Point(3, 3);
			this.listViewEvents.MultiSelect = false;
			this.listViewEvents.Name = "listViewEvents";
			this.listViewEvents.Size = new System.Drawing.Size(287, 460);
			this.listViewEvents.TabIndex = 4;
			this.listViewEvents.UseCompatibleStateImageBehavior = false;
			this.listViewEvents.View = System.Windows.Forms.View.Details;
			this.listViewEvents.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listViewEvents_AfterLabelEdit);
			this.listViewEvents.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewEvents_KeyDown);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Width = 200;
			// 
			// tabSettings
			// 
			this.tabSettings.Controls.Add(this.listViewSettings);
			this.tabSettings.Location = new System.Drawing.Point(4, 22);
			this.tabSettings.Name = "tabSettings";
			this.tabSettings.Size = new System.Drawing.Size(586, 466);
			this.tabSettings.TabIndex = 2;
			this.tabSettings.Text = "Setting Keys";
			this.tabSettings.UseVisualStyleBackColor = true;
			// 
			// listViewSettings
			// 
			this.listViewSettings.Alignment = System.Windows.Forms.ListViewAlignment.Left;
			this.listViewSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewSettings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
			this.listViewSettings.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewSettings.HideSelection = false;
			this.listViewSettings.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem7,
            listViewItem8,
            listViewItem9});
			this.listViewSettings.LabelEdit = true;
			this.listViewSettings.Location = new System.Drawing.Point(3, 3);
			this.listViewSettings.MultiSelect = false;
			this.listViewSettings.Name = "listViewSettings";
			this.listViewSettings.Size = new System.Drawing.Size(287, 460);
			this.listViewSettings.TabIndex = 4;
			this.listViewSettings.UseCompatibleStateImageBehavior = false;
			this.listViewSettings.View = System.Windows.Forms.View.Details;
			this.listViewSettings.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listViewSettings_AfterLabelEdit);
			this.listViewSettings.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewSettings_KeyDown);
			// 
			// columnHeader2
			// 
			this.columnHeader2.Width = 200;
			// 
			// KeyEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(594, 520);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MainMenuStrip = this.menuStrip1;
			this.MinimumSize = new System.Drawing.Size(390, 350);
			this.Name = "KeyEditor";
			this.Text = "Key Editor";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.tabComplex.ResumeLayout(false);
			this.tabEvents.ResumeLayout(false);
			this.tabSettings.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private LockPanelLogic lockPanelLogic1;
		private System.Windows.Forms.ListView listViewCustoms;
		private System.Windows.Forms.ColumnHeader KeyName;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem newKeyToolStripMenuItem;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabComplex;
		private System.Windows.Forms.TabPage tabEvents;
		private System.Windows.Forms.TabPage tabSettings;
		private System.Windows.Forms.ListView listViewEvents;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ListView listViewSettings;
		private System.Windows.Forms.ColumnHeader columnHeader2;
	}
}
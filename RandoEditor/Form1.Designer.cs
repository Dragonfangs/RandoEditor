using System.Windows.Forms;

namespace RandoEditor
{
    partial class Form1
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

		public class MyPanel : System.Windows.Forms.Panel
		{
			public MyPanel()
			{
				this.DoubleBuffered = true;

				this.UpdateStyles();
			}
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.panel1 = new RandoEditor.Form1.MyPanel();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.comboBoxEvent = new System.Windows.Forms.ComboBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.keysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.customKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.lockPanelLogic1 = new RandoEditor.LockPanelLogic();
			this.chkOneWayConnection = new System.Windows.Forms.CheckBox();
			this.chkTwoWayConnection = new System.Windows.Forms.CheckBox();
			this.txtRandomId = new System.Windows.Forms.TextBox();
			this.chkNewNode = new System.Windows.Forms.CheckBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.Color.Black;
			this.panel1.Location = new System.Drawing.Point(48, 27);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(572, 672);
			this.panel1.TabIndex = 0;
			this.panel1.SizeChanged += new System.EventHandler(this.panel1_SizeChanged);
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
			this.panel1.MouseHover += new System.EventHandler(this.panel1_MouseHover);
			this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
			this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
			this.panel1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseWheel);
			// 
			// comboBox1
			// 
			this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(626, 27);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(189, 21);
			this.comboBox1.TabIndex = 1;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// comboBoxEvent
			// 
			this.comboBoxEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxEvent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxEvent.FormattingEnabled = true;
			this.comboBoxEvent.Location = new System.Drawing.Point(626, 54);
			this.comboBoxEvent.Name = "comboBoxEvent";
			this.comboBoxEvent.Size = new System.Drawing.Size(189, 21);
			this.comboBoxEvent.TabIndex = 3;
			this.comboBoxEvent.SelectedIndexChanged += new System.EventHandler(this.comboBoxEvent_SelectedIndexChanged);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.keysToolStripMenuItem,
            this.settingsToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(822, 24);
			this.menuStrip1.TabIndex = 4;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.newToolStripMenuItem.Text = "New";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.openToolStripMenuItem.Text = "Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.saveAsToolStripMenuItem.Text = "Save As...";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// keysToolStripMenuItem
			// 
			this.keysToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customKeysToolStripMenuItem});
			this.keysToolStripMenuItem.Name = "keysToolStripMenuItem";
			this.keysToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
			this.keysToolStripMenuItem.Text = "Keys";
			// 
			// customKeysToolStripMenuItem
			// 
			this.customKeysToolStripMenuItem.Name = "customKeysToolStripMenuItem";
			this.customKeysToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
			this.customKeysToolStripMenuItem.Text = "Custom keys";
			this.customKeysToolStripMenuItem.Click += new System.EventHandler(this.customKeysToolStripMenuItem_Click);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.settingsToolStripMenuItem.Text = "Settings";
			this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
			// 
			// lockPanelLogic1
			// 
			this.lockPanelLogic1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lockPanelLogic1.Location = new System.Drawing.Point(626, 54);
			this.lockPanelLogic1.Name = "lockPanelLogic1";
			this.lockPanelLogic1.Size = new System.Drawing.Size(189, 632);
			this.lockPanelLogic1.TabIndex = 5;
			// 
			// chkOneWayConnection
			// 
			this.chkOneWayConnection.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkOneWayConnection.BackgroundImage = global::RandoEditor.Properties.Resources.SingleConnection;
			this.chkOneWayConnection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.chkOneWayConnection.Location = new System.Drawing.Point(13, 81);
			this.chkOneWayConnection.Name = "chkOneWayConnection";
			this.chkOneWayConnection.Size = new System.Drawing.Size(29, 29);
			this.chkOneWayConnection.TabIndex = 7;
			this.toolTip.SetToolTip(this.chkOneWayConnection, "Create One-Way Connection (Alt)");
			this.chkOneWayConnection.UseVisualStyleBackColor = true;
			this.chkOneWayConnection.CheckedChanged += new System.EventHandler(this.chkOneWayConnection_CheckedChanged);
			// 
			// chkTwoWayConnection
			// 
			this.chkTwoWayConnection.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkTwoWayConnection.BackgroundImage = global::RandoEditor.Properties.Resources.DoubleConnection;
			this.chkTwoWayConnection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.chkTwoWayConnection.Location = new System.Drawing.Point(13, 116);
			this.chkTwoWayConnection.Name = "chkTwoWayConnection";
			this.chkTwoWayConnection.Size = new System.Drawing.Size(29, 29);
			this.chkTwoWayConnection.TabIndex = 8;
			this.toolTip.SetToolTip(this.chkTwoWayConnection, "Create Two-Way Connection (Shift)");
			this.chkTwoWayConnection.UseVisualStyleBackColor = true;
			this.chkTwoWayConnection.CheckedChanged += new System.EventHandler(this.chkTwoWayConnection_CheckedChanged);
			// 
			// txtRandomId
			// 
			this.txtRandomId.Location = new System.Drawing.Point(626, 55);
			this.txtRandomId.Name = "txtRandomId";
			this.txtRandomId.Size = new System.Drawing.Size(189, 20);
			this.txtRandomId.TabIndex = 9;
			this.txtRandomId.TextChanged += new System.EventHandler(this.txtRandomId_TextChanged);
			// 
			// chkNewNode
			// 
			this.chkNewNode.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkNewNode.BackgroundImage = global::RandoEditor.Properties.Resources.crossroads;
			this.chkNewNode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.chkNewNode.Location = new System.Drawing.Point(13, 46);
			this.chkNewNode.Name = "chkNewNode";
			this.chkNewNode.Size = new System.Drawing.Size(29, 29);
			this.chkNewNode.TabIndex = 6;
			this.toolTip.SetToolTip(this.chkNewNode, "Create new node (Ctrl)");
			this.chkNewNode.UseVisualStyleBackColor = true;
			this.chkNewNode.CheckedChanged += new System.EventHandler(this.chkNewNode_CheckedChanged);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(822, 711);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.txtRandomId);
			this.Controls.Add(this.chkTwoWayConnection);
			this.Controls.Add(this.chkOneWayConnection);
			this.Controls.Add(this.chkNewNode);
			this.Controls.Add(this.lockPanelLogic1);
			this.Controls.Add(this.comboBoxEvent);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.panel1);
			this.MainMenuStrip = this.menuStrip1;
			this.MinimumSize = new System.Drawing.Size(700, 400);
			this.Name = "Form1";
			this.Text = "Form1";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion

		private MyPanel panel1;
		private System.Windows.Forms.ComboBox comboBox1;
		private ComboBox comboBoxEvent;
		private MenuStrip menuStrip1;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem saveToolStripMenuItem;
		private LockPanelLogic lockPanelLogic1;
		private ToolStripMenuItem keysToolStripMenuItem;
		private ToolStripMenuItem customKeysToolStripMenuItem;
		private CheckBox chkNewNode;
		private CheckBox chkOneWayConnection;
		private CheckBox chkTwoWayConnection;
		private ToolStripMenuItem settingsToolStripMenuItem;
		private TextBox txtRandomId;
		private ToolStripMenuItem newToolStripMenuItem;
		private ToolStripMenuItem openToolStripMenuItem;
		private ToolStripMenuItem saveAsToolStripMenuItem;
		private ToolTip toolTip;
	}
}


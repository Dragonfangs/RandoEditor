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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.panel1 = new RandoEditor.Form1.MyPanel();
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
			this.chkNewBlankNode = new System.Windows.Forms.CheckBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.chkNewLockNode = new System.Windows.Forms.CheckBox();
			this.chkNewRandomNode = new System.Windows.Forms.CheckBox();
			this.chkNewEventNode = new System.Windows.Forms.CheckBox();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
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
			// comboBoxEvent
			// 
			this.comboBoxEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxEvent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxEvent.FormattingEnabled = true;
			this.comboBoxEvent.Location = new System.Drawing.Point(626, 27);
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
			this.lockPanelLogic1.Location = new System.Drawing.Point(626, 28);
			this.lockPanelLogic1.Name = "lockPanelLogic1";
			this.lockPanelLogic1.Size = new System.Drawing.Size(189, 658);
			this.lockPanelLogic1.TabIndex = 5;
			// 
			// chkOneWayConnection
			// 
			this.chkOneWayConnection.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkOneWayConnection.BackgroundImage = global::RandoEditor.Properties.Resources.SingleConnection;
			this.chkOneWayConnection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.chkOneWayConnection.Location = new System.Drawing.Point(12, 186);
			this.chkOneWayConnection.Name = "chkOneWayConnection";
			this.chkOneWayConnection.Size = new System.Drawing.Size(29, 29);
			this.chkOneWayConnection.TabIndex = 7;
			this.toolTip.SetToolTip(this.chkOneWayConnection, "Create One-Way Connection (Alt)");
			this.chkOneWayConnection.UseVisualStyleBackColor = true;
			this.chkOneWayConnection.CheckedChanged += new System.EventHandler(this.CheckBoxChecked);
			// 
			// chkTwoWayConnection
			// 
			this.chkTwoWayConnection.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkTwoWayConnection.BackgroundImage = global::RandoEditor.Properties.Resources.DoubleConnection;
			this.chkTwoWayConnection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.chkTwoWayConnection.Location = new System.Drawing.Point(12, 221);
			this.chkTwoWayConnection.Name = "chkTwoWayConnection";
			this.chkTwoWayConnection.Size = new System.Drawing.Size(29, 29);
			this.chkTwoWayConnection.TabIndex = 8;
			this.toolTip.SetToolTip(this.chkTwoWayConnection, "Create Two-Way Connection (Shift)");
			this.chkTwoWayConnection.UseVisualStyleBackColor = true;
			this.chkTwoWayConnection.CheckedChanged += new System.EventHandler(this.CheckBoxChecked);
			// 
			// txtRandomId
			// 
			this.txtRandomId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtRandomId.Location = new System.Drawing.Point(626, 28);
			this.txtRandomId.Name = "txtRandomId";
			this.txtRandomId.Size = new System.Drawing.Size(189, 20);
			this.txtRandomId.TabIndex = 9;
			this.txtRandomId.TextChanged += new System.EventHandler(this.txtRandomId_TextChanged);
			// 
			// chkNewBlankNode
			// 
			this.chkNewBlankNode.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkNewBlankNode.BackgroundImage = global::RandoEditor.Properties.Resources.crossroads;
			this.chkNewBlankNode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.chkNewBlankNode.Location = new System.Drawing.Point(13, 46);
			this.chkNewBlankNode.Name = "chkNewBlankNode";
			this.chkNewBlankNode.Size = new System.Drawing.Size(29, 29);
			this.chkNewBlankNode.TabIndex = 6;
			this.toolTip.SetToolTip(this.chkNewBlankNode, "Create new blank node");
			this.chkNewBlankNode.UseVisualStyleBackColor = true;
			this.chkNewBlankNode.CheckedChanged += new System.EventHandler(this.CheckBoxChecked);
			// 
			// chkNewLockNode
			// 
			this.chkNewLockNode.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkNewLockNode.BackgroundImage = global::RandoEditor.Properties.Resources.keyhole;
			this.chkNewLockNode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.chkNewLockNode.Location = new System.Drawing.Point(13, 81);
			this.chkNewLockNode.Name = "chkNewLockNode";
			this.chkNewLockNode.Size = new System.Drawing.Size(29, 29);
			this.chkNewLockNode.TabIndex = 10;
			this.toolTip.SetToolTip(this.chkNewLockNode, "Create new lock node");
			this.chkNewLockNode.UseVisualStyleBackColor = true;
			this.chkNewLockNode.CheckedChanged += new System.EventHandler(this.CheckBoxChecked);
			// 
			// chkNewRandomNode
			// 
			this.chkNewRandomNode.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkNewRandomNode.BackgroundImage = global::RandoEditor.Properties.Resources.question;
			this.chkNewRandomNode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.chkNewRandomNode.Location = new System.Drawing.Point(13, 116);
			this.chkNewRandomNode.Name = "chkNewRandomNode";
			this.chkNewRandomNode.Size = new System.Drawing.Size(29, 29);
			this.chkNewRandomNode.TabIndex = 11;
			this.toolTip.SetToolTip(this.chkNewRandomNode, "Create new randomized key node");
			this.chkNewRandomNode.UseVisualStyleBackColor = true;
			this.chkNewRandomNode.CheckedChanged += new System.EventHandler(this.CheckBoxChecked);
			// 
			// chkNewEventNode
			// 
			this.chkNewEventNode.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkNewEventNode.BackgroundImage = global::RandoEditor.Properties.Resources.exclamation;
			this.chkNewEventNode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.chkNewEventNode.Location = new System.Drawing.Point(13, 151);
			this.chkNewEventNode.Name = "chkNewEventNode";
			this.chkNewEventNode.Size = new System.Drawing.Size(29, 29);
			this.chkNewEventNode.TabIndex = 12;
			this.toolTip.SetToolTip(this.chkNewEventNode, "Create new event key node");
			this.chkNewEventNode.UseVisualStyleBackColor = true;
			this.chkNewEventNode.CheckedChanged += new System.EventHandler(this.CheckBoxChecked);
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(61, 4);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(822, 711);
			this.Controls.Add(this.chkNewEventNode);
			this.Controls.Add(this.chkNewRandomNode);
			this.Controls.Add(this.chkNewLockNode);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.txtRandomId);
			this.Controls.Add(this.chkTwoWayConnection);
			this.Controls.Add(this.chkOneWayConnection);
			this.Controls.Add(this.chkNewBlankNode);
			this.Controls.Add(this.lockPanelLogic1);
			this.Controls.Add(this.comboBoxEvent);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.MinimumSize = new System.Drawing.Size(700, 400);
			this.Name = "Form1";
			this.Text = "Logic Editor";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion

		private MyPanel panel1;
		private ComboBox comboBoxEvent;
		private MenuStrip menuStrip1;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem saveToolStripMenuItem;
		private LockPanelLogic lockPanelLogic1;
		private ToolStripMenuItem keysToolStripMenuItem;
		private ToolStripMenuItem customKeysToolStripMenuItem;
		private CheckBox chkNewBlankNode;
		private CheckBox chkOneWayConnection;
		private CheckBox chkTwoWayConnection;
		private ToolStripMenuItem settingsToolStripMenuItem;
		private TextBox txtRandomId;
		private ToolStripMenuItem newToolStripMenuItem;
		private ToolStripMenuItem openToolStripMenuItem;
		private ToolStripMenuItem saveAsToolStripMenuItem;
		private ToolTip toolTip;
		private CheckBox chkNewLockNode;
		private CheckBox chkNewRandomNode;
		private CheckBox chkNewEventNode;
		private ContextMenuStrip contextMenuStrip;
	}
}


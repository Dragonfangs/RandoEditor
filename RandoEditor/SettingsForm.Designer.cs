namespace RandoEditor
{
	partial class SettingsForm
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
			this.chkSimpleNodes = new System.Windows.Forms.CheckBox();
			this.trkMapQuality = new System.Windows.Forms.TrackBar();
			this.lblMapQuality = new System.Windows.Forms.Label();
			this.lblQualityTrackMin = new System.Windows.Forms.Label();
			this.lblQualityTrackMax = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.trkMapQuality)).BeginInit();
			this.SuspendLayout();
			// 
			// chkSimpleNodes
			// 
			this.chkSimpleNodes.AutoSize = true;
			this.chkSimpleNodes.Location = new System.Drawing.Point(13, 13);
			this.chkSimpleNodes.Name = "chkSimpleNodes";
			this.chkSimpleNodes.Size = new System.Drawing.Size(131, 17);
			this.chkSimpleNodes.TabIndex = 0;
			this.chkSimpleNodes.Text = "Simple Node Graphics";
			this.chkSimpleNodes.UseVisualStyleBackColor = true;
			this.chkSimpleNodes.CheckedChanged += new System.EventHandler(this.chkSimpleNodes_CheckedChanged);
			// 
			// trkMapQuality
			// 
			this.trkMapQuality.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.trkMapQuality.Location = new System.Drawing.Point(15, 74);
			this.trkMapQuality.Name = "trkMapQuality";
			this.trkMapQuality.Size = new System.Drawing.Size(267, 45);
			this.trkMapQuality.TabIndex = 1;
			this.trkMapQuality.Scroll += new System.EventHandler(this.trkMapQuality_Scroll);
			// 
			// lblMapQuality
			// 
			this.lblMapQuality.AutoSize = true;
			this.lblMapQuality.Location = new System.Drawing.Point(12, 58);
			this.lblMapQuality.Name = "lblMapQuality";
			this.lblMapQuality.Size = new System.Drawing.Size(63, 13);
			this.lblMapQuality.TabIndex = 2;
			this.lblMapQuality.Text = "Map Quality";
			// 
			// lblQualityTrackMin
			// 
			this.lblQualityTrackMin.AutoSize = true;
			this.lblQualityTrackMin.Location = new System.Drawing.Point(15, 105);
			this.lblQualityTrackMin.Name = "lblQualityTrackMin";
			this.lblQualityTrackMin.Size = new System.Drawing.Size(67, 13);
			this.lblQualityTrackMin.TabIndex = 3;
			this.lblQualityTrackMin.Text = "Performance";
			// 
			// lblQualityTrackMax
			// 
			this.lblQualityTrackMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblQualityTrackMax.AutoSize = true;
			this.lblQualityTrackMax.Location = new System.Drawing.Point(246, 105);
			this.lblQualityTrackMax.Name = "lblQualityTrackMax";
			this.lblQualityTrackMax.Size = new System.Drawing.Size(39, 13);
			this.lblQualityTrackMax.TabIndex = 4;
			this.lblQualityTrackMax.Text = "Quality";
			// 
			// SettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(294, 255);
			this.Controls.Add(this.lblQualityTrackMax);
			this.Controls.Add(this.lblQualityTrackMin);
			this.Controls.Add(this.lblMapQuality);
			this.Controls.Add(this.trkMapQuality);
			this.Controls.Add(this.chkSimpleNodes);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MinimumSize = new System.Drawing.Size(250, 200);
			this.Name = "SettingsForm";
			this.Text = "Settings";
			this.Shown += new System.EventHandler(this.SettingsForm_Shown);
			((System.ComponentModel.ISupportInitialize)(this.trkMapQuality)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox chkSimpleNodes;
		private System.Windows.Forms.TrackBar trkMapQuality;
		private System.Windows.Forms.Label lblMapQuality;
		private System.Windows.Forms.Label lblQualityTrackMin;
		private System.Windows.Forms.Label lblQualityTrackMax;
	}
}
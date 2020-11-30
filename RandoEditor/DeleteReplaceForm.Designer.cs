namespace RandoEditor
{
	partial class DeleteReplaceForm
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
			this.components = new System.ComponentModel.Container();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonReplace = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.labelWarning = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDelete.Location = new System.Drawing.Point(329, 107);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(75, 23);
			this.buttonDelete.TabIndex = 0;
			this.buttonDelete.Text = "Delete";
			this.toolTip1.SetToolTip(this.buttonDelete, "Will remove all instances of deleted key from requirements, removing any complex " +
        "statement that will turn out empty as a result.");
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonReplace
			// 
			this.buttonReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonReplace.Location = new System.Drawing.Point(410, 107);
			this.buttonReplace.Name = "buttonReplace";
			this.buttonReplace.Size = new System.Drawing.Size(75, 23);
			this.buttonReplace.TabIndex = 1;
			this.buttonReplace.Text = "Replace";
			this.toolTip1.SetToolTip(this.buttonReplace, "Will replace the removed key with another key of your choice");
			this.buttonReplace.UseVisualStyleBackColor = true;
			this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.Location = new System.Drawing.Point(491, 107);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.toolTip1.SetToolTip(this.buttonCancel, "Don\'t delete key and go back to previous screen");
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// labelWarning
			// 
			this.labelWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelWarning.AutoSize = true;
			this.labelWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelWarning.Location = new System.Drawing.Point(170, 28);
			this.labelWarning.MaximumSize = new System.Drawing.Size(400, 0);
			this.labelWarning.Name = "labelWarning";
			this.labelWarning.Size = new System.Drawing.Size(396, 32);
			this.labelWarning.TabIndex = 3;
			this.labelWarning.Text = "The key \"SomeName\" is in use in one or more requirements, how would you like to u" +
    "pdate the affected requirements?";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::RandoEditor.Properties.Resources.WarningIcon;
			this.pictureBox1.Location = new System.Drawing.Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(137, 118);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 4;
			this.pictureBox1.TabStop = false;
			// 
			// DeleteActions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(578, 142);
			this.ControlBox = false;
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.labelWarning);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonReplace);
			this.Controls.Add(this.buttonDelete);
			this.Name = "DeleteActions";
			this.Text = "Delete Actions";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button buttonReplace;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label labelWarning;
		private System.Windows.Forms.PictureBox pictureBox1;
	}
}
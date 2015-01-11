namespace subup
{
    partial class SubdownForm
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
            this.btnChoosePath = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tbxSearchPath = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.cbxSubtitleList = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // btnChoosePath
            // 
            this.btnChoosePath.Location = new System.Drawing.Point(310, 13);
            this.btnChoosePath.Name = "btnChoosePath";
            this.btnChoosePath.Size = new System.Drawing.Size(75, 23);
            this.btnChoosePath.TabIndex = 0;
            this.btnChoosePath.Text = "open";
            this.btnChoosePath.UseVisualStyleBackColor = true;
            this.btnChoosePath.Click += new System.EventHandler(this.btnChoosePath_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tbxSearchPath
            // 
            this.tbxSearchPath.Location = new System.Drawing.Point(13, 15);
            this.tbxSearchPath.Name = "tbxSearchPath";
            this.tbxSearchPath.Size = new System.Drawing.Size(291, 20);
            this.tbxSearchPath.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(310, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "download";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // cbxSubtitleList
            // 
            this.cbxSubtitleList.FormattingEnabled = true;
            this.cbxSubtitleList.Location = new System.Drawing.Point(13, 42);
            this.cbxSubtitleList.Name = "cbxSubtitleList";
            this.cbxSubtitleList.Size = new System.Drawing.Size(291, 184);
            this.cbxSubtitleList.TabIndex = 4;
            // 
            // SubdownForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 262);
            this.Controls.Add(this.cbxSubtitleList);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbxSearchPath);
            this.Controls.Add(this.btnChoosePath);
            this.Name = "SubdownForm";
            this.Text = "Subtitle download";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnChoosePath;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox tbxSearchPath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckedListBox cbxSubtitleList;
    }
}


namespace YoutubeDownloader
{
    partial class MainForm
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
            this.btnInstance = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // btnInstance
            // 
            this.btnInstance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInstance.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInstance.Location = new System.Drawing.Point(153, 21);
            this.btnInstance.Name = "btnInstance";
            this.btnInstance.Size = new System.Drawing.Size(290, 30);
            this.btnInstance.TabIndex = 0;
            this.btnInstance.Text = "Create New Instance";
            this.btnInstance.UseVisualStyleBackColor = true;
            this.btnInstance.Click += new System.EventHandler(this.btnInstance_ClickAsync);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Location = new System.Drawing.Point(13, 73);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(553, 365);
            this.panel1.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnInstance);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnInstance;
        private System.Windows.Forms.Panel panel1;
    }
}
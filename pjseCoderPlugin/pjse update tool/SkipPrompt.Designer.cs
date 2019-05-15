namespace pjse
{
    partial class SkipPrompt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SkipPrompt));
            this.label1 = new System.Windows.Forms.Label();
            this.llURL = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.btnLater = new System.Windows.Forms.Button();
            this.btnVisit = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Name = "label1";
            // 
            // llURL
            // 
            resources.ApplyResources(this.llURL, "llURL");
            this.tableLayoutPanel1.SetColumnSpan(this.llURL, 3);
            this.llURL.Name = "llURL";
            this.llURL.TabStop = true;
            this.llURL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llURL_LinkClicked);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.btnIgnore, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.llURL, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnLater, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnVisit, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // btnIgnore
            // 
            resources.ApplyResources(this.btnIgnore, "btnIgnore");
            this.btnIgnore.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.UseVisualStyleBackColor = true;
            // 
            // btnLater
            // 
            resources.ApplyResources(this.btnLater, "btnLater");
            this.btnLater.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnLater.Name = "btnLater";
            this.btnLater.UseVisualStyleBackColor = true;
            // 
            // btnVisit
            // 
            resources.ApplyResources(this.btnVisit, "btnVisit");
            this.btnVisit.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnVisit.Name = "btnVisit";
            this.btnVisit.UseVisualStyleBackColor = true;
            // 
            // SkipPrompt
            // 
            this.AcceptButton = this.btnVisit;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnLater;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SkipPrompt";
            this.ShowInTaskbar = false;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnLater;
        private System.Windows.Forms.Button btnVisit;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.LinkLabel llURL;
    }
}
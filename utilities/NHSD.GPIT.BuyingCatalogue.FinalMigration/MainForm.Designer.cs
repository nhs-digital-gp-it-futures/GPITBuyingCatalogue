
namespace NHSD.GPIT.BuyingCatalogue.FinalMigration
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.runMigrationButton = new System.Windows.Forms.Button();
            this.reconcileButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // outputTextBox
            // 
            this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTextBox.Location = new System.Drawing.Point(12, 83);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputTextBox.Size = new System.Drawing.Size(801, 404);
            this.outputTextBox.TabIndex = 0;
            // 
            // runMigrationButton
            // 
            this.runMigrationButton.Location = new System.Drawing.Point(12, 21);
            this.runMigrationButton.Name = "runMigrationButton";
            this.runMigrationButton.Size = new System.Drawing.Size(119, 23);
            this.runMigrationButton.TabIndex = 1;
            this.runMigrationButton.Text = "Run Migration";
            this.runMigrationButton.UseVisualStyleBackColor = true;
            this.runMigrationButton.Click += new System.EventHandler(this.runMigrationButton_Click);
            // 
            // reconcileButton
            // 
            this.reconcileButton.Location = new System.Drawing.Point(166, 21);
            this.reconcileButton.Name = "reconcileButton";
            this.reconcileButton.Size = new System.Drawing.Size(119, 23);
            this.reconcileButton.TabIndex = 2;
            this.reconcileButton.Text = "Reconcile";
            this.reconcileButton.UseVisualStyleBackColor = true;
            this.reconcileButton.Click += new System.EventHandler(this.reconcileButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 499);
            this.Controls.Add(this.reconcileButton);
            this.Controls.Add(this.runMigrationButton);
            this.Controls.Add(this.outputTextBox);
            this.Name = "MainForm";
            this.Text = "NHSD GPIT Buying Catalogue Final Migration Utility";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Button runMigrationButton;
        private System.Windows.Forms.Button reconcileButton;
    }
}


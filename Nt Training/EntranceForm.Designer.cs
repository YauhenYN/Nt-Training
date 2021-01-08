namespace Nt_Training
{
    partial class EntranceForm
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
            this.toGraphicsForm = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // toGraphicsForm
            // 
            this.toGraphicsForm.Location = new System.Drawing.Point(291, 214);
            this.toGraphicsForm.Name = "toGraphicsForm";
            this.toGraphicsForm.Size = new System.Drawing.Size(187, 47);
            this.toGraphicsForm.TabIndex = 0;
            this.toGraphicsForm.Text = "Создать";
            this.toGraphicsForm.UseVisualStyleBackColor = true;
            this.toGraphicsForm.Click += new System.EventHandler(this.toGraphicsForm_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(291, 267);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(187, 47);
            this.button1.TabIndex = 1;
            this.button1.Text = "Загрузить";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // EntranceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.toGraphicsForm);
            this.Name = "EntranceForm";
            this.Text = "EntranceForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button toGraphicsForm;
        private System.Windows.Forms.Button button1;
    }
}

namespace TAK
{
    partial class TAK
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
            this.pionWhite = new System.Windows.Forms.Button();
            this.superWhite = new System.Windows.Forms.Button();
            this.pionBlack = new System.Windows.Forms.Button();
            this.superBlack = new System.Windows.Forms.Button();
            this.standWhite = new System.Windows.Forms.Label();
            this.standBlack = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pionWhite
            // 
            this.pionWhite.BackColor = System.Drawing.Color.NavajoWhite;
            this.pionWhite.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pionWhite.Location = new System.Drawing.Point(69, 155);
            this.pionWhite.Name = "pionWhite";
            this.pionWhite.Size = new System.Drawing.Size(75, 75);
            this.pionWhite.TabIndex = 0;
            this.pionWhite.Text = "21";
            this.pionWhite.UseVisualStyleBackColor = false;
            this.pionWhite.Click += new System.EventHandler(this.PionWhite_Click);
            // 
            // superWhite
            // 
            this.superWhite.BackColor = System.Drawing.Color.NavajoWhite;
            this.superWhite.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.superWhite.Location = new System.Drawing.Point(85, 303);
            this.superWhite.Name = "superWhite";
            this.superWhite.Size = new System.Drawing.Size(40, 75);
            this.superWhite.TabIndex = 0;
            this.superWhite.Text = "1";
            this.superWhite.UseVisualStyleBackColor = false;
            this.superWhite.Click += new System.EventHandler(this.SuperPion_Click);
            // 
            // pionBlack
            // 
            this.pionBlack.BackColor = System.Drawing.Color.DarkGray;
            this.pionBlack.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pionBlack.ForeColor = System.Drawing.Color.Black;
            this.pionBlack.Location = new System.Drawing.Point(661, 155);
            this.pionBlack.Name = "pionBlack";
            this.pionBlack.Size = new System.Drawing.Size(75, 75);
            this.pionBlack.TabIndex = 0;
            this.pionBlack.Text = "21";
            this.pionBlack.UseVisualStyleBackColor = false;
            this.pionBlack.Click += new System.EventHandler(this.PionBlack_Click);
            // 
            // superBlack
            // 
            this.superBlack.BackColor = System.Drawing.Color.DarkGray;
            this.superBlack.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.superBlack.ForeColor = System.Drawing.Color.Black;
            this.superBlack.Location = new System.Drawing.Point(677, 303);
            this.superBlack.Name = "superBlack";
            this.superBlack.Size = new System.Drawing.Size(40, 75);
            this.superBlack.TabIndex = 0;
            this.superBlack.Text = "1";
            this.superBlack.UseVisualStyleBackColor = false;
            this.superBlack.Click += new System.EventHandler(this.SuperPion_Click);
            // 
            // standWhite
            // 
            this.standWhite.AutoSize = true;
            this.standWhite.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.standWhite.Location = new System.Drawing.Point(71, 233);
            this.standWhite.Name = "standWhite";
            this.standWhite.Size = new System.Drawing.Size(74, 24);
            this.standWhite.TabIndex = 1;
            this.standWhite.Text = "STAND";
            this.standWhite.Visible = false;
            // 
            // standBlack
            // 
            this.standBlack.AutoSize = true;
            this.standBlack.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.standBlack.Location = new System.Drawing.Point(663, 233);
            this.standBlack.Name = "standBlack";
            this.standBlack.Size = new System.Drawing.Size(74, 24);
            this.standBlack.TabIndex = 1;
            this.standBlack.Text = "STAND";
            this.standBlack.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(347, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 55);
            this.label1.TabIndex = 1;
            this.label1.Text = "TAK";
            // 
            // TAK
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 518);
            this.Controls.Add(this.standBlack);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.standWhite);
            this.Controls.Add(this.superBlack);
            this.Controls.Add(this.pionBlack);
            this.Controls.Add(this.superWhite);
            this.Controls.Add(this.pionWhite);
            this.Name = "TAK";
            this.Text = "TAK";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button pionWhite;
        private System.Windows.Forms.Button superWhite;
        private System.Windows.Forms.Button pionBlack;
        private System.Windows.Forms.Button superBlack;
        private System.Windows.Forms.Label standWhite;
        private System.Windows.Forms.Label standBlack;
        private System.Windows.Forms.Label label1;
    }
}


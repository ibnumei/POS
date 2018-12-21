namespace try_bi
{
    partial class Form_First_Opened
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_First_Opened));
            this.b_biensi = new Bunifu.Framework.UI.BunifuThinButton2();
            this.b_connection = new Bunifu.Framework.UI.BunifuThinButton2();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // b_biensi
            // 
            this.b_biensi.ActiveBorderThickness = 2;
            this.b_biensi.ActiveCornerRadius = 20;
            this.b_biensi.ActiveFillColor = System.Drawing.Color.White;
            this.b_biensi.ActiveForecolor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.b_biensi.ActiveLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.b_biensi.BackColor = System.Drawing.Color.WhiteSmoke;
            this.b_biensi.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("b_biensi.BackgroundImage")));
            this.b_biensi.ButtonText = "BIENSI POS";
            this.b_biensi.Cursor = System.Windows.Forms.Cursors.Hand;
            this.b_biensi.Font = new System.Drawing.Font("Arial Narrow", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.b_biensi.ForeColor = System.Drawing.Color.SeaGreen;
            this.b_biensi.IdleBorderThickness = 2;
            this.b_biensi.IdleCornerRadius = 20;
            this.b_biensi.IdleFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.b_biensi.IdleForecolor = System.Drawing.Color.White;
            this.b_biensi.IdleLineColor = System.Drawing.Color.White;
            this.b_biensi.Location = new System.Drawing.Point(116, 129);
            this.b_biensi.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.b_biensi.Name = "b_biensi";
            this.b_biensi.Size = new System.Drawing.Size(304, 97);
            this.b_biensi.TabIndex = 31;
            this.b_biensi.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.b_biensi.Click += new System.EventHandler(this.b_biensi_Click);
            // 
            // b_connection
            // 
            this.b_connection.ActiveBorderThickness = 2;
            this.b_connection.ActiveCornerRadius = 20;
            this.b_connection.ActiveFillColor = System.Drawing.Color.White;
            this.b_connection.ActiveForecolor = System.Drawing.Color.LimeGreen;
            this.b_connection.ActiveLineColor = System.Drawing.Color.LimeGreen;
            this.b_connection.BackColor = System.Drawing.Color.WhiteSmoke;
            this.b_connection.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("b_connection.BackgroundImage")));
            this.b_connection.ButtonText = "CONNECTION";
            this.b_connection.Cursor = System.Windows.Forms.Cursors.Hand;
            this.b_connection.Font = new System.Drawing.Font("Arial Narrow", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.b_connection.ForeColor = System.Drawing.Color.SeaGreen;
            this.b_connection.IdleBorderThickness = 2;
            this.b_connection.IdleCornerRadius = 20;
            this.b_connection.IdleFillColor = System.Drawing.Color.LimeGreen;
            this.b_connection.IdleForecolor = System.Drawing.Color.White;
            this.b_connection.IdleLineColor = System.Drawing.Color.White;
            this.b_connection.Location = new System.Drawing.Point(632, 129);
            this.b_connection.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.b_connection.Name = "b_connection";
            this.b_connection.Size = new System.Drawing.Size(304, 97);
            this.b_connection.TabIndex = 30;
            this.b_connection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.b_connection.Click += new System.EventHandler(this.b_connection_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 29);
            this.label1.TabIndex = 32;
            this.label1.Text = "V   1.1 Patch 1";
            // 
            // Form_First_Opened
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1052, 354);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.b_biensi);
            this.Controls.Add(this.b_connection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "Form_First_Opened";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Form_First_Opened_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Bunifu.Framework.UI.BunifuThinButton2 b_biensi;
        private Bunifu.Framework.UI.BunifuThinButton2 b_connection;
        private System.Windows.Forms.Label label1;
    }
}
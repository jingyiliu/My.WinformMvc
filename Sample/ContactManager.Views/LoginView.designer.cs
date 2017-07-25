using System.Windows.Forms;

namespace ContactManager.Views
{
    partial class LoginView
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginView));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LoginName = new System.Windows.Forms.TextBox();
            this.Password = new System.Windows.Forms.TextBox();
            this.btLogin = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblMessageVisibility = new System.Windows.Forms.Label();
            this.plTop = new System.Windows.Forms.Panel();
            this.plBottom = new System.Windows.Forms.Panel();
            this.lbClose = new System.Windows.Forms.LinkLabel();
            this.plCenter = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label31 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.plBottom.SuspendLayout();
            this.plCenter.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "User Name :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 22);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password :";
            // 
            // LoginName
            // 
            this.LoginName.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginName.Location = new System.Drawing.Point(100, 0);
            this.LoginName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.LoginName.Name = "LoginName";
            this.LoginName.Size = new System.Drawing.Size(191, 23);
            this.LoginName.TabIndex = 2;
            // 
            // Password
            // 
            this.Password.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Password.Location = new System.Drawing.Point(100, 31);
            this.Password.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Password.Name = "Password";
            this.Password.PasswordChar = '*';
            this.Password.Size = new System.Drawing.Size(191, 23);
            this.Password.TabIndex = 3;
            // 
            // btLogin
            // 
            this.btLogin.Location = new System.Drawing.Point(100, 62);
            this.btLogin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btLogin.Name = "btLogin";
            this.btLogin.Size = new System.Drawing.Size(93, 28);
            this.btLogin.TabIndex = 4;
            this.btLogin.Text = "Login";
            this.btLogin.UseVisualStyleBackColor = true;
            this.btLogin.Click += new System.EventHandler(this.btLogin_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.lblMessageVisibility);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(792, 79);
            this.panel1.TabIndex = 6;
            // 
            // lblMessageVisibility
            // 
            this.lblMessageVisibility.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessageVisibility.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessageVisibility.ForeColor = System.Drawing.Color.IndianRed;
            this.lblMessageVisibility.Location = new System.Drawing.Point(0, 0);
            this.lblMessageVisibility.Name = "lblMessageVisibility";
            this.lblMessageVisibility.Size = new System.Drawing.Size(792, 79);
            this.lblMessageVisibility.TabIndex = 0;
            this.lblMessageVisibility.Text = "Label Context Message";
            this.lblMessageVisibility.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMessageVisibility.Visible = false;
            // 
            // plTop
            // 
            this.plTop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("plTop.BackgroundImage")));
            this.plTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.plTop.Location = new System.Drawing.Point(0, 0);
            this.plTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.plTop.Name = "plTop";
            this.plTop.Size = new System.Drawing.Size(792, 79);
            this.plTop.TabIndex = 8;
            // 
            // plBottom
            // 
            this.plBottom.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("plBottom.BackgroundImage")));
            this.plBottom.Controls.Add(this.lbClose);
            this.plBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.plBottom.Location = new System.Drawing.Point(0, 469);
            this.plBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.plBottom.Name = "plBottom";
            this.plBottom.Size = new System.Drawing.Size(792, 97);
            this.plBottom.TabIndex = 9;
            // 
            // lbClose
            // 
            this.lbClose.BackColor = System.Drawing.Color.Transparent;
            this.lbClose.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbClose.ForeColor = System.Drawing.Color.White;
            this.lbClose.Image = ((System.Drawing.Image)(resources.GetObject("lbClose.Image")));
            this.lbClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbClose.LinkColor = System.Drawing.Color.White;
            this.lbClose.Location = new System.Drawing.Point(13, 13);
            this.lbClose.Name = "lbClose";
            this.lbClose.Size = new System.Drawing.Size(83, 26);
            this.lbClose.TabIndex = 0;
            this.lbClose.TabStop = true;
            this.lbClose.Text = "Close";
            this.lbClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbClose.Click += new System.EventHandler(this.lbExit_Click);
            // 
            // plCenter
            // 
            this.plCenter.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("plCenter.BackgroundImage")));
            this.plCenter.Controls.Add(this.panel4);
            this.plCenter.Controls.Add(this.panel2);
            this.plCenter.Controls.Add(this.panel1);
            this.plCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plCenter.Location = new System.Drawing.Point(0, 79);
            this.plCenter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.plCenter.Name = "plCenter";
            this.plCenter.Size = new System.Drawing.Size(792, 390);
            this.plCenter.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.Controls.Add(this.panel5);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Location = new System.Drawing.Point(132, 84);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(274, 228);
            this.panel4.TabIndex = 9;
            // 
            // panel5
            // 
            this.panel5.Location = new System.Drawing.Point(36, 83);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(200, 100);
            this.panel5.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(268, 69);
            this.label4.TabIndex = 12;
            this.label4.Text = "My Winform.Mvc Tutorial";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Location = new System.Drawing.Point(413, 84);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(370, 228);
            this.panel2.TabIndex = 8;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.LoginName);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.btLogin);
            this.panel3.Controls.Add(this.Password);
            this.panel3.Location = new System.Drawing.Point(9, 9);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(352, 112);
            this.panel3.TabIndex = 0;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(6, 17);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(123, 24);
            this.label31.TabIndex = 8;
            this.label31.Text = "Signing In ...";
            // 
            // LoginView
            // 
            this.AcceptButton = this.btLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.CancelButton = this.lbClose;
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Controls.Add(this.plCenter);
            this.Controls.Add(this.plBottom);
            this.Controls.Add(this.plTop);
            this.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "LoginView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login BaseView";
            this.panel1.ResumeLayout(false);
            this.plBottom.ResumeLayout(false);
            this.plCenter.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox LoginName;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Button btLogin;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblMessageVisibility;
        private System.Windows.Forms.Panel plBottom;
        private System.Windows.Forms.Panel plTop;
        private System.Windows.Forms.Panel plCenter;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.LinkLabel lbClose;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel5;
    }
}
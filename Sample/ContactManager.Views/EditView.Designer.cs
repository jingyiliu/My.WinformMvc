using System.Windows.Forms;

namespace ContactManager.Views
{
    partial class EditView
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
            System.Windows.Forms.Label emailLabel;
            System.Windows.Forms.Label firstNameLabel;
            System.Windows.Forms.Label lastNameLabel;
            System.Windows.Forms.Label phoneNumberLabel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditView));
            this.Email = new System.Windows.Forms.TextBox();
            this.FirstName = new System.Windows.Forms.TextBox();
            this.LastName = new System.Windows.Forms.TextBox();
            this.PhoneNumber = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btOk = new System.Windows.Forms.Button();
            this.plContextMessage = new System.Windows.Forms.Panel();
            this.lblErrorMessage = new System.Windows.Forms.Label();
            this.lkViewList = new System.Windows.Forms.LinkLabel();
            emailLabel = new System.Windows.Forms.Label();
            firstNameLabel = new System.Windows.Forms.Label();
            lastNameLabel = new System.Windows.Forms.Label();
            phoneNumberLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.plContextMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // emailLabel
            // 
            emailLabel.AutoSize = true;
            emailLabel.Location = new System.Drawing.Point(39, 242);
            emailLabel.Name = "emailLabel";
            emailLabel.Size = new System.Drawing.Size(41, 12);
            emailLabel.TabIndex = 3;
            emailLabel.Text = "Email:";
            // 
            // firstNameLabel
            // 
            firstNameLabel.AutoSize = true;
            firstNameLabel.Location = new System.Drawing.Point(39, 126);
            firstNameLabel.Name = "firstNameLabel";
            firstNameLabel.Size = new System.Drawing.Size(71, 12);
            firstNameLabel.TabIndex = 7;
            firstNameLabel.Text = "First Name:";
            // 
            // lastNameLabel
            // 
            lastNameLabel.AutoSize = true;
            lastNameLabel.Location = new System.Drawing.Point(39, 164);
            lastNameLabel.Name = "lastNameLabel";
            lastNameLabel.Size = new System.Drawing.Size(65, 12);
            lastNameLabel.TabIndex = 17;
            lastNameLabel.Text = "Last Name:";
            // 
            // phoneNumberLabel
            // 
            phoneNumberLabel.AutoSize = true;
            phoneNumberLabel.Location = new System.Drawing.Point(39, 203);
            phoneNumberLabel.Name = "phoneNumberLabel";
            phoneNumberLabel.Size = new System.Drawing.Size(83, 12);
            phoneNumberLabel.TabIndex = 19;
            phoneNumberLabel.Text = "Phone Number:";
            // 
            // Email
            // 
            this.Email.Location = new System.Drawing.Point(124, 239);
            this.Email.Name = "Email";
            this.Email.Size = new System.Drawing.Size(280, 21);
            this.Email.TabIndex = 3;
            // 
            // FirstName
            // 
            this.FirstName.Location = new System.Drawing.Point(124, 123);
            this.FirstName.Name = "FirstName";
            this.FirstName.Size = new System.Drawing.Size(280, 21);
            this.FirstName.TabIndex = 0;
            // 
            // LastName
            // 
            this.LastName.Location = new System.Drawing.Point(124, 162);
            this.LastName.Name = "LastName";
            this.LastName.Size = new System.Drawing.Size(280, 21);
            this.LastName.TabIndex = 1;
            // 
            // PhoneNumber
            // 
            this.PhoneNumber.Location = new System.Drawing.Point(124, 200);
            this.PhoneNumber.Name = "PhoneNumber";
            this.PhoneNumber.Size = new System.Drawing.Size(280, 21);
            this.PhoneNumber.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(448, 75);
            this.panel1.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(448, 75);
            this.label1.TabIndex = 0;
            this.label1.Text = "CREATE OR MODIFY YOUR CONTACT";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btOk
            // 
            this.btOk.Location = new System.Drawing.Point(291, 287);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(113, 28);
            this.btOk.TabIndex = 4;
            this.btOk.Text = "Ok";
            this.btOk.UseVisualStyleBackColor = true;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // plContextMessage
            // 
            this.plContextMessage.Controls.Add(this.lblErrorMessage);
            this.plContextMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.plContextMessage.Location = new System.Drawing.Point(0, 75);
            this.plContextMessage.Name = "plContextMessage";
            this.plContextMessage.Size = new System.Drawing.Size(448, 45);
            this.plContextMessage.TabIndex = 24;
            // 
            // lblErrorMessage
            // 
            this.lblErrorMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblErrorMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblErrorMessage.ForeColor = System.Drawing.Color.IndianRed;
            this.lblErrorMessage.Location = new System.Drawing.Point(0, 0);
            this.lblErrorMessage.Name = "lblErrorMessage";
            this.lblErrorMessage.Size = new System.Drawing.Size(448, 45);
            this.lblErrorMessage.TabIndex = 0;
            this.lblErrorMessage.Text = "label2";
            this.lblErrorMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblErrorMessage.Visible = false;
            // 
            // lkViewList
            // 
            this.lkViewList.AutoSize = true;
            this.lkViewList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lkViewList.Location = new System.Drawing.Point(37, 291);
            this.lkViewList.Name = "lkViewList";
            this.lkViewList.Size = new System.Drawing.Size(59, 16);
            this.lkViewList.TabIndex = 25;
            this.lkViewList.TabStop = true;
            this.lkViewList.Text = "Go to list";
            this.lkViewList.Click += new System.EventHandler(this.lkViewList_Click);
            // 
            // EditView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 328);
            this.Controls.Add(this.lkViewList);
            this.Controls.Add(this.plContextMessage);
            this.Controls.Add(this.btOk);
            this.Controls.Add(this.panel1);
            this.Controls.Add(emailLabel);
            this.Controls.Add(this.Email);
            this.Controls.Add(firstNameLabel);
            this.Controls.Add(this.FirstName);
            this.Controls.Add(lastNameLabel);
            this.Controls.Add(this.LastName);
            this.Controls.Add(phoneNumberLabel);
            this.Controls.Add(this.PhoneNumber);
            this.Name = "EditView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EditView";
            this.panel1.ResumeLayout(false);
            this.plContextMessage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Email;
        private System.Windows.Forms.TextBox FirstName;
        private System.Windows.Forms.TextBox LastName;
        private System.Windows.Forms.TextBox PhoneNumber;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btOk;
        private System.Windows.Forms.Panel plContextMessage;
        private System.Windows.Forms.Label lblErrorMessage;
        private System.Windows.Forms.LinkLabel lkViewList;
    }
}
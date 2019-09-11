namespace asychClientSocketBeispiel {
    partial class ChatForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.chatText = new System.Windows.Forms.TextBox();
            this.chatEingabeFeld = new System.Windows.Forms.TextBox();
            this.chatSendeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chatText
            // 
            this.chatText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatText.Location = new System.Drawing.Point(12, 12);
            this.chatText.Multiline = true;
            this.chatText.Name = "chatText";
            this.chatText.ReadOnly = true;
            this.chatText.Size = new System.Drawing.Size(380, 286);
            this.chatText.TabIndex = 0;
            // 
            // chatEingabeFeld
            // 
            this.chatEingabeFeld.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatEingabeFeld.Location = new System.Drawing.Point(12, 304);
            this.chatEingabeFeld.Multiline = true;
            this.chatEingabeFeld.Name = "chatEingabeFeld";
            this.chatEingabeFeld.Size = new System.Drawing.Size(380, 80);
            this.chatEingabeFeld.TabIndex = 1;
            // 
            // chatSendeButton
            // 
            this.chatSendeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chatSendeButton.Location = new System.Drawing.Point(317, 389);
            this.chatSendeButton.Name = "chatSendeButton";
            this.chatSendeButton.Size = new System.Drawing.Size(75, 23);
            this.chatSendeButton.TabIndex = 2;
            this.chatSendeButton.Text = "Senden";
            this.chatSendeButton.UseVisualStyleBackColor = true;
            this.chatSendeButton.Click += new System.EventHandler(this.chatSendeButton_Click);
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 424);
            this.Controls.Add(this.chatSendeButton);
            this.Controls.Add(this.chatEingabeFeld);
            this.Controls.Add(this.chatText);
            this.Name = "ChatForm";
            this.Text = "ChatForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox chatText;
        private System.Windows.Forms.TextBox chatEingabeFeld;
        private System.Windows.Forms.Button chatSendeButton;
    }
}
namespace asychClientSocketBeispiel {
    partial class Form1 {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent() {
            this.button2 = new System.Windows.Forms.Button();
            this.verbindenButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.pfadTextBox = new System.Windows.Forms.TextBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ServerPortText = new System.Windows.Forms.TextBox();
            this.ServerIPText = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.connectionText = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listView2 = new System.Windows.Forms.ListView();
            this.DateinameCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.GroeßeCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FortschrittCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.peersListe = new System.Windows.Forms.ListView();
            this.IdCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NameCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.IPCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.OSCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(919, 73);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(159, 28);
            this.button2.TabIndex = 33;
            this.button2.Text = "Chat Fenster öffnen";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // verbindenButton
            // 
            this.verbindenButton.Location = new System.Drawing.Point(411, 868);
            this.verbindenButton.Margin = new System.Windows.Forms.Padding(4);
            this.verbindenButton.Name = "verbindenButton";
            this.verbindenButton.Size = new System.Drawing.Size(100, 28);
            this.verbindenButton.TabIndex = 32;
            this.verbindenButton.Text = "Verbinden";
            this.verbindenButton.UseVisualStyleBackColor = true;
            this.verbindenButton.Click += new System.EventHandler(this.verbindenButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(972, 867);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 28);
            this.button1.TabIndex = 31;
            this.button1.Text = "Öffnen";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // pfadTextBox
            // 
            this.pfadTextBox.Location = new System.Drawing.Point(736, 869);
            this.pfadTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.pfadTextBox.Name = "pfadTextBox";
            this.pfadTextBox.Size = new System.Drawing.Size(225, 22);
            this.pfadTextBox.TabIndex = 30;
            this.pfadTextBox.Text = "C:\\Users\\Ben\\Downloads";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(736, 832);
            this.nameTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(225, 22);
            this.nameTextBox.TabIndex = 29;
            this.nameTextBox.Text = "Ben";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(556, 869);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(161, 25);
            this.label7.TabIndex = 28;
            this.label7.Text = "Freigabe Ordner:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(556, 834);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 25);
            this.label8.TabIndex = 27;
            this.label8.Text = "Name:";
            // 
            // ServerPortText
            // 
            this.ServerPortText.Location = new System.Drawing.Point(175, 869);
            this.ServerPortText.Margin = new System.Windows.Forms.Padding(4);
            this.ServerPortText.Name = "ServerPortText";
            this.ServerPortText.Size = new System.Drawing.Size(225, 22);
            this.ServerPortText.TabIndex = 26;
            this.ServerPortText.Text = "5000";
            // 
            // ServerIPText
            // 
            this.ServerIPText.Location = new System.Drawing.Point(175, 832);
            this.ServerIPText.Margin = new System.Windows.Forms.Padding(4);
            this.ServerIPText.Name = "ServerIPText";
            this.ServerIPText.Size = new System.Drawing.Size(225, 22);
            this.ServerIPText.TabIndex = 25;
            this.ServerIPText.Text = "192.168.2.100";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(15, 869);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 25);
            this.label6.TabIndex = 24;
            this.label6.Text = "Server Port:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 834);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 25);
            this.label5.TabIndex = 23;
            this.label5.Text = "Server IP:";
            // 
            // connectionText
            // 
            this.connectionText.AutoSize = true;
            this.connectionText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectionText.ForeColor = System.Drawing.Color.Red;
            this.connectionText.Location = new System.Drawing.Point(127, 9);
            this.connectionText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.connectionText.Name = "connectionText";
            this.connectionText.Size = new System.Drawing.Size(180, 31);
            this.connectionText.TabIndex = 22;
            this.connectionText.Text = "Disconnected";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 9);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 31);
            this.label3.TabIndex = 21;
            this.label3.Text = "Status:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(560, 74);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 31);
            this.label2.TabIndex = 20;
            this.label2.Text = "Dateien";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 74);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 31);
            this.label1.TabIndex = 19;
            this.label1.Text = "Clients";
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.DateinameCol,
            this.GroeßeCol,
            this.FortschrittCol});
            this.listView2.Location = new System.Drawing.Point(567, 109);
            this.listView2.Margin = new System.Windows.Forms.Padding(4);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(509, 694);
            this.listView2.TabIndex = 18;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // DateinameCol
            // 
            this.DateinameCol.Text = "Name";
            this.DateinameCol.Width = 100;
            // 
            // GroeßeCol
            // 
            this.GroeßeCol.Text = "Größe";
            this.GroeßeCol.Width = 100;
            // 
            // FortschrittCol
            // 
            this.FortschrittCol.Text = "Fortschritt";
            this.FortschrittCol.Width = 100;
            // 
            // peersListe
            // 
            this.peersListe.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.peersListe.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IdCol,
            this.NameCol,
            this.IPCol,
            this.OSCol});
            this.peersListe.Location = new System.Drawing.Point(20, 109);
            this.peersListe.Margin = new System.Windows.Forms.Padding(4);
            this.peersListe.MultiSelect = false;
            this.peersListe.Name = "peersListe";
            this.peersListe.Size = new System.Drawing.Size(537, 694);
            this.peersListe.TabIndex = 17;
            this.peersListe.UseCompatibleStateImageBehavior = false;
            this.peersListe.View = System.Windows.Forms.View.Details;
            // 
            // IdCol
            // 
            this.IdCol.Text = "Id";
            this.IdCol.Width = 100;
            // 
            // NameCol
            // 
            this.NameCol.Text = "Name";
            this.NameCol.Width = 100;
            // 
            // IPCol
            // 
            this.IPCol.Text = "IP";
            this.IPCol.Width = 100;
            // 
            // OSCol
            // 
            this.OSCol.Text = "OS";
            this.OSCol.Width = 100;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1089, 937);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.verbindenButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pfadTextBox);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.ServerPortText);
            this.Controls.Add(this.ServerIPText);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.connectionText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.peersListe);
            this.MaximumSize = new System.Drawing.Size(1107, 984);
            this.MinimumSize = new System.Drawing.Size(1107, 984);
            this.Name = "Form1";
            this.Text = "Uncloud Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.Button verbindenButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox pfadTextBox;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox ServerPortText;
        private System.Windows.Forms.TextBox ServerIPText;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label connectionText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader DateinameCol;
        private System.Windows.Forms.ColumnHeader GroeßeCol;
        private System.Windows.Forms.ColumnHeader FortschrittCol;
        public System.Windows.Forms.ListView peersListe;
        private System.Windows.Forms.ColumnHeader IdCol;
        private System.Windows.Forms.ColumnHeader NameCol;
        private System.Windows.Forms.ColumnHeader IPCol;
        private System.Windows.Forms.ColumnHeader OSCol;
    }
}


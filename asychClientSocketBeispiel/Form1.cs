using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace asychClientSocketBeispiel {
    public partial class Form1 : Form {

        public static bool run = true;

        public Form1() {
            InitializeComponent();
            peersListe.FullRowSelect = true;
        }

        private void button1_Click(object sender, EventArgs e) {

        }

        private void verbindenButton_Click(object sender, EventArgs e) {

            Thread neuerThread = new Thread(threadMethode);
            neuerThread.Start();
            
        }

        public void threadMethode() {
            AsynchronousClient.StartClient(ServerIPText.Text, Convert.ToInt32(ServerPortText.Text), nameTextBox.Text, this);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            run = false;
            try {
                AsynchronousClient.receiveDone.Set();
                AsynchronousClient.receiveDone.Close();
            } catch {

            }          
            Application.Exit();
            
        }

        private void button1_Click_1(object sender, EventArgs e) {
            FolderBrowserDialog objDialog = new FolderBrowserDialog();
            objDialog.Description = "Freigabe Ordner";
            objDialog.SelectedPath = @"C:\";       // Vorgabe Pfad (und danach der gewählte Pfad)
            DialogResult objResult = objDialog.ShowDialog(this);
            if (objResult == DialogResult.OK) {
                pfadTextBox.Text = objDialog.SelectedPath;
            }
        }

        private void peersListe_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                var selectedItem = peersListe.SelectedItems[0].SubItems[1];
            } catch {

            }
        }
    }
}

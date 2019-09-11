using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace asychClientSocketBeispiel {
    public partial class ChatForm : Form {
        public ChatForm() {
            InitializeComponent();
        }

        private void chatSendeButton_Click(object sender, EventArgs e) {
            StateObject chatPeer = new StateObject();
            foreach (StateObject item in Form1.chatObjekte) {
                if (this.Text.Contains(item.peerName)) {
                    chatPeer = item;
                    item.chatForm = this;
                }
            }
            AsynchronousSocketListener.Send(chatPeer.workSocket, "beg{5☻" + chatEingabeFeld.Text+"}end");
            chatText.Text = chatText.Text + "\n" + chatEingabeFeld.Text;
            chatEingabeFeld.Text = "";
        }
    }
}

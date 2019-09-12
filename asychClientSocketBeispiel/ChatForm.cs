using System;
using System.Net.Sockets;
using System.Threading;
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
            try {
                chatPeer.workSocket.BeginConnect(chatPeer.workSocket.RemoteEndPoint, ConnectCallback, chatPeer);
            } catch {

            }
           
            Thread.Sleep(100);
            if (chatPeer.workSocket.Connected) {
                AsynchronousSocketListener.Send(chatPeer.workSocket, "beg{5☻" + chatEingabeFeld.Text + "}end");
            }
            
            chatText.Text = chatText.Text + Environment.NewLine + chatEingabeFeld.Text;
            chatEingabeFeld.Text = "";
        }

        public void ConnectCallback(IAsyncResult ar) {
            try {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}

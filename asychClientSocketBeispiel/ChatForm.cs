using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace asychClientSocketBeispiel {
    public partial class ChatForm : Form {

        public static ManualResetEvent connectDone = new ManualResetEvent(false);
        public static ManualResetEvent sendDone = new ManualResetEvent(false);
        public static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public ChatForm() {
            InitializeComponent();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[ipHostInfo.AddressList.Length - 1];
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 5002);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                //Console.WriteLine("gebe semaphore frei");
                
                //while (Form1.run) {
                    // Set the event to nonsignaled state.  
                    //allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    //Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    Thread.Sleep(100);                
                    // Wait until a connection is made before continuing.  
                    //allDone.WaitOne();
                    //}
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            
        }
        public static Semaphore semaphoreTextSenden = new Semaphore(1, 1);
        public static List<Peer> peersListe = new List<Peer>();

        public void AcceptCallback(IAsyncResult ar) {
            // Signal the main thread to continue.  
            //allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            bool gefunden = false;

            foreach (var peer in peersListe) {
                if (peer.socket.RemoteEndPoint.ToString() == handler.RemoteEndPoint.ToString()) {
                    gefunden = true;
                }
            }
            if (!gefunden) {
                Peer p = new Peer();
                p.socket = handler;
                peersListe.Add(p);
            }

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        }


        private void chatSendeButton_Click(object sender, EventArgs e) {
            StateObject chatPeer = new StateObject();
            foreach (StateObject item in Form1.chatObjekte) {
                if (this.Text.Contains(item.peerName)) {
                    chatPeer = item;
                    item.chatForm = this;
                }
            }


            string _ip = chatPeer.workSocket.RemoteEndPoint.ToString();
            string[] ipArray = _ip.Split(':');
            foreach (string item in ipArray) {
                if (item.Contains("192")) {
                    int zahl = -1;
                    try {
                        zahl = Convert.ToInt32(item[item.Length - 1]);
                    } catch {

                    }
                    if (zahl != -1 && zahl != 93) {
                        _ip = item;
                        break;
                    }else {
                        break;
                        _ip = item.Substring(0, ipArray[3].Length - 1);
                    }
                }
            }
            //_ip = ipArray[3].Substring(0, ipArray[3].Length - 1);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(_ip), 5002);
            Socket client = new Socket(SocketType.Stream, ProtocolType.Tcp);

            client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
            connectDone.WaitOne();

            StateObject state = new StateObject();
            state.workSocket = client;
            Thread.Sleep(100);
            try {
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);

            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }

            Thread.Sleep(100);
            //sendDone.WaitOne();
            sendThread(client);

            // Release the socket.  
            //client.Shutdown(SocketShutdown.Both);
            //client.Close();

            //try {
            //    chatPeer.workSocket.BeginDisconnect(true, DisconnectCallback, chatPeer);
            //    chatPeer.workSocket.Close();
            //    Thread.Sleep(100);
            //    chatPeer.workSocket.BeginConnect(chatPeer.workSocket.RemoteEndPoint, ConnectCallback, chatPeer);
            //} catch (Exception ex){
            //    Console.WriteLine(ex.ToString());
            //}

            //Thread.Sleep(100);
            //if (chatPeer.workSocket.Connected) {
            //    AsynchronousSocketListener.Send(chatPeer.workSocket, "beg{5☻" + chatEingabeFeld.Text + "}end");
            //}
            semaphoreTextSenden.WaitOne();
            chatText.Text = chatText.Text + Environment.NewLine + chatEingabeFeld.Text;
            chatEingabeFeld.Text = "";
        }

        void sendThread(Object client) {
            StateObject chatPeer = new StateObject();
            foreach (StateObject item in Form1.chatObjekte) {
                if (this.Text.Contains(item.peerName)) {
                    chatPeer = item;
                    item.chatForm = this;
                }
            }

            string HostName = Dns.GetHostName();

            IPHostEntry hostInfo = Dns.GetHostEntry(HostName);
            //IpAdresse = hostInfo.AddressList[hostInfo.AddressList.Length - 1].ToString();

            IPAddress ipAddress = hostInfo.AddressList[hostInfo.AddressList.Length - 1];

            // Send test data to the remote device.  
            string text = "beg{" + "5" + ":" + chatPeer.peerName + ":" + ipAddress.ToString() + ":" + chatEingabeFeld.Text + ":" + "}end";
            semaphoreTextSenden.Release();
            Send((Socket)client, text);

        }

        private static void Send(Socket client, String data) {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            //Console.WriteLine("sende zu " + client.RemoteEndPoint.ToString() + " " + data);
            // Begin sending the data to the remote device.  
            if (Form1.run) {
                client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
            }
        }

        private static void SendCallback(IAsyncResult ar) {
            try {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to Peer.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar) {
            try {
                sendDone.Set();
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                state.sb = new StringBuilder();
                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0) {

                    // There  might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

                    Invoke((MethodInvoker)delegate {
                        chatText.Text = chatText.Text + Environment.NewLine + state.sb;
                    });

                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public void ConnectCallback(IAsyncResult ar) {
            try {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        public void DisconnectCallback(IAsyncResult ar) {
            try {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndDisconnect(ar);
                client.Close();
                
                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace asychClientSocketBeispiel {


    class AsynchronousClient {

        public static List<Peer> peersListe = new List<Peer>();

        // The port number for the remote device.  
        private const int port = 5000;
        public static IPAddress ipAddress;
        public static string name = "";
        public static Form1 formStatic;

        // ManualResetEvent instances signal completion.  
        public static ManualResetEvent connectDone = new ManualResetEvent(false);
        public static ManualResetEvent sendDone = new ManualResetEvent(false);
        public static ManualResetEvent receiveDone = new ManualResetEvent(false);

        // The response from the remote device.  
        private static String response = String.Empty;

        public static void StartClient(string ip, int port, string _name, Form1 form) {
            formStatic = form;
            name = _name;
            // Connect to a remote device.  
            try {

                string HostName = Dns.GetHostName();

                IPHostEntry hostInfo = Dns.GetHostEntry(HostName);
                //IpAdresse = hostInfo.AddressList[hostInfo.AddressList.Length - 1].ToString();

                string[] ipArray = ip.Split('.');
                string ipString = ipArray[0] + "." + ipArray[1] + "." + ipArray[2] + ".";
                foreach (IPAddress item in hostInfo.AddressList) {
                    if (item.ToString().Contains(ipString)) {
                        ipAddress = item;
                        break;
                    }
                }
                //ipAddress = hostInfo.AddressList[hostInfo.AddressList.Length - 1];

                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(IPAddress.Parse(ip).AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                AsyncCallback callback = new AsyncCallback(ConnectCallback);
                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, callback, client);

                connectDone.WaitOne();
                //client.BeginDisconnect(true, DisconnectCallback, client);
                form.Invoke((MethodInvoker)delegate {
                    form.connectionText.Text = "Verbunden mit " + ip + ":" + port;
                    form.connectionText.ForeColor = Color.Green;
                    form.verbindenButton.Enabled = false;
                });

                

                try {
                    ParameterizedThreadStart pts = new ParameterizedThreadStart(sendThread);
                    Thread sendThreadObj = new Thread(pts);
                    sendThreadObj.Start(client);
                } catch {

                }


                //Send(client, "This is a test<EOF>");

                
                sendDone.WaitOne();

                // Receive the response from the remote device.  
                //try {
                //    ParameterizedThreadStart pts = new ParameterizedThreadStart(receiveThread);
                //    Thread receiveThreadObj = new Thread(pts);
                //    receiveThreadObj.Start(client);
                //} catch {

                //}
                Receive(client);
                receiveDone.WaitOne();

                // Write the response to the console.  
                Console.WriteLine("Response received : {0}", response);

                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            } catch (Exception e) {
                
                Console.WriteLine(e.ToString());
            }
        }

        private static void DisconnectCallback(IAsyncResult ar) {
            // Complete the disconnect request.
            Socket client = (Socket)ar.AsyncState;
            client.EndDisconnect(ar);
        }

        static void sendThread(Object client) {      
            // Send test data to the remote device.  
            string text = "beg{" + "1" + ":" + Form1.eigenerName + ":" + ipAddress + ":Windows:♥" + "}end";
            while (Form1.run) {
                text = "beg{" + "1" + ":" + Form1.eigenerName + ":" + ipAddress + ":Windows:♥" + "}end";
                Send((Socket)client, text);
                Thread.Sleep(1000);
            }
        }

        private static void ConnectCallback(IAsyncResult ar) {
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

        private static void Receive(Socket client) {
            try {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                if (Form1.run) {
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar) {
            try {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                state.sb = new StringBuilder();
                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0) {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                    //Console.WriteLine("Empfangen: "+state.sb.ToString());
                    string response = state.sb.ToString();

                    int begin = response.IndexOf("beg{");
                    int ende = response.IndexOf("}end");

                    string responseOhneHeaderUndTailer = "";
                    for (int j = begin + 4; j < ende; j++) {
                        responseOhneHeaderUndTailer += response[j];
                    }
                    int aktion = -1;

                    if (responseOhneHeaderUndTailer.Length > 1) {
                        aktion = Int32.Parse("" + responseOhneHeaderUndTailer[0]);
                    }
                    if (aktion == 1) {

                        int indexAktionsTrenner = responseOhneHeaderUndTailer.IndexOf("☻");
                        string responeOhneAktion = "";
                        string[] responeOhneAktionArray = responseOhneHeaderUndTailer.Split('☻');
                        responeOhneAktion = responeOhneAktionArray[1];
                        //for (int j = indexAktionsTrenner + 1; j < responseOhneHeaderUndTailer.Length; j++) {
                        //    responeOhneAktion += responseOhneHeaderUndTailer[j];
                        //}

                        List<Peer> peersListeLokal = new List<Peer>();

                        List<string> peersAlsStrings = new List<string>();
                        string [] peersAlsStringsArray = responeOhneAktion.Split('♥');


                        foreach (string item in peersAlsStringsArray) {
                            if (!item.Equals("")) {
                                Peer p = new Peer();
                                string[] einzelnerPeerAlsArray = item.Split(':');
                                p.ip = einzelnerPeerAlsArray[0];
                                p.name = einzelnerPeerAlsArray[1];
                                p.os = einzelnerPeerAlsArray[2];

                                peersListeLokal.Add(p);
                            }
                            
                        }

                        if(peersListeLokal.Count != peersListe.Count) {                                                     
                            peersListe = peersListeLokal;
                            formStatic.Invoke((MethodInvoker)delegate {
                                int k= 1;
                                formStatic.peersListe.Items.Clear();
                                foreach (Peer item in peersListe) {
                                    ListViewItem newItem = new ListViewItem(Convert.ToString(k++));
                                    newItem.SubItems.Add(item.name);
                                    newItem.SubItems.Add(item.ip);
                                    newItem.SubItems.Add(item.os);

                                    formStatic.peersListe.Items.Add(newItem);
                                }
                            });

                        } else {
                            for (int i = 0; i < peersListe.Count; i++) {
                                if (!peersListe[i].ip.Equals(peersListeLokal[i].ip) || !peersListe[i].name.Equals(peersListeLokal[i].name)) {
                                    peersListe = peersListeLokal;
                                    formStatic.Invoke((MethodInvoker)delegate {
                                        int k = 1;
                                        formStatic.peersListe.Items.Clear();
                                        foreach (Peer item in peersListe) {
                                            ListViewItem newItem = new ListViewItem(Convert.ToString(k++));
                                            newItem.SubItems.Add(item.name);
                                            newItem.SubItems.Add(item.ip);
                                            newItem.SubItems.Add(item.os);

                                            formStatic.peersListe.Items.Add(newItem);
                                        }
                                    });
                                }
                            }
                        }
                        
                    }



                        // Get the rest of the data.  
                        if (Form1.run) {
                            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                        }else {
                            client.Disconnect(false);
                            client.Close();
                            client.Dispose();
                            Application.Exit();
                        }
                    } else {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1) {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            } catch (Exception e) {
                Form1.run = false;
                try {
                    formStatic.Invoke((MethodInvoker)delegate {
                        formStatic.connectionText.Text = "Disconnected";
                        formStatic.connectionText.ForeColor = Color.Red;
                        formStatic.verbindenButton.Enabled = true;

                    });
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
                
                Console.WriteLine(e.Message);
            }
        }

        private static void Send(Socket client, String data) {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            // Begin sending the data to the remote device.  
            if (Form1.run) {
                try {
                    client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);

                }
            }
        }

        private static void SendCallback(IAsyncResult ar) {
            try {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        //public static int Main(String[] args) {
        //    StartClient();
        //    return 0;
        //}

    }
}

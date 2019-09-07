using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asychClientSocketBeispiel {
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

    namespace asynchServerSocketBeispiel {

        // State object for reading client data asynchronously  
        public class StateObject {
            // Client  socket.  
            public Socket workSocket = null;
            // Size of receive buffer.  
            public const int BufferSize = 1024;
            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];
            // Received data string.  
            public StringBuilder sb = new StringBuilder();
        }

        public class Peer {
            public Socket socket { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public string ip { get; set; }
            public string os { get; set; }
            public long letzterKontakt { get; set; }
        }


        class AsynchronousSocketListener {

            // Thread signal.  
            public static ManualResetEvent allDone = new ManualResetEvent(false);
            public static Semaphore fuerPeersListe = new Semaphore(0, 1);
            public static Form1 formStatic;
            public static bool run = true;
            public AsynchronousSocketListener() {
            }

            public static void StartListening(Form1 form) {
                formStatic = form;
                // Establish the local endpoint for the socket.  
                // The DNS name of the computer  
                // running the listener is "host.contoso.com".  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[ipHostInfo.AddressList.Length - 1];
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 5001);

                // Create a TCP/IP socket.  
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.  
                try {
                    listener.Bind(localEndPoint);
                    listener.Listen(100);

                    Console.WriteLine("gebe semaphore frei");
                    fuerPeersListe.Release();
                    while (run) {
                        // Set the event to nonsignaled state.  
                        //allDone.Reset();

                        // Start an asynchronous socket to listen for connections.  
                        //Console.WriteLine("Waiting for a connection...");
                        listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                        Thread.Sleep(100);
                        // Wait until a connection is made before continuing.  
                        //allDone.WaitOne();
                    }

                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }

                //Console.WriteLine("\nPress ENTER to continue...");
                //Console.Read();

            }

            public static List<Socket> peersListeSockets = new List<Socket>();
            public static List<Peer> peersListe = new List<Peer>();

            public static void AcceptCallback(IAsyncResult ar) {
                // Signal the main thread to continue.  
                //allDone.Set();

                // Get the socket that handles the client request.  
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);
                bool gefunden = false;
                Console.WriteLine("warte auf semaphor zum hinzufügen");
                fuerPeersListe.WaitOne();
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
                fuerPeersListe.Release();



                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = handler;
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }

            public static void ReadCallback(IAsyncResult ar) {
                String content = String.Empty;

                // Retrieve the state object and the handler socket  
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                state.sb = new StringBuilder();
                Socket handler = state.workSocket;

                // Read data from the client socket.
                int bytesRead = 0;
                try {
                    bytesRead = handler.EndReceive(ar);

                } catch {
                    return;
                }

                if (bytesRead > 0) {
                    // There  might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read   
                    // more data.  
                    content = state.sb.ToString();
                    if (content.IndexOf("}end") > -1) {
                        // All the data has been read from the   
                        // client. Display it on the console.  
                        Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
                        // Echo the data back to the client.  
                        int begin = content.IndexOf("beg{");
                        int ende = content.IndexOf("}end");

                        string contentOhneHeaderUndTailer = "";
                        for (int j = begin + 4; j < ende; j++) {
                            contentOhneHeaderUndTailer += content[j];
                        }
                        int aktion = -1;
                        string ip = "";
                        string name = "";
                        string os = "";

                        aktion = Int32.Parse("" + contentOhneHeaderUndTailer[0]);
                        if (aktion == 1) {


                            contentOhneHeaderUndTailer = contentOhneHeaderUndTailer.Substring(contentOhneHeaderUndTailer.IndexOf(':') + 1);
                            for (int j = 0; j < contentOhneHeaderUndTailer.Length; j++) {
                                if (contentOhneHeaderUndTailer[j] == ':') {
                                    break;
                                }
                                name += contentOhneHeaderUndTailer[j];
                            }
                            contentOhneHeaderUndTailer = contentOhneHeaderUndTailer.Substring(contentOhneHeaderUndTailer.IndexOf(':') + 1);
                            for (int j = 0; j < contentOhneHeaderUndTailer.Length; j++) {
                                if (contentOhneHeaderUndTailer[j] == ':') {
                                    break;
                                }
                                ip += contentOhneHeaderUndTailer[j];
                            }
                            os = "Windows 10";
                            //bool gefunden = false;
                            //foreach (var peer in peersListe) {
                            //    if (peer.socket.RemoteEndPoint.ToString() == handler.RemoteEndPoint.ToString()) {
                            //        gefunden = true;
                            //    }
                            //}
                            Console.WriteLine("warte auf semaphore read");
                            fuerPeersListe.WaitOne();
                            foreach (Peer p in peersListe) {
                                string tmp = p.socket.RemoteEndPoint.ToString().Substring(0, p.socket.RemoteEndPoint.ToString().IndexOf(':'));
                                if (tmp.Equals(ip)) {
                                    p.ip = ip;
                                    p.name = name;
                                    p.os = os;
                                    p.letzterKontakt = DateTime.Now.Ticks;

                                }
                            }


                            formStatic.Invoke((MethodInvoker)delegate {
                                int i = 1;
                                formStatic.peersListe.Items.Clear();
                                foreach (Peer item in peersListe) {
                                    ListViewItem newItem = new ListViewItem(Convert.ToString(i++));
                                    newItem.SubItems.Add(item.name);
                                    newItem.SubItems.Add(item.ip);
                                    newItem.SubItems.Add("Windows 10");

                                    formStatic.peersListe.Items.Add(newItem);
                                }
                            });

                            string answer = "beg{1☻";
                            //answer += "Ben:192.168.2.100:Windows 10♥Klaus:192.168.2.130:Windows 10♥";
                           
                            answer += "}end";
                            Console.WriteLine(answer);
                            Console.WriteLine("gebe semaphore frei read");
                            fuerPeersListe.Release();
                            Send(handler, answer);
                        }
                        //Send(handler, content);
                        state.buffer = new byte[1024];
                        try {
                            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

                        } catch (Exception ex) {
                            Console.WriteLine(ex.Message);
                        }
                    } else {
                        // Not all data received. Get more.  
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                    }
                }
            }

            private static void Send(Socket handler, String data) {
                // Convert the string data to byte data using ASCII encoding.  
                byte[] byteData = Encoding.UTF8.GetBytes(data);

                // Begin sending the data to the remote device.  
                handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
            }

            public static void peersObserverThread() {
                while (run) {
                    Console.WriteLine("warte auf semaphore observer");
                    fuerPeersListe.WaitOne(100);
                    List<Peer> abgelaufenePeers = new List<Peer>();
                    bool listeHatSichGeaendert = false;
                    foreach (Peer item in peersListe) {
                        long tmp = DateTime.Now.Ticks - item.letzterKontakt;
                        Console.WriteLine(tmp);
                        if (DateTime.Now.Ticks - item.letzterKontakt > 100000000) {
                            abgelaufenePeers.Add(item);
                            listeHatSichGeaendert = true;
                        }
                    }
                    foreach (Peer item in abgelaufenePeers) {
                        peersListe.Remove(item);
                    }
                    if (listeHatSichGeaendert) {
                        formStatic.Invoke((MethodInvoker)delegate {
                            int i = 1;
                            formStatic.peersListe.Items.Clear();
                            foreach (Peer item in peersListe) {
                                ListViewItem newItem = new ListViewItem(Convert.ToString(i++));
                                newItem.SubItems.Add(item.name);
                                newItem.SubItems.Add(item.ip);
                                newItem.SubItems.Add("Windows 10");

                                formStatic.peersListe.Items.Add(newItem);
                            }
                        });
                    }
                    Console.WriteLine("gebe semaphore frei observer");
                    fuerPeersListe.Release();
                    Thread.Sleep(1000);
                }

            }

            private static void SendCallback(IAsyncResult ar) {
                try {
                    // Retrieve the socket from the state object.  
                    Socket handler = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.  
                    int bytesSent = handler.EndSend(ar);
                    Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                    //handler.Shutdown(SocketShutdown.Both);
                    //handler.Close();

                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }
            }

            //public static int Main(String[] args) {
            //    StartListening();
            //    return 0;
            //}
        }
    }

}

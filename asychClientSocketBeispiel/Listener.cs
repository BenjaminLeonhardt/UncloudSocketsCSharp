using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asychClientSocketBeispiel {
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    enum aktionEnum {
        heartbeat=1,sendeAlleDateien,sendeDatei,chatOeffnen,chatMessage
    }

    // State object for reading client data asynchronously  
    public class StateObject {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 65535;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
        public string dateiName = "";
        public long dateiGroesse = 0;
        public ChatForm chatForm;
        public string peerName;
        public string ip;
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

        public static void StartListening(Object form) {
            formStatic = (Form1)form;
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

                //Console.WriteLine("gebe semaphore frei");
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

                    aktion = Int32.Parse("" + contentOhneHeaderUndTailer[0]);
                    if (aktion == (int)aktionEnum.sendeAlleDateien) {


                        DirectoryInfo directoryInfo = new DirectoryInfo(formStatic.pfadTextBox.Text);
                        FileInfo[] fileInfoArray = directoryInfo.GetFiles();
                        string answer = "beg{2☻";
                        foreach (FileInfo item in fileInfoArray) {
                            answer += item.Name + ";" + item.Length + "♦";
                        }
                        answer += "}end";

                        Console.WriteLine(answer);

                        Send(handler, answer);
                    } else if (aktion == (int)aktionEnum.sendeDatei) {
                        string[] mesageItems = contentOhneHeaderUndTailer.Split(':');
                        if (mesageItems[3].Contains("..\\")) {
                            return;
                        }

                        SendDatei(handler, mesageItems[3]);
                    } else if (aktion == (int)aktionEnum.chatOeffnen) {
                        
                        string[] contentArray = contentOhneHeaderUndTailer.Split(':');
                        string name = contentArray[1];
                        string ip = contentArray[2];
                        
                        bool gefunden = false;
                        StateObject tmpObjekt = null;
                        foreach (StateObject item in Form1.chatObjekte) {
                            if (item.peerName.Equals(name)) {
                                tmpObjekt = item;
                                gefunden = true;
                                break;
                            }
                        }

                        if (gefunden) {
                            
                            try {

                                ParameterizedThreadStart pts = new ParameterizedThreadStart(chatThread);
                                Thread sendThreadObj = new Thread(pts);
                                sendThreadObj.Start(name);
                                
                            } catch (Exception ex) {
                                Console.WriteLine(ex.ToString());
                            }


                        } else {
                            tmpObjekt = state;
                            tmpObjekt.ip = ip;
                            tmpObjekt.peerName = name;
                            Form1.chatObjekte.Add(tmpObjekt);
                           
                            try {

                                ParameterizedThreadStart pts = new ParameterizedThreadStart(chatThread);
                                Thread sendThreadObj = new Thread(pts);
                                sendThreadObj.Start(name);
                                
                            } catch (Exception ex) {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                    } else if (aktion == (int)aktionEnum.chatMessage) {
                        try {
                            state.chatForm.Invoke((MethodInvoker)delegate {
                                state.chatForm.chatText.Text = state.chatForm.chatText.Text + "\n" + contentOhneHeaderUndTailer.Substring(2);
                            });
                        } catch(Exception ex) {
                            Console.WriteLine(ex.ToString());
                        }
                        
                    }
                    } else {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        public static void chatThread(Object peerName) {
            
            ChatForm chatform = new ChatForm();
            StateObject tmpStateObject = null;
            string name = (string)peerName;
            foreach (StateObject item in Form1.chatObjekte) {
                if (name.Contains(item.peerName)) {
                    item.chatForm = chatform;
                    tmpStateObject = item;
                    break;
                }
            }

            ParameterizedThreadStart pts = new ParameterizedThreadStart(chatFensterThread);
            Thread sendThreadObj = new Thread(pts);
            sendThreadObj.Start(chatform);
            
            
            
            Thread.Sleep(100);
            try{
                tmpStateObject.workSocket.BeginReceive(tmpStateObject.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), tmpStateObject);

            }
            catch(Exception ex){
                Console.WriteLine(ex.ToString());
            }
            
        }
        public static void chatFensterThread(Object fensterObj) {
            ChatForm chatform = (ChatForm)fensterObj;
            
            foreach (StateObject item in Form1.chatObjekte) {
                if (chatform == item.chatForm) {
                    chatform.Text = "Chat mit " + item.peerName;
                    chatform.ShowDialog();
                }
            }
            //Application.Run(chatform);
        }


        public static void Send(Socket handler, String data) {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }


        private static void SendDatei(Socket handler, String dateiname){
            try
            {
                FileStream fileStream = File.Open(formStatic.pfadTextBox.Text + "\\" + dateiname, FileMode.Open);
                DirectoryInfo directoryInfo = new DirectoryInfo(formStatic.pfadTextBox.Text);
                FileInfo[] fileInfoArray = directoryInfo.GetFiles();
                byte[] byteData = new byte[65535];
                foreach (FileInfo item in fileInfoArray)
                {
                    if (item.Name.Equals(dateiname))
                    {
                        byteData = new byte[item.Length];
                    }
                }
                fileStream.Read(byteData, 0, byteData.Length);
                fileStream.Close();
                // Convert the string data to byte data using ASCII encoding.  


                // Begin sending the data to the remote device.  
                handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

        private static void SendCallback(IAsyncResult ar) {
            try {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to peer.", bytesSent);

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}



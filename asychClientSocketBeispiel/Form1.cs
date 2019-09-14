using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace asychClientSocketBeispiel {
    public partial class Form1 : Form {

        public static bool run = true;
        public static int peersPort = 5001;

        // ManualResetEvent instances signal completion.  
        public static ManualResetEvent connectDone = new ManualResetEvent(false);
        public static ManualResetEvent sendDone = new ManualResetEvent(false);
        public static ManualResetEvent receiveDone = new ManualResetEvent(false);
        public static string IpOfSelectedPeer = "";
        public static List<StateObject> chatObjekte = new List<StateObject>();
        public static Semaphore semaphoreDateiSpeichern = new Semaphore(1, 1);
        public static string eigenerName = "";

        public static string UncloudConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Uncloud\\";
        public static string UncloudConfigFilename = "config.ini";
        public Form1() {
            InitializeComponent();
            if (File.Exists(UncloudConfigPath + UncloudConfigFilename)) {
                FileStream stream = File.Open(UncloudConfigPath + UncloudConfigFilename, FileMode.Open);
                BinaryReader reader = new BinaryReader(stream);
                string[] dateiInhalt = reader.ReadString().Split(';');
                ServerIPText.Text = dateiInhalt[0];
                ServerPortText.Text = dateiInhalt[1];
                nameTextBox.Text = dateiInhalt[2];
                eigenerName = nameTextBox.Text;
                pfadTextBox.Text = dateiInhalt[3];
                reader.Close();
                stream.Close();
            } else {
                Directory.CreateDirectory(UncloudConfigPath);
                FileStream stream = File.Open(UncloudConfigPath + UncloudConfigFilename, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write("192.168.2.100;5000;Ben;" + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                writer.Close();
                stream.Close();
            }
            peersListe.FullRowSelect = true;
            filesView2.FullRowSelect = true;
            try {
                ParameterizedThreadStart pts = new ParameterizedThreadStart(AsynchronousSocketListener.StartListening);
                Thread sendThreadObj = new Thread(pts);
                sendThreadObj.Start(this);
            } catch {

            }

        }

        private void button1_Click(object sender, EventArgs e) {

        }

        private void verbindenButton_Click(object sender, EventArgs e) {


            FileStream stream = File.Open(UncloudConfigPath + UncloudConfigFilename, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(ServerIPText.Text + ";" + ServerPortText.Text + ";" + nameTextBox.Text + ";" + pfadTextBox.Text);
            writer.Close();
            stream.Close();


            Thread neuerThread = new Thread(threadMethode);
            neuerThread.Start();

        }

        public void threadMethode() {
            AsynchronousClient.StartClient(ServerIPText.Text, Convert.ToInt32(ServerPortText.Text), nameTextBox.Text, this);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            run = false;
            AsynchronousSocketListener.run = false;
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
            objDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);       // Vorgabe Pfad (und danach der gewählte Pfad)
            DialogResult objResult = objDialog.ShowDialog(this);
            if (objResult == DialogResult.OK) {
                pfadTextBox.Text = objDialog.SelectedPath;
            }
            FileStream stream = File.Open(UncloudConfigPath + UncloudConfigFilename, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(ServerIPText.Text + ";" + ServerPortText.Text + ";" + nameTextBox.Text + ";" + pfadTextBox.Text);
            writer.Close();
            stream.Close();
        }

        private void peersListe_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                string selectedItem = peersListe.SelectedItems[0].SubItems[2].Text;

                try {
                    ParameterizedThreadStart pts = new ParameterizedThreadStart(getFilesThread);
                    Thread sendThreadObj = new Thread(pts);
                    sendThreadObj.Start(selectedItem);
                } catch {

                }

            } catch {

            }
        }

        public void getFilesThread(Object ip) {
            string _ip = (string)ip;
            IpOfSelectedPeer = _ip;
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(_ip), peersPort);
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

            //Send(client, "");
            //sendDone.WaitOne();


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
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                    //Console.WriteLine("Empfangen: " + state.sb.ToString());
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
                    if (aktion == 2) {
                        string[] ohneAktion = responseOhneHeaderUndTailer.Split('☻');
                        string[] dateienMitGroesse = ohneAktion[1].Split('|');

                        Invoke((MethodInvoker)delegate {
                            filesView2.Items.Clear();
                            foreach (string item in dateienMitGroesse) {
                                string[] dateiUndGroesse = item.Split(';');
                                if (dateiUndGroesse.Length == 2) {
                                    ListViewItem newItem = new ListViewItem(Convert.ToString(dateiUndGroesse[0]));
                                    newItem.SubItems.Add(dateiUndGroesse[1]);

                                    filesView2.Items.Add(newItem);
                                }

                            }
                        });
                    } else if (aktion == 3) {
                        string[] mesageItems = responseOhneHeaderUndTailer.Split(':');
                        client.SendFile(mesageItems[3]);
                    }
                    if (Form1.run) {
                        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);

                    } else {
                        client.Disconnect(false);
                        client.Close();
                        client.Dispose();
                        Application.Exit();
                    }
                }
                client.Disconnect(false);
                client.Close();
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

        }

        void sendThread(Object client) {

            string HostName = Dns.GetHostName();

            IPHostEntry hostInfo = Dns.GetHostEntry(HostName);
            //IpAdresse = hostInfo.AddressList[hostInfo.AddressList.Length - 1].ToString();

            IPAddress ipAddress = hostInfo.AddressList[hostInfo.AddressList.Length - 1];

            // Send test data to the remote device.  
            string text = "beg{" + "2" + ":" + nameTextBox.Text + ":" + ipAddress.ToString() + ":♥" + "}end";
            Send((Socket)client, text);

        }

        private static void Send(Socket client, String data) {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            //Console.WriteLine("sende zu " + client.RemoteEndPoint.ToString() + " " + data);
            // Begin sending the data to the remote device.  
            if (Form1.run) {
                try {
                    client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
                } catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
                
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

        private void filesView2_SelectedIndexChanged(object sender, EventArgs e) {

        }

        public void getThisFileThread(Object filename) {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(IpOfSelectedPeer), peersPort);
            Socket client = new Socket(SocketType.Stream, ProtocolType.Tcp);

            client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
            connectDone.WaitOne();

            StateObject state = new StateObject();
            state.workSocket = client;
            state.dateiName = (string)filename;
            Invoke((MethodInvoker)delegate {
                state.dateiGroesse = Convert.ToInt64(filesView2.SelectedItems[0].SubItems[1].Text);
            });


            Thread.Sleep(100);
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallbackFile), state);

            Thread.Sleep(100);
            //sendDone.WaitOne();
            sendThreadFile(client, (string)filename);

        }

        private void ReceiveCallbackFile(IAsyncResult ar) {

            // Retrieve the state object and the client socket   
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.  
            int bytesRead = client.EndReceive(ar);

            if (bytesRead > 0) {
                // There might be more data, so store the data received so far.  
                string dateiName = state.dateiName;
                //long dateiGroesse = state.dateiGroesse;
                try {
                    //Invoke((MethodInvoker)delegate {
                    //    dateiName = filesView2.SelectedItems[0].SubItems[0].Text;
                    //    dateiGroesse = filesView2.SelectedItems[0].SubItems[1].Text;
                    //});
                    string freigabeOrdner = pfadTextBox.Text;
                    long dateigroesseLong = state.dateiGroesse;
                    if (dateigroesseLong > 65535) {
                        DirectoryInfo directoryInfo = new DirectoryInfo(freigabeOrdner);
                        FileInfo[] fileInfoArray = directoryInfo.GetFiles();
                        foreach (FileInfo item in fileInfoArray) {
                            if (item.Name.Equals(dateiName)) {

                                long fortschrittInProzent = (((item.Length + bytesRead) * 100) / dateigroesseLong);
                                Invoke((MethodInvoker)delegate {
                                    ListViewItem.ListViewSubItem newItem = new ListViewItem.ListViewSubItem();
                                    newItem.Text = Convert.ToString(fortschrittInProzent) + " %";
                                    foreach (ListViewItem listItem in filesView2.Items) {
                                        if (listItem.Text.Equals(dateiName)) {
                                            if (listItem.SubItems.Count == 2) {
                                                listItem.SubItems.Add(newItem);
                                            } else if (listItem.SubItems.Count == 3) {
                                                listItem.SubItems[2] = newItem;
                                            }
                                        }
                                    }

                                });
                            }
                        }
                    } else {
                        Invoke((MethodInvoker)delegate {
                            ListViewItem.ListViewSubItem newItem = new ListViewItem.ListViewSubItem();
                            newItem.Text = "100 %";                           
                            foreach (ListViewItem listItem in filesView2.Items) {
                                if (listItem.Text.Equals(dateiName)) {
                                    if (listItem.SubItems.Count == 2) {
                                        listItem.SubItems.Add(newItem);
                                    } else if (listItem.SubItems.Count == 3) {
                                        listItem.SubItems[2] = newItem;
                                    }
                                }
                            }
                        });
                    }


                    semaphoreDateiSpeichern.WaitOne(10);
                    FileStream fileStream = File.Open(freigabeOrdner + "\\" + dateiName, FileMode.Append, FileAccess.Write, FileShare.None);
                    fileStream.Write(state.buffer, 0, bytesRead);
                    fileStream.Close();
                    semaphoreDateiSpeichern.Release();
                } catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }


                //Console.WriteLine(freigabeOrdner + "\\" + dateiName +" gespeichert");
                string response = state.sb.ToString();

            }
            try {
                //client.BeginSend(stateFile.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(AsynchronousFileSendCallback), state);
                //Thread.Sleep(100);
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallbackFile), state);

            } catch {

            }
        }

        private static void AsynchronousFileSendCallback(IAsyncResult ar) {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            client.EndSendFile(ar);
            sendDone.Set();
        }


        void sendThreadFile(Object client, string filename) {

            string HostName = Dns.GetHostName();

            IPHostEntry hostInfo = Dns.GetHostEntry(HostName);
            //IpAdresse = hostInfo.AddressList[hostInfo.AddressList.Length - 1].ToString();

            IPAddress ipAddress = hostInfo.AddressList[hostInfo.AddressList.Length - 1];

            // Send test data to the remote device.  
            string text = "beg{" + "3" + ":" + nameTextBox.Text + ":" + ipAddress.ToString() + ":" + filename + ":♥" + "}end";
            Send((Socket)client, text);

        }

        private void chatButton_Click(object sender, EventArgs e) {
            string NameDesAusgewaehltenPeers = "";
            try{
                NameDesAusgewaehltenPeers = peersListe.SelectedItems[0].SubItems[1].Text;
            }
            catch (Exception ex){
                Console.WriteLine(ex.ToString());
            }
            
            if (IpOfSelectedPeer.Equals("")|| NameDesAusgewaehltenPeers.Equals("")) {
               MessageBox.Show("Es wurde kein Peer ausgewählt...", "Achtung", MessageBoxButtons.OK);
               return;
            }

            
            
            bool gefunden = false;
            StateObject tmpObjekt = null;
            foreach (StateObject item in chatObjekte) {
                if (item.peerName.Equals(NameDesAusgewaehltenPeers)) {
                    tmpObjekt = item;
                    gefunden = true;
                }
            }

            if (gefunden) {
                //ChatForm chatform = new ChatForm();
                try {
                    ChatForm chatformNeu = new ChatForm();
                    chatformNeu.Text = "Chat mit " + tmpObjekt.peerName;
                    tmpObjekt.chatForm = chatformNeu;
                    chatformNeu.ShowDialog();
                    
                    //Application.Run(chatform);
                } catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
                
                //chatform.Text = "Chat mit " + tmpObjekt.peerName;
                //chatform.Show();
                //tmpObjekt.chatForm = chatform;
            } else {

                ChatForm chatform = new ChatForm();
                chatform.Text = "Chat mit " + NameDesAusgewaehltenPeers;
                chatform.Show();
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(IpOfSelectedPeer), peersPort);
                Socket client = new Socket(SocketType.Stream, ProtocolType.Tcp);

                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                StateObject state = new StateObject();
                state.workSocket = client;
                state.chatForm = chatform;
                state.peerName = NameDesAusgewaehltenPeers;
                chatObjekte.Add(state);
                Thread.Sleep(100);
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallbackChat), state);

                Thread.Sleep(100);
                //sendDone.WaitOne();
                sendThreadChat(client);
            }
            

        }

        void sendThreadChat(Object client) {

            string HostName = Dns.GetHostName();

            IPHostEntry hostInfo = Dns.GetHostEntry(HostName);
            //IpAdresse = hostInfo.AddressList[hostInfo.AddressList.Length - 1].ToString();

            IPAddress ipAddress = hostInfo.AddressList[hostInfo.AddressList.Length - 1];

            // Send test data to the remote device.  
            string text = "beg{" + "4" + ":" + nameTextBox.Text + ":" + ipAddress.ToString() + ":♥" + "}end";
            Send((Socket)client, text);

        }

        private void ReceiveCallbackChat(IAsyncResult ar) {
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
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                    //Console.WriteLine("Empfangen: " + state.sb.ToString());
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
                    if (aktion == 4) {
                    }
                }
            }catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        private void filesView2_DoubleClick(object sender, EventArgs e) {
            try {
                string selectedItem = filesView2.SelectedItems[0].SubItems[0].Text;
                if (File.Exists(pfadTextBox.Text + "\\" + selectedItem)) {
                    DialogResult result = MessageBox.Show("Datei vorhanden. Überschreiben?", "Achtung", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No) {
                        return;
                    } else {
                        File.Delete(pfadTextBox.Text + "\\" + selectedItem);
                    }
                }
                try {
                    ParameterizedThreadStart pts = new ParameterizedThreadStart(getThisFileThread);
                    Thread sendThreadObj = new Thread(pts);
                    sendThreadObj.Start(selectedItem);
                } catch {

                }

            } catch {

            }
        }

        private void nameTextBox_KeyUp(object sender, KeyEventArgs e) {
            eigenerName = nameTextBox.Text;
            if (File.Exists(UncloudConfigPath + UncloudConfigFilename)) {
                FileStream stream = File.Open(UncloudConfigPath + UncloudConfigFilename, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(ServerIPText.Text + ";" + ServerPortText.Text + ";" + nameTextBox.Text + ";" + pfadTextBox.Text);
                writer.Close();
                stream.Close();
            } else {
                Directory.CreateDirectory(UncloudConfigPath);
                FileStream stream = File.Open(UncloudConfigPath + UncloudConfigFilename, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write("192.168.2.100;5000;" + nameTextBox.Text + ";" + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                writer.Close();
                stream.Close();
            }
        }
    }
}
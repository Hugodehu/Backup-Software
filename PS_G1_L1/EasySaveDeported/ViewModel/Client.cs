using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PS_G1_L1.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EasySaveDeported.ViewModel
{
    public sealed class Client
    {
        private static Client _instance;
        private MainWindow main;

        public string Message = string.Empty;

        private Socket ClientSocket;

        public bool IsRunning = false;

        public int Progression = 0;

        public Client(MainWindow mainWindow)
        {
            Socket client = StartClient(main);
            this.main = mainWindow;
        }

        public static Client Instanciate(MainWindow main)
        {
            if (_instance == null)
                _instance = new Client(main);

            return _instance;
        }

        public Socket StartClient(MainWindow main)
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            string connectMessage = "Connected";
            try
            {
                int port = 13000;

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

                //MessageBox.Show("Essaie de conenxion");

                ClientSocket.Connect(endPoint);

                ClientSocket.Send(Encoding.ASCII.GetBytes(connectMessage), 0, connectMessage.Length, SocketFlags.None);
                byte[] messageFromServer = new byte[ClientSocket.ReceiveBufferSize];
                ClientSocket.Receive(messageFromServer);

                MessageBox.Show("Client is connected !");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("Connexion au serveur impossible.\nVeuillez vérifier que le serveur est allumé.");

                main.Close();
            }

            BackgroundWorker workerReceive = new BackgroundWorker();
            workerReceive.DoWork += new DoWorkEventHandler(receive_DoWork);
            workerReceive.ProgressChanged += new ProgressChangedEventHandler(receive_ProgressChanged);
            workerReceive.RunWorkerCompleted += new RunWorkerCompletedEventHandler(receive_RunWorkerCompleted);
            workerReceive.WorkerReportsProgress = true;
            workerReceive.WorkerSupportsCancellation = true;
            workerReceive.RunWorkerAsync();

            BackgroundWorker workerSend = new BackgroundWorker();
            workerSend.DoWork += new DoWorkEventHandler(send_DoWork);
            workerSend.ProgressChanged += new ProgressChangedEventHandler(send_ProgressChanged);
            workerSend.RunWorkerCompleted += new RunWorkerCompletedEventHandler(send_RunWorkerCompleted);
            workerSend.WorkerReportsProgress = true;
            workerSend.WorkerSupportsCancellation = true;
            workerSend.RunWorkerAsync();

            return ClientSocket;
        }

        public void SendMessage(Socket clientSocket, string messageFromClient)
        {
            clientSocket.Send(Encoding.ASCII.GetBytes(messageFromClient), 0, messageFromClient.Length, SocketFlags.None);
        }

        public void ReceiveMessage(Socket clientSocket)
        {
            int index = 0;
            while (true)
            {
                byte[] messageFromServer = new byte[clientSocket.ReceiveBufferSize];
                int size = clientSocket.Receive(messageFromServer);
                string message = Encoding.ASCII.GetString(messageFromServer);

                //MessageBox.Show($"Received message: {message}");

                if (message.Contains("RUN"))
                {
                    //MessageBox.Show("Lancer la sauvegarde côté client");
                    IsRunning = true;

                    main.Dispatcher.Invoke(new Action(() =>
                        main.ButtonStop.Content = "Pause"));

                    main.Dispatcher.Invoke(new Action(() =>
                        main.ButtonStop.IsEnabled = true));

                    main.Dispatcher.Invoke(new Action(() =>
                        main.ButtonCancel.IsEnabled = true));

                    main.Dispatcher.Invoke(new Action(() =>
                        main.ButtonRun.IsEnabled = false));

                    main.workerProgress.RunWorkerAsync();
                }
                else if (message.Contains("JOBS"))
                {
                    var messageList = message.Split("?");
                    string jobs = messageList.Last();
                    List<JobModel> jobModels = JsonConvert.DeserializeObject<List<JobModel>>(jobs);
                    main.Dispatcher.Invoke(new Action(() => main.DataGridWork.ItemsSource = jobModels));
                }
                else if (message.Contains("PAUSE"))
                {
                    //MessageBox.Show("Lancer la sauvegarde côté client");
                    IsRunning = false;

                    main.Dispatcher.Invoke(new Action(() => main.ButtonStop.Content = "Continuer"));
                    main.Dispatcher.Invoke(new Action(() => main.ButtonRun.IsEnabled = false));

                    main.workerProgress.CancelAsync();
                }
                else if (message.Contains("RESUME"))
                {
                    //MessageBox.Show("Relancer la sauvegarde côté client");
                    IsRunning = true;

                    main.Dispatcher.Invoke(new Action(() => main.ButtonStop.Content = "Pause"));
                    main.Dispatcher.Invoke(new Action(() => main.ButtonRun.IsEnabled = false));

                    main.workerProgress.RunWorkerAsync();
                }
                else if (message.Contains("CANCEL"))
                {
                    //MessageBox.Show("Lancer la sauvegarde côté client");
                    IsRunning = false;

                    main.Dispatcher.Invoke(new Action(() => main.ButtonStop.Content = "Continuer"));
                    main.Dispatcher.Invoke(new Action(() => main.ButtonStop.IsEnabled = false));
                    main.Dispatcher.Invoke(new Action(() => main.ButtonCancel.IsEnabled = false));
                    main.Dispatcher.Invoke(new Action(() => main.ButtonRun.IsEnabled = true));
                    main.Dispatcher.Invoke(new Action(() => main.ProgressBarItem.Value = 0));
                    main.Dispatcher.Invoke(new Action(() => main.DataGridWork.ItemsSource = null));

                    main.workerProgress.CancelAsync();
                }
                else if (message == "Disconnected")
                {
                    //MessageBox.Show("Closing application, connection with host is down.");
                    Application.Exit();
                }
                else
                {
                    if (IsRunning)
                    {
                        //Data of progress
                        if (Progression < 100)
                        {
                            Progression = Convert.ToInt32(Encoding.ASCII.GetString(messageFromServer));
                            main.Dispatcher.Invoke(new Action(() => { main.ProgressBarItem.Value = Progression; }));
                        }
                        else
                        {
                            if (index == 0)
                                MessageBox.Show($"Sauvegarde finie !");
                            index++;
                            Progression = 0;
                            main.Dispatcher.Invoke(new Action(() => main.ButtonStop.Content = "Continuer"));
                            main.Dispatcher.Invoke(new Action(() => main.ButtonStop.IsEnabled = false));
                            main.Dispatcher.Invoke(new Action(() => main.ButtonCancel.IsEnabled = false));
                            main.Dispatcher.Invoke(new Action(() => main.ButtonRun.IsEnabled = true));
                            main.Dispatcher.Invoke(new Action(() => main.ProgressBarItem.Value = 0));
                            main.Dispatcher.Invoke(new Action(() => main.DataGridWork.ItemsSource = null));

                        }
                    }
                    else
                    {
                        //Data from JSON

                    }
                }
                //MessageBox.Show($"{Encoding.ASCII.GetString(messageFromServer, 0, size)}");
            }
        }

        private void receive_DoWork(object sender, DoWorkEventArgs e)
        {
            var testMessage = "Connection";
            ClientSocket.Send(Encoding.ASCII.GetBytes(testMessage), 0, testMessage.Length, SocketFlags.None);
            while (true)
            {
                try
                {
                    ReceiveMessage(ClientSocket);
                    Thread.Sleep(1000);
                }
                catch
                {

                }
            }
        }

        private void receive_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void receive_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void send_DoWork(object sender, DoWorkEventArgs e)
        {
            //MessageBox.Show("Start send backgroundWorker");
            while (true)
            {
                if (Message != string.Empty)
                {
                    try
                    {
                        ClientSocket.Send(Encoding.ASCII.GetBytes(Message));
                        Message = string.Empty;
                    }
                    catch
                    {

                    }
                }
                Thread.Sleep(100);
            }
        }

        private void send_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void send_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

    }
}

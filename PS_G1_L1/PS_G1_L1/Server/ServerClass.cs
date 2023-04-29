using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace PS_G1_L1.Server
{
    public sealed class ServerClass
    {
        private string _ipAddress = "127.0.0.1";
        private int _port = 13000;

        private string _basicAnswer = "Received";

        private Socket clientSocket;

        public string Message { get; set; } = string.Empty;
        public string Progress { get; set; } = string.Empty;

        private static ServerClass _instance;

        public bool CommandReceived { get; set; } = false;
        public string Command { get; set; } = string.Empty;

        public bool IsPaused { get; set; } = false;
        public bool IsRunning { get; set; } = false;
        public bool IsCanceled { get; set; } = false;
        public bool IsConnected { get; set; } = false;
        public bool IsResume { get; set; } = false;
        public bool IsRun { get; set; } = false;

        BackgroundWorker receiveWorker;
        public BackgroundWorker sendWorker;
        private MainWindow main;

        public ServerClass(MainWindow main)
        {
            this.main = main;
        }

        public static ServerClass Instantiate(MainWindow main)
        {
            if (_instance == null)
                _instance = new ServerClass(main);

            return _instance;
        }

        public void StartServer()
        {
            Socket serverListener = CreateSocket();
            clientSocket = serverListener.Accept();
            IsConnected = true;

            receiveWorker = new BackgroundWorker();
            receiveWorker.DoWork += new DoWorkEventHandler(receive_DoWork);
            receiveWorker.ProgressChanged += new ProgressChangedEventHandler(receive_ProgressChanged);
            receiveWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(receive_RunWorkerCompleted);
            receiveWorker.WorkerReportsProgress = true;
            receiveWorker.WorkerSupportsCancellation = true;
            receiveWorker.RunWorkerAsync();

            sendWorker = new BackgroundWorker();
            sendWorker.DoWork += new DoWorkEventHandler(send_DoWork);
            sendWorker.ProgressChanged += new ProgressChangedEventHandler(send_ProgressChanged);
            sendWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(send_RunWorkerCompleted);
            sendWorker.WorkerReportsProgress = true;
            sendWorker.WorkerSupportsCancellation = true;
            sendWorker.RunWorkerAsync();
        }

        #region BackgroundWorker Actions

        private void receive_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    ReceiveMessage(clientSocket);

                    Thread.Sleep(100);
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
            while (true)
            {
                if(Progress != string.Empty)
                {
                    try
                    {
                        clientSocket.Send(Encoding.ASCII.GetBytes(Progress));
                        Progress = string.Empty;

                        Thread.Sleep(100);
                    }
                    catch
                    {

                    }
                }
                if (Message != string.Empty)
                {
                    try
                    {
                        clientSocket.Send(Encoding.ASCII.GetBytes(Message));
                        Message = string.Empty;
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void send_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            clientSocket.Send(Encoding.ASCII.GetBytes(e.ProgressPercentage.ToString()));
            Progress = string.Empty;

            Thread.Sleep(50);
        }

        private void send_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        #endregion

        private Socket CreateSocket()
        {
            Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newSocket.Bind(new IPEndPoint(IPAddress.Parse(_ipAddress), _port));

            newSocket.Listen(10);
            MessageBox.Show("ServerClass : ON");

            return newSocket;
        }

        private void ReceiveMessage(Socket clientSocket)
        {
            byte[] buffer;
            while (true)
            {
                buffer = new byte[clientSocket.ReceiveBufferSize];
                int size = clientSocket.Receive(buffer);
                if (size > 0)
                {
                    string message = Encoding.ASCII.GetString(buffer, 0, size);
                    if(message == "Connection")
                    {
                        Command = message;
                        //MessageBox.Show("Lancer la sauvegarde");
                        CommandReceived = true;

                        IsRunning = true;
                        IsCanceled = false;
                        IsPaused = false;
                        IsResume = false;
                        IsRun = false;
                    }
                    if (message == "RUN")
                    {
                        Command = message;
                        //MessageBox.Show("Lancer la sauvegarde");
                        CommandReceived = true;

                        IsRunning = false;
                        IsCanceled = false;
                        IsPaused = false;
                        IsResume = false;
                        IsRun = true;
                    }
                    else if (message == "PAUSE")
                    {
                        Command = message;
                        //MessageBox.Show("Mettre en pause la sauvegarde");
                        CommandReceived = true;

                        IsRunning = false;
                        IsCanceled = false;
                        IsPaused = true;
                        IsResume = false;
                        IsRun = false;
                        main.job.Pause = IsPaused;
                    }
                    else if (message == "CANCEL")
                    {
                        Command = message;
                        //MessageBox.Show("Annuler la sauvegarde");
                        CommandReceived = true;

                        IsRunning = false;
                        IsCanceled = true;
                        IsPaused = false;
                        IsResume = false;
                        IsRun = false;
                    }
                    else if (message == "RESUME")
                    {
                        Command = message;
                        //MessageBox.Show("Continuer la sauvegarde");
                        CommandReceived = true;

                        IsRunning = true;
                        IsCanceled = false;
                        IsPaused = false;
                        IsResume = true;
                        IsRun = false;

                        main.job.Pause = IsPaused;
                    }
                    else if (message == "Disconnected")
                    {
                        DisconnectClient(clientSocket);
                        IsConnected = false;
                    }
                    else
                    {
                        Command = string.Empty;
                        CommandReceived = false;
                    }
                    SendMessage(clientSocket, _basicAnswer);
                }
            }
        }

        private void DisconnectClient(Socket clientSocket)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

            _instance = null;

            MessageBox.Show("Client has been disconnected");
        }

        private void SendMessage(Socket clientSocket ,string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }
    }
}

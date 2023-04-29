using System.Windows;
using PS_G1_L1.View;
using PS_G1_L1.ViewModel;
using PS_G1_L1.Server;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System;

namespace PS_G1_L1
{
    public enum Button
    {
        Save,
        Work,
        Parameters,
        CreateJob,
        ModifyJob,
        ReadJob,
        DeleteJob,
        ReadMultiJob
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public JobViewModel job;

        public ServerClass server;

        BackgroundWorker serverWorker;

        public MainWindow()
        {
            InitializeComponent();

            job = new JobViewModel();
            server = ServerClass.Instantiate(this);

            ButtonAdd.Visibility = Visibility.Hidden;
            ButtonModify.Visibility = Visibility.Hidden;
            ButtonRead.Visibility = Visibility.Hidden;
            ButtonDelete.Visibility = Visibility.Hidden;

            TitleWorkSelected.Visibility = Visibility.Hidden;
            TextBlockWorkName.Visibility = Visibility.Hidden;

            SetActiveWindow(Button.Save);

        }

        #region Functions


        private void server_DoWork(object sender, DoWorkEventArgs e)
        {
            /*server = ServerClass.Instantiate(this);
            server.StartServer();

            serverWorker.CancelAsync();*/
        }

        private void server_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        private void server_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Server Started");
        }

        private void SetActiveWindow(Button buttonPressed)
        {
            switch(buttonPressed)
            {
                case Button.Save:
                    UpdateButtons(buttonPressed);

                    PanelView.Content = new Save(this);
                    break;
                case Button.Work:
                    UpdateButtons(buttonPressed);

                    PanelView.Content = new Work(this);
                    break;
                case Button.Parameters:
                    UpdateButtons(buttonPressed);

                    PanelView.Content = new Params();
                    break;
                case Button.CreateJob:
                    UpdateButtons(buttonPressed);

                    PanelView.Content = new CreateJob(this);
                    break;
                case Button.ModifyJob:
                    UpdateButtons(buttonPressed);

                    PanelView.Content = new ModifyJob(this);
                    break;
                case Button.ReadJob:
                    UpdateButtons(buttonPressed);

                    PanelView.Content = new ReadJob(this);
                    break;
                case Button.DeleteJob:
                    UpdateButtons(buttonPressed);

                    PanelView.Content = new DeleteJob(this);
                    break;
                case Button.ReadMultiJob:
                    UpdateButtons(buttonPressed);

                    PanelView.Content = new ReadMultiWork(this);
                    break;
            }
        }

        public void UpdateButtons(Button button)
        {
            if (button == Button.Save)
            {
                ButtonSave.IsEnabled = false;
                ButtonWork.IsEnabled = true;
                ButtonParams.IsEnabled = true;

                ButtonAdd.Visibility = Visibility.Hidden;
                ButtonModify.Visibility = Visibility.Hidden;
                ButtonRead.Visibility = Visibility.Hidden;
                ButtonDelete.Visibility = Visibility.Hidden;

                TitleWorkSelected.Visibility = Visibility.Hidden;
                TextBlockWorkName.Visibility = Visibility.Hidden;
            }
            else if (button == Button.Work)
            {
                ButtonSave.IsEnabled = true;
                ButtonWork.IsEnabled = false;
                ButtonParams.IsEnabled = true;

                ButtonAdd.Visibility = Visibility.Visible;
                ButtonModify.Visibility = Visibility.Visible;
                ButtonRead.Visibility = Visibility.Visible;
                ButtonDelete.Visibility = Visibility.Visible;

                TitleWorkSelected.Visibility = Visibility.Visible;
                TextBlockWorkName.Visibility = Visibility.Visible;
            }
            else if (button ==  Button.Parameters)
            {
                ButtonSave.IsEnabled = true;
                ButtonWork.IsEnabled = true;
                ButtonParams.IsEnabled = false;

                ButtonAdd.Visibility = Visibility.Hidden;
                ButtonModify.Visibility = Visibility.Hidden;
                ButtonRead.Visibility = Visibility.Hidden;
                ButtonDelete.Visibility = Visibility.Hidden;

                TitleWorkSelected.Visibility = Visibility.Hidden;
                TextBlockWorkName.Visibility = Visibility.Hidden;
            }
        }

        #endregion


        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SetActiveWindow(Button.Save);
        }

        private void ButtonWork_Click(object sender, RoutedEventArgs e)
        {
            SetActiveWindow(Button.Work);
        }

        private void ButtonParams_Click(object sender, RoutedEventArgs e)
        {
            SetActiveWindow(Button.Parameters);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            SetActiveWindow(Button.CreateJob);
        }

        private void ButtonRead_Click(object sender, RoutedEventArgs e)
        {
            if (TextBlockWorkName.Text == "N/A")
            {
                SetActiveWindow(Button.ReadMultiJob);
            }
            else
            {
                SetActiveWindow(Button.ReadJob);
            }
        }

        private void ButtonModify_Click(object sender, RoutedEventArgs e)
        {
            SetActiveWindow(Button.ModifyJob);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            SetActiveWindow(Button.DeleteJob);
        }

        private void ButtonRemote_Click(object sender, RoutedEventArgs e)
        {
            /*serverWorker = new BackgroundWorker();
            serverWorker.DoWork += new DoWorkEventHandler(server_DoWork);
            serverWorker.ProgressChanged += new ProgressChangedEventHandler(server_ProgressChanged);
            serverWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(server_RunWorkerCompleted);
            serverWorker.WorkerReportsProgress = true;
            serverWorker.WorkerSupportsCancellation = true;
            serverWorker.RunWorkerAsync();*/

            server.StartServer();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Process.GetCurrentProcess().Close();
            Process[] processes = Process.GetProcessesByName("PS_G1_L1");
            foreach (Process process in processes)
            {
                process.Kill();
            }
        }
    }
}

using EasySaveDeported.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;

namespace EasySaveDeported
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client client;
        public BackgroundWorker workerProgress;
        public MainWindow()
        {
            InitializeComponent();
            client = Client.Instanciate(this);

            //workerProgress = new BackgroundWorker();
            //workerProgress.DoWork += new DoWorkEventHandler(progress_DoWork);
            //workerProgress.ProgressChanged += new ProgressChangedEventHandler(progress_ProgressChanged);
            //workerProgress.RunWorkerCompleted += new RunWorkerCompletedEventHandler(progress_RunWorkerCompleted);
            //workerProgress.WorkerReportsProgress = true;
            //workerProgress.WorkerSupportsCancellation = true;
        }

        #region BackgroundWorker Actions

        private void progress_DoWork(object sender, DoWorkEventArgs e)
        {
            //MessageBox.Show("Progress backgroundWorker launch");
            while (client.IsRunning)
            {
                workerProgress.ReportProgress(client.Progression);
            }
        }

        private void progress_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() => ProgressBarItem.Value = e.ProgressPercentage);
        }

        private void progress_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        #endregion

        private void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            client.Message = "RUN";
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            client.Message = "CANCEL";
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonStop.Content == "Pause")
            {
                ButtonStop.Content = "Continuer";
                client.Message = "PAUSE";
            }
            else
            {
                ButtonStop.Content = "Pause";
                client.Message = "RESUME";
            }
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            client.Message = "Disconnected";
            if (workerProgress != null)
            {
                workerProgress.CancelAsync();
                workerProgress.Dispose();
            }
        }
    }
}
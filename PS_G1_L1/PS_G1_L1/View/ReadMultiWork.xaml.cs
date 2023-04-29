using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using PS_G1_L1.ViewModel;
using PS_G1_L1.Model;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Security.Policy;
using PS_G1_L1.Server;
using Newtonsoft.Json;

namespace PS_G1_L1.View
{
    /// <summary>
    /// Logique d'interaction pour ReadMultiWork.xaml
    /// </summary>
    public partial class ReadMultiWork : Page
    {
        private JobViewModel job;

        public List<BackgroundWorker> workers = new List<BackgroundWorker>();

        public BackgroundWorker worker;
        public BackgroundWorker publicBW;

        public bool Pause = false;

        public int completedWorker = 0;

        private MainWindow main;

        private ServerClass server;
        private bool OnWindow = true;

        BackgroundWorker windowWorker = new BackgroundWorker();

        public ReadMultiWork(MainWindow main)
        {
            InitializeComponent();

            job = main.job;

            publicBW = new BackgroundWorker();

            server = main.server;

            if (server.IsRunning)
            {
                server.Message = "Test";
            }

            this.main = main;

            this.main.ButtonAdd.Visibility = Visibility.Hidden;
            this.main.ButtonDelete.Visibility = Visibility.Hidden;
            this.main.ButtonRead.Visibility = Visibility.Hidden;
            this.main.ButtonModify.Visibility = Visibility.Hidden;

            this.main.ButtonWork.IsEnabled = true;

            job.GetJobs();
            job.MultiJob = main.job.MultiJob;

            if (server.IsRunning)
            {
                string jobs = JsonConvert.SerializeObject(job.MultiJob, Formatting.Indented);
                var test = "JOBS§" + jobs;
                server.Message = test;
            }
            DataContext = job;

            if (DataGridWorks.Items.Count > 0)
                DataGridWorks.Items.RemoveAt(DataGridWorks.Items.Count);


            windowWorker.DoWork += new DoWorkEventHandler(window_DoWork);
            windowWorker.ProgressChanged += new ProgressChangedEventHandler(window_ProgressChanged);
            windowWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(window_RunWorkerCompleted);
            windowWorker.WorkerReportsProgress = true;
            windowWorker.WorkerSupportsCancellation = true;
            windowWorker.RunWorkerAsync();
        }

        #region BackgroundWorker Actions

        private void window_DoWork(object sender, DoWorkEventArgs e)
        {
            //MessageBox.Show("Window Open");
            while (OnWindow)
            {
                try
                {
                    if (server.IsConnected)
                    {
                        if (server.IsRun)
                        {
                            server.IsRun = false;
                            server.IsRunning = true;
                            server.Message = "RUN";
                            workers.Clear();
                            completedWorker = 0;

                            foreach (JobModel model in job.MultiJob)
                            {
                                int index = 0;

                                BackgroundWorker backupWorker = new BackgroundWorker();
                                backupWorker.DoWork += new DoWorkEventHandler(worker_DoWork);
                                backupWorker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                                backupWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                                backupWorker.WorkerReportsProgress = true;
                                backupWorker.WorkerSupportsCancellation = true;

                                foreach (JobModel jobModel in job.Jobs)
                                {
                                    if (jobModel.Name == model.Name)
                                    {
                                        break;
                                    }
                                    index++;
                                }

                                this.Dispatcher.Invoke(new Action(() => backupWorker.RunWorkerAsync(index)));
                                workers.Add(backupWorker);
                            }

                            ButtonResume.IsEnabled = false;

                        }
                        else if (server.IsPaused)
                        {
                            Pause = true;
                            if (ButtonStop.Content != "Continuer")
                                ButtonStop.Content = "Continuer";
                        }
                        else if (server.IsResume)
                        {
                            server.IsResume = false;
                            if (ButtonStop.Content != "Pause")
                                ButtonStop.Content = "Pause";
                            Pause = false;
                            workers.Clear();
                            completedWorker = 0;

                            foreach (JobModel model in job.MultiJob)
                            {
                                int index = 0;

                                BackgroundWorker backupWorker = new BackgroundWorker();
                                backupWorker.DoWork += new DoWorkEventHandler(worker_DoWork);
                                backupWorker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                                backupWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                                backupWorker.WorkerReportsProgress = true;
                                backupWorker.WorkerSupportsCancellation = true;

                                foreach (JobModel jobModel in job.Jobs)
                                {
                                    if (jobModel.Name == model.Name)
                                    {
                                        break;
                                    }
                                    index++;
                                }

                                backupWorker.RunWorkerAsync(index);
                                workers.Add(backupWorker);
                            }

                            ButtonResume.IsEnabled = false;
                        }
                        else if (server.IsCanceled)
                        {
                            try
                            {
                                server.Message = "CANCEL";
                            }
                            catch { }

                            foreach (BackgroundWorker bW in workers)
                            {
                                try
                                {
                                    bW.CancelAsync();
                                    bW.Dispose();
                                }
                                catch
                                {

                                }
                            }

                            MessageBox.Show("Canceled");
                            workers.Clear();
                            OnWindow = false;
                            main.Dispatcher.Invoke(new Action(() => main.PanelView.Content = new Work(main)));
                            server.IsCanceled = false;
                        }
                    }
                }
                catch { }

            }
        }

        private void window_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (server.IsRunning)
            {
                server.sendWorker.ReportProgress((int)ProgressBarItem.Value);

                Thread.Sleep(50);
            }
        }

        private void window_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //MessageBox.Show("Quit window");
        }

        #endregion

        private void ButtonResume_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (server.IsConnected)
                    server.Message = "RUN";
            }
            catch { }

            workers.Clear();
            completedWorker = 0;

            foreach (JobModel model in job.MultiJob)
            {
                int index = 0;

                BackgroundWorker backupWorker = new BackgroundWorker();
                backupWorker.DoWork += new DoWorkEventHandler(worker_DoWork);
                backupWorker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                backupWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                backupWorker.WorkerReportsProgress = true;
                backupWorker.WorkerSupportsCancellation = true;

                foreach (JobModel jobModel in job.Jobs)
                {
                    if (jobModel.Name == model.Name)
                    {
                        break;
                    }
                    index++;
                }

                backupWorker.RunWorkerAsync(index);
                workers.Add(backupWorker);
            }

            ButtonResume.IsEnabled = false;
        }

        #region BackgroundWorker actions

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            job.SaveJobs((int)e.Argument, sender);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarItem.Value = e.ProgressPercentage;
            try
            {
                windowWorker.ReportProgress(e.ProgressPercentage);
            }
            catch (Exception ex) { }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            completedWorker++;

            if (completedWorker == workers.Count)
            {
                MessageBox.Show("Work complete");
                OnWindow = false;
                main.PanelView.Content = new Work(main);
            }
        }

        #endregion

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonStop.Content == "Pause")
            {
                ButtonStop.Content = "Continuer";
                job.Pause = true;
                MessageBox.Show("Pause");
                try
                {
                    if (server.IsConnected)
                        server.Message = "PAUSE";
                }
                catch { }

            }
            else
            {
                ButtonStop.Content = "Pause";
                job.Pause = false;

                try
                {
                    if (server.IsConnected)
                        server.Message = "RESUME";
                }
                catch { }
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (server.IsConnected)
                    server.Message = "CANCEL";
            }
            catch { }

            foreach (BackgroundWorker bW in workers)
            {
                try
                {
                    bW.CancelAsync();
                    bW.Dispose();
                }
                catch
                {

                }
            }

            MessageBox.Show("Canceled");
            workers.Clear();
            OnWindow = false;
            main.PanelView.Content = new Work(main);
        }
    }
}

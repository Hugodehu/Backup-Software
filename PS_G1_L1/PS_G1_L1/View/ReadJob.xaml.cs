using PS_G1_L1.Language;
using PS_G1_L1.Model;
using PS_G1_L1.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace PS_G1_L1.View
{
    /// <summary>
    /// Logique d'interaction pour ReadJob.xaml
    /// </summary>
    public partial class ReadJob : Page
    {
        private JobViewModel backUp;
        public BackgroundWorker worker;
        public int ID = 0;

        MainWindow main;

        ReadMultiWork multiWorkView;

        public ReadJob(MainWindow main)
        {
            InitializeComponent();

            this.main = main;
            multiWorkView = new ReadMultiWork(main);

            ComboBoxTypeFileFolder.Items.Add("");
            ComboBoxTypeFileFolder.Items.Add(LanguageManager.GetString("File"));
            ComboBoxTypeFileFolder.Items.Add(LanguageManager.GetString("Folder"));

            ComboBoxEncrypt.Items.Add("");
            ComboBoxEncrypt.Items.Add(LanguageManager.GetString("Yes"));
            ComboBoxEncrypt.Items.Add(LanguageManager.GetString("No"));

            ComboBoxTypeWorkSave.Items.Add("");
            ComboBoxTypeWorkSave.Items.Add(LanguageManager.GetString("Full"));
            ComboBoxTypeWorkSave.Items.Add(LanguageManager.GetString("Differential"));

            backUp = main.job;
            backUp.GetJobs();

            JobModel job = new JobModel();

            foreach (JobModel item in backUp.Jobs)
            {
                if (item.Name == main.TextBlockWorkName.Text)
                {
                    job = item;
                    break;
                }
            }

            TextBoxBackUpName.Text = job.Name;
            TextBoxBackUpSourcePath.Text = job.Source;
            TextBoxBackUpDestinationPath.Text = job.Destination;

            if (job.Type == BackUpType.DIFFERENTIAL)
            {
                ComboBoxTypeWorkSave.SelectedIndex = 2;
            }
            else
            {
                ComboBoxTypeWorkSave.SelectedIndex = 1;
            }

            if (job.TypeOfSave == "Fichier")
            {
                ComboBoxTypeFileFolder.SelectedIndex = 1;
                TitleSourcePath.Text = LanguageManager.GetString("CreateJobFileSource");
                TitleDestinationPath.Text = LanguageManager.GetString("CreateJobFileDestination");
            }
            else
            {
                ComboBoxTypeFileFolder.SelectedIndex = 2;
                TitleSourcePath.Text = LanguageManager.GetString("CreateJobFolderDestination");
                TitleDestinationPath.Text = LanguageManager.GetString("CreateJobFolderSource");
            }

            if (job.IsEncrypt == true)
            {
                ComboBoxEncrypt.SelectedIndex = 1;
            }
            else
            {
                ComboBoxEncrypt.SelectedIndex = 2;
            }
            
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            main.PanelView.Content = new Work(main);
        }

        private void ButtonLaunch_Click(object sender, RoutedEventArgs e)
        {
            main.PanelView.Content = multiWorkView;
            /*int index = 0;

            //Find Job
            foreach(JobModel job in backUp.Jobs)
            {
                if (job.Name == TextBoxBackUpName.Text)
                {
                    break;
                }
                index++;
            }

            ID = index;

            //worker = multiWorkView.publicBW;
            main.Dispatcher.Invoke(() => main.job.MultiJob.Add(backUp.GetJob(ID)));
            main.PanelView.Content = multiWorkView;*/

            /*worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();*/
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            multiWorkView.ProgressBarItem.Value = e.ProgressPercentage;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            backUp.SaveJobs(ID, worker);
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Travail terminé");

            main.PanelView.Content = new Work(main);

            main.PanelView.Content = new Work(main);
            multiWorkView.ProgressBarItem.Value = 0;

            worker.Dispose();
        }
    }
}

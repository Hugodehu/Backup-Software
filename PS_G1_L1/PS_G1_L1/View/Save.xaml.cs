using PS_G1_L1.Language;
using PS_G1_L1.Model;
using PS_G1_L1.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace PS_G1_L1.View
{
    /// <summary>
    /// Logique d'interaction pour Save.xaml
    /// </summary>
    public partial class Save : Page
    {
        public MainWindow main;

        JobViewModel job;
        public BackgroundWorker worker;

        ReadMultiWork multiWorkView;

        private bool _isEncrypt;

        private string _sourcePath;
        private string _destinationPath;

        public Save(MainWindow main)
        {
            InitializeComponent();

            this.main = main;

            job = main.job;
            multiWorkView = new ReadMultiWork(main);

            ComboTypeDestination.Items.Add(LanguageManager.GetString("Folder"));
            ComboTypeDestination.IsEnabled = false;

            ComboCryptage.Items.Add(LanguageManager.GetString("Yes"));
            ComboCryptage.Items.Add(LanguageManager.GetString("No"));

            ComboLogs.Items.Add("JSON");
            ComboLogs.Items.Add("XML");

            ComboTypeSource.SelectedIndex = 0;
            ComboTypeDestination.SelectedIndex = 0;
            ComboCryptage.SelectedIndex = 0;
            ComboLogs.SelectedIndex = 0;

            TextBoxTypeDestinationPath.Text = UserDefaultParameterViewModel.Instantiate().UserDefaultParameterModel.UserSavePath;
        }

        private void ButtonExplorerSource_Click(object sender, RoutedEventArgs e)
        {
            if (ComboTypeSource.SelectedIndex == 0)
            {
                //File

                OpenFileDialog dialog = new OpenFileDialog();

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    TextBoxSourcePath.Text = dialog.FileName;
                }
            }
            else
            {
                //Folder
                FolderBrowserDialog dialog = new FolderBrowserDialog();

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    TextBoxSourcePath.Text = dialog.SelectedPath;
                }
            }
        }

        private void ButtonExplorerTypeDestination_Click(object sender, RoutedEventArgs e)
        {
            //Folder
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                TextBoxTypeDestinationPath.Text = dialog.SelectedPath;
            }
        }

        private void ComboTypeSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxSourcePath.Text = "";
        }

        private void ComboTypeDestination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxTypeDestinationPath.Text = "";
        }

        #region Functions

        private bool CheckData()
        {
            if (TextBoxSourcePath.Text != "" && TextBoxTypeDestinationPath.Text != "")
                return true;
            else
                return false;
        }

        private void PrintError()
        {
            string errors = "";

            if (TextBoxSourcePath.Text == "")
            {
                if (ComboTypeSource.SelectedItem.ToString() == LanguageManager.GetString("File"))
                {
                    errors += LanguageManager.GetString("ErrorSelectFileSource");
                }
                else if (ComboTypeSource.SelectedItem.ToString() == LanguageManager.GetString("Folder"))
                {
                    errors += LanguageManager.GetString("ErrorSelectFolderSource");
                }
            }
            else if (TextBoxTypeDestinationPath.Text == "")
            {
                if (ComboTypeDestination.SelectedItem.ToString() == LanguageManager.GetString("File"))
                {
                    errors += LanguageManager.GetString("ErrorSelectFileDestination");
                }
                else if (ComboTypeDestination.SelectedItem.ToString() == LanguageManager.GetString("Folder"))
                {
                    errors += LanguageManager.GetString("ErrorSelectFolderDestination");
                }
            }

            System.Windows.Forms.MessageBox.Show(errors);
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                JobViewModel job = new JobViewModel();

                bool encrypt = false;

                if (ComboCryptage.SelectedIndex == 0)
                {
                    encrypt = true;
                }
                else
                {
                    encrypt = false;
                }

                _sourcePath = TextBoxSourcePath.Text;
                _destinationPath = TextBoxTypeDestinationPath.Text;


                worker = multiWorkView.publicBW;
                main.PanelView.Content = multiWorkView;

                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;

                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);


                worker.RunWorkerAsync();
            }
            else
            {
                PrintError();
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            multiWorkView.ProgressBarItem.Value = e.ProgressPercentage;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            JobModel backUp = new JobModel();
            backUp.Name = "Instant Save";
            backUp.Source = _sourcePath;
            backUp.Destination = _destinationPath;
            backUp.IsEncrypt = _isEncrypt;
            backUp.Type = BackUpType.FULL;

            main.job.MultiJob.Add(backUp);

            job.Copy("Instant Save", _sourcePath, _destinationPath, _isEncrypt, worker, BackUpType.FULL);
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                System.Windows.Forms.MessageBox.Show(e.Error.ToString());
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Copie Terminée");

                main.PanelView.Content = new Save(main);

                TextBoxSourcePath.Text = "";
                TextBoxTypeDestinationPath.Text = "";

                main.job.MultiJob.Clear();

                worker.Dispose();
            }
        }
    }
}

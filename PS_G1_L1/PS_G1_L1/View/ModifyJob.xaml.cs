
using PS_G1_L1.Language;
using PS_G1_L1.Model;
using PS_G1_L1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PS_G1_L1.View
{
    /// <summary>
    /// Logique d'interaction pour ModifyJob.xaml
    /// </summary>
    public partial class ModifyJob : Page
    {
        private JobViewModel backUp;

        private MainWindow main;

        public ModifyJob(MainWindow main)
        {
            InitializeComponent();

            this.main = main;

            backUp = new JobViewModel();

            ComboBoxTypeFileFolder.Items.Add("");
            ComboBoxTypeFileFolder.Items.Add(LanguageManager.GetString("File"));
            ComboBoxTypeFileFolder.Items.Add(LanguageManager.GetString("Folder"));

            ComboBoxEncrypt.Items.Add("");
            ComboBoxEncrypt.Items.Add(LanguageManager.GetString("Yes"));
            ComboBoxEncrypt.Items.Add(LanguageManager.GetString("No"));

            ComboBoxTypeWorkSave.Items.Add("");
            ComboBoxTypeWorkSave.Items.Add(LanguageManager.GetString("Full"));
            ComboBoxTypeWorkSave.Items.Add(LanguageManager.GetString("Differential"));

            backUp = new JobViewModel();
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

        private void buttonValidate_Click(object sender, RoutedEventArgs e)
        {
            bool isEncrypt;
            if (CheckData())
            {
                int index = 0;

                foreach(JobModel job in backUp.Jobs)
                {
                    if (job.Name == TextBoxBackUpName.Text)
                    {
                        break;
                    }
                    index++;
                }

                if (ComboBoxEncrypt.SelectedIndex == 1)
                {
                    isEncrypt = true;
                }
                else
                {
                    isEncrypt = false;
                }
                if (ComboBoxTypeWorkSave.SelectedIndex == 1)
                {
                    backUp.ModifyJobs(index, TextBoxBackUpName.Text, TextBoxBackUpSourcePath.Text, TextBoxBackUpDestinationPath.Text, BackUpType.FULL, ComboBoxTypeFileFolder.SelectedItem.ToString(), isEncrypt);
                }
                else
                {
                    backUp.ModifyJobs(index, TextBoxBackUpName.Text, TextBoxBackUpSourcePath.Text, TextBoxBackUpDestinationPath.Text, BackUpType.DIFFERENTIAL, ComboBoxTypeFileFolder.SelectedItem.ToString(), isEncrypt);
                }
                main.PanelView.Content = new Work(main);
            }
        }

        #region Functions

        private bool CheckData()
        {
            List<string> errorsList = new List<string>();
            bool isOk = false;

            if (TextBoxBackUpName.Text == string.Empty)
            {
                errorsList.Add(LanguageManager.GetString("ErrorNameSaveEmpty") + "\n");
                isOk = false;
            }
            else if (TextBoxBackUpSourcePath.Text == string.Empty)
            {
                errorsList.Add(LanguageManager.GetString("ErrorSourceEmpty") + "\n");
                isOk = false;
            }
            else if (TextBoxBackUpDestinationPath.Text == string.Empty)
            {
                errorsList.Add(LanguageManager.GetString("ErrorDestinationEmpty") + "\n");
                isOk = false;
            }
            else if (ComboBoxTypeFileFolder.SelectedIndex == 0)
            {
                errorsList.Add(LanguageManager.GetString("ErrorTypeEmpty") + "\n");
                isOk = false;
            }
            else if (ComboBoxEncrypt.SelectedIndex == 0)
            {
                errorsList.Add(LanguageManager.GetString("ErrorEncryptEmpty") + "\n");
                isOk = false;
            }
            else if (ComboBoxTypeWorkSave.SelectedIndex == 0)
            {
                errorsList.Add(LanguageManager.GetString("ErrorTypeOfWork") + "\n");
                isOk = false;
            }
            else
            {
                isOk = true;
            }

            if (isOk == false)
            {
                PrintError(errorsList);
            }

            return isOk;
        }

        private void PrintError(List<string> errors)
        {
            string result = "";
            foreach (string error in errors)
            {
                result += error;
            }

            System.Windows.Forms.MessageBox.Show(result);
        }

        private void ShowAllItems()
        {
            TitleDestinationPath.Visibility = Visibility.Visible;
            TitleEncrypt.Visibility = Visibility.Visible;
            TitleSourcePath.Visibility = Visibility.Visible;
            TitleTypeWorkSave.Visibility = Visibility.Visible;

            TextBoxBackUpDestinationPath.Visibility = Visibility.Visible;
            TextBoxBackUpSourcePath.Visibility = Visibility.Visible;
            ComboBoxEncrypt.Visibility = Visibility.Visible;
            ComboBoxTypeWorkSave.Visibility = Visibility.Visible;

            buttonValidate.Visibility = Visibility.Visible;
        }

        private void HideAllItems()
        {
            TitleDestinationPath.Visibility = Visibility.Hidden;
            TitleEncrypt.Visibility = Visibility.Hidden;
            TitleSourcePath.Visibility = Visibility.Hidden;
            TitleTypeWorkSave.Visibility = Visibility.Hidden;

            TextBoxBackUpDestinationPath.Visibility = Visibility.Hidden;
            TextBoxBackUpSourcePath.Visibility = Visibility.Hidden;
            ComboBoxEncrypt.Visibility = Visibility.Hidden;
            ComboBoxTypeWorkSave.Visibility = Visibility.Hidden;

            buttonValidate.Visibility = Visibility.Hidden;

        }

        #endregion

        private void ComboBoxTypeFileFolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxTypeFileFolder.SelectedIndex == 0)
            {
                HideAllItems();
            }
            else
            {
                ShowAllItems();
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            main.PanelView.Content = new Work(main);
        }

        private void ButtonSource_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxTypeFileFolder.SelectedIndex == 1)
            {
                //File

                OpenFileDialog dialog = new OpenFileDialog();

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    TextBoxBackUpSourcePath.Text = dialog.FileName;
                }
            }
            else if (ComboBoxTypeFileFolder.SelectedIndex == 2)
            {
                //Folder
                FolderBrowserDialog dialog = new FolderBrowserDialog();

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    TextBoxBackUpSourcePath.Text = dialog.SelectedPath;
                }
            }
        }

        private void ButtonDestination_Click(object sender, RoutedEventArgs e)
        {
            //Folder
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                TextBoxBackUpDestinationPath.Text = dialog.SelectedPath;
            }
        }
    }
}

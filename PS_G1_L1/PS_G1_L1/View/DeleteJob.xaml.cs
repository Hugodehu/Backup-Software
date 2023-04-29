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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PS_G1_L1.View
{
    /// <summary>
    /// Logique d'interaction pour DeleteJob.xaml
    /// </summary>
    public partial class DeleteJob : Page
    {
        private JobViewModel backUp;

        MainWindow main;

        public DeleteJob(MainWindow main)
        {
            InitializeComponent();

            backUp = new JobViewModel();

            this.main = main;

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
            MessageBoxResult result = new MessageBoxResult();
            result = MessageBox.Show(LanguageManager.GetString("DeleteJobConfirmTitle"), LanguageManager.GetString("DeleteJobMessage"), MessageBoxButton.YesNo);

            int index = 0;

            foreach (JobModel job in backUp.Jobs)
            {
                if (job.Name == TextBoxBackUpName.Text)
                {
                    break;
                }
                index++;
            }

            if (result == MessageBoxResult.Yes)
            {
                //Delete the work
                backUp.DeleteJob(index);
                main.PanelView.Content = new Work(main);
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            main.PanelView.Content = new Work(main);
        }
    }
}

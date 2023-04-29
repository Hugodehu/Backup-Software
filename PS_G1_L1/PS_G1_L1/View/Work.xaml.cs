
using Haley.Utils;
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
    /// Logique d'interaction pour Work.xaml
    /// </summary>
    public partial class Work : Page
    {
        JobViewModel viewModel;

        MainWindow main;

        public Work(MainWindow main)
        {
            InitializeComponent();

            main.TitleWorkSelected.Visibility = Visibility.Visible;
            main.TextBlockWorkName.Visibility = Visibility.Visible;

            this.main = main;
            viewModel = new JobViewModel();
            viewModel.GetJobs();
            DataContext = viewModel;

            main.TextBlockWorkName.Text = "";
            main.ButtonAdd.Visibility = Visibility.Visible;

            main.ButtonRead.Visibility = Visibility.Hidden;
            main.ButtonModify.Visibility = Visibility.Hidden;
            main.ButtonDelete.Visibility = Visibility.Hidden;

            TextBlockNumberOfSave.Text = LanguageManager.GetString("NumberSaveTitle") + viewModel.Jobs.Count.ToString();
        }

        #region Functions


        #endregion

        private void DataGridWork_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            main.ButtonRead.Visibility = Visibility.Visible;
            main.ButtonModify.Visibility = Visibility.Visible;
            main.ButtonDelete.Visibility = Visibility.Visible;

            main.job.MultiJob.Clear();

            if (DataGridWork.SelectedItems.Count > 1)
            {
                main.TextBlockWorkName.Text = "N/A";
                foreach(JobModel job in DataGridWork.SelectedItems)
                {
                    if (viewModel.Jobs.Contains(job))
                        main.job.MultiJob.Add(job);
                }
            }
            else
            {
                main.TextBlockWorkName.Text = viewModel.Jobs[DataGridWork.SelectedIndex].Name;
                foreach (JobModel job in DataGridWork.SelectedItems)
                {
                    if (viewModel.Jobs.Contains(job))
                        main.job.MultiJob.Add(job);
                }
            }
        }
    }
}

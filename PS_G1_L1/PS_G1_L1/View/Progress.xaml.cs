using PS_G1_L1.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
    /// Logique d'interaction pour Progress.xaml
    /// </summary>
    public partial class Progress : Page
    {
        public Progress(string WorkName = "")
        {
            InitializeComponent();

            SetInitialInformations(WorkName);
        }

        #region Functions

        public Progress GetPage()
        {
            return this;
        }

        public void ChangeProgressBar(double Pourcent, double ActualNmber, double Max)
        {
            ProgressBarItem.Value = Pourcent;
            TextBlockPourcent.Text = $"{ActualNmber}/{Max} - {(int)Pourcent}%";
            Thread.Sleep(100);
            //System.Windows.Forms.MessageBox.Show($"Pourcent : {Pourcent}\nActualNmber : {ActualNmber}\nMax : {Max}");
        }

        public void SetInitialInformations(string WorkName)
        {
            if (UserDefaultParameterViewModel.Instantiate().UserDefaultParameterModel.Language == Model.Enums.LanguageEnum.FR)
                TextBoxWorkName.Text = "Travail : " + WorkName;
            else if (UserDefaultParameterViewModel.Instantiate().UserDefaultParameterModel.Language == Model.Enums.LanguageEnum.EN)
                TextBoxWorkName.Text = "Work : " + WorkName;
        }

        public void SetInfo(string WorkInProgress)
        {
            if (UserDefaultParameterViewModel.Instantiate().UserDefaultParameterModel.Language == Model.Enums.LanguageEnum.FR)
                TextBoxLoading.Text = "Chargement en cours : " + WorkInProgress;
            else if (UserDefaultParameterViewModel.Instantiate().UserDefaultParameterModel.Language == Model.Enums.LanguageEnum.EN)
                TextBoxLoading.Text = "Loading : " + WorkInProgress;
        }

        #endregion
    }
}

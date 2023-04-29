using PS_G1_L1.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PS_G1_L1.View
{
    /// <summary>
    /// Logique d'interaction pour ProgressView.xaml
    /// </summary>
    public partial class ProgressView : Window
    {
        public BackgroundWorker worker;

        public ProgressView()
        {
            InitializeComponent();
            SetTitle();

            worker = new BackgroundWorker();
        }

        #region Functions

        public void SetTitle()
        {
            if (UserDefaultParameterViewModel.Instantiate().UserDefaultParameterModel.Language == Model.Enums.LanguageEnum.FR)
                TextBlockTitle.Text = "Chargement en cours";
            else if (UserDefaultParameterViewModel.Instantiate().UserDefaultParameterModel.Language == Model.Enums.LanguageEnum.EN)
                TextBlockTitle.Text = "Loading";
        }

        #endregion
    }
}

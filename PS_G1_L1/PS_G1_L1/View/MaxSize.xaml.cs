using PS_G1_L1.Constant;
using PS_G1_L1.ViewModel;
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
using System.Windows.Shapes;

namespace PS_G1_L1.View
{
    /// <summary>
    /// Logique d'interaction pour MaxSize.xaml
    /// </summary>
    public partial class MaxSize : Window
    {
        private UserDefaultParameterViewModel _viewModel;
        public MaxSize()
        {
            InitializeComponent();
            _viewModel = new UserDefaultParameterViewModel();
            ComboBoxUnit.Items.Add("Ko");
            ComboBoxUnit.Items.Add("Mo");
            ComboBoxUnit.Items.Add("Go");
            TextBoxSize.Text = Deconvert(UserConst.MaxSizeFile).ToString();
        }

        private void ButtonValidate_Click(object sender, RoutedEventArgs e)
        {
            ConvertSize(ComboBoxUnit.SelectedItem.ToString(), int.Parse(TextBoxSize.Text));
            this.Close();
        }

        private void ConvertSize(string unit, int size)
        {
            if (unit == "Ko")
            {
                _viewModel.SetMaxFileSize((int)(size * MathF.Pow(1024, 1)));
            }
            else if (unit == "Mo")
            {
                _viewModel.SetMaxFileSize((int)(size * MathF.Pow(1024, 2)));
            }
            else
            {
                _viewModel.SetMaxFileSize((int)(size * MathF.Pow(1024, 3)));
            }
        }
        private int Deconvert(int size)
        {
            if ((size / MathF.Pow(1024, 1)) < 1024)
            {
                ComboBoxUnit.SelectedIndex = 0;
                return (int)(size / MathF.Pow(1024, 1));
            }
            else if ((size / MathF.Pow(1024, 2)) < 1024)
            {
                ComboBoxUnit.SelectedIndex = 1;
                return (int)(size / MathF.Pow(1024, 2));
            }
            else if ((size / MathF.Pow(1024, 3)) < 1024)
            {
                ComboBoxUnit.SelectedIndex = 2;
                return (int)(size / MathF.Pow(1024, 3));

            }
            else
            {
                ComboBoxUnit.SelectedIndex = 0;
                return (int)(size / MathF.Pow(1024, 1));
            }
        }
    }
}

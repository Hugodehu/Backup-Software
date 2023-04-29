using PS_G1_L1.Model.Enums;
using PS_G1_L1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using static System.Net.Mime.MediaTypeNames;

namespace PS_G1_L1.View
{
    /// <summary>
    /// Logique d'interaction pour Params.xaml
    /// </summary>
    public partial class Params : Page
    {
        private UserDefaultParameterViewModel parameters = UserDefaultParameterViewModel.Instantiate();
        public Params()
        {
            InitializeComponent();

            ComboBoxLogs.Items.Add("JSON");
            ComboBoxLogs.Items.Add("XML");

            ComboBoxLanguage.Items.Add("Français");
            ComboBoxLanguage.Items.Add("English");

            TextBoxDefaultSavePath.Text = parameters.UserDefaultParameterModel.UserSavePath;

            if (parameters.UserDefaultParameterModel.Format == FormatEnum.JSON)
                ComboBoxLogs.SelectedIndex = 0;
            else
                ComboBoxLogs.SelectedIndex = 1;

            if (parameters.UserDefaultParameterModel.Language == LanguageEnum.FR)
                ComboBoxLanguage.SelectedIndex = 0;
            else
                ComboBoxLanguage.SelectedIndex = 1;
        }

        private void ButtonValidateParams_Click(object sender, RoutedEventArgs e)
        {
            parameters.UserDefaultParameterModel.UserSavePath = TextBoxDefaultSavePath.Text;

            if (ComboBoxLogs.SelectedIndex == 0)
                parameters.SetFormat(FormatEnum.JSON);
            else
                parameters.SetFormat(FormatEnum.XML);

            if (ComboBoxLanguage.SelectedIndex == 0)  
                parameters.SetLanguage(LanguageEnum.FR);
            else
                parameters.SetLanguage(LanguageEnum.EN);
            parameters.UserDefaultParameterModel.WriteData();
        }

        private void ButtonSource_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                TextBoxDefaultSavePath.Text = dialog.SelectedPath;
            }
        }

        private void ButtonExtensionEncrypt_Click(object sender, RoutedEventArgs e)
        {
            ExtensionEncrypt extensionEncryptView = new ExtensionEncrypt();
            extensionEncryptView.ShowDialog();
        }

        private void ButtonExtensionPriority_Click(object sender, RoutedEventArgs e)
        {
            ExtensionPriority extensionPriorityView = new ExtensionPriority();
            extensionPriorityView.ShowDialog();
        }

        private void ButtonMaxSize_Click(object sender, RoutedEventArgs e)
        {
            MaxSize maxSizeView = new MaxSize();
            maxSizeView.ShowDialog();
        }

        private void ButtonSoftWare_Click(object sender, RoutedEventArgs e)
        {
            BusinessSoftware businessSoftware = new BusinessSoftware();
            businessSoftware.ShowDialog();
        }
    }
}

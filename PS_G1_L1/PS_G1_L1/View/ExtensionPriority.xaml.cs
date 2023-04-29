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
    /// Logique d'interaction pour ExtensionPriority.xaml
    /// </summary>
    public partial class ExtensionPriority : Window
    {
        private UserDefaultParameterViewModel viewModel;
        public ExtensionPriority()
        {
            InitializeComponent();
            viewModel = new UserDefaultParameterViewModel();
            List<string> extensionFilePrioritary = UserConst.ExtensionFilePrioritary;
            foreach (string extension in extensionFilePrioritary)
            {
                ListBoxExtension.Items.Add(extension);
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ListBoxExtension.Items.Contains(GenerateExtension(TextBoxExtension.Text)))
            {
                if (GenerateExtension(TextBoxExtension.Text) != "")
                {
                    viewModel.AddExtensionFilePrioritary(GenerateExtension(TextBoxExtension.Text));
                    ListBoxExtension.Items.Add(GenerateExtension(TextBoxExtension.Text));
                }
                else
                    MessageBox.Show("L'extension que vous venez d'entrer n'est pas valide !");
            }
            else
            {
                MessageBox.Show("L'extension existe déjà");
            }
        }

        private string GenerateExtension(string extension)
        {
            int pointCounter = 0;
            foreach (char letter in extension)
            {
                if (letter == '.')
                {
                    pointCounter++;
                }
            }

            if (pointCounter == 0)
            {
                return "." + extension;
            }
            else if (pointCounter == 1)
            {
                return extension;
            }
            else
            {
                return "";
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            viewModel.DeleteEncryptExtension(GenerateExtension((string)ListBoxExtension.SelectedItem));
            if (ListBoxExtension.Items.Contains(ListBoxExtension.SelectedItem))
            {
                ListBoxExtension.Items.Remove(ListBoxExtension.SelectedItem);
                ListBoxExtension.SelectedItem = 0;
            }
        }

        private void ListBoxExtension_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxExtension.Items.Count > 0)
            {
                if (ListBoxExtension.SelectedIndex >= 0)
                {
                    TextBoxExtension.Text = ListBoxExtension.Items[ListBoxExtension.SelectedIndex].ToString();
                }
                else
                {
                    TextBoxExtension.Text = "";
                }
            }
        }
    }
}

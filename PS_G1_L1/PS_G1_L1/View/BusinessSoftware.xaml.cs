using System.Windows;
using PS_G1_L1.Constant;
using PS_G1_L1.ViewModel;

namespace PS_G1_L1.View
{
    /// <summary>
    ///     Logique d'interaction pour BusinessSoftware.xaml
    /// </summary>
    public partial class BusinessSoftware : Window
    {
        private UserDefaultParameterViewModel parameters;

        public BusinessSoftware()
        {
            InitializeComponent();
            parameters = new UserDefaultParameterViewModel();
            var businessSoftwareProcesses = UserConst.BusinessSoftwareProcesses;
            foreach (var processName in businessSoftwareProcesses) ListBoxBusinessSoftware.Items.Add(processName);
        }

        private void AddBusinessSoftware_Click(object sender, RoutedEventArgs e)
        {
            string input = TextBoxBusinessSoftware.Text;
            
            if (input == "")
            {
                MessageBox.Show("Veuillez renseigner une application.");
                return;
            }

            if (ListBoxBusinessSoftware.Items.Contains(input))
            {
                MessageBox.Show("Cette application est déjà renseignée.");
                return;
            }

            parameters.AddBusinessSoftwareProcess(input);
            ListBoxBusinessSoftware.Items.Add(input);
        }

        private void RemoveBusinessSoftware_Click(object sender, RoutedEventArgs e)
        {
            if (!ListBoxBusinessSoftware.Items.Contains(ListBoxBusinessSoftware.SelectedItem)) return;
            parameters.DeleteBusinessSoftwareProcess(ListBoxBusinessSoftware.SelectedItem.ToString());
            ListBoxBusinessSoftware.Items.Remove(ListBoxBusinessSoftware.SelectedItem);
            ListBoxBusinessSoftware.SelectedItem = 0;
        }
    }
}
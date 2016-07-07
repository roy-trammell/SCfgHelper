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

using SystemLogic;
using SystemLogic.Extensions;
using SystemLogic.Interfaces;

namespace SCfgHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ISCfgCommand m_scCommand;

        public ISCfgCommand SC
        {
            get { return m_scCommand; }
            set { m_scCommand = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetAllControlsEnabledState(bool isEnabled)
        {
            txtServiceName.IsEnabled = isEnabled;
            txtDisplayName.IsEnabled = isEnabled;
            txtPath.IsEnabled = isEnabled;
            txtUsername.IsEnabled = isEnabled;
            txtPasswordBox.IsEnabled = isEnabled;
        }

        private void rbtnCreateService_Click(object sender, RoutedEventArgs e)
        {
            SetAllControlsEnabledState(true);
            btnConfigureService.Content = "CREATE";
        }

        private void rbtnDeleteService_Click(object sender, RoutedEventArgs e)
        {
            SetAllControlsEnabledState(false);
            btnConfigureService.Content = "DELETE";

            txtServiceName.IsEnabled = true;
        }

        private void btnConfigureService_Click(object sender, RoutedEventArgs e)
        {
            if (txtServiceName.Text.Length < 1)
            {
                MessageBox.Show("Enter Service Name", "Input Validation Error", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            try
            {
                if (rbtnCreateService.IsChecked == true)
                    SC = new SCfgCommand(ConfigOperation.Create);
                if (rbtnDeleteService.IsChecked == true)
                    SC = new SCfgCommand(ConfigOperation.Delete);

                SC.ServiceName = txtServiceName.Text;
                SC.Arguments = RetrieveArgumentsList().BuildDictionary();
                SC.ExecCommand();

                MessageBox.Show("Complete.", "Service Configuration Status", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(NullReferenceException nullRefEx)
            {
                MessageBox.Show("Select either 'Create Service' or 'Delete Service'", "Input Validation Error", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Service Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<KeyValuePair<string, string>> RetrieveArgumentsList()
        {
            List<KeyValuePair<string, string>> argumentList = new List<KeyValuePair<string, string>>();
            bool allControlsValidated = false;

            if (SC.ConfigOperationType == ConfigOperation.Create)
            {
                allControlsValidated = (
                    txtDisplayName.Text.Length > 0 &&
                    txtPath.Text.Length > 0 &&
                    txtUsername.Text.Length > 0 &&
                    txtPasswordBox.Password.Length > 0
                );

                if (allControlsValidated)
                {
                    argumentList.Add(new KeyValuePair<string, string>("DisplayName", txtDisplayName.Text));
                    argumentList.Add(new KeyValuePair<string, string>("binPath", txtPath.Text));
                    argumentList.Add(new KeyValuePair<string, string>("obj", txtUsername.Text));
                    argumentList.Add(new KeyValuePair<string, string>("password", txtPasswordBox.Password));
                }
                else
                {
                    throw new Exception("Enter Values for Enabled Controls");
                }
            }

            return argumentList;
        }
    }
}

using OzekiDemo.DataObjects;
using OzekiDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static OzekiDemo.DataOperation.DataOperation;

namespace OzekiDemo
{
    /// <summary>
    /// Interaction logic for Phone.xaml
    /// </summary>
    public partial class PhoneSetting : Window
    {
        enum RegStatus : int
        {
            RegistrationSuccessful = 1,
            RegistrationNotSuccessful = 0
        }
        public AccountModel Model { get; set; }
        int counter = 0;
        public int RegStat = 0;
        public PhoneSetting(Window owner, AccountModel model)
        {
            Model = model;
            Owner = owner;
            InitializeComponent();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            bool iCounterResult = Int32.TryParse(txtCounter?.Text, out counter);
            if (counter > 30 || counter < 1)
            {
                MsgBox("Refresh Interval is not within specified range");
                //MessageBox.Show("Refresh Interval is not within specified range", "Reliance Ozeki");
                return;
            }

            if (!Validate("User name", txtUsername.Text))
                return;

            if (!Validate("Caller ID", txtCallerID.Text))
                return;

            if (!Validate("Domain", txtDomain.Text))
                return;

            if (!Validate("Password",txtPassword.Password))
                return;

            if (!Validate("Counter", txtCounter.Text))
                return;

            SetAccountModel(txtUsername.Text, txtDomain.Text, txtPassword.Password, txtCallerID.Text);

            Settings setting = new Settings()
            {
                Domain = txtDomain.Text,
                Username = txtUsername.Text,
                Password = txtPassword.Password,
                CallerId = txtCallerID.Text,
                Restart = chkservicestart?.IsChecked,
                //TODO : service Duration
                ServiceDuration = CalculateServiceDuration()
            };
            RegStat = (int)RegStatus.RegistrationSuccessful;
            setting.RegistrationStatus = RegStat;

            //Save into DB
            bool settingResult = SaveSettings(setting);
            if (settingResult == false)
            {
                MsgBox("oops! please try again");
                //MessageBox.Show("oops! please try again", "Relaince Zoiper");
                return;
            }

            DialogResult = true;
            Clear();
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
        private void txtCounter_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }



        private void Clear()
        {
            txtDomain.Text = string.Empty;
            txtCallerID.Text = string.Empty;
            txtPassword.Password = string.Empty;
            txtUsername.Text = string.Empty;
            chkservicestart.IsChecked = false;
        }

        private void SetAccountModel(string username, string domain, string password, string callerID)
        {
            Model = new AccountModel
            {
                DisplayName = username,
                RegisterName = callerID,
                UserName = callerID,
                Domain = domain,
                Password = password
            };
        }

        private bool Validate(string propertyName, string value)
        {
            if (value == null || string.IsNullOrEmpty(value.Trim()))
            {
                MsgBox(string.Format("{0} cannot be empty!", propertyName));
                //MessageBox.Show(string.Format("{0} cannot be empty!", propertyName), "Reliance Ozeki");
                return false;
            }

            return true;
        }

        private int CalculateServiceDuration()
        {
            string duration = cboDuration?.Text;
            int result = 0;
            switch (duration)
            {
                case "Minute":
                    //TODO : Calculate counter with minute
                    result = counter;
                    break;
                case "Hour":
                    //TODO : Calculate counter with hour
                    result = counter * 60;
                    break;
                case "Day":
                    //TODO : Calculate counter with day
                    result = counter * 60 * 24;
                    break;
                default:
                    MsgBox("Please select a duration");
                   // MessageBox.Show("Please select a duration", "Reliance Ozeki");
                    break;
            }
            return result;
        }


        private void MsgBox(string message)
        {
            MessageBox.Show(message, "Reliance Ozeki");
        }
    }
}

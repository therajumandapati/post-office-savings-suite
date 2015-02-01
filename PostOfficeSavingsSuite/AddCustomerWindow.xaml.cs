using PostOfficeSavingsSuite.Models;
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

namespace PostOfficeSavingsSuite
{
    /// <summary>
    /// Interaction logic for AddCustomer.xaml
    /// </summary>
    public partial class AddCustomerWindow : Window
    {
        public CustomerDetailsDirty CustomerDetails;
        public AddCustomerWindow()
        {
            InitializeComponent();
            CustomerDetails = new CustomerDetailsDirty(SaveButton);
        }

        private void AcccountNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            var accountNumber = sender as TextBox;
            Int32 selectionStart = accountNumber.SelectionStart;
            Int32 selectionLength = accountNumber.SelectionLength;
            String newText = String.Empty;
            int count = 0;
            foreach (Char c in accountNumber.Text.ToCharArray())
            {
                if (Char.IsDigit(c) || Char.IsControl(c) || (c == '.' && count == 0))
                {
                    newText += c;
                    if (c == '.')
                        count += 1;
                }
            }
            accountNumber.Text = newText;
            accountNumber.SelectionStart = selectionStart <= accountNumber.Text.Length ? selectionStart : accountNumber.Text.Length;
            Error.Visibility = Visibility.Collapsed;
            CustomerDetails.HasChanged("AccountNumber", false);
            if (accountNumber.Text.Length == 5)
            {
                var isValid = !DbWorker.CheckIfAccountNumberIsTaken(accountNumber.Text);
                if (isValid)
                {
                    Error.Visibility = Visibility.Collapsed;
                    CustomerDetails.HasChanged("AccountNumber", true);
                }
                else 
                {
                    Error.Visibility = Visibility.Visible;
                    CustomerDetails.HasChanged("AccountNumber", false);
                }
            }
        }

        private void CustomerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var nameTextBox = sender as TextBox;
            if(nameTextBox.Text.Length == 0)
            {
                CustomerDetails.HasChanged("Name", false);
                return;
            }
            CustomerDetails.HasChanged("Name", true);
        }

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            var amountTextBox = sender as TextBox;
            Int32 selectionStart = amountTextBox.SelectionStart;
            Int32 selectionLength = amountTextBox.SelectionLength;
            String newText = String.Empty;
            int count = 0;
            foreach (Char c in amountTextBox.Text.ToCharArray())
            {
                if (Char.IsDigit(c) || Char.IsControl(c) || (c == '.' && count == 0))
                {
                    newText += c;
                    if (c == '.')
                        count += 1;
                }
            }
            amountTextBox.Text = newText;
            amountTextBox.SelectionStart = selectionStart <= amountTextBox.Text.Length ? selectionStart : amountTextBox.Text.Length;
            if (amountTextBox.Text.Length == 0)
            {
                CustomerDetails.HasChanged("Amount", false);
                return;
            }
            CustomerDetails.HasChanged("Amount", true);
        }

        private void AccStartedDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var accountDate = sender as DatePicker;
            if (accountDate.SelectedDate == null) 
            {
                CustomerDetails.HasChanged("AccountStartedOn", false);
                return;
            }
            CustomerDetails.HasChanged("AccountStartedOn", true);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var successful = DbWorker.AddCustomer(new DbCustomer 
            {
                Name = CustomerName.Text,
                AccountNumber = AccountNumber.Text,
                Amount = Amount.Text,
                AccountStarted = AccStartedDate.SelectedDate.Value.ToString("dd-MM-yyyy")
            });
            if (successful) 
            {
                AddCustomerSuccessful.Visibility = Visibility.Visible;
                AddCustomerForm.Visibility = Visibility.Collapsed;
            }
        }

        private void CreateNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            AddCustomerForm.Visibility = Visibility.Visible;
            AddCustomerSuccessful.Visibility = Visibility.Collapsed;
            AccountNumber.Text = String.Empty;
            CustomerName.Text = String.Empty;
            Amount.Text = String.Empty;
            AccStartedDate.SelectedDate = null;
            CustomerDetails.ResetStatus();
        }
    }

    public class CustomerDetailsDirty 
    {
        private bool _accountNumber;
        private bool _name;
        private bool _amount;
        private bool _accountStartedOn;
        private Button _targetElement;

        public CustomerDetailsDirty(Button targetElement)
        {
            _targetElement = targetElement;
        }

        public void ResetStatus() 
        {
            _accountNumber = false;
            _name = false;
            _amount = false;
            _accountStartedOn = false;
        }

        public bool HasChanged(string key, bool value) 
        {
            switch (key) 
            {
                case "AccountNumber":
                    _accountNumber = value;
                    break;
                case "Name":
                    _name = value;
                    break;
                case "Amount":
                    _amount = value;
                    break;
                case "AccountStartedOn":
                    _accountStartedOn = value;
                    break;
                default:
                    break;
            }
            return CanWeEnableSave();
        }

        private bool CanWeEnableSave() 
        {
            if (_accountNumber == true && _name == true && _amount == true && _accountStartedOn == true) 
            {
                _targetElement.IsEnabled = true;
                return true;
            }
            _targetElement.IsEnabled = false;
            return false;
        }
    }
}

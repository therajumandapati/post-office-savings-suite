using PostOfficeSavingsSuite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
using System.Windows.Shapes;

namespace PostOfficeSavingsSuite
{
    /// <summary>
    /// Interaction logic for CreateForm.xaml
    /// </summary>
    public partial class CreateFormWindow : Window
    {
        public List<Customer> Customers = DbWorker.GetCustomers("");
        public Customer SelectedCustomer;
        public List<Customer> SelectedCustomers = new List<Customer>();
        public CreateFormWindow()
        {
            InitializeComponent();
            SelectedCustomer = new Customer {};
            this.DataContext = SelectedCustomer;
            AccountNumberBox.ItemsSource = Customers;
        }
        
        private void CreateForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit ?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void AccountNumberBox_TextChanged(object sender, RoutedEventArgs e)
        {
            var textBox = sender as AutoCompleteBox;
            if(string.IsNullOrEmpty(textBox.Text)) return;
            if (!CheckIfAccountNumberExists(textBox.Text)) 
            {
                Customers = DbWorker.GetCustomers(textBox.Text);
            }
            textBox.ItemsSource = Customers;
        }

        private bool CheckIfAccountNumberExists(string accountNumber) 
        {
            foreach (var customer in Customers) 
            {
                if (customer.AccountNumber.ToString().StartsWith(accountNumber))
                    return true;
            }
            return false;
        }

        private void AccountNumberBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var autocompletebox = sender as AutoCompleteBox;
            if (autocompletebox.SelectedItem == null) return;
            var selectedCustomer = autocompletebox.SelectedItem as Customer;
            SelectedCustomer.Name = selectedCustomer.Name;
            SelectedCustomer.AccountNumber = selectedCustomer.AccountNumber;
            SelectedCustomer.Amount = selectedCustomer.Amount;
            SelectedCustomer.StartMonth = selectedCustomer.StartMonth;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCustomer.AccountNumber == 0.0) return;
            SelectedCustomers.Add(SelectedCustomer);
            AccountNumberBox.Text = "";
            ClearSelectedCustomer();
            customersList.ItemsSource = SelectedCustomers;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSelectedCustomer();
            AccountNumberBox.Text = "";
        }

        private void ClearSelectedCustomer() 
        {
            SelectedCustomer.Name = null;
            SelectedCustomer.AccountNumber = 0.0D;
            SelectedCustomer.Amount = 0.0D;
            SelectedCustomer.StartMonth = new DateTime();
        }
    }
}

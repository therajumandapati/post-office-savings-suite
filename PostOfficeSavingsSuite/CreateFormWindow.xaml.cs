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
        public Customer SelectedCustomer { get; set; }
        public CreateFormWindow()
        {
            InitializeComponent();
            AccountNumberBox.ItemsSource = Customers;
        }

        private List<string> GetAccountNumbers()
        {
            return new List<string> { "12345", "23456" };
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
    }
}

using PostOfficeSavingsSuite.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public int ExtraMonthsCount = 0;
        public SelectedCustomers SelectedCustomers = new SelectedCustomers();
        public List<Customer> SavedCustomersList;
        public CreateFormWindow()
        {
            InitializeComponent();
            SelectedCustomer = new Customer {};
            this.DataContext = SelectedCustomer;
            AccountNumberBox.ItemsSource = Customers;
            customersList.ItemsSource = SelectedCustomers.List;
            selectedDate.SelectedDate = DateTime.Today;
            SelectedCustomers.List.CollectionChanged += List_CollectionChanged;
        }

        void List_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var list = sender as ObservableCollection<Customer>;
            SaveButton.IsEnabled = list.Count > 0 ? true : false;
        }
        
        private void CreateForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        //    MessageBoxResult result = MessageBox.Show("Are you sure you want to exit ?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //    if (result == MessageBoxResult.No)
        //    {
        //        e.Cancel = true;
        //    }
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
            if (autocompletebox.SelectedItem == null)
            {
                ClearButton.IsEnabled = false;
                return;
            };
            var selectedCustomer = autocompletebox.SelectedItem as Customer;
            SelectedCustomer.Name = selectedCustomer.Name;
            SelectedCustomer.AccountNumber = selectedCustomer.AccountNumber;
            SelectedCustomer.Amount = selectedCustomer.Amount;
            SelectedCustomer.StartMonth = selectedCustomer.StartMonth;
            ExtraMonths.IsEnabled = true;
            ClearButton.IsEnabled = true;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCustomer.AccountNumber == 0.0) return;
            if (SelectedCustomers.List.Count > 25) 
            {
                MessageBox.Show("You cannot add more than 25 customers.");
                return;
            }
            if (CheckIfAlreadyAdded(SelectedCustomer.AccountNumber)) 
            {
                MessageBox.Show("Account already added.");
                return;
            }
            var successful = SelectedCustomers.Add(new Customer 
            {
                Name = SelectedCustomer.Name,
                AccountNumber = SelectedCustomer.AccountNumber,
                Amount = SelectedCustomer.Amount,
                ExtraAmount = SelectedCustomer.ExtraAmount,
                StartMonth = SelectedCustomer.StartMonth
            });
            ExtraMonthsCount = 0;
            ExtraMonths.SelectedItem = null;
            if (!successful)
            {
                MessageBox.Show("Total amount cannot exceed Rs.10000.");
                return;
            }
            else {
                NumberOfCustomers.Text = String.Format("Number of accounts: {0}", SelectedCustomers.List.Count.ToString());
                TotalAmount.Text = String.Format("Total amount: {0}", SelectedCustomers.Total);
                ExtraMonths.IsEnabled = false;
                AccountNumberBox.Text = "";
                ClearSelectedCustomer();
            }
        }

        private bool CheckIfAlreadyAdded(double p)
        {
            var list = SelectedCustomers.List.Select(x => x.AccountNumber).ToList();
            return list.Contains(p);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSelectedCustomer();
            ExtraMonths.IsEnabled = false;
            ExtraMonthsCount = 0;
            ExtraMonths.SelectedItem = null;
            AccountNumberBox.Text = "";
        }

        private void ClearSelectedCustomer() 
        {
            SelectedCustomer.Name = null;
            SelectedCustomer.AccountNumber = 0.0D;
            SelectedCustomer.Amount = 0.0D;
            SelectedCustomer.ExtraAmount = 0.0D;
            SelectedCustomer.StartMonth = new DateTime();
        }

        private void RemoveEntryButton_Click(object sender, RoutedEventArgs e)
        {
            var customer = ((Button)sender).Tag as Customer;
            SelectedCustomers.Remove(customer);
            NumberOfCustomers.Text = String.Format("Number of accounts: {0}", SelectedCustomers.List.Count.ToString());
            TotalAmount.Text = String.Format("Total amount: {0}", SelectedCustomers.Total);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDateAppropriate()) 
            {
                MessageBox.Show("Please select a date", "Error!");
                return;
            }
            SelectedCustomers.Saved = true;
            SavedCustomersList = SelectedCustomers.List.OrderBy(x => x.AccountNumber).ToList();
            AddButton.IsEnabled = false;
            ClearButton.IsEnabled = false;
            PrintButton.IsEnabled = true;
            SaveButton.IsEnabled = false;
            //allocate a serial number
            //show the data
            SelectedCustomers.SerialNumber = DbWorker.GenerateNextSerialNumber();
            MessageBox.Show(String.Format("Paying Rs.{0} for {1} customers", SavedCustomersList.Select(x => x.PayingTotal).Sum(), SavedCustomersList.Count), "Form saved!");
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintForm printForm = new PrintForm(SavedCustomersList);
            printForm.Show();
        }

        private bool IsDateAppropriate() 
        {
            var date = selectedDate.SelectedDate;
            if (date == null) return false;
            if (date < DateTime.Today) return false;
            return true;
        }

        private void ExtraMonths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            var months = (e.AddedItems[0] as ComboBoxItem).Content as string;
            ExtraMonthsCount = Convert.ToInt32(months);
            SelectedCustomer.ExtraAmount = ExtraMonthsCount * SelectedCustomer.Amount;
        }
    }
}

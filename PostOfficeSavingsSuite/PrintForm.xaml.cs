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

namespace PostOfficeSavingsSuite
{
    /// <summary>
    /// Interaction logic for PrintForm.xaml
    /// </summary>
    public partial class PrintForm : Window
    {
        private List<Models.Customer> SavedCustomersList = new List<Models.Customer>();

        public PrintForm()
        {
            InitializeComponent();
        }

        public PrintForm(List<Models.Customer> SavedCustomersList)
        {
            InitializeComponent();
            this.SavedCustomersList = SavedCustomersList;
            UpdateForm();
        }

        private void UpdateForm() 
        {
            Date.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            var i = 0;
            foreach (var customer in SavedCustomersList) 
            {
                var name = (TextBlock)this.CustomerListGrid.FindName((string)("Name" + i));
                name.Text = customer.Name;
                var amount = (TextBlock)this.CustomerListGrid.FindName((string)("Amount" + i));
                amount.Text = Math.Round((customer.Amount + customer.ExtraAmount), 0).ToString();
                var accountNumber = (TextBlock)this.CustomerListGrid.FindName((string)("AccountNumber" + i));
                accountNumber.Text = Math.Round(customer.AccountNumber, 0).ToString();
                var total = (TextBlock)this.CustomerListGrid.FindName((string)("Total" + i));
                total.Text = Math.Round(customer.Total, 0).ToString();
                i++;
            }
            var totalAmount = Convert.ToInt32(SavedCustomersList.Select(x => x.PayingTotal).Sum());
            this.TotalAmount.Text = totalAmount.ToString() + "/-";
            this.TotalAmountInWords.Text = Helper.ConvertCurrencyIntoWords(Convert.ToInt32(totalAmount)) + " only";
        }
    }
}

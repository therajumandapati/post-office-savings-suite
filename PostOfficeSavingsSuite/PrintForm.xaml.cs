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
            this.TotalAmountInWords.Text = words(Convert.ToInt32(totalAmount)) + " only";
        }

        public string words(int numbers)
        {
            int number = numbers;

            if (number == 0) return "Zero";
            if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";
            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }
            string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ", "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ","Eighty ", "Ninety "};
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };
            num[0] = number % 1000; // units
            num[1] = number / 1000;
            num[2] = number / 100000;
            num[1] = num[1] - 100 * num[2]; // thousands
            num[3] = number / 10000000; // crores
            num[2] = num[2] - 100 * num[3]; // lakhs
            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10; // ones
                t = num[i] / 10;
                h = num[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }
    }
}

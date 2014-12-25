using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostOfficeSavingsSuite.Models
{
    public class SelectedCustomers
    {
        public ObservableCollection<Customer> List = new ObservableCollection<Customer>();
        public string SerialNumber { get; set; }
        public bool Saved { get; set; }
        public bool Add(Customer customer)
        {
            if (Saved) return false;
            if (List.Select(x => x.PayingTotal).Sum() + customer.PayingTotal > 10000) return false;
            List.Add(customer);
            return true;
        }

        public void Remove(Customer customer) 
        {
            if (Saved) return;
            List.Remove(customer);
        }

        public double Total
        {
            get
            {
                return List.Select(x => x.Amount + x.ExtraAmount).Sum();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostOfficeSavingsSuite.Models
{
    public class Customer : INotifyPropertyChanged
    {
        private Double _accountNumber;
        private string _name;
        private Double _amount;
        private Double _extra;
        private DateTime _startMonth;
        public Double AccountNumber {
            get { return _accountNumber; }
            set 
            {
                if (value == _accountNumber)
                    return;
                _accountNumber = value;
                OnPropertyChanged("AccountNumber");
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name)
                    return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public Double Amount
        {
            get { return _amount; }
            set
            {
                if (value == _amount)
                    return;
                _amount = value;
                OnPropertyChanged("Amount");
                OnPropertyChanged("PayingTotal");
            }
        }
        public Double ExtraAmount 
        {
            get { return _extra; }
            set
            {
                if (value == _extra)
                    return;
                _extra = value;
                OnPropertyChanged("ExtraAmount");
                OnPropertyChanged("PayingTotal");
            }
        }
        public Double PayingTotal
        {
            get { return _amount + _extra; }
        }
        public DateTime StartMonth
        {
            get { return _startMonth; }
            set
            {
                if (value == _startMonth)
                    return;
                _startMonth = value;
                OnPropertyChanged("StartMonth");
                OnPropertyChanged("Total");
                OnPropertyChanged("StartMonthString");
            }
        }
        public Double Total { 
            get {
                return GetTotal(_startMonth);
            } 
        }

        public string StartMonthString
        {
            get { return GetDateInString(_startMonth); }
        }

        private string GetDateInString(DateTime date)
        {
            if (_startMonth == new DateTime()) return "";
            return date.ToString("y");
        }

        private int GetTotal(DateTime dt2) 
        {
            var dt1 = DateTime.Today;
            int months = ((dt1.Year - dt2.Year) * 12) + (dt1.Month - dt2.Month) + 1;
            return months * Convert.ToInt32(Math.Round(_amount, 0));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) 
        {
            if (PropertyChanged != null) 
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));    
            }
        }
    }
}

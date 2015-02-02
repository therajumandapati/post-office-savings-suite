using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostOfficeSavingsSuite.Models
{
    public class DbCustomer
    {
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public DateTime AccountStarted { get; set; }
    }
}

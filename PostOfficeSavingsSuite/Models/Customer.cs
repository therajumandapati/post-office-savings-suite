using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostOfficeSavingsSuite.Models
{
    public class Customer
    {
        public Double AccountNumber { get; set; }
        public string Name { get; set; }
        public Double Amount { get; set; }
        public DateTime StartMonth{ get; set; }
    }
}

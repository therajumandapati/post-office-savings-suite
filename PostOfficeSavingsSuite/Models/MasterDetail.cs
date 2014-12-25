using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostOfficeSavingsSuite.Models
{
    public class MasterDetail
    {
        public string SerialNumber { get; set; }
        public Double AccountNumber { get; set; }
        public DateTime AccountStart { get; set; }
        public string Name { get; set; }
        public Double Amount { get; set; }
        public Double Extra { get; set; }
        public Double PayingTotal { get; set; }
        public Double Total { get; set; }
    }
}

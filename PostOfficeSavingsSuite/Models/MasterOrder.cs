using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostOfficeSavingsSuite.Models
{
    public class MasterOrder
    {
        public string SLNo { get; set; }
        public DateTime Date { get; set; }
        public Double TotalAmount { get; set; }
        public string TotalAmountInWords { get; set; }
    }
}

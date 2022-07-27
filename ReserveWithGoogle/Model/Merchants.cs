
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReserveWithGoogle.Model
{    
    public class Detail
    {
        public int party_size { get; set; }
        public int total_slots { get; set; }
        public int open_slots { get; set; }
    }

    public class Merchant
    {
        public string name { get; set; }
        public string service_id { get; set; }
        public List<Detail> details { get; set; }
    }

    public class Merchants
    {
        public List<Merchant> merchants { get; set; }
    }
}

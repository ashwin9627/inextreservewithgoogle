using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReserveWithGoogle.Model
{
    public class Availability
    {
        public int spots_total { get; set; }
        public int spots_open { get; set; }
        public int duration_sec { get; set; }
        public string service_id { get; set; }
        public int start_sec { get; set; }
        public string merchant_id { get; set; }
        public Resources resources { get; set; }
        public string confirmation_mode { get; set; }
    }

    public class Metadata
    {
        public string processing_instruction { get; set; }
        public int shard_number { get; set; }
        public int total_shards { get; set; }
        public string nonce { get; set; }
        public int generation_timestamp { get; set; }
    }

    public class Resources
    {
        public int party_size { get; set; }
    }    

    public class ServiceAvailability
    {
        public List<Availability> availability { get; set; }
    }


    public class Availabilityclass
    {
            public Metadata metadata { get; set; }
            public List<ServiceAvailability> service_availability { get; set; }        
    }
}

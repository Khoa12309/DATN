using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class Address
    {
        public Guid id { get; set; }
        public Guid? AccountId { get; set; }
        public string ?Name { get; set; }
        public string ?PhoneNumber { get; set; }
        public string ?SpecificAddress { get; set; }
        public string ?Ward { get; set; }
        public string ?City { get; set; }
        public string ?District { get; set; }
        public string ?Province { get; set; }
        public string? Description { get; set; }
        public string ?DefaultAddress { get; set; }

        public virtual Account? Account { get; set; }
    }
}

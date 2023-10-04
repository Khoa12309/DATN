using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class Role
    {
        public Guid id { get; set; }

        public string name { get; set; }

        public int Status { get; set; }

        public List<Account> Accounts { get; set; }
    }
}

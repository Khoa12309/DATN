using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid? AccountID { get; set; }
        public string Type { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public string Action { get; set; }
        public string Avatar { get; set; }
        public Account? Account { get; set; }
    }
}

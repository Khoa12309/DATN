using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class Account
    {
        public Guid Id { get; set; } 
        public Guid? IdRole { get; set; } 
        public string Name  { get; set; } 
        public string Email { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public DateTime Create_date { get; set; }
        public DateTime Update_date { get; set; }
        public Role? Role { get; set; }
        public List<Address>? Address { get; set; }
        public List<Bill>? Bill { get; set; }
        public List<Notification>? Notification { get; set; }
        public List<Cart>? Carts { get; set; }
        public List<RefreshToken>?  refreshTokens { get; set; }
    }
}

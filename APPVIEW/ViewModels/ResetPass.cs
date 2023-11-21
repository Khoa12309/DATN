using System.ComponentModel.DataAnnotations;

namespace APPVIEW.ViewModels
{
    public class ResetPass
    {
        [Required]
        public string Email { get; set; }
    }
}

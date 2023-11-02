namespace APPVIEW.ViewModels
{
    public class RegisterVm
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }=string.Empty;
        public DateTime Create_date { get; set; }=DateTime.Now;
        public string ConfirmPassword { get; set; }
    }
}

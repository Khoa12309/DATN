namespace APPVIEW.ViewModels
{
    public class ChangePasswordVm
    {
        public Guid  IdUser { get; set; }
        public string OldPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string NewPassWord { get; set; }
    }
}

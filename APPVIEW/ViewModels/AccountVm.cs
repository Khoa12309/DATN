namespace APPVIEW.ViewModels
{
    public class AccountVm
    {
        public Guid Id { get; set; }
        public Guid ?Id_Role { get; set; }
        public string ?Email { get; set; }
        public string ?Password { get; set; }
        public string Avatar { get; set; }
        public Guid AccountId { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? SpecificAddress { get; set; }
        public string? Ward { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Province { get; set; }
        public string? DefaultAddress { get; set; }
        public string? Description { get; set; }
    }
}

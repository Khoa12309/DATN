namespace APPVIEW.ViewModels
{
    public class VoucherVm
    {
        public Guid Id { get; set; }
        public Guid? AccountId { get; set; }
        public int Value { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public double DiscountAmount { get; set; }
        public bool Used { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

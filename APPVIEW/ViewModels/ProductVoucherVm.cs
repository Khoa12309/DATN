namespace APPVIEW.ViewModels
{
    public class ProductVoucherVm
    {
        public Guid Id { get; set; }
        public Guid? Id_Category { get; set; }
        public Guid? Id_Size { get; set; }
        public Guid? Id_Product { get; set; }
        public Guid? Id_Material { get; set; }
        public Guid? Id_Color { get; set; }
        public Guid? Id_supplier { get; set; }
        public int Quantity { get; set; }
        public string Desciption { get; set; }
        public float Price { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public DateTime Create_date { get; set; }
        public DateTime Update_date { get; set; }
        public DateTime Create_by { get; set; }
        public DateTime Update_by { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class ProductDetail
    {
        public Guid Id { get; set; }
        public Guid? Id_Category { get; set; }
        public Guid? Id_Size { get; set; }
        public Guid? Id_Product { get; set; }
        public Guid? Id_Material { get; set; }
        public Guid? Id_Color { get; set; }
        public int Quantity { get; set; }
        public string Desciption { get; set; }
        public double Price { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public DateTime Create_date { get; set; }
        public DateTime Update_date { get; set; }
        public DateTime Create_by { get; set; }
        public DateTime Update_by { get; set; }

        public List<Image> images { get; set; }
        public Size? Size { get; set; }
        public Category? Category { get; set; }
        public Material? Material { get; set; }
        public Color? Color { get; set; }
        public Product? Product { get; set; }

        public List<CartDetail> Carts { get; set; }

        public  List<BillDetail> BillDetails { get; set; }
    }
}

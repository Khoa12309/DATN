using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using APPDATA.Models;

namespace MCV.Configurations
{
    public class ProductetailConfiguration : IEntityTypeConfiguration<ProductDetail>
    {
        public void Configure(EntityTypeBuilder<ProductDetail> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasOne(c => c.Product).WithMany(c => c.ProductDetails).HasForeignKey(c => c.Id_Product);


            builder.HasOne(c => c.Size).WithMany(c => c.ProductDetails).HasForeignKey(c => c.Id_Size);
            builder.HasOne(c => c.Category).WithMany(c => c.ProductDetails).HasForeignKey(c => c.Id_Category);
            builder.HasOne(c => c.Material).WithMany(c => c.ProductDetails).HasForeignKey(c => c.Id_Material);
            builder.HasOne(c => c.Color).WithMany(c => c.ProductDetails).HasForeignKey(c => c.Id_Color);

            builder.HasOne(c => c.Supplier).WithMany(c => c.ProductDetails).HasForeignKey(c => c.Id_supplier);


        }

    }
}

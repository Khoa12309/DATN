using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using APPDATA.Models;

namespace MCV.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasOne(c => c.Supplier).WithMany(c => c.Product).HasForeignKey(c => c.Id_supplier);
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using APPDATA.Models;

namespace MCV.Configurations
{
    public class BIllDetailConfiguration : IEntityTypeConfiguration<BillDetail>
    {
        public void Configure(EntityTypeBuilder<BillDetail> builder)
        {
            builder.HasKey(c => c.id);

            builder.HasOne(c => c.Bill).WithMany(c => c.BillDetails).HasForeignKey(c => c.BIllId);
            builder.HasOne(c => c.ProductDetails).WithMany(c => c.BillDetails).HasForeignKey(c => c.ProductDetailID);

        }
    
    }
}

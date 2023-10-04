using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using APPDATA.Models;

namespace MCV.Configurations
{
    public class BillConfiguration : IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.HasKey(c => c.id);
            builder.HasOne(c => c.Account).WithMany(c => c.Bill).HasForeignKey(c => c.AccountId);  
            builder.HasOne(c => c.Voucher).WithMany(c => c.Bill).HasForeignKey(c => c.Voucherid);
        }

    }
}

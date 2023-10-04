using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using APPDATA.Models;

namespace APPDATA.Configurations
{
    internal class PaymentMethodDetailConfiguration : IEntityTypeConfiguration<PaymentMethodDetail>
    {
        public void Configure(EntityTypeBuilder<PaymentMethodDetail> builder)
        {
            builder.HasKey(c => c.id);
            builder.HasOne(c => c.Bill).WithMany(c => c.PaymentMethodDetails).HasForeignKey(c => c.BillId);
            builder.HasOne(c => c.PaymentMethods).WithMany(c => c.PaymentMethodDetails).HasForeignKey(c => c.PaymentMethodID);
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using APPDATA.Models;

namespace APPDATA.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasOne(c => c.Account).WithMany(c => c.Notification).HasForeignKey(c => c.AccountID);
        }
    }
}

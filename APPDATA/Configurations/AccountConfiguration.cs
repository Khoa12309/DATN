using APPDATA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCV.Configurations
{
    public class AccountConfiguration: IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasOne(c => c.Role).WithMany(c => c.Accounts).HasForeignKey(c => c.IdRole);
        }
    }
}

using APPDATA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Configurations
{
    public class BillHistoryConfiguration : IEntityTypeConfiguration<BillHistory>
    {
        public void Configure(EntityTypeBuilder<BillHistory> builder)
        {
            builder.HasKey(c => c.id);
            builder.HasOne(c => c.Bill).WithMany(c => c.BillHistory).HasForeignKey(c => c.BIllId);
        }


    }
}

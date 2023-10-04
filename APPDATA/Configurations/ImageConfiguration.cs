using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using APPDATA.Models;

namespace MCV.Configurations
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasOne(c => c.ProductDetails).WithMany(c=>c.images).HasForeignKey(c => c.IdProductdetail);
        }
    }
}

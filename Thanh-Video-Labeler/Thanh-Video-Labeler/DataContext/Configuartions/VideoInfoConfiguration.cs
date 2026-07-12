using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thanh_Video_Labeler.Models;

namespace Thanh_Video_Labeler.DataContext.Configuartions
{
    public class VideoInfoConfiguration: IEntityTypeConfiguration<VideoInfo>
    {
        public void Configure(EntityTypeBuilder<VideoInfo> builder)
        {
            // builder
            //     .HasIndex(s => s.SchoolYearId)
            //     .HasDatabaseName("IX_Class_SchoolYearId");
        }
    }
}
using System.Data.Entity.ModelConfiguration;
using GData.Ef6.Entities.InternetCatalog;

namespace GData.Ef6.MappingConfigurations
{
    public class DownloadEntryConfiguration : EntityTypeConfiguration<DownloadEntry>
    {
        public DownloadEntryConfiguration()
        {
            ToTable("DownloadEntries");
        }
    }
}
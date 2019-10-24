using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GData.Ef6.Entities.InternetCatalog
{
    public class DictionaryInfo : IEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string DownloadUrl { get; set; }
    }

    public class DownloadEntry : IEntity
    {
        public int Id { get; set; }
        public string Word { get; set; }

        public ICollection<DownloadLog> DownloadLogs { get; set; }
    }

    public class DownloadLog : IEntity
    {
        public int Id { get; set; }
        public int DownloadEntryId { get; set; }
        public int DictionaryInfoId { get; set; }
        public int DownloadStatusId { get; set; }

        public WordDownloadStatusEnum DownloadStatus { get; set; }
        public DownloadEntry DownloadEntry { get; set; }
        public DictionaryInfo DictionaryInfo { get; set; }
    }

    public enum WordDownloadStatusEnum
    {
        Downloaded,
        Pending,
        NotFound,
        Error
    }
}

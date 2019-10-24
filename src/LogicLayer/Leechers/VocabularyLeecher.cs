using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GDomain;

namespace LogicLayer.Leechers
{
    public class DownloadJob
    {
        public string Word { get; set; }
        public string HtmlContent { get; set; }
        public WordDownloadStatusEnum Status { get; set; }
    }

    public class VocabularyLeecher
    {
        private readonly List<string> _downloadList;
        private readonly List<DownloadJob> _downloadJobs;

        public int DelayInMs { get; set; } = 500;

        public VocabularyLeecher(List<string> downloadList)
        {
            _downloadList = downloadList;
            _downloadJobs = new List<DownloadJob>();

            PopulateDownloadJobsList();
        }

        private void PopulateDownloadJobsList()
        {
            foreach (string word in _downloadList)
            {
                _downloadJobs.Add(new DownloadJob
                {
                    Word = word,
                    Status = WordDownloadStatusEnum.Pending
                });
            }
        }

        private string GetDownloadString(string word)
        {
            return $"http://www.vocabulary.com/dictionary/{word}";
        }    

        public void Download()
        {
            foreach (DownloadJob downloadJob in _downloadJobs)
            {
                try
                {
                    WebClient client = new WebClient();

                    string wordDownloadUrl = GetDownloadString(downloadJob.Word);

                    byte[] data = client.DownloadData(wordDownloadUrl);

                    downloadJob.HtmlContent = Encoding.UTF8.GetString(data);
                    downloadJob.Status = WordDownloadStatusEnum.Downloaded;

                    client.Dispose();
                }
                catch (WebException e)
                {
                    if (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.NotFound)
                    {
                        downloadJob.Status = WordDownloadStatusEnum.NotFound;
                    }
                }
                catch (Exception)
                {
                    downloadJob.Status = WordDownloadStatusEnum.Error;
                }
            }
        }
    }
}

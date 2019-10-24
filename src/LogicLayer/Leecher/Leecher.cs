using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GDomain;
using HtmlAgilityPack;
using LogicLayer.Extensions;

namespace LogicLayer.Leecher
{
    public class DownloadJob
    {
        public DownloadJob()
        {
            Status = WordDownloadStatusEnum.Pending;
        }

        public string Word { get; set; }
        public string ListName { get; set; }
        public DictionariesEnum Dictionary { get; set; }
        public WordDownloadStatusEnum Status { get; set; }
        public string HtmlContent { get; set; }
    }

    public class Client
    {

    }

    public class Reader
    {
        public IEnumerable<string> ReadDirtyVocabularyList(string txtFilePath)
        {
            List<string> wordsArray = File.ReadAllLines(txtFilePath, Encoding.UTF8).ToList();

            if (string.IsNullOrWhiteSpace(wordsArray.Last()))
                wordsArray.Remove(wordsArray.Last());

            return wordsArray;
        }

        public IEnumerable<string> ReadCleanList(string txtFilePath)
        {
            List<string> wordsArray = File.ReadAllLines(txtFilePath, Encoding.UTF8).ToList();

            if (string.IsNullOrWhiteSpace(wordsArray.Last()))
                wordsArray.Remove(wordsArray.Last());

            return wordsArray;
        }
    }

    public class VocabularyList
    {
        public string ListTitle { get; set; }
        public IEnumerable<VocabularyListItem> Words { get; set; }
    }
    public class VocabularyListItem
    {
        public string Word { get; set; }
        public string Meaning { get; set; }
        public string Example { get; set; }
        public string ExampleSource { get; set; }
        public string Description { get; set; }
    }
    
    public class VocabularyListDownloader
    {
        private HtmlNode _documentNode;
        public VocabularyListDownloader(string entryPointHtmlFilePath)
        {
            byte[] data = File.ReadAllBytes(entryPointHtmlFilePath);
            string htmlString = Encoding.UTF8.GetString(data);

            HtmlDocument document = new HtmlDocument();
            document.Load(htmlString);

            _documentNode = document.DocumentNode;
        }

        public VocabularyListDownloader(Uri entryPointPageUri)
        {
            using (var client = new WebClient())
            {
                byte[] data = client.DownloadData(entryPointPageUri);
                var htmlString = Encoding.UTF8.GetString(data);

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(htmlString);

                _documentNode = document.DocumentNode;
            }        }

        public void DownloadAndWrite(string fileSystemWritePath)
        {
            var downloadList = GetUrls();

            foreach (var downloadUri in downloadList)
            {
                string html = Download(downloadUri);

                if (!Directory.Exists(fileSystemWritePath))
                    Directory.CreateDirectory(fileSystemWritePath);

                File.WriteAllText(fileSystemWritePath, html, Encoding.UTF8);
            }
        }

        private string Download(Uri uri)
        {
            using (var client = new WebClient())
            {
                var data = client.DownloadData(uri);
                return Encoding.UTF8.GetString(data);
            }
        }

        private IEnumerable<string> ReadListUrls()
        {
            var wordListDivs = _documentNode.GetSpecificElements("div", "class", "wordlist shortlisting");

            var aElements = wordListDivs.Select(div => div.Element("a"));

            var hrefValues = aElements.Select(a => a.GetAttributeValue("href", "")).ToList();

            var lists = hrefValues.Select(str => str.Split('/').Last());

            return lists;
        }

        private List<Uri> GetUrls()
        {
            const string baseUrl = "https://www.vocabulary.com/lists/";

            var listIdentifiers = ReadListUrls();
            var uris = new List<Uri>();

            foreach (var partialUri in listIdentifiers)
            {
                uris.Add(new Uri(baseUrl + partialUri));
            }

            return uris;
        }
    }


    public class Leecherx
    {
        public int DelayInMs { get; set; } = 500;

        public const string PlaceHolder = "#WORD#";
        private List<DownloadJob> _downloadList = new List<DownloadJob>();

        public void AddToDownloadList(DownloadJob job)
        {
            if (_downloadList.All(t => t.Dictionary != job.Dictionary && t.Word == job.Word))
            {
                _downloadList.Add(job);
            }
        }

        public void AddToDownloadList(string textFilePath, DictionariesEnum dictionary)
        {
            List<string> words = GetWordsList(textFilePath);

            foreach (var word in words)
            {
                if (_downloadList.All(t => t.Dictionary != dictionary && t.Word == word))
                {
                    _downloadList.Add(new DownloadJob { Dictionary = dictionary, Word = word });
                }
            }
        }

        public void Download()
        {
            var pendingDownloads = _downloadList.Where(job => job.Status == WordDownloadStatusEnum.Pending);

            using (var client = new WebClient())
            {
                foreach (var downloadJob in pendingDownloads)
                {
                    try
                    {
                        // todo: indirmeden önce veritabanına bağlanıp dosya var mı yok mu kontrol etmeli

                        string downloadDataUrl = PrepareDownloadUrl(downloadJob.Word, downloadJob.Dictionary);
                        var data = client.DownloadData(downloadDataUrl);
                        string htmlContent = Encoding.UTF8.GetString(data);

                        downloadJob.HtmlContent = htmlContent;
                        downloadJob.Status = WordDownloadStatusEnum.Downloaded;

                        Thread.Sleep(DelayInMs);
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

        private void Save(string saveDirectory, DownloadJob downloadJob)
        {
            if (!Directory.Exists($@"{saveDirectory}\{downloadJob.Dictionary}"))
            {
                Directory.CreateDirectory($"{saveDirectory}\\{downloadJob.Dictionary}");
            }

            if (File.Exists($"{saveDirectory}\\{downloadJob.Dictionary}\\{downloadJob.Word.ToLower(CultureInfo.InvariantCulture)}.html"))
            {
                return;
            }
        }

        private List<string> GetWordsList(string fullFilePath)
        {
            List<string> wordsArray = File.ReadAllLines(fullFilePath, Encoding.UTF8).ToList();

            if (string.IsNullOrWhiteSpace(wordsArray.Last()))
                wordsArray.Remove(wordsArray.Last());

            return wordsArray;
        }

        public string PrepareDownloadUrl(string word, DictionariesEnum dictionary)
        {
            string rawUrl = GetDownloadString(dictionary);
            string urlToDownload = rawUrl.Replace(PlaceHolder, word);

            return urlToDownload;
        }

        public string GetDownloadString(DictionariesEnum dictionary)
        {
            // connect to the db and fetch the url

            switch (dictionary)
            {
                case DictionariesEnum.AmericanHeritage:
                    return "http://www.thefreedictionary.com/#WORD#";
                case DictionariesEnum.Vocabulary:
                    return "http://www.vocabulary.com/dictionary/#WORD#";
                case DictionariesEnum.OxfordAmerican:
                    return "http://www.oxforddictionaries.com/definition/american_english/#WORD#";
                default:
                    return "";
            } 
        }
    }
}

using System.IO;
using System.Net;
using System.Text;

namespace LogicLayerTests
{
    public class TestHelper
    {
        public string Download(string url)
        {
            using (var client = new WebClient())
            {
                var data = client.DownloadData(url);
                return Encoding.UTF8.GetString(data);
            }
        }

        public string OpenReadReturnHtmlString(string fileName, string folder = "files")
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            byte[] dataArray = File.ReadAllBytes($"{directory}/{folder}/{fileName}");

            return Encoding.UTF8.GetString(dataArray);
        }

        public Stream OpenReadReturnStream(string fileName, string folder = "files")
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            byte[] dataArray = File.ReadAllBytes($"{directory}/{folder}/{fileName}");

            return new MemoryStream(dataArray);
        }

    }
}
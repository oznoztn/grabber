using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicLayerTests
{
    [TestClass]
    public class Tools
    {
        private readonly TestHelper _testHelper = new TestHelper();

        public class Container
        {
            public string Word { get; set; }
            public string Definition { get; set; }
            public string Type { get; set; }

        }

        [TestMethod]
        public void ReadDirectoriesAndDeleteFilesWithUnknownChars()
        {
            var filesContainsUnknowChar = new List<string>();

            var dirs = Directory.GetDirectories(@"C:\Files\Downloads");
            foreach (var dir in dirs)
            {
                var subDirs = Directory.GetDirectories(dir);

                foreach (var subDir in subDirs)
                {
                    var files = Directory.GetFiles(subDir);

                    foreach (var file in files)
                    {
                        if (file.Contains("�"))
                            filesContainsUnknowChar.Add(file);
                    }
                }
            }

            foreach (var containsUnknowChar in filesContainsUnknowChar)
            {
                File.Delete(containsUnknowChar);
            }
        }

        [TestMethod]
        public List<string> ReadTextFilesAndDetectFileWithUnknownChar()
        {
            var brokenWords = new List<string>();

            string directoryToLookup = @"C:\Files\word_lists";

            string[] listPaths = Directory.GetFiles(directoryToLookup);

            foreach (var listPath in listPaths)
            {
                string[] wordsArray = File.ReadAllLines(listPath, Encoding.UTF8);

                foreach (var word in wordsArray)
                {
                    if (word.Contains("�"))
                        brokenWords.Add($"{word}-{listPath}");
                }
            }

            return brokenWords;
        }
    }
}

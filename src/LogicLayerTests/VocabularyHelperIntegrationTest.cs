using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GDomain;
using LogicLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicLayerTests
{
    [TestClass]
    public class VocabularyHelperIntegrationTest
    {
        private readonly TestHelper _testHelper = new TestHelper();

        public VocabularyHelper Create(string html)
        {
            return new VocabularyHelper(html);
        }

        private string Download(string url)
        {
            using (var client = new WebClient())
            {
                var data = client.DownloadData(url);
                return Encoding.UTF8.GetString(data);
            }
        }

        [TestMethod]
        public void Populate_WordLove_FullCoverage()
        {
            var html = Download("https://www.vocabulary.com/dictionary/love");
            var helper = Create(html);
            VocabularyWord word = helper.Populate();

            // Grup 1, 3. Meaning'in 1 tane Sense'i var.

            // 2 tane grup var
            Assert.AreEqual(2, word.FullMeaningGroups.Count);

            // 1. grupta 7 tane anlam, 2. grupta 1 tane anlam
            Assert.AreEqual(7, word.FullMeaningGroups[0].Meanings.Count); 
            Assert.AreEqual(1, word.FullMeaningGroups[1].Meanings.Count);

            // 1. grupta toplam 2 tane Sense olmalı
            Assert.AreEqual(2, word.FullMeaningGroups.First().Meanings.Count(meaning => meaning.SubMeanings.Any()));

            // 1. grup 1. meaning text: "a strong positive emotion of regard and affection"
            // 2. grup 1. meaning text: "a score of zero in tennis or squash"
            Assert.AreEqual("a strong positive emotion of regard and affection", word.FullMeaningGroups.First().Meanings.First().Text);
            Assert.AreEqual("a score of zero in tennis or squash", word.FullMeaningGroups.Last().Meanings.First().Text);

            // 1. grup 3. meaning'de 1 tane Sense olmalı ve bu Sense text olmalı: "a beloved person; used as terms of endearment"
            Assert.AreEqual(1, word.FullMeaningGroups.First().Meanings[3 - 1].SubMeanings.Count);
            Assert.AreEqual("a beloved person; used as terms of endearment", word.FullMeaningGroups.First().Meanings[3 - 1].SubMeanings.First().Text);

            // 1. gruptaki herhangi bir Meaning, Sense barındırıyor
            // 2. grupta Sense barındıran Meaning yok
            Assert.IsTrue(word.FullMeaningGroups.First().Meanings.Any(meaning => meaning.SubMeanings.Any()));
            Assert.IsFalse(word.FullMeaningGroups.Last().Meanings.Any(meaning => meaning.SubMeanings.Any()));

            // Primary Meaning Section var.
            //  2 grup
            //     1. grupta 2 meaning
            //       1. n tipinde "a strong positive emotion of regard and affection"
            //       2. v tipinde "have a great affection or liking for"
            //     2. grupta 1 meaning "a score of zero in tennis or squash"
            Assert.IsTrue(helper.HasPrimaryMeaningsSection());
            Assert.AreEqual(2, helper.GetPrimaryMeaningGroups().Count);
            Assert.AreEqual(2, helper.GetPrimaryMeaningGroups().First().Meanings.Count);

            Assert.AreEqual("n", word.PrimaryMeaningGroups[0].Meanings[0].Type);
            Assert.AreEqual("v", word.PrimaryMeaningGroups[0].Meanings[1].Type);
            Assert.AreEqual("a strong positive emotion of regard and affection", word.PrimaryMeaningGroups[0].Meanings[0].Text);
            Assert.AreEqual("have a great affection or liking for", word.PrimaryMeaningGroups[0].Meanings[1].Text);
            Assert.AreEqual("n", word.PrimaryMeaningGroups[1].Meanings.First().Type);
            Assert.AreEqual("a score of zero in tennis or squash", word.PrimaryMeaningGroups[1].Meanings.First().Text);

        }

        [TestMethod]
        public void Populate_WordUxoricide_FullCoverage()
        {
            var html = Download("https://www.vocabulary.com/dictionary/uxoricide");
            var helper = Create(html);

            VocabularyWord word = helper.Populate();

            // 1 anlam grubu
            // 2 farklı anlam
            //   hiç birinde sense yok
            //   hiç birinde örnek cümle yok

            // Primary meanings bölümü yok

            Assert.AreEqual("uxoricide", word.Text);

            Assert.AreEqual(1, word.FullMeaningGroups.Count);

            Assert.AreEqual(2, word.FullMeaningGroups.First().Meanings.Count);

            Assert.AreEqual("the murder of a wife by her husband", word.FullMeaningGroups.First().Meanings[0].Text);
            Assert.AreEqual("a husband who murders his wife", word.FullMeaningGroups.First().Meanings[1].Text);

            Assert.AreEqual(0, word.FullMeaningGroups.First().Meanings[0].Illustrations.Count);
            Assert.AreEqual(0, word.FullMeaningGroups.First().Meanings[1].Illustrations.Count);


            Assert.IsTrue(word.ShortBlurb.StartsWith("Uxoricide is a fancy way"));
            Assert.IsTrue(word.ShortBlurb.EndsWith("criminal trial."));

            Assert.IsTrue(word.LongBlurb.StartsWith("Through history"));
            Assert.IsTrue(word.LongBlurb.EndsWith("\"cutter, killer, or slayer.\""));
        }

        public void Populate_WordPlay_FullCoverage()
        {
            string html = _testHelper.OpenReadReturnHtmlString("vocabulary.com_play.html", "files");
            VocabularyHelper helper = Create(html);
            VocabularyWord word = helper.Populate();
        }
    }
}

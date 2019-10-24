using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDomain;
using HtmlAgilityPack;
using LogicLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicLayerTests
{
    
    [TestClass]
    public class AmericanHeritageTests
    {
        private readonly TestHelper _testHelper = new TestHelper();

        public AmericanHeritageHelper CreateAmericanHeritageHelper(string html)
        {
            return new AmericanHeritageHelper(html);
        }

        [TestMethod]
        public void GetIllustrations_2IllustrationsSeperatedBySemicolon_FirstItemShouldEndWithDot()
        {
            string html = "<div class='ds-list'>" +
                          "<font><i>a love of language; love for the game of golf.</i></font>" +
                          "</div>";
            AmericanHeritageHelper help = CreateAmericanHeritageHelper(html);
            HtmlNode node = HtmlNode.CreateNode(html);

            List<string> resultList = help.GetIllustrations(node);

            Assert.AreEqual(resultList[0], "a love of language.");
            Assert.AreEqual(resultList[1], "love for the game of golf.");
        }

        [TestMethod]
        public void GetIllustrations_3IllustrationsSeperatedBySemicolon_FirstTwoItemsShouldEndWithDot()
        {
            string html =
                "<div class='ds-list'><font><i>a love of language; love for the game of golf; a night of love.</i></font><div>";
            AmericanHeritageHelper help = CreateAmericanHeritageHelper(html);
            HtmlNode node = HtmlNode.CreateNode(html);

            // Act
            List<string> resultList = help.GetIllustrations(node);

            // Assert
            Assert.AreEqual(resultList[0], "a love of language.");
            Assert.AreEqual(resultList[1], "love for the game of golf.");
            Assert.AreEqual(resultList[2], "a night of love.");
        }


        [TestMethod]
        public void GetIllustrations_2IllustrationsSeperatedByDot_ReturnsAListOf2Items()
        {
            string html = "<div class='ds-list'>" +
                          "<font><i>We love our parents. I love my friends.</i></font>" +
                          "</div>";

            AmericanHeritageHelper help = CreateAmericanHeritageHelper(html);
            HtmlNode node = HtmlNode.CreateNode(html);

            var result = help.GetIllustrations(node);

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void GetIllustrations_MeaningSubsenseContainsIllustration_MeaningShouldHaveNoIllustration()
        {
            string html = "<div class='ds-list'>" +
                          "<div class='sds-list'>" +
                          "<span class='illustration'>We love our parents. I love my friends.</span>" +
                          "</div>" +
                          "</div>";

            AmericanHeritageHelper help = CreateAmericanHeritageHelper(html);
            HtmlNode node = HtmlNode.CreateNode(html);

            var meaningIllustrationsCount = help.GetIllustrations(node).Count;

            Assert.AreEqual(0, meaningIllustrationsCount);
        }

        [TestMethod]
        public void GetIllustrations_MeaningAndSubsenseContainsIllustration_OnlyTheMeaningsIllustrationShouldBeCounted()
        {
            string html = "<div class='ds-list'>" +
                          "<font><i>We love our parents.</i></font>" +
                          "<div class='sds-list'>" +
                          "<font><i>I love my friends.</i></font>" +
                          "</div>" +
                          "</div>";

            AmericanHeritageHelper help = CreateAmericanHeritageHelper(html);
            HtmlNode node = HtmlNode.CreateNode(html);

            var meaningIllustrationsCount = help.GetIllustrations(node).Count;

            Assert.AreEqual(1, meaningIllustrationsCount);
        }

        [TestMethod]
        public void GetDsList_HtmlContainsOneSection3Dslist_ShouldReturnDsListOf3Items()
        {
            string html = "<div id='results'>" +
                          "<table>" +
                          "<div class='pseg'>" +
                          "<div class='ds-list'> </div>" +
                          "<div class='ds-list'> </div>" +
                          "<div class='ds-list'> </div>" +
                          "</div>" +
                          "</table>" +
                          "</div>";

            var meaningGroups = CreateAmericanHeritageHelper(html).GetDsListsGroupedV2().First().ToList();
            var dsLists = meaningGroups.First();

            Assert.AreEqual(dsLists.Count, 3);

        }

        [TestMethod]
        public void GetWord_ValidHtml_ShouldReturnDemon()
        {
            string html = "<html><title>American Heritage Dictionary Entry: demon</title></html>";

            AmericanHeritageHelper tfd = CreateAmericanHeritageHelper(html);

            var word = tfd.GetWord();

            Assert.AreEqual("demon", word);
        }

        [TestMethod]
        public void GetWord_InvalidHtml_ShouldReturnEmptyString()
        {
            string html = "<html></html>";

            AmericanHeritageHelper tfd = CreateAmericanHeritageHelper(html);

            var word = tfd.GetWord();

            Assert.AreEqual("", word);
        }

        [TestMethod]
        public void GetWord_InvalidHtmlContainsMultipleTitleCausesException_ShouldReturnExceptionMessage()
        {
            string html = "<html>" +
                          "<title>Demon - definition of demon by The Free Dictionary</title>" +
                          "<title>Demon - definition of demon by The Free Dictionary</title>" +
                          "</html>";

            AmericanHeritageHelper tfd = CreateAmericanHeritageHelper(html);

            var word = tfd.GetWord();

            Assert.AreEqual(word, "Sequence contains more than one element");
        }



        [TestMethod]
        public void GetTextComponents_ContainsBoldAndItalic_ShouldContainOrdinalContextMeaningInOrder()
        {
            string html = "<div id='results'>" +
                              "<table>" +
                              "<div class='pseg'>" +
                                  "<div class='ds-list'>" +
                                  "<b>1.</b> " +
                                  "<i>Context</i>" +
                                  "Plain meaning text." +
                                  "</div>" +
                              "</div>" +
                              "</table>" +
                          "</div>";

            var helper = CreateAmericanHeritageHelper(html);

            List<List<HtmlNode>> meaningGroups = helper.GetDsListsGroupedV2().First().ToList();
            List<HtmlNode> dsLists = meaningGroups.First();

            List<TextComponent> component = helper.GetTextComponents(dsLists.First());

            Assert.AreEqual(component[0].Style, "b");
            Assert.AreEqual(component[1].Style, "i");
            Assert.AreEqual(component[2].Style, "text");
        }

        [TestMethod]
        public void GetTextComponents_ContainsDiv_ShouldntCountDivs()
        {
            string html = "<div id='results'>" +
                              "<table>" +
                                  "<div class='pseg'>" +
                                      "<div class='ds-list'>" +
                                      "<b>1.</b> " +
                                      "<i>Context</i>" +
                                      "Plain meaning text." +
                                      "<div class='sds-list'> </div>" +
                                      "</div>" +
                                  "</div>" +
                              "</table>" +
                          "</div>";

            var helper = CreateAmericanHeritageHelper(html);
            var meaningGroups = CreateAmericanHeritageHelper(html).GetDsListsGroupedV2().First().ToList();
            var dsLists = meaningGroups.First();
            var dsList = dsLists.First();

            var components = helper.GetTextComponents(dsList);

            Assert.AreEqual(components.Count, 3);

            Assert.AreEqual(components[0].Style, "b");
            Assert.AreEqual(components[1].Style, "i");
            Assert.AreEqual(components[2].Style, "text");
        }

        [TestMethod]
        public void GetTextComponents_ContainsDsListThatHasContextAndSdsList_ShouldReturnBoldAndItalic()
        {
            string hx = "<div id='results'>" +
                        "<table>" +
                        "<div class='pseg'>" +
                        "<div class='ds-list'> " +
                        "<b>1.</b> " +
                        "<i>Italic Context Text</i>" +
                        "<div class='sds-list'></div>" +
                        "<div class='sds-list'></div>" +
                        "</div>" +
                        "</div>" +
                        "</table>" +
                        "</div>";

            var helper = CreateAmericanHeritageHelper(hx);
            var meaningGroups = helper.GetDsListsGroupedV2().First().ToList();
            var dsLists = meaningGroups.First();
            var dsList = dsLists.First();

            var components = helper.GetTextComponents(dsList);

            Assert.AreEqual(components.Count, 2);
        }

        [TestMethod]
        public void GetTextComponents_MeaningContainsAnIllustration_IllustrationComponentShouldntBeInTheReturnList()
        {
            /*
             6. Music (italic)
                a. To perform on an instrument: play on an accordion.
                b. To emit sound or be sounded in performance: The band is playing.
            */

            var html = _testHelper.OpenReadReturnHtmlString("ah_play.html");
            var helper = CreateAmericanHeritageHelper(html);

            var meaningDsLists = helper.GetDsListsGroupedV2().First().ToList().First();
            var sixthMeanings = meaningDsLists[5];
            var firstSubMeaning = sixthMeanings.ChildNodes.First(node => node.Name == "div");

            List<TextComponent> components = helper.GetTextComponents(firstSubMeaning);
            bool result = components.Any(comp => comp.Text == "play on an accordion.");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Populate_Test()
        {
            /*             
                1.
                    a. Bible In the book of Revelation, the place of the gathering of armies for the final battle before the end of the world.
                    b. The battle involving these armies.
                2. A decisive or catastrophic conflict.             
             */
            string html = _testHelper.OpenReadReturnHtmlString("ah_armageddon.html", "files");
            var helper = CreateAmericanHeritageHelper(html);

            List<AmericanHeritageWord> discreteMeaningWords = helper.Populate();
            AmericanHeritageWord word = discreteMeaningWords.First();

            var nounGroup = word.Groups.First();

            Assert.AreEqual("n.", nounGroup.Type);

            Assert.AreEqual("armageddon", word.Text);

            Assert.AreEqual(2, nounGroup.Meanings.Count);
            Assert.AreEqual(0, nounGroup.Meanings.Last().SubMeanings.Count);

            Assert.AreEqual(0, nounGroup.Meanings[0].Illustrations.Count);
            Assert.AreEqual(0, nounGroup.Meanings[1].Illustrations.Count);

            Assert.AreEqual(0, nounGroup.Meanings[0].Illustrations.Count);
            Assert.AreEqual(0, nounGroup.Meanings[1].Illustrations.Count);

            Assert.AreEqual("", nounGroup.Meanings.First().Text);
            Assert.AreEqual("A decisive or catastrophic conflict.", nounGroup.Meanings.Last().Text);

            Assert.AreEqual("In the book of Revelation, the place of the gathering of armies for the final battle before the end of the world.", nounGroup.Meanings[0].SubMeanings[0].Text);
            Assert.AreEqual("The battle involving these armies.", nounGroup.Meanings[0].SubMeanings[1].Text);

            Assert.AreEqual("Bible", nounGroup.Meanings.First().SubMeanings.First().Context);
        }

        [TestMethod]
        public void SetContextSense_MeaningHasSenseRegisterButNotContext()
        {
            var html = "<div class='ds-list'><b>1.</b><i>Informal</i>meaning text</div>";
            var helper = CreateAmericanHeritageHelper("");

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var dsList = doc.DocumentNode.FirstChild;
            var meaning = helper.ConstructMeaning(dsList);

            Assert.AreEqual("", meaning.Context);
            Assert.AreEqual("Informal", meaning.SenseRegister);
        }

        [TestMethod]
        public void SetContextSense_MeaningHasContextButNotSenseRegister()
        {
            var html = "<div class='ds-list'><b>1.</b><i>Computing</i>meaning text</div>";
            var helper = CreateAmericanHeritageHelper("");

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var dsList = doc.DocumentNode.FirstChild;
            var meaning = helper.ConstructMeaning(dsList);

            Assert.AreEqual("", meaning.SenseRegister);
            Assert.AreEqual("Computing", meaning.Context);
        }

        [TestMethod]
        public void GetIdioms_PlainIdiomSingleDefinitionNoIllustration()
        {
            var html =
                "<div id='results'>" +
                "<table>" +
                "<div class='idmseg'>" +
                "<b><i>idiom_text</i></b>" +
                "<div class='ds-single'>Idiom_definition</div>" +
                "</div>" +
                "</table>" +
                "</div>";

            var helper = CreateAmericanHeritageHelper(html);

            Word idiom = helper.GetIdioms().First();

            Assert.AreEqual(idiom.Text, "idiom_text");
            Assert.AreEqual(1, idiom.Meanings.Count);

            Assert.AreEqual("Idiom_definition", idiom.Meanings.First().Text);
            Assert.AreEqual(0, idiom.Meanings.First().Illustrations.Count);
        }

        [TestMethod]
        public void ConstructMeaning_FullCoverage_Case1()
        {
            /*
              2.
                a. To take part in a sport or game: He's just a beginner and doesn't play well.
                b. To participate in betting; gamble.
             */
            string html = _testHelper.OpenReadReturnHtmlString("ah_play.html");
            var helper = CreateAmericanHeritageHelper(html);

            var meaningGroups = helper.GetDsListsGroupedV2().First().ToList();
            var firstGroupOfDsLists = meaningGroups.First();
            var secondMeaning = firstGroupOfDsLists[1];

            var meaning = helper.ConstructMeaning(secondMeaning);

            Assert.AreEqual(2, meaning.SubMeanings.Count);
            Assert.AreEqual("To take part in a sport or game.", meaning.SubMeanings.First().Text);
            Assert.AreEqual("To participate in betting; gamble.", meaning.SubMeanings.Last().Text);

            Assert.AreEqual(1, meaning.SubMeanings.First().Illustrations.Count);
            Assert.AreEqual(0, meaning.SubMeanings.Last().Illustrations.Count);

            Assert.AreEqual("", meaning.Context);
            Assert.AreEqual("", meaning.SenseRegister);
            Assert.AreEqual("", meaning.GrammaticalNote);
            Assert.AreEqual("", meaning.Type);
            Assert.AreEqual("", meaning.UsageForm);

            Assert.AreEqual("", meaning.SubMeanings.First().Context);
            Assert.AreEqual("", meaning.SubMeanings.First().SenseRegister);
            Assert.AreEqual("", meaning.SubMeanings.First().GrammaticalNote);
            Assert.AreEqual("", meaning.SubMeanings.First().Type);
            Assert.AreEqual("", meaning.SubMeanings.First().UsageForm);

            Assert.AreEqual("", meaning.SubMeanings.Last().Context);
            Assert.AreEqual("", meaning.SubMeanings.Last().SenseRegister);
            Assert.AreEqual("", meaning.SubMeanings.Last().GrammaticalNote);
            Assert.AreEqual("", meaning.SubMeanings.Last().Type);
            Assert.AreEqual("", meaning.SubMeanings.Last().UsageForm);

            Assert.AreEqual("He's just a beginner and doesn't play well.", meaning.SubMeanings.First().Illustrations.First().Text);
        }

        [TestMethod]
        public void ConstructMeaning_FullCoverage_Case2()
        {
            /*
                6. Music
                    a. To perform on an instrument: play on an accordion.
                    b. To emit sound or be sounded in performance: The band is playing.       
            */

            string html = _testHelper.OpenReadReturnHtmlString("ah_play.html");
            var helper = CreateAmericanHeritageHelper(html);

            var meaningGroups = helper.GetDsListsGroupedV2().First().ToList();
            var firstGroupOfDsLists = meaningGroups.First(); // ds-lists
            var secondMeaning = firstGroupOfDsLists[5];

            AmericanHeritageMeaning meaning = helper.ConstructMeaning(secondMeaning);

            Assert.AreEqual(2, meaning.SubMeanings.Count);
            Assert.AreEqual("To perform on an instrument.", meaning.SubMeanings.First().Text);
            Assert.AreEqual("To emit sound or be sounded in performance.", meaning.SubMeanings.Last().Text);

            Assert.AreEqual(1, meaning.SubMeanings.First().Illustrations.Count);
            Assert.AreEqual(1, meaning.SubMeanings.Last().Illustrations.Count);

            Assert.AreEqual("Music", meaning.Context);
            Assert.AreEqual("", meaning.SenseRegister);
            Assert.AreEqual("", meaning.GrammaticalNote);
            Assert.AreEqual("", meaning.Type);
            Assert.AreEqual("", meaning.UsageForm);

            Assert.AreEqual("", meaning.SubMeanings.First().Context);
            Assert.AreEqual("", meaning.SubMeanings.First().SenseRegister);
            Assert.AreEqual("", meaning.SubMeanings.First().GrammaticalNote);
            Assert.AreEqual("", meaning.SubMeanings.First().Type);
            Assert.AreEqual("", meaning.SubMeanings.First().UsageForm);

            Assert.AreEqual("", meaning.SubMeanings.Last().Context);
            Assert.AreEqual("", meaning.SubMeanings.Last().SenseRegister);
            Assert.AreEqual("", meaning.SubMeanings.Last().GrammaticalNote);
            Assert.AreEqual("", meaning.SubMeanings.Last().Type);
            Assert.AreEqual("", meaning.SubMeanings.Last().UsageForm);

            Assert.AreEqual("play on an accordion.", meaning.SubMeanings.First().Illustrations.First().Text);
            Assert.AreEqual("The band is playing.", meaning.SubMeanings.Last().Illustrations.First().Text);
        }

        [TestMethod]
        public void ConstructMeaning_FullCoverage_Case3()
        {
            /*
                23. Informal To urinate or defecate: I left the meeting early because I really had to go!
             */

            string html = _testHelper.OpenReadReturnHtmlString("ah_go.html");
            var helper = CreateAmericanHeritageHelper(html);

            var meaningGroups = helper.GetDsListsGroupedV2().First().ToList();
            var firstGroupOfDsLists = meaningGroups.First();

            var secondMeaning = firstGroupOfDsLists[22];

            AmericanHeritageMeaning meaning = helper.ConstructMeaning(secondMeaning);

            Assert.AreEqual("To urinate or defecate.", meaning.Text);

            Assert.AreEqual(0, meaning.SubMeanings.Count);

            Assert.AreEqual("", meaning.Context);
            Assert.AreEqual("Informal", meaning.SenseRegister);
            Assert.AreEqual("", meaning.GrammaticalNote);
            Assert.AreEqual("", meaning.Type);
            Assert.AreEqual("", meaning.UsageForm);

            Assert.AreEqual("I left the meeting early because I really had to go!", meaning.Illustrations.First().Text);
        }

        [TestMethod]
        public void DetectGroupText_Meaning_has_group_type_text_and_other_stuffs()
        {
            var html =
                "<div id='results'>" +
                "<table>" +
                "<div class='pseg'>" +
                "<i>n.</i><i>pl.</i><b>fruit</b>or<b>fruits</b>" +
                "<div class='ds-list'>" +
                "</div>" +
                "</div>" +
                "</table>" +
                "</div>";

            var helper = CreateAmericanHeritageHelper(html);

            var dsList = helper.GetDsListsGroupedV2().First().First().First();
            string result = helper.DetectGroupText(dsList);

            Assert.AreEqual("n. pl. fruit or fruits", result);
        }

        [TestMethod]
        public void DetectGroupText_MeaningHasOnlyGroupType()
        {
            var html =
                "<div id='results'><table>" +
                "<div class='pseg'>" +
                "<i>n.</i>" +
                "<div class='ds-list'>" +
                "</div>" +
                "</div></table>" +
                "</div>";

            var helper = CreateAmericanHeritageHelper(html);

            var dsList = helper.GetDsListsGroupedV2().First().First().First();
            string result = helper.DetectGroupText(dsList);

            Assert.AreEqual("n.", result);
        }
        
        [TestMethod]
        public void DefinitionTextHasNote_ExtraInformationsAboutTheMeaningShouldntBeInMeaningTextPropertyButUsageFormProperty_1()
        {
            string html = "<div id='results'>" +
                              "<table>" +
                                  "<div class='pseg'>" +
                                     "<i>n.</i>" +
                                      "<div class='ds-list'>" +
                                      "<b>1.</b> " +
                                      "<i>Context</i>" +
                                      "Plain meaning text." +
                                      "<div class='sds-list'>often Go The starting point:</div>" +
                                      "</div>" +
                                  "</div>" +
                              "</table>" +
                          "</div>";

            var helper = CreateAmericanHeritageHelper(html);
            var word = helper.Populate().First();

            var theSub = word.Groups.First().Meanings.First().SubMeanings.First();

            Assert.AreEqual("The starting point.", theSub.Text);
            Assert.AreEqual("often Go", theSub.UsageForm);
        }

        [TestMethod]
        public void DefinitionTextHasNote_ExtraInformationsAboutTheMeaningShouldntBeInMeaningTextPropertyButUsageFormProperty_2()
        {
            string html = "<div id='results'>" +
                              "<table>" +
                                  "<div class='pseg'>" +
                                     "<i>n.</i>" +
                                      "<div class='ds-list'>" +
                                      "<b>1.</b> " +
                                      "<i>Context</i>" +
                                      "Plain meaning text." +
                                      "<div class='sds-list'>often often Go The starting point:</div>" +
                                      "</div>" +
                                  "</div>" +
                              "</table>" +
                          "</div>";

            var helper = CreateAmericanHeritageHelper(html);
            var word = helper.Populate().First();

            var theSub = word.Groups.First().Meanings.First().SubMeanings.First();

            Assert.AreEqual("The starting point.", theSub.Text);
            Assert.AreEqual("often often Go", theSub.UsageForm);
        }
    }



}

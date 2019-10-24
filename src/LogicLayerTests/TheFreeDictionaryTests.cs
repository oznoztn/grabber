using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using GDomain;
using HtmlAgilityPack;
using LogicLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicLayerTests
{
    [TestClass]
    public class TheFreeDictionaryHelperTests
    {
        private readonly TestHelper _testHelper = new TestHelper();

        public TheFreeDictionaryHelper CreateTheFreeDictionaryHelper(string html)
        {
            return new TheFreeDictionaryHelper(html);
        }

        [TestMethod]
        public void GetIllustrations_2IllustrationsSeperatedBySemicolon_FirstItemShouldEndWithDot()
        {
            string html = "<div class='ds-list'>" +
                          "<span class='illustration'>a love of language; love for the game of golf.</span>" +
                          "</div>";
            TheFreeDictionaryHelper help = CreateTheFreeDictionaryHelper(html);
            HtmlNode node = HtmlNode.CreateNode(html);

            List<string> resultList = help.GetIllustrations(node);

            Assert.AreEqual(resultList[0], "a love of language.");
            Assert.AreEqual(resultList[1], "love for the game of golf.");
        }

        [TestMethod]
        public void GetIllustrations_3IllustrationsSeperatedBySemicolon_FirstTwoItemsShouldEndWithDot()
        {
            string html =
                "<div class='ds-list'><span class='illustration'>a love of language; love for the game of golf; a night of love.</span><div>";
            TheFreeDictionaryHelper help = CreateTheFreeDictionaryHelper(html);
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
                          "<span class='illustration'>We love our parents. I love my friends.</span>" +
                          "</div>";

            TheFreeDictionaryHelper help = CreateTheFreeDictionaryHelper(html);
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

            TheFreeDictionaryHelper help = CreateTheFreeDictionaryHelper(html);
            HtmlNode node = HtmlNode.CreateNode(html);

            var meaningIllustrationsCount = help.GetIllustrations(node).Count;

            Assert.AreEqual(0, meaningIllustrationsCount);
        }

        [TestMethod]
        public void GetIllustrations_MeaningAndSubsenseContainsIllustration_OnlyTheMeaningsIllustrationShouldBeCounted()
        {
            string html = "<div class='ds-list'>" +
                              "<span class='illustration'>We love our parents.</span>" +
                              "<div class='sds-list'>" +
                                  "<span class='illustration'>I love my friends.</span>" +
                              "</div>" +
                          "</div>";

            TheFreeDictionaryHelper help = CreateTheFreeDictionaryHelper(html);
            HtmlNode node = HtmlNode.CreateNode(html);

            var meaningIllustrationsCount = help.GetIllustrations(node).Count;

            Assert.AreEqual(1, meaningIllustrationsCount);
        }

        [TestMethod]
        public void GetDsList_HtmlContainsOneSection3Dslist_ShouldReturnDsListOf3Items()
        {
            string html = "<section data-src='hm'>" +
                          "<div class='pseg'>" +
                          "<div class='ds-list'> </div>" +
                          "<div class='ds-list'> </div>" +
                          "<div class='ds-list'> </div>" +
                          "</div>" +
                          "</section>";

            var meaningGroups = CreateTheFreeDictionaryHelper(html).GetDsListsGrouped(DictionariesEnum.AmericanHeritage).ToList();
            var dsLists = meaningGroups.First();

            Assert.AreEqual(dsLists.Count, 3);

        }

        [TestMethod]
        public void GetDsList_HtmlContainsTwoSections_ShouldReturnDsListOfGiveSectionOnly()
        {
            string html = "<section data-src='hm'>" +
                          "<div class='pseg'>" +
                          "<div class='ds-list'> </div>" +
                          "</section>" +
                          "</div>" +
                          "<section data-src='rHouse'>" +
                          "<div class='pseg'>" +
                          "<div class='ds-list'> </div>" +
                          "<div class='ds-list'> </div>" +
                          "</div>" +
                          "</section>";

            var meaningGroups = CreateTheFreeDictionaryHelper(html).GetDsListsGrouped(DictionariesEnum.RandomHouseKernermanWebster).ToList();
            var dsLists = meaningGroups.First();

            Assert.AreEqual(dsLists.Count, 2);
        }

        [TestMethod]
        public void GetDsList_HtmlContainsNoMatcingSection_ShouldReturnEmptyList()
        {
            string html = "<section data-src='someSection'>" +
                              "<div class='pseg'>" +
                                  "<div class='ds-list'></div>" +
                              "</div>" +
                          "</section>";

            var meaningGroups = CreateTheFreeDictionaryHelper(html).GetDsListsGrouped(DictionariesEnum.AmericanHeritage).ToList();
            var dsLists = meaningGroups.FirstOrDefault();

            Assert.IsNull(dsLists);
        }

        [TestMethod]
        public void GetWord_ValidHtml_ShouldReturnDemon()
        {
            string html = "<html><title>Demon - definition of demon by The Free Dictionary</title></html>";

            TheFreeDictionaryHelper tfd = CreateTheFreeDictionaryHelper(html);

            var word = tfd.GetWord();

            Assert.AreEqual("Demon", word);
        }

        [TestMethod]
        public void GetWord_InvalidHtml_ShouldReturnEmptyString()
        {
            string html = "<html></html>";

            TheFreeDictionaryHelper tfd = CreateTheFreeDictionaryHelper(html);

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

            TheFreeDictionaryHelper tfd = CreateTheFreeDictionaryHelper(html);

            var word = tfd.GetWord();

            Assert.AreEqual(word, "Sequence contains more than one element");
        }



        [TestMethod]
        public void GetTextComponents_ContainsBoldAndItalic_ShouldContainOrdinalContextMeaningInOrder()
        {
            string html = "<section data-src='hm'><div class='pseg'>" +
                            "<div class='ds-list'>" +
                                "<b>1.</b> " +
                                "<i>Context</i>" +
                                "Plain meaning text." +
                            "</div>" +
                        "</div></section>";

            var helper = CreateTheFreeDictionaryHelper(html);

            List<List<HtmlNode>> meaningGroups = helper.GetDsListsGrouped(DictionariesEnum.AmericanHeritage).ToList();
            List<HtmlNode> dsLists = meaningGroups.First();

            List<TextComponent> component = helper.GetTextComponents(dsLists.First());

            Assert.AreEqual(component[0].Style, "b");
            Assert.AreEqual(component[1].Style, "i");
            Assert.AreEqual(component[2].Style, "text");
        }

        [TestMethod]
        public void GetTextComponents_ContainsDiv_ShouldntCountDivs()
        {
            string html = "<section data-src='hm'><div class='pseg'>" +
                            "<div class='ds-list'>" +
                                "<b>1.</b> " +
                                "<i>Context</i>" +
                                "Plain meaning text." + 

                                "<div class='sds-list'>" +
                                "</div>" +
                            "</div>" +
                        "</div></section>";

            var helper = CreateTheFreeDictionaryHelper(html);
            var meaningGroups = CreateTheFreeDictionaryHelper(html).GetDsListsGrouped(DictionariesEnum.AmericanHeritage).ToList();
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
            string hx = "<section data-src='hm'><div class='pseg'>" +
                            "<div class='ds-list'> " +
                                "<b>1.</b> " +
                                "<i>Italic Context Text</i>" +
                                "<div class='sds-list'></div>" +
                                "<div class='sds-list'></div>" +
                            "</div>" +
                        "</div></section>";

            var helper = CreateTheFreeDictionaryHelper(hx);
            var meaningGroups = helper.GetDsListsGrouped(DictionariesEnum.AmericanHeritage).ToList();
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

            var html = _testHelper.OpenReadReturnHtmlString("tfd_play.html");
            var helper = CreateTheFreeDictionaryHelper(html);

            var meaningDsLists = helper.GetDsListsGrouped(DictionariesEnum.AmericanHeritage).ToList().First();
            var sixthMeanings = meaningDsLists[5];
            var firstSubMeaning = sixthMeanings.ChildNodes.First(node => node.Name == "div");

            List<TextComponent> components = helper.GetTextComponents(firstSubMeaning);
            bool result = components.Any(comp => comp.Text == "play on an accordion.");

            Assert.IsFalse(result);
        }
        
        [Ignore]
        [TestMethod]
        public void Populate_Test()
        {
            /*             
                1.
                    a. Bible In the book of Revelation, the place of the gathering of armies for the final battle before the end of the world.
                    b. The battle involving these armies.
                2. A decisive or catastrophic conflict.             
             */
            string html = _testHelper.OpenReadReturnHtmlString("tfd_armageddon.html", "files");
            var helper = CreateTheFreeDictionaryHelper(html);

            Word word = helper.Populate(DictionariesEnum.AmericanHeritage);
            var nounGroup = word.Groups.First();

            Assert.AreEqual("n.", nounGroup.Type);

            Assert.AreEqual("Armageddon", word.Text);
            
            Assert.AreEqual(2, nounGroup.Meanings.Count);
            Assert.AreEqual(0, nounGroup.Meanings.Last().SubMeanings.Count);
                               
            Assert.AreEqual(0, nounGroup.Meanings[0].Illustrations.Count);
            Assert.AreEqual(0, nounGroup.Meanings[1].Illustrations.Count);
                               
            Assert.AreEqual(0, nounGroup.Meanings[0].Illustrations.Count);
            Assert.AreEqual(0, nounGroup.Meanings[1].Illustrations.Count);

            Assert.AreEqual("1.", nounGroup.Meanings.First().Text);
            Assert.AreEqual("2. A decisive or catastrophic conflict.", nounGroup.Meanings.Last().Text);

            Assert.AreEqual("Bible In the book of Revelation, the place of the gathering of armies for the final battle before the end of the world.", nounGroup.Meanings[0].SubMeanings[0].Text);
            Assert.AreEqual("The battle involving these armies.", nounGroup.Meanings[0].SubMeanings[1].Text);

            Assert.AreEqual("Bible", nounGroup.Meanings.First().SubMeanings.First().Context);
        }

        [TestMethod]
        public void SetContextSense_MeaningHasSenseRegisterButNotContext()
        {
            var html = "<div class='ds-list'><b>1.</b><i>Informal</i>meaning text</div>";
            var helper = CreateTheFreeDictionaryHelper("");

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
            var helper = CreateTheFreeDictionaryHelper("");

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
                "<section data-src='hm'>" +
                    "<div class='idmseg'>" +
                        "<b><i>idiom_text</i></b>" +
                        "<div class='ds-single'>idiom_definition</div>" +
                    "</div>" +
                "</section>";

            var helper = CreateTheFreeDictionaryHelper(html);

            Word idiom = helper.GetIdioms(DictionariesEnum.AmericanHeritage).First();

            Assert.AreEqual(idiom.Text, "idiom_text");
            Assert.AreEqual(1, idiom.Meanings.Count);

            Assert.AreEqual("idiom_definition", idiom.Meanings.First().Text);
            Assert.AreEqual(0, idiom.Meanings.First().Illustrations.Count);
        }

        [TestMethod]
        public void GetIdioms_PlainIdiomSingleDefinitionHasSenseRegisterAndIllustration()
        {
            var html =
                "<section data-src='hm'>" +
                    "<div class='idmseg'>" +
                        "<b><i>idiom_text</i></b>" +
                        "<i>sense_register_informal</i>" +
                        "<div class='ds-single'>idiom_definition" +
                            "<span class='illustration'>illustration_text</span>" +
                        "</div>" +
                    "</div>" +
                "</section>";

            var helper = CreateTheFreeDictionaryHelper(html);

            Word idiom = helper.GetIdioms(DictionariesEnum.AmericanHeritage).First();

            Assert.AreEqual("sense_register_informal", idiom.Meanings.First().SenseRegister);

            Assert.AreEqual(1, idiom.Meanings.First().Illustrations.Count);
            Assert.AreEqual("illustration_text", idiom.Meanings.First().Illustrations.First().Text);
        }

        [TestMethod]
        public void GetIdioms_IdiomHasMoreThanOneDefinitions()
        {
            var html =
                "<section data-src='hm'>" +
                    "<div class='idmseg'>" +
                        "<b><i>idiom_text</i></b>" +
                        "<div class='ds-list'>" +
                            "<b>1.</b>" +
                            "sense_1" +
                            "<span class='illustration'>illustration_text1</span>" +
                        "</div>" +

                        "<div class='ds-list'>" +
                            "<b>2.</b>" +
                            "sense_2" +
                            "<span class='illustration'>illustration_text2</span>" +
                        "</div>" +

                        "<div class='ds-list'>" +
                            "<b>3.</b>" +
                            "sense_3" +
                            "<i>Informal</i>"+
                            "<span class='illustration'>illustration_text3</span>" +
                        "</div>" +
                    "</div>" +
                "</section>";

            var helper = CreateTheFreeDictionaryHelper(html);

            Word idiom = helper.GetIdioms(DictionariesEnum.AmericanHeritage).First();

            Assert.AreEqual("idiom_text", idiom.Text);

            Assert.AreEqual("sense_1", idiom.Meanings[0].Text);
            Assert.AreEqual("sense_2", idiom.Meanings[1].Text);
            Assert.AreEqual("sense_3", idiom.Meanings[2].Text);

            Assert.AreEqual("", idiom.Meanings[0].SenseRegister);
            Assert.AreEqual("", idiom.Meanings[1].SenseRegister);

            Assert.AreEqual("Informal", idiom.Meanings.Last().SenseRegister);
        }

        [TestMethod]
        public void GetIdioms_IdiomHasMeaningThatHasSubMeanings_ShouldProperlyReadSdsListItems()
        {
            var html =
                "<section data-src='hm'>" +
                    "<div class='idmseg'>" +
                        "<b><i>idiom_text</i></b>" +
                        "<div class='ds-list'>" +
                            "<b>1.</b>" +
                            "<i>First_Meaning_Context_Text</i>" +
                            "<div class='sds-list'><b>a.</b>Submeaning1_Text" +
                                "<span class='illustration'>submeaning_1_illustration</span>" +
                            "</div>" +
                            "<div class='sds-list'><b>b.</b>Submeaning2_Text" +
                                "<span class='illustration'>submeaning_2_illustration</span>" +
                            "</div>" +
                        "</div>" +

                        "<div class='ds-list'>" +
                            "<b>2.</b>" +
                            "sense_2" +
                            "<span class='illustration'>illustration_text2</span>" +
                        "</div>" +
                    "</div>" +
                "</section>";

            var helper = CreateTheFreeDictionaryHelper(html);

            Word idiom = helper.GetIdioms(DictionariesEnum.AmericanHeritage).First();

            Assert.AreEqual("idiom_text", idiom.Text);
            Assert.AreEqual("First_Meaning_Context_Text", idiom.Meanings.First().Context);

            Assert.AreEqual("Submeaning1_Text", idiom.Meanings.First().SubMeanings.First().Text);
            Assert.AreEqual("Submeaning2_Text", idiom.Meanings.First().SubMeanings.Last().Text);

            Assert.AreEqual("submeaning_1_illustration", idiom.Meanings.First().SubMeanings.First().Illustrations.First().Text);
            Assert.AreEqual("submeaning_2_illustration", idiom.Meanings.First().SubMeanings.Last().Illustrations.First().Text);
        }


        [TestMethod]
        public void ConstructMeaning_FullCoverage_Case1()
        {
            /*
              2.
                a. To take part in a sport or game: He's just a beginner and doesn't play well.
                b. To participate in betting; gamble.
             */
            string html = _testHelper.OpenReadReturnHtmlString("tfd_play.html");
            var helper = CreateTheFreeDictionaryHelper(html);

            var meaningGroups = helper.GetDsListsGrouped(DictionariesEnum.AmericanHeritage).ToList();
            var firstGroupOfDsLists = meaningGroups.First();
            var secondMeaning = firstGroupOfDsLists[1];            
            
            TheFreeDictionaryMeaning meaning = helper.ConstructMeaning(secondMeaning);

            Assert.AreEqual(2, meaning.SubMeanings.Count);
            Assert.AreEqual("To take part in a sport or game:", meaning.SubMeanings.First().Text);
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

            string html = _testHelper.OpenReadReturnHtmlString("tfd_play.html");
            var helper = CreateTheFreeDictionaryHelper(html);

            var meaningGroups = helper.GetDsListsGrouped(DictionariesEnum.AmericanHeritage).ToList();
            var firstGroupOfDsLists = meaningGroups.First(); // ds-lists
            var secondMeaning = firstGroupOfDsLists[5];

            TheFreeDictionaryMeaning meaning = helper.ConstructMeaning(secondMeaning);

            Assert.AreEqual(2, meaning.SubMeanings.Count);
            Assert.AreEqual("To perform on an instrument:", meaning.SubMeanings.First().Text);
            Assert.AreEqual("To emit sound or be sounded in performance:", meaning.SubMeanings.Last().Text);

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

            string html = _testHelper.OpenReadReturnHtmlString("tfd_go.html");
            var helper = CreateTheFreeDictionaryHelper(html);

            var meaningGroups = helper.GetDsListsGrouped(DictionariesEnum.AmericanHeritage).ToList();
            var firstGroupOfDsLists = meaningGroups.First();

            var secondMeaning = firstGroupOfDsLists[22];

            TheFreeDictionaryMeaning meaning = helper.ConstructMeaning(secondMeaning);

            Assert.AreEqual("To urinate or defecate:", meaning.Text);

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
                "<section data-src='hm'>" +
                "<div class='pseg'>" +
                "<i>n.</i><i>pl.</i><b>fruit</b>or<b>fruits</b>" +
                "<div class='ds-list'>" +
                "</div>" +
                "</div>" +
                "</section>";

            var helper = CreateTheFreeDictionaryHelper(html);

            var dsList = helper.GetDsListsGrouped(DictionariesEnum.AmericanHeritage).First().First();
            string result = helper.DetectGroupText(dsList);

            Assert.AreEqual("n. pl. fruit or fruits", result);
        }

        [TestMethod]
        public void DetectGroupText_MeaningHasOnlyGroupType()
        {
            var html =
                "<section data-src='hm'>" +
                    "<div class='pseg'>" +
                        "<i>n.</i>" +
                        "<div class='ds-list'>" +
                        "</div>" +
                    "</div>" +
                "</section>";

            var helper = CreateTheFreeDictionaryHelper(html);

            var dsList = helper.GetDsListsGrouped(DictionariesEnum.AmericanHeritage).First().First();
            string result = helper.DetectGroupText(dsList);

            Assert.AreEqual("n.", result);
        }
    }
}
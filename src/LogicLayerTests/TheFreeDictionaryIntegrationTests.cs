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
    public class TheFreeDictionaryIntegrationTests
    {
        private readonly TestHelper _testHelper = new TestHelper();

        public TheFreeDictionaryHelper CreateTheFreeDictionaryHelper(string html)
        {
            return new TheFreeDictionaryHelper(html);
        }

        [Ignore]
        [TestMethod]
        public void Do()
        {
            var helper = CreateTheFreeDictionaryHelper(_testHelper.OpenReadReturnHtmlString("tfd_sunstroke.html"));

            Word word = helper.Populate(DictionariesEnum.AmericanHeritage);

            var firstGroupFirstMeaning = word.Groups.First().Meanings.First();
            Assert.AreEqual("Heat stroke caused by exposure to the sun and characterized by a rise in temperature, convulsions, and coma. Also called insolation, siriasis.", firstGroupFirstMeaning.Text);
        }

        [TestMethod]
        public void CheckForCompletelyDifferentMeanings_AmericanHeritageHasNotCompletelyDifferentMeanings_ShouldReturnFalse()
        {
            var helper = CreateTheFreeDictionaryHelper(_testHelper.OpenReadReturnHtmlString("tfd_demon.html"));
            bool result = helper.CheckForCompletelyDifferentMeanings(DictionariesEnum.AmericanHeritage);

            Assert.IsFalse(result);}

        [TestMethod]
        public void CheckForCompletelyDifferentMeanings_AmericanHeritageHasCompletelyDifferentMeanings_ShouldReturnTrue()
        {
            var helper = CreateTheFreeDictionaryHelper(_testHelper.OpenReadReturnHtmlString("tfd_go.html"));
            bool result = helper.CheckForCompletelyDifferentMeanings(DictionariesEnum.AmericanHeritage);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DetectGroupText_Meaning_has_group_type_text_and_other_stuffs()
        {
            // third div.pseg which means the third meaning group:
            // n. pl. goes

            var helper = CreateTheFreeDictionaryHelper(_testHelper.OpenReadReturnHtmlString("tfd_go.html"));

            var dsList = helper.GetDsListsGrouped(DictionariesEnum.AmericanHeritage)[0][0];
            string result = helper.DetectGroupText(dsList);

            Assert.AreEqual("v. intr.", result);
        }

        [TestMethod]
        public void DetectGroupText_MeaningHasOnlyGroupType()
        {
            // first div.pseg which means the first meaning group:
            // v.intr.

            var helper = CreateTheFreeDictionaryHelper(_testHelper.OpenReadReturnHtmlString("tfd_play.html"));

            List<List<HtmlNode>> dsListGrouped = helper.GetDsListsGrouped(DictionariesEnum.AmericanHeritage);
            var dsList = dsListGrouped[0][0];
            string result = helper.DetectGroupText(dsList);

            Assert.AreEqual("v. intr.", result);
        }

        [TestMethod]
        public void GetIdioms_PlainIdiomSingleDefinitionNoIllustration()
        {
            /*
                from the word go
                  From the very beginning.
             */

            var html = _testHelper.OpenReadReturnHtmlString("tfd_go.html");
            var helper = CreateTheFreeDictionaryHelper(html);
            var idioms = helper.GetIdioms(DictionariesEnum.AmericanHeritage);

            var firstIdiom = idioms.First();

            Assert.AreEqual("from the word go", firstIdiom.Text);
            Assert.AreEqual("From the very beginning.", firstIdiom.Meanings.First().Text);

            Assert.AreEqual("", firstIdiom.Meanings.First().SenseRegister);
            Assert.AreEqual("", firstIdiom.Meanings.First().Context);
            Assert.AreEqual("", firstIdiom.Meanings.First().GrammaticalNote);

            Assert.AreEqual(1, firstIdiom.Meanings.Count);
            Assert.AreEqual(0, firstIdiom.Meanings.First().Illustrations.Count);

        }

        [TestMethod]
        public void GetIdioms_PlainIdiomSingleDefinitionHasSenseRegisterAndIllustration()
        {
            /*
                go belly up Informal
                    To undergo total financial failure: "A record number of ... banks went belly up" (New Republic).
             
             */
            var html = _testHelper.OpenReadReturnHtmlString("tfd_go.html");
            var helper = CreateTheFreeDictionaryHelper(html);
            var idioms = helper.GetIdioms(DictionariesEnum.AmericanHeritage);

            var fifthIdiom = idioms[4];

            Assert.AreEqual("go belly up", fifthIdiom.Text);
            Assert.AreEqual("Informal", fifthIdiom.Meanings.First().SenseRegister);

            Assert.AreEqual(1, fifthIdiom.Meanings.First().Illustrations.Count);
            Assert.AreEqual("\"A record number of ... banks went belly up\" (New Republic).", 
                fifthIdiom.Meanings.First().Illustrations.First().Text);
        }

        [TestMethod]
        public void GetIdioms_IdiomHasMoreThanOneDefinitionsAndSenseRegister()
        {
            /*
                go to the wall Informal
                    1. To lose a conflict or be defeated; yield: Despite their efforts, the team went to the wall.
                    2. To be forced into bankruptcy; fail.
                    3. To make an all-out effort, especially in defending another.             
            */
            var html = _testHelper.OpenReadReturnHtmlString("tfd_go.html");
            var helper = CreateTheFreeDictionaryHelper(html);
            var idioms = helper.GetIdioms(DictionariesEnum.AmericanHeritage);

            var idiom = idioms[34];

            Assert.AreEqual("go to the wall", idiom.Text);
            Assert.AreEqual("Informal", idiom.Meanings.First().SenseRegister);

            Assert.AreEqual("To lose a conflict or be defeated; yield:", idiom.Meanings[0].Text);
            Assert.AreEqual("To be forced into bankruptcy; fail.", idiom.Meanings[1].Text);
            Assert.AreEqual("To make an all-out effort, especially in defending another.", idiom.Meanings[2].Text);

            Assert.AreEqual("Informal", idiom.Meanings[0].SenseRegister);
            Assert.AreEqual("Informal", idiom.Meanings[1].SenseRegister);
            Assert.AreEqual("Informal", idiom.Meanings[2].SenseRegister);

            Assert.AreEqual(1, idiom.Meanings[0].Illustrations.Count);
            Assert.AreEqual("Despite their efforts, the team went to the wall.", idiom.Meanings[0].Illustrations.First().Text);
        }

        [TestMethod]
        public void GetPhrasalVerbs_PlainPhrasalVerbHasNoSenseRegisterAndIllustration()
        {
            /*
                play around
                    To philander.
             */
            var html = _testHelper.OpenReadReturnHtmlString("tfd_play.html");
            var helper = CreateTheFreeDictionaryHelper(html);

            var phrasalVerbs = helper.GetPhrasalVerbs(DictionariesEnum.AmericanHeritage);

            Word pv = phrasalVerbs[1];

            Assert.AreEqual("play around", pv.Text);
            Assert.AreEqual("To philander.", pv.Meanings.First().Text);

            Assert.AreEqual("", pv.Meanings.First().Context);
            Assert.AreEqual("", pv.Meanings.First().GrammaticalNote);
            Assert.AreEqual("", pv.Meanings.First().Type);
            Assert.AreEqual("", pv.Meanings.First().UsageForm);

            Assert.AreEqual(1, pv.Meanings.Count);
            Assert.AreEqual(0, pv.Meanings.First().Illustrations.Count);
        }

        [TestMethod]
        public void GetPhrasalVerbs_PlainPhrasalVerbHasNoSenseRegisterAndIllustrations()
        {
            /*
                play along Informal
                    To cooperate or pretend to cooperate: decided to play along with the robbers for a while.
             */

            var html = _testHelper.OpenReadReturnHtmlString("tfd_play.html");
            var helper = CreateTheFreeDictionaryHelper(html);

            var phrasalVerbs = helper.GetPhrasalVerbs(DictionariesEnum.AmericanHeritage);

            Word pv = phrasalVerbs[0];

            Assert.AreEqual("play along", pv.Text);
            Assert.AreEqual("Informal", pv.Meanings.First().SenseRegister);
            Assert.AreEqual("To cooperate or pretend to cooperate:", pv.Meanings.First().Text);
            Assert.AreEqual("decided to play along with the robbers for a while.", pv.Meanings.First().Illustrations.First().Text);

            Assert.AreEqual("Informal", pv.Meanings.First().SenseRegister);
            Assert.AreEqual("", pv.Meanings.First().Context);
            Assert.AreEqual("", pv.Meanings.First().GrammaticalNote);
            Assert.AreEqual("", pv.Meanings.First().Type);
            Assert.AreEqual("", pv.Meanings.First().UsageForm);
            
            Assert.AreEqual(1, pv.Meanings.Count);
            Assert.AreEqual(1, pv.Meanings.First().Illustrations.Count);
        }
        
        [TestMethod]
        public void GetPhrasalVerbs_PlainPhrasalVerbHasNoSenseRegisterAndIllustrationa()
        {
            /*
                play at
                    1. To participate in; engage in.
                    2. To do or take part in halfheartedly.
             */

            var html = _testHelper.OpenReadReturnHtmlString("tfd_play.html");
            var helper = CreateTheFreeDictionaryHelper(html);

            var phrasalVerbs = helper.GetPhrasalVerbs(DictionariesEnum.AmericanHeritage);

            Word pv = phrasalVerbs[2];

            Assert.AreEqual("play at", pv.Text);
            Assert.AreEqual("To participate in; engage in.", pv.Meanings.First().Text);
            Assert.AreEqual("To do or take part in halfheartedly.", pv.Meanings.Last().Text);

            Assert.AreEqual("", pv.Meanings.First().SenseRegister);
            Assert.AreEqual("", pv.Meanings.First().Context);
            Assert.AreEqual("", pv.Meanings.First().GrammaticalNote);
            Assert.AreEqual("", pv.Meanings.First().Type);
            Assert.AreEqual("", pv.Meanings.First().UsageForm);

            Assert.AreEqual("", pv.Meanings.Last().SenseRegister);
            Assert.AreEqual("", pv.Meanings.Last().Context);
            Assert.AreEqual("", pv.Meanings.Last().GrammaticalNote);
            Assert.AreEqual("", pv.Meanings.Last().Type);
            Assert.AreEqual("", pv.Meanings.Last().UsageForm);
            
            Assert.AreEqual(2, pv.Meanings.Count);
            Assert.AreEqual(0, pv.Meanings.First().Illustrations.Count);
            Assert.AreEqual(0, pv.Meanings.Last().Illustrations.Count);
        }
    }
}

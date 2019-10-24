using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDomain;
using LogicLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicLayerTests
{
    [TestClass]
    public class AmericanHeritageIntegrationTests
    {
        private readonly TestHelper _testHelper = new TestHelper();

        public AmericanHeritageHelper CreateAmericanHeritageHelper(string html)
        {
            return new AmericanHeritageHelper(html);
        }

        [TestMethod]
        public void Do()
        {
            var helper = CreateAmericanHeritageHelper(_testHelper.OpenReadReturnHtmlString("ah_sunstroke.html"));

            var word = helper.Populate();
            
            var firstGroupFirstMeaning = word.First().Groups.First().Meanings.First();
            Assert.AreEqual("Heat stroke caused by exposure to the sun and characterized by a rise in temperature, convulsions, and coma. Also called insolation, siriasis.", firstGroupFirstMeaning.Text);

        }


        [TestMethod]
        public void HasDicreetMeanings_Go_HasTwoDiscreetMeanings_ShouldReturnTrue()
        {
            var helper = CreateAmericanHeritageHelper(_testHelper.OpenReadReturnHtmlString("ah_go.html"));
            bool result = helper.HasDicreetMeanings();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasDicreetMeanings_Play_HasNoDiscreetMeanings_ShouldReturnFalse()
        {
            var helper = CreateAmericanHeritageHelper(_testHelper.OpenReadReturnHtmlString("ah_play.html"));
            bool result = helper.HasDicreetMeanings();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetIdioms_PlainIdiomSingleDefinitionNoIllustration()
        {
            /*
                from the word go
                  From the very beginning.
             */

            var html = _testHelper.OpenReadReturnHtmlString("ah_go.html");
            var helper = CreateAmericanHeritageHelper(html);
            var idioms = helper.GetIdioms();

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
            var html = _testHelper.OpenReadReturnHtmlString("ah_go.html");
            var helper = CreateAmericanHeritageHelper(html);
            var idioms = helper.GetIdioms();

            var fifthIdiom = idioms[4];

            Assert.AreEqual("go belly up", fifthIdiom.Text);
            Assert.AreEqual("Informal", fifthIdiom.Meanings.First().SenseRegister);

            Assert.AreEqual(1, fifthIdiom.Meanings.First().Illustrations.Count);
            Assert.AreEqual("\"A record number of ... banks went belly up\" (New Republic).", fifthIdiom.Meanings.First().Illustrations.First().Text);
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
            var html = _testHelper.OpenReadReturnHtmlString("ah_go.html");
            var helper = CreateAmericanHeritageHelper(html);
            var idioms = helper.GetIdioms();

            var idiom = idioms[31];

            Assert.AreEqual("go to the wall", idiom.Text);
            Assert.AreEqual("Informal", idiom.Meanings.First().SenseRegister);

            Assert.AreEqual("To lose a conflict or be defeated; yield.", idiom.Meanings[0].Text);
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
            var html = _testHelper.OpenReadReturnHtmlString("ah_play.html");
            var helper = CreateAmericanHeritageHelper(html);

            Word pv = helper.PopulateNthPhrasalVerb(2);


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

            var html = _testHelper.OpenReadReturnHtmlString("ah_play.html");
            var helper = CreateAmericanHeritageHelper(html);

            var phrasalVerbs = helper.GetPhrasalVerbs();

            Word pv = phrasalVerbs[0];

            Assert.AreEqual("play along", pv.Text);
            Assert.AreEqual("Informal", pv.Meanings.First().SenseRegister);
            Assert.AreEqual("To cooperate or pretend to cooperate.", pv.Meanings.First().Text);
            Assert.AreEqual("decided to play along with the robbers for a while.",
                pv.Meanings.First().Illustrations.First().Text);

            Assert.AreEqual("Informal", pv.Meanings.First().SenseRegister);
            Assert.AreEqual("", pv.Meanings.First().Context);
            Assert.AreEqual("", pv.Meanings.First().GrammaticalNote);
            Assert.AreEqual("", pv.Meanings.First().Type);
            Assert.AreEqual("", pv.Meanings.First().UsageForm);
            
            Assert.AreEqual(1, pv.Meanings.Count);
            Assert.AreEqual(1, pv.Meanings.First().Illustrations.Count);
        }
        
        public void GetPhrasalVerbs_PlainPhrasalVerbHasNoSenseRegisterAndIllustrationa()
        {
            /*
                play at
                    1. To participate in; engage in.
                    2. To do or take part in halfheartedly.
             */

            var html = _testHelper.OpenReadReturnHtmlString("ah_play.html");
            var helper = CreateAmericanHeritageHelper(html);

            var phrasalVerbs = helper.GetPhrasalVerbs();

            Word pv = phrasalVerbs[2];

            Assert.AreEqual("play at", pv.Text);
            Assert.AreEqual("1. To participate in; engage in.", pv.Meanings.First().Text);
            Assert.AreEqual("2. To do or take part in halfheartedly.", pv.Meanings.Last().Text);

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

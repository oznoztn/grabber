using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using GDomain;
using LogicLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicLayerTests
{
    [TestClass]
    public class AmericanHeritageIntegrationTestsNew
    {
        private readonly TestHelper _testHelper = new TestHelper();
        private readonly string _goHtml;
        private readonly string _propellerHtml;
        private readonly string _demonHtml;
        private readonly string _playHtml;
        private readonly string _ravageHtml;
        private readonly string _armageddonHtml;

        public AmericanHeritageIntegrationTestsNew()
        {
            _goHtml = _testHelper.OpenReadReturnHtmlString("ah_go.html");
            _propellerHtml = _testHelper.OpenReadReturnHtmlString("ah_propeller.html");
            _demonHtml = _testHelper.OpenReadReturnHtmlString("ah_demon.html");
            _playHtml = _testHelper.OpenReadReturnHtmlString("ah_play.html");
            _ravageHtml = _testHelper.OpenReadReturnHtmlString("ah_ravage.html");
            _armageddonHtml = _testHelper.OpenReadReturnHtmlString("ah_armageddon.html");
        }

        public AmericanHeritageHelper CreateAmericanHeritageHelper(string html)
        {
            return new AmericanHeritageHelper(html);
        }

        public AmericanHeritageMeaning GetNthMeaning(string html, int nthDiscreteGroup = 1, int nthTypeGroup = 1, int nthMeaning = 1)
        {
            return CreateAmericanHeritageHelper(html).PopulateNthMeaning(nthDiscreteGroup, nthTypeGroup, nthMeaning);
        }

        public Word GetNthPhrasalVerb(string html, int nth = 1)
        {
            return CreateAmericanHeritageHelper(html).PopulateNthPhrasalVerb(nth);
        }

        public Word GetNthIdiom(string html, int nth = 1)
        {
            return CreateAmericanHeritageHelper(html).PopulateNthIdiom(nth);
        }

        [TestMethod]
        public void DetectGroupText_Meaning_has_group_type_text_and_other_stuffs()
        {
            // third div.pseg which means the third meaning group:
            // n. pl. goes

            var helper = CreateAmericanHeritageHelper(_testHelper.OpenReadReturnHtmlString("ah_go.html"));

            var dsList = helper.GetDsListsGroupedV2()[0][0][0];
            string result = helper.DetectGroupText(dsList);

            Assert.AreEqual("v. intr.", result);
        }

        [TestMethod]
        public void DetectGroupText_MeaningHasOnlyGroupType()
        {
            // first div.pseg which means the first meaning group:
            // v.intr.

            var helper = CreateAmericanHeritageHelper(_testHelper.OpenReadReturnHtmlString("ah_go.html"));

            var dsList = helper.GetDsListsGroupedV2()[0][2][0];
            string result = helper.DetectGroupText(dsList);

            Assert.AreEqual("n. pl. goes", result);
        }

        [TestMethod]
        public void PopulateNthMeaning_CheckMeaningsNthOrder_MeaningHasNoOrderText_ShouldReturnOne()
        {
            // n.
            // A device for propelling an aircraft or boat, ...
            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_propellerHtml);

            AmericanHeritageMeaning meaning = helper.PopulateNthMeaning();

            Assert.AreEqual(1, meaning.Nth);
        }

        [TestMethod]
        public void PopulateNthMeaning_MeaningText_CheckForFirstMeaningsText()
        {
            // 1. To move or travel; proceed: 
            //  We will go by bus. Solicitors went from door to door seeking donations. How fast can the boat go?
            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);

            AmericanHeritageMeaning meaning = helper.PopulateNthMeaning();

            Assert.AreEqual("To move or travel; proceed.", meaning.Text);
        }

        [TestMethod]
        public void PopulateNthMeaning_MeaningText_BaseMeaningHasNoDefiniton_ShouldReturnEmpty()
        {
            /*
            4.
                a.To extend between two points or in a certain direction; run: curtains that go from the ceiling to the floor.
                b.To give entry; lead: a stairway that goes to the basement.
            */

            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);
            AmericanHeritageMeaning meaning = helper.PopulateNthMeaning(nthMeaning: 4);

            Assert.AreEqual("", meaning.Text);
        }

        [TestMethod]
        public void PopulateNthMeaning_MeaningText_SubMeaningsDefinitionCheck()
        {
            /*
                4.
                    a.To extend between two points or in a certain direction; run: curtains that go from the ceiling to the floor.
                    b.To give entry; lead: a stairway that goes to the basement.
            */

            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);
            AmericanHeritageMeaning meaning = helper.PopulateNthMeaning(nthMeaning: 4);

            Assert.AreEqual("To extend between two points or in a certain direction; run.", meaning.SubMeanings.First().Text);
        }

        [TestMethod]
        public void PopulateNthMeaning_CheckMeaningsNthOrder_FirstMeaning_ShouldReturnOne()
        {
            // 1. To move or travel; proceed: 
            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);

            AmericanHeritageMeaning meaning = helper.PopulateNthMeaning();

            Assert.AreEqual(1, meaning.Nth);
        }

        [TestMethod]
        public void PopulateNthMeaning_CheckMeaningsNthOrder_FifthMeaning_ShouldReturnFive()
        {
            // 1. To move or travel; proceed: 
            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);

            AmericanHeritageMeaning meaning = helper.PopulateNthMeaning(nthMeaning: 5);

            Assert.AreEqual(5, meaning.Nth);
        }

        [TestMethod]
        public void PopulateNthMeaning_SubMeaningsNthOrder_SecondSubOfTheThird_ShouldReturn2()
        {
            /*
                3.
                    a. To pursue a certain course: messages that go through diplomatic channels to the ambassador.
                    b. To resort to another, as for aid: went directly to the voters of her district.            
            */

            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);
            AmericanHeritageMeaning meaning = helper.PopulateNthMeaning(nthMeaning: 3);

            Assert.AreEqual(2, meaning.SubMeanings.Last().Nth);
        }

        [TestMethod]
        public void PopulateNthMeaning_Illustrations_Has3IllustrationsSeperatedByDots()
        {
            /*
                1. To move or travel; proceed: 
                    We will go by bus.
                    Solicitors went from door to door seeking donations. 
                    How fast can the boat go?
             */

            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);
            AmericanHeritageMeaning meaning = helper.PopulateNthMeaning(nthMeaning: 1);

            Assert.AreEqual("We will go by bus.", meaning.Illustrations[0].Text);
            Assert.AreEqual("Solicitors went from door to door seeking donations.", meaning.Illustrations[1].Text);
            Assert.AreEqual("How fast can the boat go?", meaning.Illustrations[2].Text);
        }

        [TestMethod]
        public void PopulateNthMeaning_Illustrations_Has3IllustrationsSeperatedBySemicolons()
        {
            /*
                3. One who is extremely zealous, skillful, or diligent: worked away like a demon; a real demon at math.
             */

            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_demonHtml);
            AmericanHeritageMeaning meaning = helper.PopulateNthMeaning(nthDiscreteGroup: 2, nthMeaning: 3);

            Assert.AreEqual("worked away like a demon.", meaning.Illustrations[0].Text);
            Assert.AreEqual("a real demon at math.", meaning.Illustrations[1].Text);
        }

        [TestMethod]
        public void PopulateNthMeaning_IllustrationEndsWithQuestionMark()
        {
            /*
                9. Informal To say or utter. Used chiefly in verbal narration: First I go, "Thank you," then he goes, "What for?"
            */
            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);
            var meaning = helper.PopulateNthMeaning(nthMeaning: 9, nthTypeGroup: 2);

            Assert.AreEqual(9, meaning.Nth);
            Assert.AreEqual("First I go, \"Thank you,\" then he goes, \"What for?\"", meaning.Illustrations.First().Text);
        }

        [TestMethod]
        public void PopulateNthMeaning_CheckSecondSubMeaningsIllutration()
        {
            /*
               6.
                 a. To have currency.
                 b. To pass from one person to another; circulate: Wild rumors were going around the office.
             */

            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);
            AmericanHeritageMeaning meaning = helper.PopulateNthMeaning(nthMeaning: 6);
            var secondSubmeaning = meaning.SubMeanings.Last();

            Assert.AreEqual("Wild rumors were going around the office.", secondSubmeaning.Illustrations.First().Text);
        }

        [TestMethod]
        public void PopulateNthMeaning_IllustrationHasAuthorInfo()
        {
            /*
               go downn
                8. Slang To occur; happen: "a collection of memorable pieces about the general craziness that was going down in those days" (James Atlas).
            */

            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);
            var phrasalVerb = helper.PopulateNthPhrasalVerb(6);
            var meaning = phrasalVerb.Meanings[7];

            Assert.AreEqual("\"a collection of memorable pieces about the general craziness that was going down in those days\" (James Atlas).", 
                meaning.Illustrations.First().Text);
        }

        [TestMethod]
        public void DsSingleHasSenseRegister_Informal()
        {
            /*
                8. Informal Used as an intensifier or to indicate annoyance when joined by and to a coordinate verb: She went and complained to Personnel.
             */

            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);

            var meaning = helper.PopulateNthMeaning(nthMeaning: 8);

            Assert.AreEqual("Informal", meaning.SenseRegister);
        }

        [TestMethod]
        public void DsSingleHasSenseRegister_Obsolete()
        {
            /*
                25. Obsolete To walk.
             */

            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);

            var meaning = helper.PopulateNthMeaning(nthMeaning: 25);

            Assert.AreEqual("Obsolete", meaning.SenseRegister);
        }

        [TestMethod]
        public void DsSingleHasContextInformation_Sports()
        {
            /*
                7. Sports To have as a record: went 3 for 4 against their best pitcher.
             */

            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);

            var meaning = helper.PopulateNthMeaning(nthMeaning: 7, nthTypeGroup: 2);

            Assert.AreEqual("Sports", meaning.Context);
        }

        [TestMethod]
        public void DsSingleHasNoSenseRegister()
        {
            /*
                1. To proceed or move according to: I was free to go my own way.
             */

            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);

            var meaning = helper.PopulateNthMeaning(nthMeaning: 1, nthTypeGroup: 2);

            Assert.AreEqual("", meaning.SenseRegister);
        }

        [TestMethod]
        public void DsSingleHasNoContextInformation()
        {
            /*
                1. To proceed or move according to: I was free to go my own way.
             */

            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);

            var meaning = helper.PopulateNthMeaning(nthMeaning: 1, nthTypeGroup: 2);
            
            Assert.AreEqual("", meaning.Context);
        }

        [TestMethod]
        public void DsListHasNoSenseRegister() // base meaning has not sense register
        {
            /*
               6.
                 a. To have currency.
                 b. To pass from one person to another; circulate: Wild rumors were going around the office.
             */

            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);
            AmericanHeritageMeaning meaning = helper.PopulateNthMeaning(nthMeaning: 6);

            Assert.AreEqual("", meaning.SenseRegister);
        }

        [TestMethod]
        public void DsListHasSenseRegister_Informal() // base meaning has sense register
        {
            /*
                4. Informal
                    a. To bet: go $20 on the black horse.
                    b. To bid: I'll go $500 on the vase.        
            */

            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);
            AmericanHeritageMeaning meaning = helper.PopulateNthMeaning(nthMeaning:4, nthTypeGroup:2);

            Assert.AreEqual("Informal", meaning.SenseRegister);
        }

        [TestMethod]
        public void SdsListHasSenreRegister_Informal()
        {
            /*
                5. Informal
                    a. The go-ahead.
                    b. often Go The starting point: "And from Go there was something deliciously illicit about the whole affair" (Erica Abeel).
                    c. Informal A situation in which planned operations can be effectuated: The space mission is a go.
             */
            AmericanHeritageHelper helper = CreateAmericanHeritageHelper(_goHtml);
            AmericanHeritageMeaning meaning = helper.PopulateNthMeaning(nthMeaning: 5, nthTypeGroup: 3);
            var secondSubMeaning = meaning.SubMeanings[2];

            Assert.AreEqual("Informal", secondSubMeaning.SenseRegister);
        }

        [TestMethod]
        public void SdsHasUsageForm_ravages()
        {
            /*
             2.
                a. Destruction, damage, or harm: The storm resulted in the ravage of the countryside.
                b. ravages Destructive or harmful effects: the ravages of disease.
            */
            var meaning = GetNthMeaning(_testHelper.OpenReadReturnHtmlString("ah_ravage.html"), nthMeaning:2, nthTypeGroup:3);
            var subMeaning = meaning.SubMeanings.Last();

            Assert.AreEqual("ravages", subMeaning.UsageForm);
        }

        [TestMethod]
        public void PhrasalVerb_SdsHasSenseRegister_Slang()
        {
            /*
                8. Slang To occur; happen: "a collection of memorable pieces about the general craziness that was going down in those days" (James Atlas).
             */
            var pv = GetNthPhrasalVerb(_goHtml, 6);
            var m = pv.Meanings[7];

            Assert.AreEqual("Slang", m.SenseRegister);
        }

        [TestMethod]
        public void SdsHasContextInfo_ContextTextShouldntAppearInTextProperty()
        {
            /*
                1.
                    a. Bible In the book of Revelation, the place of the gathering of armies for the final battle before the end of the world.             
            */

            var meaning = GetNthMeaning(_armageddonHtml);

            Assert.AreEqual("Bible", meaning.SubMeanings.First().Context);
            Assert.AreEqual("In the book of Revelation, the place of the gathering of armies for the final battle before the end of the world.", meaning.SubMeanings.First().Text);
        }


        [TestMethod]
        public void PhrasalVerb_SdsHasTwoSenseRegisters__Vulgar_Slang()
        {
            /*
                11. Vulgar Slang To perform fellatio or cunnilingus.
             */
            var pv = GetNthPhrasalVerb(_goHtml, 6);
            var m = pv.Meanings[10];

            Assert.AreEqual("Vulgar Slang", m.SenseRegister);
        }

        [TestMethod]
        public void PhrasalVerb_SdsHasSenseRegion_ChieflyBritish()
        {
            /*
                7. Chiefly British To leave a university.
             */
            var pv = GetNthPhrasalVerb(_goHtml, 6);
            var m = pv.Meanings[6];

            Assert.AreEqual("Chiefly British", m.SenseRegion);
        }

        [TestMethod]
        public void PhrasalVerb_TheBaseHasSenreRegister_Informal()
        {
            /*
                play along Informal
                    To cooperate or pretend to cooperate: decided to play along with the robbers for a while.
             */

            // Senre register ana anlamda olduðundan, Informal inner meaning'e atanýr:
            Word pv = GetNthPhrasalVerb(_playHtml, 1);

            Assert.AreEqual("Informal", pv.Meanings.First().SenseRegister);

        }

        [TestMethod]
        public void PhrasalVerb_CheckPhrasalVerbsTextProperty_ContainsParanthesis()
        {
            /*
               play on (or upon)
            */
            Word pv = GetNthPhrasalVerb(_playHtml, 7);

            Assert.AreEqual("play on (or upon)", pv.Text);
        }

        [TestMethod]
        public void PhrasalVerb_CheckPhrasalVerbsTextProperty_ContainsNothing()
        {
            /*
                play around
                    To philander.
            */
            Word pv = GetNthPhrasalVerb(_playHtml, 2);

            Assert.AreEqual("play around", pv.Text);
        }

        [TestMethod]
        public void PhrasalVerb_CheckPhrasalVerbsTextProperty_ContainsSenseReg()
        {
            /*
               play along Informal
               To cooperate or pretend to cooperate: decided to play along with the robbers for a while.
            */
            Word pv = GetNthPhrasalVerb(_playHtml, 1);

            Assert.AreEqual("play along", pv.Text);
        }

        [TestMethod]
        public void PhrasalVerb_DsListContextInformation_Sports()
        {
            /*
                play off
             >>     1. Sports
                        a. To establish the winner of (a tie) by playing in an additional game or series of games.
                        b. To participate in a playoff.
                    2. To set (one individual or party) in opposition to another so as to advance one's own interests: a parent who played off one child against another.
            */

            Word pv = GetNthPhrasalVerb(_playHtml, 6);

            Assert.AreEqual("Sports", pv.Meanings.First().Context);
        }

        [TestMethod]
        public void SdsHasContextInfo_SportsContextTextShouldntAppearInTextProperty()
        {
            /*
                play off
             >>     1. Sports
                        a. To establish the winner of (a tie) by playing in an additional game or series of games.
                        b. To participate in a playoff.
                    2. To set (one individual or party) in opposition to another so as to advance one's own interests: a parent who played off one child against another.
            */

            Word pv = GetNthPhrasalVerb(_playHtml, 6);
            
            Assert.AreEqual("", pv.Meanings.First().Text);
            Assert.AreEqual("Sports", pv.Meanings.First().Context);

        }

        [TestMethod]
        public void PhrasalVerb_DsListContainsNoContextInformation()
        {
            /*
                play off
                    1. Sports
                        a. To establish the winner of (a tie) by playing in an additional game or series of games.
                        b. To participate in a playoff.
                 >> 2. To set (one individual or party) in opposition to another so as to advance one's own interests: a parent who played off one child against another.
            */

            Word pv = GetNthPhrasalVerb(_playHtml, 6);

            Assert.AreEqual("", pv.Meanings.Last().Context);
        }

        [TestMethod]
        public void Go_HasTwoDiscreteMeaningGroups_PopulateShouldReturnAListOfTwoWords()
        {
            List<AmericanHeritageWord> word = CreateAmericanHeritageHelper(_goHtml).Populate();

            Assert.AreEqual(2, word.Count);
        }

        [TestMethod]
        public void Ravage_HasNoDiscreteMeaningGroups_PopulateShouldReturnAListOfOneWord()
        {
            List<AmericanHeritageWord> word = CreateAmericanHeritageHelper(_ravageHtml).Populate();

            Assert.AreEqual(1, word.Count);
        }

        [TestMethod]
        public void Ravage_WordHas3MeaningTypeGroups()
        {
            /*
                v.tr.
                    1. To bring heavy destruction on; devastate: A tornado ravaged the town.
                    2. To pillage; sack: Enemy soldiers ravaged the village.
                v.intr.
                    To wreak destruction.
                n.
                    1. The act or practice of pillaging or destroying: the marauders' ravage of the village.
                    2.
                        a. Destruction, damage, or harm: The storm resulted in the ravage of the countryside.
                        b. ravages Destructive or harmful effects: the ravages of disease.             
            */

            var words = CreateAmericanHeritageHelper(_ravageHtml).Populate();
            var word = words.First();

            Assert.AreEqual(3, word.Groups.Count);
        }

        [TestMethod]
        public void Ravage_FirstTypeGroupHasTwoMeanings()
        {
            /*
                v.tr.
                    1. To bring heavy destruction on; devastate: A tornado ravaged the town.
                    2. To pillage; sack: Enemy soldiers ravaged the village.
            */

            var words = CreateAmericanHeritageHelper(_ravageHtml).Populate();
            var word = words.First();

            var firstTypeGroup = word.Groups[0];

            Assert.AreEqual(2, firstTypeGroup.Meanings.Count);
        }

        [TestMethod]
        public void Go_SecondTypeGroupHasTenMeanings()
        {
            /*
                v.tr.
                    1. To bring heavy destruction on; devastate: A tornado ravaged the town.
                    2. To pillage; sack: Enemy soldiers ravaged the village.
                v.intr.
                    To wreak destruction.
            */
            var words = CreateAmericanHeritageHelper(_ravageHtml).Populate();
            var word = words.First();

            var firstTypeGroup = word.Groups[1];

            Assert.AreEqual(1, firstTypeGroup.Meanings.Count);
        }


        // HOLOCAUST
        public void GetUsageNote_Test()
        {

        }

        public void GetWordHistory_Test()
        {

        }

    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using GDomain;
using HtmlAgilityPack;
using LogicLayer;
using LogicLayer.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicLayerTests
{
    internal class WordUnderTestContainer
    {
        public string Word { get; set; }
        public string HtmlString { get; set; }
    }

    [TestClass]
    public class OxfordAmericanHelperIntegrationTests
    {
        private readonly TestHelper _helper = new TestHelper();
        private readonly List<WordUnderTestContainer> _wordsUnderTest = new List<WordUnderTestContainer>();
        private readonly string _god;
        private readonly string _satan;
        private readonly string _play;
        private readonly string _schlub;
        private readonly string _go;
        private readonly string _histrionic;
        private readonly string _cpu;
        private readonly string _affect;
        private readonly string _draft;
        private readonly string _mofo;
        private readonly string _bank;

        public OxfordAmericanHelperIntegrationTests()
        {
            _god = _helper.OpenReadReturnHtmlString("oxfordamerican_god.html");
            _satan = _helper.OpenReadReturnHtmlString("oxfordamerican_satan.html");
            _play = _helper.OpenReadReturnHtmlString("oxfordamerican_play.html");
            _schlub = _helper.OpenReadReturnHtmlString("oxfordamerican_schlub.html");
            _go = _helper.OpenReadReturnHtmlString("oxfordamerican_go.html");
            _cpu = _helper.OpenReadReturnHtmlString("oxfordamerican_cpu.html");
            _histrionic = _helper.OpenReadReturnHtmlString("oxfordamerican_histrionic.html");
            _affect = _helper.OpenReadReturnHtmlString("oxfordamerican_affect.html");
            _draft = _helper.OpenReadReturnHtmlString("oxfordamerican_draft.html");
            _mofo = _helper.OpenReadReturnHtmlString("oxfordamerican_mofo.html");
            _bank = _helper.OpenReadReturnHtmlString("oxfordamerican_bank.html");
        }

        public OxfordAmericanHelper Create(string html)
        {
            return new OxfordAmericanHelper(html);
        }

        [TestMethod]
        public void MeaningTextTest_HasShortFor()
        {
            var helper = Create(_mofo);
            var word = helper.Populate().First();

            Assert.AreEqual("short for motherfucker", word.Groups.First().Meanings.First().Text);
        }

        [TestMethod]
        public void GetWordText_ValidHtml()
        {
            var html = _play;
            var helper = Create(html);

            Assert.AreEqual("play", helper.GetWordText());
        }

        [TestMethod]
        public void GetWordText_InvalidHtml_SReturnEmptyString()
        {
            var html = _helper.OpenReadReturnHtmlString("oxfordamerican_notfound.html", "files");
            var helper = Create(html);

            Assert.AreEqual("", helper.GetWordText());
        }

        [TestMethod]
        public void IsMeaning_MeaningLi_ShouldReturnTrue()
        {
            var html = _play;
            var helper = Create(html);

            var firstMeaning = helper.GetLiNodesForEachSection().First().First();

            var result = helper.IsMeaning(firstMeaning);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMeaning_SenseLi_ShouldReturnFalse()
        {
            var html = _play;
            var helper = Create(html);

            var firstMeaning = helper.GetLiNodesForEachSection().First().First();
            var firstSense = helper.GetMeaningSubsenses(firstMeaning).First();

            var result = helper.IsMeaning(firstSense);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetDefinitionSections_HtmlContains2DefinitionSection_ReturnAListOfTwoItems()
        {
            // The word has 2 definition sections because the word has two different form: verb and noun.
            var html = _play;
            var helper = Create(html);
               
            Assert.AreEqual(2, helper.GetDefinitionSections().Count);
        }

        [TestMethod]
        public void GetWordTypes_TheWordIsBothVerbAndNoun_ShouldReturnAListOfTwoItems()
        {
            var html = _play;
            var helper = Create(html);

            Assert.AreEqual(2, helper.GetWordTypes().Count);
        }

        [TestMethod]
        public void GetWordTypes_TheWordIsBothVerbAndNoun_FirstItemShouldBeVerb()
        {
            var html = _play;
            var helper = Create(html);

            Assert.AreEqual("verb", helper.GetWordTypes().First());
        }

        [TestMethod]
        public void GetWordTypes_TheWordOnlyHasNounForm_ReturnsOneItemList()
        {
            var html = _satan;
            var helper = Create(html);

            Assert.AreEqual(1, helper.GetWordTypes().Count);
            Assert.AreEqual("proper noun", helper.GetWordTypes().First());
        }

        [TestMethod]
        public void IsValidHtml_ValidHtml_ShouldReturnTrue()
        {
            var html = _satan;
            var helper = Create(html);

            Assert.IsTrue(helper.IsValidHtml());
        }

        [TestMethod]
        public void IsValidHtml_InvalidHtml_ShouldReturnTrue()
        {
            var html = _helper.OpenReadReturnHtmlString("oxfordamerican_notfound.html", "files");
            var helper = Create(html);

            Assert.IsFalse(helper.IsValidHtml());
        }

        [TestMethod]
        public void HasMultipleGroups_WordHasMultipleMeaningGroups_ReturnsTrue()
        {
            var html = _go;
            var helper = Create(html);

            Assert.IsTrue(helper.HasMultipleGroups);
        }

        [TestMethod]
        public void HasMultipleGroups_WordHasOnlyOneMeaningGroup_ReturnsFalse()
        {
            // only one meaning group
            var html = _god;
            var helper = Create(html);

            Assert.IsFalse(helper.HasMultipleGroups);
        }

        [TestMethod]
        public void GetSectionsWithLiItems_TwoSections_ListOfTwoItems()
        {
            var html = _play;
            var helper = Create(html);

            var list = helper.GetLiNodesForEachSection();

            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void GetSectionsWithLiItems_TwoSectionsContainFiveandSixItems_ListOfTwoItems()
        {
            var html = _play;
            var helper = Create(html);

            var list = helper.GetLiNodesForEachSection();

            Assert.AreEqual(6, list.First().Count);
            Assert.AreEqual(5, list.Last().Count);
        }

        [TestMethod]
        public void GetSectionsWithLiItems_InvalidHtml_EmptyList()
        {
            var html = _helper.OpenReadReturnHtmlString("oxfordamerican_notfound.html", "files");
            var helper = Create(html);

            var list = helper.GetLiNodesForEachSection();

            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void GetMeaningOrSenseText_HtmContainsMeaningText_ReturnMeaningText()
        {
            string html = "<li><div class='trg'><p><span class='ind'>\"Meaning Text\"</span></p></div></li>";
            var helper = Create(html);
            
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode li = doc.DocumentNode.ChildNodes.First();

            var text = helper.GetMeaningOrSenseText(li);

            Assert.AreEqual("\"Meaning Text\"", text);
        }

        [TestMethod]
        public void GetIllustrationsSimple2_Sec1Me1HasTwoIllustrations_ListOfTwoItems()
        {
            var html = _play;
            var helper = Create(html);

            var firstSectionsFirstLi = helper.GetLiNodesForEachSection()[0][0];
            List<Illustration> illustrations = helper.GetIllustrationsSimple(firstSectionsFirstLi);

            Assert.AreEqual(2, illustrations.Count);
        }

        [TestMethod]
        public void GetIllustrationsSimple_Sec1Me1ContainsTwoIllustrations_True()
        {
            /*
                1 no object Engage in activity for enjoyment and recreation rather than a serious or practical purpose.
                    ‘the children were playing outside’
                    ‘her friends were playing with their dolls’             
             */
            var html = _play;
            var helper = Create(html);

            var firstSectionsFirstLi = helper.GetLiNodesForEachSection()[0][0];

            List<Illustration> illustrations = helper.GetIllustrationsSimple(firstSectionsFirstLi);

            Assert.AreEqual("the children were playing outside", illustrations.First().Text.CleanQuotationMarks());
            Assert.AreEqual("her friends were playing with their dolls", illustrations.Last().Text.CleanQuotationMarks());
        }

        [TestMethod]
        public void GetIllustrationsSimple_Sec1LastMeaningHasNoIllustrations_True()
        {
            var html = _play;
            var helper = Create(html);

            var firstSectionsLastLi = helper.GetLiNodesForEachSection().First().Last();
            List<Illustration> illustrations = helper.GetIllustrationsSimple(firstSectionsLastLi);
            
            Assert.IsTrue(illustrations.Count == 0);
        }

        [TestMethod]
        public void GetMeaningSubsenses_FirstMeaningContainsFiveSenses()
        {
            var html = _play;
            var helper = Create(html);

            var firstSectionFirstMeaningLi = helper.GetLiNodesForEachSection().First().First();
            var senses = helper.GetMeaningSubsenses(firstSectionFirstMeaningLi);

            Assert.IsTrue(senses.Count == 5);
        }

        [TestMethod]
        public void GetMeaningSubsenses_SecondMeaningContainsEightSenses()
        {
            var html = _play;
            var helper = Create(html);

            var h = helper.GetLiNodesForEachSection();
            var firstSectionSecondMeaning = helper.GetLiNodesForEachSection().First()[1];
            var senses = helper.GetMeaningSubsenses(firstSectionSecondMeaning);

            Assert.IsTrue(senses.Count == 8);
        }

        [TestMethod]
        public void GetMeaningSubsenses_FirstSectionLastMeaningContainsNoSubsense()
        {
            var html = _play;
            var helper = Create(html);

            var h = helper.GetLiNodesForEachSection();
            var firstSectionLastMeaning = helper.GetLiNodesForEachSection().First().Last();
            var senses = helper.GetMeaningSubsenses(firstSectionLastMeaning);

            Assert.IsTrue(senses.Count == 0);
        }

        [TestMethod]
        public void GetMeaningSenseRegister_FirstMeaningContainsASenseRegister()
        {
            /*
                 North American 
                 informal 
                     A talentless, unattractive, or boorish person.
                        ‘the poor dumb shlub just didn't get it’
             */

            var html = _schlub;
            var helper = Create(html);

            var meaningLi = helper.GetLiNodesForEachSection().First().First();

            var result = helper.GetMeaningSenseRegister(meaningLi);

            Assert.AreEqual("informal", result);
        }

        [TestMethod]
        public void GetMeaningSenseRegister_MeaningHasBothSenseRegisterAndUsageForm_ShouldntBeCombined()
        {
            // Section 1 > Meaning 4
            // 4 (the gods) informal The gallery in a theater.
            var helper = Create(_god);

            var meaningLi = helper.GetLiNodesForEachSection()[0].Last();

            var senseRegister = helper.GetMeaningSenseRegister(meaningLi);

            Assert.AreEqual("informal", senseRegister);
        }

        [TestMethod]
        public void GetMeaningSenseRegister_FirstMeaningDoesntContainASenseRegister_ReturnEmptyString()
        {
            var html = _god;
            var helper = Create(html);

            var meaningLi = helper.GetLiNodesForEachSection().First().First();

            var result = helper.GetMeaningSenseRegister(meaningLi);

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void GetMeaningSenseRegister_FourthMeaningContainsASenseRegister_ReturnsInformalRegister()
        {
            var html = _god;
            var helper = Create(html);

            var meaningLi = helper.GetLiNodesForEachSection().First().Last();

            var result = helper.GetMeaningSenseRegister(meaningLi);

            Assert.AreEqual("informal", result);
        }

        [TestMethod]
        public void GetMeaningRegionRegister_FirstMeaningHasRegionRegister_ShoulReturnNorthAmerican()
        {
            var html = _schlub;
            var helper = Create(html);

            var meaningLi = helper.GetLiNodesForEachSection().First().First();

            var result = helper.GetMeaningRegionRegister(meaningLi);

            Assert.AreEqual("North American", result);
        }

        [TestMethod]
        public void GetMeaningRegionRegister_FirstMeaningDoesntContainRegionRegister_ShoulReturnEmptyString()
        {
            var html = _god;
            var helper = Create(html);

            var meaningLi = helper.GetLiNodesForEachSection().First().First();

            var result = helper.GetMeaningRegionRegister(meaningLi);

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void GetMeaningRegionRegister_SecondMeaningFirstSenseHasRegionRegister()
        {
            var helper = Create(_draft);

            var qq = helper.Populate();

            var firstMainMeaning = helper.GetMainMeaningsWithDefinitionSectionsAndLiNodes().First();
            var firstTypeSection = firstMainMeaning.First();
            var secondMeaning = firstTypeSection[1];
            var firstSubSense = helper.GetMeaningSubsenses(secondMeaning).First();
            

            Assert.AreEqual("North American", helper.GetMeaningRegionRegister(firstSubSense));
        }

        [TestMethod]
        public void MeaningAndItsSenseHasSenseRegionRegister()
        {
            /*
            2 (the draft) US Compulsory recruitment for military service.
                ‘25 million men were subject to the draft’
                as modifier ‘draft cards’
                    2.1 North American A procedure whereby new or existing sports players are made available for selection or reselection by the teams in a league, usually with the earlier choices being given to the weaker teams.
                    2.2 rare A group or individual selected from a larger group for a special duty, e.g., for military service.             
           */

            var helper = Create(_draft);
            var word = helper.Populate().First();

            var secondMeaning = word.Groups.First().Meanings[1];

            Assert.AreEqual("the draft", secondMeaning.UsageForm);
            Assert.AreEqual("US", secondMeaning.SenseRegion);
        }

        [TestMethod]
        public void GetUsageForm_SenseHasUsageForm()
        {
            // Section 1 > Meaning 1 > Sense 3
            // (play at) Engage in without proper seriousness or understanding
            var html = _play;
            var helper = Create(html);

            var meaningLi = helper.GetLiNodesForEachSection()[0][0];
            var senseLi = helper.GetMeaningSubsenses(meaningLi)[2];

            var result = helper.GetUsageForm(senseLi);

            Assert.AreEqual("play at", result);
        }

        [TestMethod]
        public void GetUsageForm_MeaningHasBothUsageFormAndSenseRegister_ShouldntBeCombined()
        {
            // Section 1 > Meaning 4
            // 4 (the gods) informal The gallery in a theater.
            var helper = Create(_god);

            var meaningLi = helper.GetLiNodesForEachSection()[0].Last();

            var usageForm = helper.GetUsageForm(meaningLi);

            Assert.AreEqual("the gods", usageForm);
        }


        [TestMethod]
        public void GetUsageForm_SubMeaningHasUsageForm()
        {
            // 3. be going to be/do something
            // Intend or be likely or intended to be or do something; be about to (used to express a future tense)
            var html = _go;
            var helper = Create(html);

            var meaningLi = helper.GetLiNodesForEachSection()[0][2];

            var result = helper.GetUsageForm(meaningLi);

            Assert.AreEqual("be going to be/do something", result);
        }

        [TestMethod]
        public void GetUsageForm_MeaningHasUsageForm()
        {
            // Section 2 > Meaning 1
            // (histrionics) Exaggerated dramatic behavior designed to attract attention.
            var html = _helper.OpenReadReturnHtmlString("oxfordamerican_histrionic.html");
            var helper = Create(html);

            var meaningLi = helper.GetLiNodesForEachSection()[1][0];

            var result = helper.GetUsageForm(meaningLi);

            Assert.AreEqual("histrionics", result);
        }

        [TestMethod]
        public void GetUsageForm_MeaningHasNoUsageForm_ShouldReturnEmptyString()
        {
            // Section 1 > Meaning 1
            // (play at) Engage in without proper seriousness or understanding
            var html = _go;
            var helper = Create(html);

            var meaningLi = helper.GetLiNodesForEachSection()[0][0];
           
            var result = helper.GetUsageForm(meaningLi);

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void GetUsageForm_SenseHasNoUsageForm_ShouldReturnEmptyString()
        {
            var html = _play;
            var helper = Create(_play);

            var firstSection = helper.GetLiNodesForEachSection()[0];
            var thirdMeaning = firstSection[2];

            var secondSection = helper.GetLiNodesForEachSection()[1];
            var secondMeaning = secondSection[1];

            Assert.AreEqual(0, helper.GetMeaningSectionIndex(thirdMeaning, 1));
            Assert.AreEqual(1, helper.GetMeaningSectionIndex(secondMeaning, 1));
        }

        [TestMethod]
        public void GetMeaningContext_FirstLiHasContext_ShouldBeComputing()
        {
            var html = _cpu;
            var helper = Create(_cpu);

            HtmlNode firstMeaningLi = helper.GetLiNodesForEachSection()[0][0];
            string contextText = helper.GetMeaningContext(firstMeaningLi);

            Assert.AreEqual("Computing", contextText);
        }

        [TestMethod]
        public void GetMeaningContext_SubsenseHasAContext_ShouldBePsychiatry()
        {
            /*
                1.2 [Italic_Context] Psychiatry Denoting a personality disorder ...
             */
            var html = _histrionic;
            var helper = Create(_histrionic);

            HtmlNode adjectiveFirstMeaningLi = helper.GetLiNodesForEachSection()[0][0];
            HtmlNode secondSubMeaningOfIt = helper.GetMeaningSubsenses(adjectiveFirstMeaningLi)[1];
            string contextText = helper.GetMeaningContext(secondSubMeaningOfIt);

            Assert.AreEqual("Psychiatry", contextText);
        }

        [TestMethod]
        public void Populate_IntegrationTest_FullCoverage()
        {
            // histrionic has two forms: a, n
            var helper = Create(_histrionic);

            List<OxfordAmericanWord> words = helper.Populate();
            OxfordAmericanWord word = words.First();
                        
            Group<OxfordAmericanMeaning> adjective = word.Groups.First();
            Group<OxfordAmericanMeaning> noun = word.Groups.Last();

            Assert.AreEqual(1, adjective.Meanings.Count);
            Assert.AreEqual(2, adjective.Meanings.First().SubMeanings.Count);

            Assert.AreEqual("", adjective.Meanings[0].SenseRegister);
            Assert.AreEqual("", adjective.Meanings[0].Context);
            Assert.AreEqual("", adjective.Meanings[0].UsageForm);
            Assert.AreEqual("", adjective.Meanings[0].GrammaticalNote);
            Assert.AreEqual("", adjective.Meanings[0].Type);
            
            Assert.AreEqual("formal", adjective.Meanings[0].SubMeanings[0].SenseRegister);
            Assert.AreEqual("", adjective.Meanings[0].SubMeanings[0].Context);
            Assert.AreEqual("", adjective.Meanings[0].SubMeanings[0].UsageForm);
            Assert.AreEqual("", adjective.Meanings[0].SubMeanings[0].GrammaticalNote);
            Assert.AreEqual("", adjective.Meanings[0].SubMeanings[0].Type);

            Assert.AreEqual("Psychiatry", adjective.Meanings[0].SubMeanings[1].Context);
            Assert.AreEqual("", adjective.Meanings[0].SubMeanings[1].SenseRegister);
            Assert.AreEqual("", adjective.Meanings[0].SubMeanings[1].UsageForm);
            Assert.AreEqual("", adjective.Meanings[0].SubMeanings[1].GrammaticalNote);
            Assert.AreEqual("", adjective.Meanings[0].SubMeanings[1].Type);

            Assert.AreEqual(2, noun.Meanings.Count);
            Assert.AreEqual(1, noun.Meanings[0].SubMeanings.Count);
            Assert.AreEqual(0, noun.Meanings[1].SubMeanings.Count);

            Assert.AreEqual("histrionics", noun.Meanings[0].UsageForm);
            Assert.AreEqual("", noun.Meanings[0].Context);
            Assert.AreEqual("", noun.Meanings[0].SenseRegister);
            Assert.AreEqual("", noun.Meanings[0].GrammaticalNote);
            Assert.AreEqual("", noun.Meanings[0].Type);

            Assert.AreEqual("archaic", noun.Meanings[0].SubMeanings[0].SenseRegister);
            Assert.AreEqual("", noun.Meanings[0].SubMeanings[0].Context);
            Assert.AreEqual("", noun.Meanings[0].SubMeanings[0].UsageForm);
            Assert.AreEqual("", noun.Meanings[0].SubMeanings[0].GrammaticalNote);
            Assert.AreEqual("", noun.Meanings[0].SubMeanings[0].Type);

            Assert.AreEqual("archaic", noun.Meanings[1].SenseRegister);
            Assert.AreEqual("", noun.Meanings[1].Context);
            Assert.AreEqual("", noun.Meanings[1].UsageForm);
            Assert.AreEqual("", noun.Meanings[1].GrammaticalNote);
            Assert.AreEqual("", noun.Meanings[1].Type);
        }

        [TestMethod]
        public void Populate_Bank_FullCoverage()
        {
            // the word "bank" has two distinct meanings
            var helper = Create(_bank);

            List<OxfordAmericanWord> words = helper.Populate();

            // testing the first one
            OxfordAmericanWord firstDistinctMeaning = words.First();

            // verb and noun
            Assert.AreEqual(2, firstDistinctMeaning.Groups.Count);

            var firstDistinctMeaningFirstGroup = firstDistinctMeaning.Groups[0];
            Assert.AreEqual(4, firstDistinctMeaningFirstGroup.Meanings.Count);
            Assert.AreEqual("The land alongside or sloping down to a river or lake.", firstDistinctMeaningFirstGroup.Meanings[0].Text);
            Assert.AreEqual("A slope, mass, or mound of a particular substance.", firstDistinctMeaningFirstGroup.Meanings[1].Text);
            Assert.AreEqual("A set or series of similar things, especially electrical or electronic devices, grouped together in rows.", firstDistinctMeaningFirstGroup.Meanings[2].Text);
            Assert.AreEqual("The cushion of a pool table.", firstDistinctMeaningFirstGroup.Meanings[3].Text);

            Assert.AreEqual(1, firstDistinctMeaningFirstGroup.Meanings[0].Illustrations.Count);
            Assert.AreEqual("willows lined the bank", firstDistinctMeaningFirstGroup.Meanings[0].Illustrations.First().Text);
                                                  
            Assert.AreEqual(2, firstDistinctMeaningFirstGroup.Meanings[1].Illustrations.Count);                                           
            Assert.AreEqual("a bank of snow", firstDistinctMeaningFirstGroup.Meanings[1].Illustrations[0].Text);
            Assert.AreEqual("a bank of clouds", firstDistinctMeaningFirstGroup.Meanings[1].Illustrations[1].Text);

            Assert.AreEqual(1, firstDistinctMeaningFirstGroup.Meanings[1].SubMeanings.Last().Illustrations.Count);
            Assert.AreEqual("flying with small amounts of bank", firstDistinctMeaningFirstGroup.Meanings[1].SubMeanings.Last().Illustrations.First().Text);

            var firstDistinctMeaningSecondGroup = firstDistinctMeaning.Groups[1];
            Assert.AreEqual(3, firstDistinctMeaningSecondGroup.Meanings.Count);
            Assert.AreEqual("Heap (a substance) into a mass or mound.", firstDistinctMeaningSecondGroup.Meanings[0].Text);
            Assert.AreEqual("(of an aircraft or vehicle) tilt or cause to tilt sideways in making a turn.", firstDistinctMeaningSecondGroup.Meanings[1].Text);
            Assert.AreEqual("(in pool and other games) play (a ball) so that it rebounds off a surface such as a backboard or cushion.", firstDistinctMeaningSecondGroup.Meanings[2].Text);



            // testing the second one
            OxfordAmericanWord word2 = words.Last();

            // verb and noun
            Assert.AreEqual(2, word2.Groups.Count);

        }

        [TestMethod]
        public void GetMainMeaningsWithDefinitionSectionsAndLiNodes_WordHasOnlyOneMainMeaning_ListOfOneItem()
        {
            var helper = Create(_satan);

            var q = helper.GetMainMeaningsWithDefinitionSectionsAndLiNodes();

            Assert.AreEqual(1, q.Count);
        }

        [TestMethod]
        public void GetMainMeaningsWithDefinitionSectionsAndLiNodes_WordHasTwoMainMeanings_ListOfTwoItems()
        {
            var helper = Create(_go);

            var q = helper.GetMainMeaningsWithDefinitionSectionsAndLiNodes();

            Assert.AreEqual(2, q.Count);
        }

        [TestMethod]
        public void GetMainMeaningsWithDefinitionSectionsAndLiNodes_WordHasThreeMainMeanings_ListOfThreeItems()
        {
            var helper = Create(_affect);

            var q = helper.GetMainMeaningsWithDefinitionSectionsAndLiNodes();

            Assert.AreEqual(3, q.Count);
        }

        [TestMethod]
        public void Affect()
        {
            var helper = Create(_affect);

            var wordsAsDifferentMeanings = helper.Populate();

            var firstMeaning = wordsAsDifferentMeanings[0];
            var secondMeaning = wordsAsDifferentMeanings[1];
            var thirdMeaning = wordsAsDifferentMeanings[2];

            Assert.AreEqual("Have an effect on; make a difference to.", firstMeaning.Groups.First().Meanings.First().Text);
            Assert.AreEqual("Pretend to have or feel (something)", secondMeaning.Groups.First().Meanings.First().Text);
            Assert.AreEqual("Emotion or desire, especially as influencing behavior or action.", thirdMeaning.Groups.First().Meanings.First().Text);

            Assert.AreEqual("Psychology", thirdMeaning.Groups.First().Meanings.First().Context);
        }

        [TestMethod]
        public void SimpleIllustrationHasNote()
        {
            /* 
                4 with object Perform on (a musical instrument)
                    ‘we heard someone playing a harmonica’
                    no object ‘a pianist who will play for us’
             */

            var helper = Create(_play);
            var word = helper.PopulateWithNth(nthMeaning: 4);

            var theMeaning = word.Groups.First().Meanings.First();
            var illustration = theMeaning.Illustrations.Last();

            Assert.AreEqual("‘a pianist who will play for us’", illustration.Text);
        }


        #region Grammatican Note Tests
        [TestMethod]
        public void MeaningHasNoGrammaticalNote()
        {
            /*
                10 (of a song, account, verse, etc.) have a specified content or wording.
                    ‘if you haven't heard it, the story goes like this’            
            */

            var word = Create(_go).PopulateWithNth(nthMeaning: 10);

            Assert.AreEqual("", word.Groups.First().Meanings.First().GrammaticalNote);
        }

        [TestMethod]
        public void MeaningHasGrammaticalNote()
        {
            /*
                2 no object Leave; depart.
                    ‘I really must go’       
            */

            var word = Create(_go).PopulateWithNth(nthMeaning: 2);

            Assert.AreEqual("no object", word.Groups.First().Meanings.First().GrammaticalNote);
        }

        [TestMethod]
        public void MeaningHasTwoGrammaticalNote()
        {
            /*
                 1 no object , usually with adverbial of direction Move from one place to another; travel.
                    ‘he went out to the store’
             */

            var word = Create(_go).PopulateWithNth();

            Assert.AreEqual("no object , usually with adverbial of direction", word.Groups.First().Meanings.First().GrammaticalNote);
        }

        [TestMethod]
        public void SubMeaningHasGrammaticalNote()
        {
            /*            
                1.9 in imperative Begin motion (used in a starter's order to begin a race)
                    ‘ready, set, go!’
             */
            var word = Create(_go).PopulateWithNth();
            var subMeaning = word.Groups.First().Meanings[0].SubMeanings[9 - 1];

            Assert.AreEqual("in imperative", subMeaning.GrammaticalNote);

        }

        [TestMethod]
        public void SubMeaningHasNoGrammaticalNote()
        {
            /*            
                 10.1 (go by/under) Be known or called by (a specified name)
                    ‘he now goes under the name Charles Perez’  
            */

            var word = Create(_go).PopulateWithNth(nthMeaning: 10);
            var subMeaning = word.Groups.First().Meanings[0].SubMeanings[0];

            Assert.AreEqual("", subMeaning.GrammaticalNote);
        }
        #endregion

        #region Illustrations Tests

        [TestMethod]
        public void MainMeaningHasNoSimpleIllustrations()
        {
            var helper = Create(_satan);

            var word = helper.PopulateWithNth();
            var meaning = word.Groups.First().Meanings.First();

            Assert.AreEqual(0, meaning.Illustrations.Count);
        }

        [TestMethod]
        public void MainMeaningHasNoAdvancedIllustrations()
        {
            var helper = Create(_satan);

            var word = helper.PopulateWithNth();
            var meaning = word.Groups.First().Meanings.First();

            Assert.AreEqual(0, meaning.IllustrationsAdvanced.Count);
        }

        [TestMethod]
        public void MainMeaningHasOnlyAdvancedIllustrations()
        {
            /*
                1 (in Christianity and other monotheistic religions) the creator and ruler of the universe ...
             */
            var helper = Create(_god);

            var word = helper.PopulateWithNth();
            var meaning = word.Groups.First().Meanings.First();

            Assert.AreEqual(20, meaning.IllustrationsAdvanced.Count);
            Assert.AreEqual(0, meaning.Illustrations.Count);
        }

        [TestMethod]
        public void MainMeaningHasOnlySimpleIllustrations()
        {
            /*
             1 A preliminary version of a piece of writing.
                ‘the first draft of the party's manifesto’
                as modifier ‘a draft document’
             */
            var helper = Create(_draft);

            var word = helper.PopulateWithNth();
            var meaning = word.Groups.First().Meanings.First();

            Assert.AreEqual(2, meaning.Illustrations.Count);
            Assert.AreEqual(0, meaning.IllustrationsAdvanced.Count);
        }

        [TestMethod]
        public void MainMeaningHasBothSimpleAndAdvancedIllustrations()
        {
            /*
             (in certain other religions) a superhuman being or spirit worshiped as having power over nature or human fortunes; a deity.
                ‘a moon god’
                ‘an incarnation of the god Vishnu’
             */

            var helper = Create(_god);

            var word = helper.PopulateWithNth(nthMeaning: 2);
            var meaning = word.Groups.First().Meanings.First();

            Assert.AreEqual(2, meaning.Illustrations.Count);
            Assert.AreEqual(20, meaning.IllustrationsAdvanced.Count);
        }

        [TestMethod]
        public void IllustrationHasGrammaticalNote_TestIllustrationNote()
        {
            /*
                A preliminary version of a piece of writing.
                    ‘the first draft of the party's manifesto’
                    [as modifier] ‘a draft document’
            */
            var helper = Create(_draft);

            var word = helper.PopulateWithNth();
            var meaning = word.Groups.First().Meanings.First();
            var secondIllustration = meaning.Illustrations.Last();

            Assert.AreEqual("as modifier", secondIllustration.Note);
        }

        [TestMethod]
        public void IllustrationHasGrammaticalNote_TestIllustrationText()
        {
            /*
                A preliminary version of a piece of writing.
                    ‘the first draft of the party's manifesto’
                    [as modifier] ‘a draft document’
            */
            var helper = Create(_draft);

            var word = helper.PopulateWithNth();
            var meaning = word.Groups.First().Meanings.First();
            var secondIllustration = meaning.Illustrations.Last();

            Assert.AreEqual("‘a draft document’", secondIllustration.Text);
        }

        [TestMethod]
        public void SubMeaningHasNoSimpleIllustrations()
        {
            /*
                2.1 An image, idol, animal, or other object worshiped as divine or symbolizing a god.
                    [no_simple_illustration]
             */
            var helper = Create(_god);

            var word = helper.PopulateWithNth(nthMeaning: 2);
            var meaning = word.Groups.First().Meanings.Last();
            var subMeaning = meaning.SubMeanings.First();

            Assert.AreEqual(0, subMeaning.Illustrations.Count);
        }

        [TestMethod]
        public void SubMeaningHasSimpleIllustrations()
        {
            /*
             2.2 Used as a conventional personification of fate.
                ‘he dialed the number and, the gods relenting, got through at once’
             */
            var helper = Create(_god);

            var word = helper.PopulateWithNth(nthMeaning: 2);
            var meaning = word.Groups.First().Meanings.First();
            var subMeaning = meaning.SubMeanings.Last();

            Assert.AreEqual(1, subMeaning.Illustrations.Count);
        }

        [TestMethod]
        public void SubMeaningHasAdvancedIllustrations()
        {
            /*
                2.1 An image, idol, animal, or other object worshiped as divine or symbolizing a god.
            */
            var helper = Create(_god);

            var word = helper.PopulateWithNth(nthMeaning: 2);
            var meaning = word.Groups.First().Meanings.First();
            var subMeaning = meaning.SubMeanings.First();

            Assert.AreEqual(5, subMeaning.IllustrationsAdvanced.Count);
        }


        /// Illustration Checking Tests
        [TestMethod]
        public void MainMeaningIllustrationCheck()
        {
            /*
                A preliminary version of a piece of writing.
                    ‘the first draft of the party's manifesto’        
            */
            var helper = Create(_draft);

            var word = helper.PopulateWithNth();
            var meaning = word.Groups.First().Meanings.First();
            var firstIllustration = meaning.Illustrations.First();

            Assert.AreEqual("‘the first draft of the party's manifesto’", firstIllustration.Text);
        }

        [TestMethod]
        public void SubMeaningIllustrationCheck()
        {
            /*
                2.2 Used as a conventional personification of fate.
                    ‘he dialed the number and, the gods relenting, got through at once’
            */
            var helper = Create(_god);

            var word = helper.PopulateWithNth(nthMeaning: 2); // Second meaning
            var meaning = word.Groups.First().Meanings.First();
            var subMeaning = meaning.SubMeanings.Last();
            var illustration = subMeaning.Illustrations.First();

            Assert.AreEqual("‘he dialed the number and, the gods relenting, got through at once’", illustration.Text);
        }

        #endregion

        [TestMethod]
        public void PhraseHasNoSimpleIllustration()
        {
            /*
            bring something into play
                Cause something to begin operating or to have an effect; activate.         
            */

            var helper = Create(_play);
            var phrase = helper.PopulatePhrases()[0];

            Assert.AreEqual(0, phrase.Meanings.First().Illustrations.Count);
        }

        [TestMethod]
        public void PhraseHasOneSimpleIllustration()
        {
            /*
             make (great) play of (or with)
                Draw attention to in an ostentatious manner, typically to gain prestige or advantage.
                    ‘the company made great play of its recent growth in profits’
             */

            var helper = Create(_play);
            var phrase = helper.PopulatePhrases()[2];

            Assert.AreEqual(1, phrase.Meanings.First().Illustrations.Count);
        }

        [TestMethod]
        public void PhraseHasMoreThanOneSimpleIllustrations_TwoIllustrations()
        {
            /*
             play a part
                Make a contribution to a situation.
                    ‘social and economic factors may have also played a part’
                    ‘he personally wanted to thank those nurses and staff who had played a part in his recovery’
             */
            var helper = Create(_play);
            var phrase = helper.PopulatePhrases()[16];

            Assert.AreEqual(2, phrase.Meanings.First().Illustrations.Count);
        }

        [TestMethod]
        public void PhrasesSimpleIllustrationCheck()
        {
            /*
             make (great) play of (or with)
                Draw attention to in an ostentatious manner, typically to gain prestige or advantage.
                    ‘the company made great play of its recent growth in profits’
             */

            var helper = Create(_play);
            var phrase = helper.PopulatePhrases()[2];

            Assert.AreEqual("‘the company made great play of its recent growth in profits’", 
                phrase.Meanings.First().Illustrations.First().Text);

        }

        [TestMethod]
        public void PhraseHasNoAdvancedIllustrations()
        {
            /*
                play with oneself
                    informal Masturbate.            */

            var helper = Create(_play);
            var phrase = helper.PopulatePhrases().Last();

            Assert.AreEqual(0, phrase.Meanings.First().IllustrationsAdvanced.Count);
        }

        [TestMethod]
        public void PhraseHasMoreThanOneAdvancedIllustrations_TenIllustrations()
        {
            /*
            bring something into play
                Cause something to begin operating or to have an effect; activate.         
            */

            var helper = Create(_play);
            var phrase = helper.PopulatePhrases()[0];

            Assert.AreEqual(10, phrase.Meanings.First().IllustrationsAdvanced.Count);
        }

        [TestMethod]
        public void PhrasesAdvancedIllustrationCheck()
        {
            /*
             bring something into play
            */

            var helper = Create(_play);
            var phrase = helper.PopulatePhrases()[0];

            string expected =
                "‘Extra pairs of hands have been brought into play to ensure a North Yorkshire historic hall is safely ‘put to bed’ for the winter.’";
            Assert.AreEqual(expected, phrase.Meanings.First().IllustrationsAdvanced.First().Text);

        }

        [TestMethod]
        public void Phrase_Meaning_HasSenseRegister()
        {
            /*
                from the word go
                    informal From the very beginning.
             */

            var helper = Create(_go);
            var phrase = helper.PopulatePhrases()[2];

            Assert.AreEqual("informal", phrase.Meanings.First().SenseRegister);
        }

        [TestMethod]
        public void PhraseComprisesOneMeaning()
        {
            /*
                as (or so) far as it goes
                    Bearing in mind its limitations (said when qualifying praise of something) */

            var helper = Create(_go);

            var phrase = helper.PopulatePhrases()[0];

            Assert.AreEqual(1, phrase.Meanings.Count);
        }

        [TestMethod]
        public void PhraseComprisesTwoMeanings()
        {
            /*
                 have a go at
                    1 Make an attempt at; try.
                        ‘let me have a go at straightening the rim’
                    2 Attack or criticize (someone)
                        ‘she's always having a go at me’     */

            var helper = Create(_go);

            var phrase = helper.PopulatePhrases()[8];

            Assert.AreEqual(2, phrase.Meanings.Count);
        }

        [TestMethod]
        public void PopulatePhrases_FullCoverage_OneMeaning()
        {
            /*
                 as (or so) far as it goes
                    Bearing in mind its limitations (said when qualifying praise of something)
                        ‘the book is a useful catalog as far as it goes’
             */
            var helper = Create(_go);
            var phrases = helper.PopulatePhrases();
            var phrase = phrases.First();

            Assert.AreEqual("as (or so) far as it goes", phrase.Text);

            Assert.AreEqual(1, phrase.Meanings.Count);
            Assert.AreEqual("Bearing in mind its limitations (said when qualifying praise of something)", 
                phrase.Meanings.First().Text);

            Assert.AreEqual(1, phrase.Meanings.First().Illustrations.Count);
            Assert.AreEqual("‘the book is a useful catalog as far as it goes’", phrase.Meanings.First().Illustrations.First().Text);

            Assert.AreEqual(10, phrase.Meanings.First().IllustrationsAdvanced.Count);
            Assert.AreEqual("‘His reasoning is sound so far as it goes, and he's produced an enjoyable and thought-provoking read that I highly recommend.’", phrase.Meanings.First().IllustrationsAdvanced.First().Text);
            Assert.AreEqual("‘This is true in so far as it goes, but it ignores the personal nature of the duty an employer has to each of his individual employees.’", phrase.Meanings.First().IllustrationsAdvanced.Last().Text);

            Assert.AreEqual("", phrase.Meanings.First().Context);
            Assert.AreEqual("", phrase.Meanings.First().GrammaticalNote);
            Assert.AreEqual("", phrase.Meanings.First().SenseRegion);
            Assert.AreEqual("", phrase.Meanings.First().SenseRegister);
            Assert.AreEqual("", phrase.Meanings.First().Type);
            Assert.AreEqual("", phrase.Meanings.First().UsageForm);
        }

        [TestMethod]
        public void PopulatePhrases_FullCoverage_TwoMeaning()
        {
            /*
                 have a go at
                    1 Make an attempt at; try.
                        ‘let me have a go at straightening the rim’
                    2 Attack or criticize (someone)
                        ‘she's always having a go at me’     */

            var helper = Create(_go);

            var phrase = helper.PopulatePhrases()[8];

            Assert.AreEqual(2, phrase.Meanings.Count);

            Assert.AreEqual("have a go at", phrase.Text);

            Assert.AreEqual("Make an attempt at; try.", phrase.Meanings.First().Text);
            Assert.AreEqual("Attack or criticize (someone)", phrase.Meanings.Last().Text);

            Assert.AreEqual(1, phrase.Meanings.First().Illustrations.Count);
            Assert.AreEqual(1, phrase.Meanings.Last().Illustrations.Count);
            Assert.AreEqual("‘let me have a go at straightening the rim’", phrase.Meanings.First().Illustrations.First().Text);
            Assert.AreEqual("‘she's always having a go at me’", phrase.Meanings.Last().Illustrations.First().Text);

            Assert.AreEqual(10, phrase.Meanings.First().IllustrationsAdvanced.Count);
            Assert.AreEqual(10, phrase.Meanings.Last().IllustrationsAdvanced.Count);
            Assert.AreEqual("‘About 15 racers show up to have a go at the uphill time trial.’", phrase.Meanings.First().IllustrationsAdvanced.Last().Text);
            Assert.AreEqual("‘I have always stuck up for the players and not publicly had a go at them when they've not played particularly well.’", phrase.Meanings.Last().IllustrationsAdvanced.First().Text);

            Assert.AreEqual("", phrase.Meanings.First().Context);
            Assert.AreEqual("", phrase.Meanings.First().GrammaticalNote);
            Assert.AreEqual("", phrase.Meanings.First().SenseRegion);
            Assert.AreEqual("", phrase.Meanings.First().SenseRegister);
            Assert.AreEqual("", phrase.Meanings.First().Type);
            Assert.AreEqual("", phrase.Meanings.First().UsageForm);

            Assert.AreEqual("", phrase.Meanings.Last().Context);
            Assert.AreEqual("", phrase.Meanings.Last().GrammaticalNote);
            Assert.AreEqual("", phrase.Meanings.Last().SenseRegion);
            Assert.AreEqual("", phrase.Meanings.Last().SenseRegister);
            Assert.AreEqual("", phrase.Meanings.Last().Type);
            Assert.AreEqual("", phrase.Meanings.Last().UsageForm);

        }

        [TestMethod]
        public void PopulatePhrases_FullCoverage_OneMeaningThatHasASubsense()
        {
            /*
             play something by ear
                1 Perform music without having to read from a score.
                    1.1 informal Proceed instinctively according to results and circumstances rather than according to rules or a plan.
             
             */

            var helper = Create(_play);
            var phrases = helper.PopulatePhrases();
            var phrase = phrases[4];

            var firstMeaning = phrase.Meanings.First();

            Assert.AreEqual("play something by ear", phrase.Text);

            Assert.AreEqual(1, phrase.Meanings.Count);
            Assert.AreEqual("Perform music without having to read from a score.", firstMeaning.Text);

            Assert.AreEqual("‘That's a decision for the future, and we'll play it by ear.’", firstMeaning.SubMeanings.First().IllustrationsAdvanced.First().Text);

            Assert.AreEqual(0, firstMeaning.Illustrations.Count);
            Assert.AreEqual(8, firstMeaning.IllustrationsAdvanced.Count);

            Assert.AreEqual(1, firstMeaning.SubMeanings.Count);

            Assert.AreEqual("Proceed instinctively according to results and circumstances rather than according to rules or a plan.", firstMeaning.SubMeanings.First().Text);

            Assert.AreEqual(0, firstMeaning.SubMeanings.First().Illustrations.Count);
            Assert.AreEqual(10, firstMeaning.SubMeanings.First().IllustrationsAdvanced.Count);

            Assert.AreEqual("", phrase.Meanings.First().Context);
            Assert.AreEqual("", phrase.Meanings.First().GrammaticalNote);
            Assert.AreEqual("", phrase.Meanings.First().SenseRegion);
            Assert.AreEqual("", phrase.Meanings.First().SenseRegister);
            Assert.AreEqual("", phrase.Meanings.First().Type);
            Assert.AreEqual("", phrase.Meanings.First().UsageForm);

            Assert.AreEqual("", phrase.Meanings.First().SubMeanings.First().Context);
            Assert.AreEqual("", phrase.Meanings.First().SubMeanings.First().GrammaticalNote);
            Assert.AreEqual("", phrase.Meanings.First().SubMeanings.First().SenseRegion);
            Assert.AreEqual("informal", phrase.Meanings.First().SubMeanings.First().SenseRegister);
            Assert.AreEqual("", phrase.Meanings.First().SubMeanings.First().Type);
            Assert.AreEqual("", phrase.Meanings.First().SubMeanings.First().UsageForm);
        }

        [TestMethod]
        public void PhrasalVerb_HasOneMainMeaning()
        {
            /*
             go ahead
                Proceed or be carried out without hesitation.
                    ‘the project will go ahead’
             */
            var phrasalVerb = Create(_go).PopulatePhrasalVerbs()[2];

            Assert.AreEqual(1, phrasalVerb.Meanings.Count);
        }

        [TestMethod]
        public void PhrasalVerb_HasTwoMainMeanings()
        {
            /*
             go about
                1 Begin or carry on work at (an activity); busy oneself with.
                    ‘you are going about this in the wrong way’
                2 Sailing 
                    Change to an opposite tack.
             */
            var phrasalVerb = Create(_go).PopulatePhrasalVerbs()[0];

            Assert.AreEqual(2, phrasalVerb.Meanings.Count);
        }

        [TestMethod]
        public void PhrasalVerb_DefinitionCheck()
        {
            /*
             go ahead
                Proceed or be carried out without hesitation.
                    ‘the project will go ahead’
             */
            var phrasalVerb = Create(_go).PopulatePhrasalVerbs()[2];

            Assert.AreEqual("Proceed or be carried out without hesitation.", phrasalVerb.Meanings.First().Text);
        }

        [TestMethod]
        public void PhrasalVerb_DefinitionsCheck()
        {
            /*
             go about
                1 Begin or carry on work at (an activity); busy oneself with.
                    ‘you are going about this in the wrong way’
                2 Sailing 
                    Change to an opposite tack.
             */
            var phrasalVerb = Create(_go).PopulatePhrasalVerbs()[0];

            Assert.AreEqual("Begin or carry on work at (an activity); busy oneself with.", phrasalVerb.Meanings.First().Text);
            Assert.AreEqual("Change to an opposite tack.", phrasalVerb.Meanings.Last().Text);
        }

        [TestMethod]
        public void PhrasalVerb_HasSubMeaning()
        {
            /*
             go against
                1 Oppose or resist.
                ‘he refused to go against the unions’
                    1.1 Be contrary to (a feeling or principle)
                ‘these tactics go against many of our instincts’
                    1.2 (of a judgment, decision, or result) be unfavorable for.
                ‘the tribunal's decision went against them’
             */
            var phrasalVerb = Create(_go).PopulatePhrasalVerbs()[1];

            Assert.IsTrue(phrasalVerb.Meanings.First().SubMeanings.Count > 0);
        }

        [TestMethod]
        public void PhrasalVerb_MainMeaningHasContextInfo()
        {
            /*
             go about
                1 Begin or carry on work at (an activity); busy oneself with.
                    ‘you are going about this in the wrong way’
                2 Sailing 
                    Change to an opposite tack.
             */
            var phrasalVerb = Create(_go).PopulatePhrasalVerbs()[0];

            Assert.AreEqual("Sailing", phrasalVerb.Meanings.Last().Context);
        }

        [TestMethod]
        public void PhrasalVerb_MainMeaningHasNoSimpleIllustration()
        {
            var phrasalVerb = Create(_go).PopulatePhrasalVerbs()[0];

            var secondMeaning = phrasalVerb.Meanings.Last();

            Assert.AreEqual(0, secondMeaning.Illustrations.Count);
        }


        [TestMethod]
        public void PhrasalVerb_MainMeaningHasGrammaticalNote()
        {
            /*
             go on
                1 often with present participleContinue or persevere.
                    ‘I can't go on protecting you’
             */
            var phrasalVerb = Create(_go).PopulatePhrasalVerbs()[16];

            Assert.AreEqual("often with present participle", phrasalVerb.Meanings.First().GrammaticalNote);
        }

        [TestMethod]
        public void PhrasalVerb_SubMeaningHasGrammaticalNote()
        {
            /* go on
                 1.3 informal Said when encouraging someone or expressing disbelief.
                    ‘go on, tell him!’
             */
            var phrasalVerb = Create(_go).PopulatePhrasalVerbs()[16];
            
            Assert.AreEqual("informal", phrasalVerb.Meanings.First().SubMeanings[2].SenseRegister);
        }

        [TestMethod]
        public void PhrasalVerb_SimpleIllustrationCheck()
        {
            /*
             go ahead
                Proceed or be carried out without hesitation.
                    ‘the project will go ahead’
             */
            var phrasalVerb = Create(_go).PopulatePhrasalVerbs()[2];
            var firstMeaning = phrasalVerb.Meanings.First();

            Assert.AreEqual("‘the project will go ahead’", 
                firstMeaning.Illustrations.First().Text);
        }

        [TestMethod]
        public void PhrasalVerb_PhraseHasNoAdditionalInformationWhatsoever()
        {
            /*
                go down on
                    Perform oral sex on.
             */
            var phrasalVerb = Create(_go).PopulatePhrasalVerbs()[9];

            var meaning = phrasalVerb.Meanings.First();
            Assert.AreEqual("", meaning.SenseRegister);
            Assert.AreEqual("", meaning.SenseRegion);
            Assert.AreEqual("", meaning.Context);
            Assert.AreEqual("", meaning.UsageForm);
            Assert.AreEqual("", meaning.Type);
            Assert.AreEqual("", meaning.GrammaticalNote);
        }

        [TestMethod]
        [TestCategory("Meaning Synonyms Test")]
        public void GetMeaningSynonyms_MeaningHasSynonyms_ShouldntBeEmpty()
        {
            var helper = Create(_bank);

            OxfordAmericanWord word = helper.PopulateWithNth(nthMainMeaning: 1, nthSection: 1, nthMeaning: 1);

            var firstGroup = word.Groups.First();
            OxfordAmericanMeaning firstMeaning = firstGroup.Meanings.First();
            
            Assert.IsTrue(firstMeaning.Synonyms.Any());
        }

        [TestMethod]
        [TestCategory("Meaning Synonyms Test")]
        public void GetMeaningSynonyms_MeaningHasNoSynonyms_ShouldBeEmpty()
        {
            var helper = Create(_bank);

            OxfordAmericanWord word = helper.PopulateWithNth(nthMainMeaning: 1, nthSection: 1, nthMeaning: 4);

            var firstGroup = word.Groups.First();
            OxfordAmericanMeaning fourthMeaning = firstGroup.Meanings.First();


            Assert.IsFalse(fourthMeaning.Synonyms.Any());
        }

        [TestMethod]
        [TestCategory("Meaning Synonyms Test")]
        public void GetMeaningSynonyms_MeaningHasSevenSynonyms_TheListShouldContainSevenItems()
        {
            var helper = Create(_bank);

            OxfordAmericanWord word = helper.PopulateWithNth(nthMainMeaning: 1, nthSection: 1, nthMeaning: 2);

            var firstGroup = word.Groups.First();
            OxfordAmericanMeaning secondMeaning = firstGroup.Meanings.First();

            Assert.AreEqual(7, secondMeaning.Synonyms.Count);
        }

        [TestMethod]
        [TestCategory("Meaning Synonyms Test")]
        public void GetMeaningSynonyms_FirstSynCheck()
        {
            var helper = Create(_bank);

            OxfordAmericanWord word = helper.PopulateWithNth(nthMainMeaning: 1, nthSection: 1, nthMeaning: 2);

            var firstGroup = word.Groups.First();
            OxfordAmericanMeaning secondMeaning = firstGroup.Meanings.First();

            var theSynonym = secondMeaning.Synonyms.Last();

            Assert.AreEqual("tump", theSynonym);

        }

        [TestMethod]
        [TestCategory("Meaning Synonyms Test")]
        public void GetMeaningSynonyms_LastSynCheck()
        {
            var helper = Create(_bank);

            OxfordAmericanWord word = helper.PopulateWithNth(nthMainMeaning: 1, nthSection: 1, nthMeaning: 2);

            var firstGroup = word.Groups.First();
            OxfordAmericanMeaning secondMeaning = firstGroup.Meanings.First();

            var theSynonym = secondMeaning.Synonyms.Last();

            Assert.AreEqual("tump", theSynonym);
        }
    }
}

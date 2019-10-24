using System;
using System.Collections.Generic;
using System.Linq;
using GDomain;
using HtmlAgilityPack;
using LogicLayer;
using LogicLayer.Extensions;
using LogicLayer.Leecher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace LogicLayerTests
{
    [TestClass]
    public class VocabularyHelperTests
    {
        private readonly TestHelper _testHelper = new TestHelper();
        public VocabularyHelper Create(string html)
        {
            return new VocabularyHelper(html);
        }

        [TestMethod]
        public void GetShortBlurb_ValidHtml_ShouldReturnShortBlurb()
        {
            string html =
                "<div class=\"main\">" +
                    "<div class=\"section blurb\">" +
                        "<p class=\"short\">Short definition...</p>" +
                        "<p class=\"long\">Long definition...</p>" +
                    "</div> " +
                "</div>";

            VocabularyHelper helper = new VocabularyHelper(html);

            string shortDef = helper.GetShortBlurb();

            Assert.AreEqual(shortDef, "Short definition...");
        }

        [TestMethod]
        public void GetLongBlurb_ValidHtml_ShouldReturnLongBlurb()
        {
            string html =
                "<div class=\"main\">" +
                "<div class=\"section blurb\">" +
                "<p class=\"short\">Short definition...</p>" +
                "<p class=\"long\">Long definition...</p>" +
                "</div> " +
                "</div>";

            VocabularyHelper helper = new VocabularyHelper(html);

            string longDef = helper.GetLongBlurb();

            Assert.AreEqual(longDef, "Long definition...");
        }

        [TestMethod]
        public void GetShortBlurb_InvalidHtml_ShouldReturnEmptyString()
        {
            string html = "<div class=\"main\"> </div>";

            VocabularyHelper helper = new VocabularyHelper(html);

            string shortDef = helper.GetShortBlurb();

            Assert.AreEqual(shortDef, "");
        }

        [TestMethod]
        public void GetSections_HtmContainsTwoSections_ShouldReturnAListOfTwoSections()
        {
            var html = "<div class='main'>" +
                           "<div class='definitions'>" +
                               "<div class='section definition'></div>'" +
                               "<div class='section definition'></div>'" +
                           "</div>" +
                       "</div>";


            var helper = new VocabularyHelper(html);
            var sections = helper.GetSections();

            Assert.AreEqual(sections.Count, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(AnomalyException))]
        public void GetSections_HtmContainsThreeSections_CausesAnAnomaly()
        {
            var html = "<div class='main'>" +
                           "<div class='definitions'>" +
                               "<div class='section definition'></div>'" +
                               "<div class='section definition'></div>'" +
                               "<div class='section definition'></div>'" +
                           "</div>" +
                       "</div>";


            var helper = new VocabularyHelper(html);
            var sections = helper.GetSections();

            Assert.AreEqual(sections.Count, 2);
        }

        [TestMethod]
        public void HasPrimaryMeaningSection_HtmlContainsPrimaryMeaningsSection_ShouldReturnTrue()
        {
            var html = "<div class='main'>" +
                           "<div class='definitions'>" +
                                "<div class='section definition'>" +
                                    "<table class='definitionNavigator'>" +
                                    "</table>" +
                                "</div>'" +
                                "<div class='section definition'></div>'" +
                           "</div>" +
                       "</div>";

            var helper = new VocabularyHelper(html);
            var result = helper.HasPrimaryMeaningsSection();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasPrimaryMeaningSection_HtmlDoesNotContainPrimaryMeaningsSection_ShouldReturnFalse()
        {
            var html = "<div class='main'>" +
                           "<div class='definitions'>" +
                               "<div class='section definition'></div>" +
                               "<div class='section definition'></div>'" +
                           "</div>" +
                       "</div>";

            var helper = new VocabularyHelper(html);
            var result = helper.HasPrimaryMeaningsSection();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetPrimaryMeaningSection_HtmlContainsBothPrimerAndFullDefSections_ReturnsPrimaryDefSection()
        {
            var html = "<div class='main'>" +
                           "<div class='definitions'>" +
                               "<div class='section definition'>" +
                               "<table class='definitionNavigator'></table>" +
                               "</div>" +
                               "<div class='section definition'></div>'" +
                           "</div>" +
                       "</div>";

            var helper = new VocabularyHelper(html);

            HtmlNode pmeaningsSection = helper.GetPrimaryMeaningSection();

            Assert.IsNotNull(pmeaningsSection);
        }

        [TestMethod]
        public void GetPrimaryMeaningSection_HtmlContainsOnlyFullDefSection_ReturnsNull()
        {
            var html = "<div class='main'>" +
                           "<div class='definitions'>" +
                               "<div class='section definition'></div>'" +
                           "</div>" +
                       "</div>";

            var helper = new VocabularyHelper(html);

            var pmeaningsSection = helper.GetPrimaryMeaningSection();

            Assert.IsNull(pmeaningsSection);
        }

        [TestMethod]
        public void GetFullDefinitionSection_HtmlContainsBothPrimerAndFullDefSections_ReturnsFullDefSection()
        {
            var html = "<div class='main'>" +
                           "<div class='definitions'>" +
                               "<div class='section definition'>" +
                                   "<table class='definitionNavigator'></table>" +
                               "</div>" +
                               "<div class='section definition'>" +
                                   "<label>Test Test Test</label>" +
                               "</div>'" +
                           "</div>" +
                       "</div>";

            var helper = new VocabularyHelper(html);

            var fullDefSection = helper.GetFullDefinitionSection();

            Assert.IsNotNull(fullDefSection);
            Assert.IsTrue(fullDefSection.Descendants().Any(node => node.Name == "label"));
        }

        [TestMethod]
        public void GetFullDefinitionSection_HtmlContainsOnlyFullDefSection_ReturnsFullDefSection()
        {
            var html = "<div class='main'>" +
                           "<div class='definitions'>" +
                               "<div class='section definition'>" +
                                   "<label>Test Test Test</label>" +
                               "</div>'" +
                           "</div>" +
                       "</div>";

            var helper = new VocabularyHelper(html);

            var fullDefSection = helper.GetFullDefinitionSection();

            Assert.IsNotNull(fullDefSection);
            Assert.IsTrue(fullDefSection.Descendants().Any(node => node.Name == "label"));
        }

        [TestMethod]
        public void GetWordText_HtmlContainsWordText()
        {
            var html = "<div>" +
                       "<h1 class='dynamictext'>word</h1>" +
                       "</div>";

            var helper = new VocabularyHelper(html);

            var word = helper.GetWordText();

            Assert.AreEqual("word", word);
        }

        [TestMethod]
        public void GetWordText_InvalidHtmlWordTextHeaderIsNotFound_ShouldReturnEmptyString()
        {
            var html = "<div>" +
                       "<h1 class='invalid_class_value'>word</h1>" +
                       "</div>";

            var helper = new VocabularyHelper(html);

            var word = helper.GetWordText();

            Assert.AreEqual("", word);
        }

        [TestMethod]
        public void GetTypeTextFromSenseDiv()
        {
            string html = "<div class='sense'>" +
                              "<h3 class='definition'><a class='title'>adj</a></h3>" +
                              "<div class='defContent'>" +
                                  "<div class='example'>\"How fast your new car can go?\"</div>" +
                              "</div>" +
                          "</div>";

            VocabularyHelper helper = Create(html);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.FirstChild;

            var result = helper.GetTypeTextFromSenseDiv(node);

            Assert.AreEqual("adj", result);
        }

        [TestMethod]
        public void GetDefinitionTextFromSenseDiv()
        {
            string html = "<div class='sense'>" +
                  "<h3 class='definition'>" +
                          "<a class='title'>adj</a>" +
                          "\"functioning correctly and ready for action\"" +
                  "</h3>" +
                  "<div class='defContent'>" +
                      "<div class='example'>\"How fast your new car can go?\"</div>" +
                  "</div>" +
              "</div>";

            VocabularyHelper helper = Create(html);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.FirstChild;

            var result = helper.GetDefinitionTextFromSenseDiv(node);

            Assert.AreEqual("\"functioning correctly and ready for action\"", result);
        }

        [TestMethod]
        public void GetExamples_SenseContainsNoExample()
        {
            string html =
                 "<div class='sense'>" +
                    "<div class='defContent'>" +

                    "</div>" +
                  "</div>";

            VocabularyHelper helper = Create(html);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.FirstChild;

            var meanings = helper.GetIllustrationsForSense(node);

            Assert.AreEqual(0, meanings.Count);
        }

        [TestMethod]
        public void GetExamples_SenseContainsOnlyOneExample()
        {
            string html = "<div class='sense'>" +
                            "<div class='defContent'>" +
                                "<div class='example'>\"How fast your new car can go?\"</div>" +
                            "</div>" +
                          "</div>";

            VocabularyHelper helper = Create(html);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.FirstChild;

            var meanings = helper.GetIllustrationsForSense(node);

            Assert.AreEqual(1, meanings.Count);
        }

        [TestMethod]
        public void GetExamples_SenseContainsMoreThanOneExample()
        {
            string html =
                  "<div class='sense'>" +
                    "<div class='defContent'>" +
                          "<div class='example'>\"sentence1\"</div>" +
                          "<div class='example'>\"sentence2\"</div>" +
                    "</div>" +
                  "</div>";

            VocabularyHelper helper = Create(html);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.FirstChild;

            var meanings = helper.GetIllustrationsForSense(node);

            Assert.AreEqual(2, meanings.Count);
        }

        [TestMethod]
        public void GetExamples_SenseContainsStyleElement_ShouldReturnPlainText()
        {
            string html = "<div class='sense'>" +
                            "<div class='defContent'>" +
                                "<div class='example'>\"How fast your new car <strong>can</strong> go?\"</div>" +
                            "</div>" +
                          "</div>";

            VocabularyHelper helper = Create(html);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.FirstChild;

            var illustrations = helper.GetIllustrationsForSense(node);

            Assert.AreEqual("How fast your new car can go?", illustrations.First().Text);
        }

        [TestMethod]
        public void MeaningInstances_MeaningSenseDivHasNoInstanceWhatsoever_ReturnListShoudBeEmpty()
        {
            string html = "<div class='sense'>" +
                          "<div class='defContent'>" +
                          "</div>" +
                          "</div>";

            HtmlNode node = HtmlNode.CreateNode(html);
            var helper = Create(html);

            var synonyms = helper.GetSynonyms(node);
            var antonyms = helper.GetAntonyms(node);
            var examples = helper.GetExamples(node);
            var types = helper.GetTypes(node);
            var typeOf = helper.GetTypeOf(node);

            Assert.AreEqual(synonyms.Count, 0);
            Assert.AreEqual(antonyms.Count, 0);
            Assert.AreEqual(examples.Count, 0);
            Assert.AreEqual(types.Count, 0);
            Assert.AreEqual(typeOf.Count, 0);
        }

        [TestMethod]
        public void GetSynonymInstance_MeaningSenseDivHasSynonymInstanceWithDefinition_ReturnTypeShouldntBeNull()
        {
            string html = "<div class='sense'>" +
                          "<div class='defContent'>" +
                          "<dl class='instances'>" +
                          "<dt>Synonyms: </dt>" +
                          "<dd>" +
                          "<a class='word'>fake_synonym</a>" +
                          "<div class='definition'>lorem ipsum</div>" +
                          "</dd>" +
                          "</dl>" +
                          "</div>" +
                          "</div>";

            HtmlNode node = HtmlNode.CreateNode(html);
            var helper = Create(html);

            var synonyms = helper.GetSynonyms(node);

            Assert.AreEqual("lorem ipsum", synonyms.First().Definition);            
            Assert.AreEqual("fake_synonym", synonyms.First().InstanceWords.First());            
        }
        
        [TestMethod]
        public void GetSynonymInstance_MeaningSenseDivHasOneSynonymInstance_SynonymsListShouldContainOneItem()
        {
            string html = "<div class='sense'>" +
                          "<div class='defContent'>" +
                          "<dl class='instances'>" +
                          "<dt>Synonyms: </dt>" +
                          "<dd>" +
                          "<a class='word'>fake_synonym</a>" +
                          "</dd>" +
                          "</dl>" +
                          "</div>" +
                          "</div>";

            HtmlNode node = HtmlNode.CreateNode(html);
            var helper = Create(html);

            var synonyms = helper.GetSynonyms(node);

            Assert.IsTrue(synonyms.Count == 1);
        }

        [TestMethod]
        public void GetSynonymInstance_MeaningSenseDivHasNoSynonymInstance_ReturnTypeShouldBeNull()
        {
            string html = "<div class='sense'>" +
                          "<div class='defContent'>" +
                          "<dl class='instances'>" +
                          "<dt>Antonyms:</dt>" +
                          "<dd>" +
                          "<a class='word'>fake_antonym_not_synonym</a>" +
                          "</dd>" +
                          "</dl>" +
                          "</div>" +
                          "</div>";

            HtmlNode node = HtmlNode.CreateNode(html);
            var helper = Create(html);

            var synonyms = helper.GetSynonyms(node);

            Assert.IsTrue(synonyms.Count == 0);
        }

        [TestMethod]
        public void GetSpecificInstanceTypeValues_MeaningHasTwoSynonyms_ReturnListShouldContainTwoItems()
        {
            string html = "<div class='sense'>" +
                          "<div class='defContent'>" +
                          "<dl class='instances'>" +
                          "<dt>Synonyms:</dt>" +
                          "<dd>" +
                          "<a class='word'>syn1</a>" +
                          "," +
                          "<a class='word'>syn2</a>" +
                          "</dd>" +
                          "</dl>" +
                          "</div>" +
                          "</div>";

            HtmlNode node = HtmlNode.CreateNode(html);
            var helper = Create(html);

            List<VocabularyInstanceContainer> synonyms = helper.GetSynonyms(node);

            Assert.AreEqual(1, synonyms.Count);
            Assert.AreEqual("syn1", synonyms.First().InstanceWords[0]);
            Assert.AreEqual("syn2", synonyms.First().InstanceWords[1]);
        }

        [TestMethod]
        public void GetSpecificInstanceTypeValues_MeaningHasTwoLinesOfSynonyms_ReturnListShouldBothTwoItems()
        {
            string html = "<div class='sense'>" +
                              "<div class='defContent'>" +
                                  "<dl class='instances'>" +
                                  "<dt>Synonyms:</dt>" +
                                      "<dd>" +
                                          "<a class='word'>line1_syn1</a>" +
                                          "," +
                                          "<a class='word'>line1_syn2</a>" +
                                      "</dd>" +
                                      "<dd>" +
                                          "<a class='word'>line2_syn1</a>" +
                                          "," +
                                          "<a class='word'>line2_syn2</a>" +
                                          "<div class='definition'>def 2</div>" +
                                      "</dd>" +
                                  "</dl>" +
                              "</div>" +
                          "</div>";

            HtmlNode node = HtmlNode.CreateNode(html);
            var helper = Create(html);

            var synonyms = helper.GetSynonyms(node);

            Assert.AreEqual("", synonyms.First().Definition);
            Assert.AreEqual("line1_syn1", synonyms.First().InstanceWords[0]);
            Assert.AreEqual("line1_syn2", synonyms.First().InstanceWords[1]);

            Assert.AreEqual("def 2", synonyms.Last().Definition);
            Assert.AreEqual("line2_syn1", synonyms.Last().InstanceWords[0]);
            Assert.AreEqual("line2_syn2", synonyms.Last().InstanceWords[1]);

        }

        [TestMethod]
        public void MeaningInstanceTest_HasNothingExceptTypeOfInstance()
        {
            var helper = Create(_testHelper.OpenReadReturnHtmlString("vocabulary.com_uxoricide.html"));
            
            var word = helper.Populate();
            var meaning = word.FullMeaningGroups.First().Meanings.First();

            Assert.AreEqual(meaning.AntonymContainers.Any(), false);
            Assert.AreEqual(meaning.ExampleContainers.Any(), false);
            Assert.AreEqual(meaning.SynonymContainers.Any(), false);
            Assert.AreEqual(meaning.TypeContainers.Any(), false);

            Assert.AreEqual(meaning.TypeOfContainers.Any(), true);
        }

        [TestMethod]
        public void MeaningInstanceTest_HasOneLineOfTypeOfInstance_TypeOfContainerPropertyShouldHaveOneItem()
        {
            var helper = Create(_testHelper.OpenReadReturnHtmlString("vocabulary.com_uxoricide.html"));

            var word = helper.Populate();
            var meaning = word.FullMeaningGroups.First().Meanings.First();

            var typeOfContainers = meaning.TypeOfContainers;

            Assert.AreEqual(1, typeOfContainers.Count);
            //Assert.AreEqual("unlawful premeditated killing of a human being by a human being", instanceValues.Definition);
            //Assert.AreEqual("execution", instanceValues.InstanceWords[0]);
            //Assert.AreEqual("murder", instanceValues.InstanceWords[1]);
            //Assert.AreEqual("slaying", instanceValues.InstanceWords[2]);
        }

        [TestMethod]
        public void MeaningInstanceTest_()
        {
            var helper = Create(_testHelper.OpenReadReturnHtmlString("vocabulary.com_uxoricide.html"));

            var word = helper.Populate();
            var meaning = word.FullMeaningGroups.First().Meanings.First();

            var typeOfContainers = meaning.TypeOfContainers;
            var instanceValues = typeOfContainers.First();

            Assert.AreEqual("unlawful premeditated killing of a human being by a human being", instanceValues.Definition);
            Assert.AreEqual("execution", instanceValues.InstanceWords[0]);
            Assert.AreEqual("murder", instanceValues.InstanceWords[1]);
            Assert.AreEqual("slaying", instanceValues.InstanceWords[2]);
        }


        // meanin subsense has instance?

        [TestMethod]
        public void GetPrimaryMeaningGroups_HtmlContains2PrimaryMeaningGroups()
        {
            // contains 2 groups. The first one contains two meaning the other contains only one meaning.
            var html = _testHelper.OpenReadReturnHtmlString("vocabulary.com_love.html", "files");

            var helper = Create(html);

            var groups = helper.GetPrimaryMeaningGroups();

            Assert.AreEqual(2, groups.Count);
        }

        [TestMethod]
        public void GetPrimaryMeaningGroups_FirstOneOfTheTwoGroupContainsTwoMeaning()
        {
            // contains 2 groups. The first one contains two meaning the other contains only one meaning.
            var html = _testHelper.OpenReadReturnHtmlString("vocabulary.com_love.html", "files");

            var helper = Create(html);

            var groups = helper.GetPrimaryMeaningGroups();

            Assert.AreEqual(2, groups.First().Meanings.Count);
        }

        [TestMethod]
        public void GetPrimaryMeaningGroups_FirstGroupsFirstElementsTypeShouldBeNoun()
        {
            // contains 2 groups. The first one contains two meaning the other contains only one meaning.
            var html = _testHelper.OpenReadReturnHtmlString("vocabulary.com_love.html", "files");

            var helper = Create(html);

            var groups = helper.GetPrimaryMeaningGroups();

            Assert.AreEqual("n", groups.First().Meanings.First().Type);
        }

        [TestMethod]
        public void GetPrimaryMeaningGroups_WordContainsNoPrimaryMeaningSection()
        {
            var html = _testHelper.OpenReadReturnHtmlString("vocabulary.com_uxoricide.html", "files");

            var helper = Create(html);

            var groups = helper.GetPrimaryMeaningGroups();

            Assert.AreEqual(0, groups.Count);
        }

        [TestMethod]
        public void IsValidWord_InvalidHtml_ReturnsFalse()
        {
            // there's no word defined as 'opsimath' at vocabulary.com
            //var html = _testHelper.Download("https://www.vocabulary.com/dictionary/opsimath");
            var html = _testHelper.OpenReadReturnHtmlString("vocabulary.com.html", "files");

            var helper = Create(html);

            Assert.IsFalse(helper.IsValidWord());
        }

        [TestMethod]
        public void IsValidWord_ValidHtml_ReturnsTrue()
        {
            // there's no word defined as 'opsimath' at vocabulary.com
            var html = _testHelper.OpenReadReturnHtmlString("vocabulary.com_love.html", "files");

            var helper = Create(html);

            var isValid = helper.IsValidWord();

            Assert.IsTrue(isValid);
        }
    }
}
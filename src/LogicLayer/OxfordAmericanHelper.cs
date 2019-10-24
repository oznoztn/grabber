using System.Collections.Generic;
using System.Linq;
using GDomain;
using HtmlAgilityPack;
using LogicLayer.Extensions;

// Vocabulary, YourDictionary, AmericanHeritage, OxfordAmerican, Collins, Longman, Wiktionary
namespace LogicLayer
{
    public class OxfordAmericanHelper : HelperBase
    {
        private readonly List<List<List<HtmlNode>>> _mainMeanigsWithSectionsAndLiNodes;
        public OxfordAmericanHelper(string htmlString) : base(htmlString)
        {
            _mainMeanigsWithSectionsAndLiNodes = GetMainMeaningsWithDefinitionSectionsAndLiNodes();
        }

        /// <summary>
        /// True ise, kelime birbirlerinden tamamıyla farklı anlamlar barındırıyor demektir.
        /// Örneğin 'go' kelimesinin bir japon oyunu anlamına gelmesi gibi.
        /// </summary>
        public bool HasMultipleGroups => GetElements("span", "class", "hw").Count() > 1;

        public List<HtmlNode> GetDefinitionSections()
        {
            var sections = GetElements("section", "class", "gramb");
                       
            return sections.ToList();
        }

        public List<List<HtmlNode>> GetLiNodesForEachSection()
        {
            return GetDefinitionSections().Select(section => section.GetSpecificNode("ul", "class", "semb").ChildNodes.ToList()).ToList();
        }

        public HtmlNode GetSectionByGivenMeaningLi(HtmlNode li)
        {
            HtmlNode parentNode = li;
            while (parentNode.Name != "section")
            {
                parentNode = parentNode.ParentNode;
            }
            return parentNode;
        }

        public List<List<List<HtmlNode>>> GetMainMeaningsWithDefinitionSectionsAndLiNodes()
        {
            List<List<List<HtmlNode>>> mainMeaningsWithSectionsWithLiItems = new List<List<List<HtmlNode>>>();

            var entryHeaders = GetElements("div", "class", "entryHead", isExact: false).ToList();

            foreach (var entryHeader in entryHeaders)
            {
                var meaningsSectionsList = new List<List<HtmlNode>>();

                var nextSibling = entryHeader.NextSibling;
                while (nextSibling != null && nextSibling.GetAttributeValue("class", "") != "entryHead  primary_homograph")
                {
                    nextSibling = nextSibling.NextSibling;

                    if (nextSibling != null && nextSibling.Name == "section" && nextSibling.GetAttributeValue("class", "") == "gramb")
                    {
                        var section = nextSibling;
                        var ulSemb = section.GetSpecificNode("ul", "class", "semb");
                        List<HtmlNode> sectionsLiItems = ulSemb.ChildNodes.ToList();
                       
                        meaningsSectionsList.Add(sectionsLiItems);
                    }
                }

                mainMeaningsWithSectionsWithLiItems.Add(meaningsSectionsList);
            }

            return mainMeaningsWithSectionsWithLiItems;
        }

        public List<HtmlNode> GetMeaningSubsenses(HtmlNode meaningLi)
        {
            var ol = meaningLi.FirstChild.ChildNodes.FirstOrDefault(ch => ch.Name == "ol");

            //return ol?.ChildNodes.ToList();
            return ol?.ChildNodes.ToList() ?? new List<HtmlNode>();
        }
        
        /// <summary>
        /// Verilen elementten Sense veya Meaning için tanım dönderir.
        /// </summary>
        /// <param name="element">Verilen element span.ind elementini barındıran li elementi olmalı.</param>
        /// <returns></returns>
        public string GetMeaningOrSenseText(HtmlNode element)
        {
            string str = element.GetSpecificNode("span", "class", "ind")?.InnerText ?? "";

            if (string.IsNullOrWhiteSpace(str))
            {
                // cross-reference text'i var mı?
                var crossReferenceElement = element.GetSpecificNode("div", "class", "crossReference");

                if (string.IsNullOrWhiteSpace(crossReferenceElement.InnerText))
                {
                    throw new AnomalyException();
                }
                return crossReferenceElement.InnerText;
            }

            return str;
        }

        private string GetMeaningType(HtmlNode meaningLi)
        {
            var ulSemb = meaningLi.ParentNode;
            var sectionGramb = ulSemb.ParentNode;
            var h3 = sectionGramb.GetSpecificElements("h3", "class", "ps pos", true).First();
            var typeText = h3.InnerText;
            return typeText;
        }

        public string GetWordText()
        {
            var hw = GetElement("span", "class", "hw");
            if (hw==null)
            {
                return "";
            }
            var x = hw.ChildNodes.FirstOrDefault(t => t.Name == "#text");
            return x?.InnerText ?? "";
        }

        private string ClearQuotationMarks(string inputText)
        {
            if (inputText.Contains("&rsquo;") || inputText.Contains("&lsquo;"))
            {
                inputText = inputText.Replace("&lsquo;", "").Replace("&rsquo;", "");
            }
            else if (inputText.Contains("‘") || inputText.Contains("’"))
            {
                inputText = inputText.Replace("&lsquo;", "‘").Replace("&rsquo;", "’");
            }
            return inputText;
        }

        public List<Illustration> GetIllustrationsSimple(HtmlNode liElement)
        {
            var illustrations = new List<Illustration>();

            HtmlNode firstTrg = liElement.GetSpecificNode("div", "class", "trg");      
            var exgElements = firstTrg.GetSpecificElements("div", "class", "exg", isFirstGenOnly: true);
            var exElements = exgElements.SelectMany(exg => exg.ChildNodes.Where(ch => ch.GetAttributeValue("class", "") == "ex")).ToList();

            foreach (var ex in exElements)
            {
                string illustrationText = ex.Element("em").InnerText;
                string illustrationNote = ex.Element("span") == null ? "" : ex.Element("span").InnerText;

                illustrations.Add(new Illustration { Note = $"{illustrationNote}", Text = $"{ClearQuotationMarks(illustrationText)}" });
            }

            return illustrations;
        }

        public List<Illustration> GetIllustrationsAdvanced(HtmlNode liElement)
        {
            var advancedExamplesDiv = liElement.GetSpecificNode("div", "class", "examples");
            List<string> re = 
                advancedExamplesDiv?.Descendants("em").Select(exDiv => exDiv.InnerText).ToList() ?? new List<string>();

            return re.Select(illustration => new Illustration {Text = ClearQuotationMarks(illustration)}).ToList();
        }

        public string GetMeaningGrammaticalNote(HtmlNode element)
        {
            if (IsMeaning(element))
            {
                // meaning ise <p> içerisinde aramalı
                var trg = element.ChildNodes.First();
                var p = trg.FirstChild;

                var text = p.GetSpecificNode("span", "class", "grammatical_note")?.InnerText ?? "";
                return text;
            }

            var text2 = element.GetSpecificNode("span", "class", "grammatical_note")?.InnerText ?? "";
            return text2;
        }
        public int GetMeaningSectionIndex(HtmlNode incomingMeaningLi, int nthMainMeaning)
        {
            foreach (var mmainMeaning in _mainMeanigsWithSectionsAndLiNodes)
            {
                for (int j = 0; j < mmainMeaning.Count; j++)
                {
                    var meaningTypeSection = mmainMeaning[j];

                    foreach (var meaningLi in meaningTypeSection)
                    {
                        if (meaningLi.XPath == incomingMeaningLi.XPath)
                        {
                            return j;
                        }
                    }
                }
            }
            return -1;
        }
        public string GetMeaningSenseRegister(HtmlNode meaningLi)
        {
            var meaningSection = GetSectionByGivenMeaningLi(meaningLi);
            var firstLi = meaningSection.Descendants("li").First();

            if (meaningLi == firstLi)
            {
                // gelen element ilk <li> ise, sense-register elementini section içerisinde ara
                var senseRegisterElement = meaningSection.GetSpecificNode("span", "class", "sense-registers", isExact: false, isFirstGenOnly: true);

                return senseRegisterElement?.InnerText.Trim() ?? "";
            }
            else
            {
                if (IsMeaning(meaningLi))
                {
                    var trg = meaningLi.ChildNodes.First(); // trg elem
                    var p = trg.ChildNodes.First(ch => ch.Name == "p");

                    HtmlNode s = p.GetSpecificNode("span", "class", "sense-registers", isExact: false);
                    string r = s?.InnerText.Trim() ?? "";
                    return r;
                }

                // .. tersi durumda <li> içerisinde ara.
                return meaningLi.GetSpecificNode("span", "class", "sense-registers", isExact: false, isFirstGenOnly: true)?.InnerText.Trim() ?? "";
            }
        }
        public string GetMeaningRegionRegister(HtmlNode meaningLi)
        {            
            var meaningSection = GetSectionByGivenMeaningLi(meaningLi);
            var firstLi = meaningSection.Descendants("li").First();

            if (meaningLi == firstLi)
            {
                // gelen element ilk <li> ise, region-register elementini section içerisinde ara
                var senseRegionElement = meaningSection.GetSpecificNode("span", "class", "sense-regions", isExact: false, isFirstGenOnly: true);

                return senseRegionElement?.InnerText.Trim() ?? "";
            }
            else
            {
                if (IsMeaning(meaningLi))
                {
                    var trg = meaningLi.ChildNodes.First(); // trg elem
                    var p = trg.ChildNodes.First(ch => ch.Name == "p");

                    HtmlNode sregion = p.GetSpecificNode("span", "class", "sense-regions", isExact: false);
                    string senseText = sregion?.InnerText.Trim() ?? "";
                    return senseText;
                }              
                
                // .. tersi durumda <li> içerisinde ara.
                HtmlNode senseRegionRegisterContainerNode = meaningLi.GetSpecificNode("span", "class", "sense-regions", isExact: false, isFirstGenOnly: true);
                string result = senseRegionRegisterContainerNode?.InnerText.Trim() ?? "";

                return result;
            }
        }
        public string GetMeaningContext(HtmlNode meaningLi)
        {
            var meaningSection = GetSectionByGivenMeaningLi(meaningLi);
            var firstLi = meaningSection.Descendants("li").First();

            if (meaningLi == firstLi)
            {
                // gelen element ilk <li> ise, domain_labels elementini section içerisinde ara
                var senseRegionElement = meaningSection.GetSpecificNode("span", "class", "domain_labels", isExact: false, isFirstGenOnly: true);

                return senseRegionElement?.InnerText.Trim() ?? "";
            }
            else
            {
                if (IsMeaning(meaningLi))
                {
                    var trg = meaningLi.ChildNodes.First(); // trg elem
                    var p = trg.ChildNodes.First(ch => ch.Name == "p");

                    HtmlNode sregion = p.GetSpecificNode("span", "class", "sense-regions", isExact: false);
                    string senseText = sregion?.InnerText.Trim() ?? "";
                    return senseText;
                }

                // .. tersi durumda <li> içerisinde ara.
                HtmlNode senseRegisterContainerNode = meaningLi.GetSpecificNode("span", "class", "domain_labels", isExact: false, isFirstGenOnly: true);
                string result = senseRegisterContainerNode?.InnerText.Trim() ?? "";

                return result;
            }
        }
        public List<string> GetMeaningSynonyms(HtmlNode meaningLi)
        {

            HtmlNode trg = meaningLi.GetSpecificNode("div", "class", "trg");

            var synonymsDiv = trg.GetSpecificNode("div", "class", "synonyms");
            if(synonymsDiv != null)
            {

                var exgDiv = synonymsDiv.Descendants()
                    .FirstOrDefault(descNode => descNode.GetAttributeValue("class", "") == "exs");

                if (exgDiv != null)
                {
                    var synonymsString = exgDiv.InnerText;

                    return synonymsString.Split(',').Select(str => str.Trim()).ToList();
                }

            }

            return new List<string>();
        }

        /// <summary>
        /// Returns 'play at' section of the sentence '(play at) Engage in without proper seriousness or understanding'
        /// </summary>
        /// <param name="meaningOrSenseLi"></param>
        /// <returns></returns>
        public string GetUsageForm(HtmlNode meaningOrSenseLi)
        {
            if (IsMeaning(meaningOrSenseLi))
            {
                // Meaning ise <p> içerisinde
                var p = meaningOrSenseLi.Descendants("p").FirstOrDefault();
                
                var formGroup = p.GetSpecificNode("span", "class", "form-groups", isFirstGenOnly: true);

                return formGroup?.InnerText.Trim() ?? "";
            }
            else
            {
                // ... Sense, yani SubMeaning, ise direk <li> içerisinde ara.
                var spanGroup = meaningOrSenseLi.GetSpecificNode("span", "class", "form-groups", isFirstGenOnly: true);
                return spanGroup?.InnerText.Trim() ?? "";
            }
        }

        public List<string> GetWordTypes()
        {
            return GetElements("span", "class", "pos").ToList().Select(node => node.InnerText).ToList();
        }

        public bool IsValidHtml()
        {
            return GetWordText() != "";
        }

        public bool IsMeaning(HtmlNode liElement)
        {
            var subsenseIterationSpan = liElement.GetSpecificNode("span", "class", "subsenseIteration", isFirstGenOnly:true);

            return subsenseIterationSpan == null;
        }

        private OxfordAmericanMeaning ConstructMeaning(HtmlNode meaningLi)
        {
            var meaning = new OxfordAmericanMeaning();
            meaning.Text = GetMeaningOrSenseText(meaningLi);
            meaning.SenseRegister = GetMeaningSenseRegister(meaningLi);
            meaning.SenseRegion = GetMeaningRegionRegister(meaningLi);
            meaning.Context = GetMeaningContext(meaningLi);
            meaning.GrammaticalNote = GetMeaningGrammaticalNote(meaningLi);
            meaning.UsageForm = GetUsageForm(meaningLi);
            meaning.Illustrations = GetIllustrationsSimple(meaningLi);
            meaning.IllustrationsAdvanced = GetIllustrationsAdvanced(meaningLi);
            meaning.Synonyms = GetMeaningSynonyms(meaningLi);

            return meaning;
        }

        public List<OxfordAmericanWord> Populate()
        {
            List<OxfordAmericanWord> wordList = new List<OxfordAmericanWord>();
            foreach (List<List<HtmlNode>> mainMeaning in _mainMeanigsWithSectionsAndLiNodes)
            {
                // main meaning
                var word = new OxfordAmericanWord();
                word.Text = GetWordText();
                word.UsageNote = GetUsageNote();
                word.Origin = GetOriginText();

                foreach (var section in mainMeaning)
                {
                    // her bir main meaning içindeki sektörler
                    var group = new Group<OxfordAmericanMeaning>();
                    group.Type = GetMeaningType(section[0]);

                    foreach (HtmlNode meaningLi in section)
                    {
                        // section içerisindeki li item
                        OxfordAmericanMeaning meaning = ConstructMeaning(meaningLi);

                        foreach (HtmlNode subsenseLi in GetMeaningSubsenses(meaningLi))
                        {
                            OxfordAmericanMeaning subMeaning = ConstructMeaning(subsenseLi);
                            meaning.SubMeanings.Add(subMeaning);
                        }
                        group.Meanings.Add(meaning);
                    }
                    word.Groups.Add(group);
                }
                wordList.Add(word);
            }

            return wordList;
        }

        private string GetOriginText()
        {
            return "";
        }

        private string GetUsageNote()
        {
            return "";
        }

        private HtmlNode GetPhrasesSection()
        {
            var sections = GetElements("section", "class", "etymology etym");

            foreach (var sectionNode in sections)
            {
                var childNodes = sectionNode.ChildNodes;

                foreach (var childNode in childNodes)
                {
                    if (childNode.Name == "h3" && childNode.GetAttributeValue("class", "") == "phrases-title")
                    {
                        if (childNode.InnerText == "Phrases")
                        {
                            var phrasesContainerNode = sectionNode.GetSpecificNode("ul", "class", "semb gramb");

                            return phrasesContainerNode;
                        }
                    }
                }
            }

            return null;
        }

        private HtmlNode GetPhrasalVerbsSection()
        {
            var sections = GetElements("section", "class", "etymology etym");

            foreach (var sectionNode in sections)
            {
                var childNodes = sectionNode.ChildNodes;

                foreach (var childNode in childNodes)
                {
                    if (childNode.Name == "h3" && childNode.GetAttributeValue("class", "") == "phrases-title")
                    {
                        if (childNode.InnerText == "Phrasal Verbs")
                        {
                            var phrasesContainerNode = sectionNode.GetSpecificNode("ul", "class", "semb gramb");

                            return phrasesContainerNode;
                        }
                    }
                }
            }

            return null;
        }

        private HtmlNode GetUsageSection()
        {
            return null;
        }

        private HtmlNode GetOriginSection()
        {
            return null;
        }

        public List<OxfordAmericanWord> PopulatePhrases()
        {
            HtmlNode phrasesSection = GetPhrasesSection();

            List<OxfordAmericanWord> phrases = new List<OxfordAmericanWord>();

            for (int i = 0; i < phrasesSection.ChildNodes.Count; i++)
            {
                // li  ul, li ul, ...  ikilisi şeklinde gidiyor
                if (phrasesSection.ChildNodes[i].Name == "li")
                {
                    HtmlNode ul = phrasesSection.ChildNodes[i + 1];

                    OxfordAmericanWord phrase = new OxfordAmericanWord
                    {
                        Text = phrasesSection.ChildNodes[i].InnerText
                    };

                    var ulsLiItems = ul.ChildNodes.ToList();
                    foreach (var ulsLiItem in ulsLiItems)
                    {
                        // her bir (ul içindeki) li, bir anlamı temsil ediyor

                        var firstTrg = ulsLiItem.ChildNodes[0];
                        var secondTrg = ulsLiItem.ChildNodes[1];

                        if (ulsLiItem.ChildNodes.ElementAtOrDefault(2) != null)
                        {
                            // gördüğüm kadarıyla yalnızca iki tane trg var
                            // todo: log
                            throw new AnomalyException();
                        }

                        ulsLiItem.RemoveChild(ulsLiItem.ChildNodes[1]); // son trg'yi kaldır.

                        #region Creation of the div.trg                        
                        // son trg'nin child'lerini ilk trg'ye ekle.
                        foreach (var secondTrgChildNode in secondTrg.ChildNodes.ToList())
                        {
                            // ol.subSense değil ise ekle:
                            if (!(secondTrgChildNode.Name == "ol" & secondTrgChildNode.GetAttributeValue("class", "") == "subSenses"))
                                firstTrg.AppendChild(secondTrgChildNode);
                            else
                            {
                                // ol.subSense ise orda dur.
                                HtmlNodeCollection subSenseLiChildren = secondTrgChildNode.ChildNodes.First().ChildNodes;

                                var advancedExamplesNode =
                                    subSenseLiChildren.FirstOrDefault(
                                        node => node.Name == "div" && node.GetAttributeValue("class", "") == "examples");
                                var synonymsNode =
                                    subSenseLiChildren.FirstOrDefault(
                                        node => node.Name == "div" && node.GetAttributeValue("class", "") == "synonyms");
                                var crossReferenceNode =
                                    subSenseLiChildren.FirstOrDefault(
                                        node => node.Name == "div" &&
                                                node.GetAttributeValue("class", "") == "crossReference");

                                if (advancedExamplesNode != null)
                                {
                                    subSenseLiChildren.Remove(advancedExamplesNode);
                                }
                                if (synonymsNode != null)
                                {
                                    subSenseLiChildren.Remove(synonymsNode);
                                }
                                if (crossReferenceNode != null)
                                {
                                    subSenseLiChildren.Remove(crossReferenceNode);
                                }

                                HtmlNode handMadeTrg = new HtmlNode(HtmlNodeType.Element, HtmlDocument, 0);
                                handMadeTrg.Name = "div";
                                handMadeTrg.SetAttributeValue("class", "trg");
                                handMadeTrg.ChildNodes.Add(crossReferenceNode);
                                handMadeTrg.ChildNodes.Add(advancedExamplesNode);
                                handMadeTrg.ChildNodes.Add(synonymsNode);
                                subSenseLiChildren.Add(handMadeTrg);

                                firstTrg.AppendChild(secondTrgChildNode);
                            }
                        }
                        #endregion

                        OxfordAmericanMeaning meaning = ConstructMeaning(ulsLiItem);

                        // check for subsenses
                        foreach (HtmlNode subsenseLi in GetMeaningSubsenses(ulsLiItem))
                        {
                            OxfordAmericanMeaning subMeaning = ConstructMeaning(subsenseLi);
                            meaning.SubMeanings.Add(subMeaning);
                        }

                        phrase.Meanings.Add(meaning);
                    }
                    phrases.Add(phrase);
                }
            }

            return phrases;
        }

        public List<OxfordAmericanWord> PopulatePhrasalVerbs()
        {
            HtmlNode phrasesSection = GetPhrasalVerbsSection();

            List<OxfordAmericanWord> phrases = new List<OxfordAmericanWord>();

            for (int i = 0; i < phrasesSection.ChildNodes.Count; i++)
            {
                // li  ul, li ul, ...  ikilisi şeklinde gidiyor
                if (phrasesSection.ChildNodes[i].Name == "li")
                {
                    HtmlNode ul = phrasesSection.ChildNodes[i + 1];

                    OxfordAmericanWord phrase = new OxfordAmericanWord
                    {
                        Text = phrasesSection.ChildNodes[i].InnerText
                    };

                    var ulsLiItems = ul.ChildNodes.ToList();
                    foreach (var ulsMeaningLi in ulsLiItems)
                    {
                        // (ul içindeki) her bir li, bir ana anlamı temsil ediyor
                        var firstTrg = ulsMeaningLi.ChildNodes[0];
                        var secondTrg = ulsMeaningLi.ChildNodes[1];

                        if (ulsMeaningLi.ChildNodes.ElementAtOrDefault(2) != null)
                        {
                            // gördüğüm kadarıyla yalnızca iki tane trg var
                            // todo: log
                            throw new AnomalyException();
                        }
                        
                        // 2. trg'yi kaldır.
                        ulsMeaningLi.RemoveChild(secondTrg);

                        // 2. deki elementleri 1. ye ekle
                        foreach (var trgChild in secondTrg.ChildNodes.ToList())  
                        {
                            firstTrg.AppendChild(trgChild);
                        }

                        var olSubsense = firstTrg.ChildNodes.FirstOrDefault(node => node.Name == "ol" && node.GetAttributeValue("class", "") == "subSenses");
                        if (olSubsense != null)
                        {
                            foreach (var subSenseLi in olSubsense.ChildNodes.ToList())
                            {
                                var crossReferenceNode =
                                    subSenseLi.ChildNodes.FirstOrDefault(
                                        node => node.Name == "div" && node.GetAttributeValue("class", "") == "crossReference");
                                var advancedExamplesNode =
                                    subSenseLi.ChildNodes.FirstOrDefault(
                                        node => node.Name == "div" && node.GetAttributeValue("class", "") == "examples");
                                var synonymsNode =
                                    subSenseLi.ChildNodes.FirstOrDefault(
                                        node => node.Name == "div" && node.GetAttributeValue("class", "") == "synonyms");

                                if (advancedExamplesNode != null)
                                    subSenseLi.ChildNodes.Remove(advancedExamplesNode);

                                if (crossReferenceNode != null)
                                    subSenseLi.ChildNodes.Remove(crossReferenceNode);

                                if (synonymsNode != null)
                                    subSenseLi.ChildNodes.Remove(synonymsNode);

                                var customTrgDiv = HtmlDocument.CreateElement("div");
                                customTrgDiv.SetAttributeValue("class", "trg");

                                if (crossReferenceNode != null)
                                    customTrgDiv.AppendChild(crossReferenceNode);
                                if (advancedExamplesNode != null)
                                    customTrgDiv.AppendChild(advancedExamplesNode);
                                if (synonymsNode != null)
                                    customTrgDiv.AppendChild(synonymsNode);

                                subSenseLi.AppendChild(customTrgDiv);
                            }
                        }

                        OxfordAmericanMeaning meaning = ConstructMeaning(ulsMeaningLi);

                        // check for subsenses
                        foreach (HtmlNode subsenseLi in GetMeaningSubsenses(ulsMeaningLi))
                        {
                            OxfordAmericanMeaning subMeaning = ConstructMeaning(subsenseLi);
                            meaning.SubMeanings.Add(subMeaning);
                        }
                        phrase.Meanings.Add(meaning);
                    }
                    phrases.Add(phrase);
                }
            }

            return phrases;
        }

        public OxfordAmericanWord PopulateWithNth(int nthMainMeaning = 1, int nthSection = 1, int nthMeaning = 1)
        {
            var requestedMainMeaning = _mainMeanigsWithSectionsAndLiNodes[nthMainMeaning - 1];
            var requestedSection = requestedMainMeaning[nthSection - 1];
            var requestedMeaningLi = requestedSection[nthMeaning - 1];

            // main meaning
            var word = new OxfordAmericanWord();
            word.Text = GetWordText();

            var group = new Group<OxfordAmericanMeaning>();
            group.Type = GetMeaningType(requestedSection.First());

            // section içerisindeki li item
            OxfordAmericanMeaning meaning = ConstructMeaning(requestedMeaningLi);

            foreach (HtmlNode subsenseLi in GetMeaningSubsenses(requestedMeaningLi))
            {
                OxfordAmericanMeaning subMeaning = ConstructMeaning(subsenseLi);
                meaning.SubMeanings.Add(subMeaning);
            }

            group.Meanings.Add(meaning);
            word.Groups.Add(group);

            return word;
        }
    }
}

// NOT: İki tane sense region varsa örneğin US Australian gibi bu iki sense-region tek span içerisinde bulunuyor. 
// Yani GetMeaningSenseRegion metodu iş yapacaktır. 
// Bu bilgi kesin. Kelime: 'thong'
using System.Collections.Generic;
using System.Linq;
using GDomain;
using HtmlAgilityPack;
using LogicLayer.Extensions;

namespace LogicLayer
{
    public class VocabularyHelper : HelperBase
    {
        public VocabularyHelper(string htmlString) : base(htmlString)
        {

        }

        /// <summary>
        /// Kelimeye ait kýsa tanýmý getirir.
        /// </summary>
        /// <returns></returns>
        public string GetShortBlurb()
        {
            // Gets all paragraphs that have class attribute
            IEnumerable<HtmlNode> allParagraphsWithClassAttr = GetElements("p", "class");

            HtmlNode shortDefNode = allParagraphsWithClassAttr.FirstOrDefault(node => node.Attributes.Any(attr => attr.Value == "short"));

            return shortDefNode == null ? "" : shortDefNode.InnerText;
        }

        /// <summary>
        /// Kelimemye ait uzun tanýmý getirir.
        /// </summary>
        /// <returns></returns>
        public string GetLongBlurb()
        {
            // Gets all paragraphs that have class attribute
            IEnumerable<HtmlNode> allParagraphsWithClassAttr = GetElements("p", "class");

            HtmlNode shortDefNode = allParagraphsWithClassAttr.FirstOrDefault(node => node.Attributes.Any(attr => attr.Value == "long"));

            return shortDefNode == null ? "" : shortDefNode.InnerText;
        }

        public List<string> GetSpecificInstanceTypeValues(HtmlNode senseElement, VocabularyInstanceType instanceType)
        {
            return GetInstanceContainers(senseElement, instanceType).SelectMany(container => container.InstanceWords).ToList();
        }

        public List<VocabularyInstanceContainer> GetSynonyms(HtmlNode senseOrMeaningElement)
        {
            return GetInstanceContainers(senseOrMeaningElement, VocabularyInstanceType.Synonym).ToList();
        }

        public List<VocabularyInstanceContainer> GetAntonyms(HtmlNode senseOrMeaningElement)
        {
            return GetInstanceContainers(senseOrMeaningElement, VocabularyInstanceType.Antonym).ToList();
        }

        public List<VocabularyInstanceContainer> GetExamples(HtmlNode senseOrMeaningElement)
        {
            return GetInstanceContainers(senseOrMeaningElement, VocabularyInstanceType.Example).ToList();
        }

        public List<VocabularyInstanceContainer> GetTypes(HtmlNode senseOrMeaningElement)
        {
            return GetInstanceContainers(senseOrMeaningElement, VocabularyInstanceType.Types).ToList();
        }

        public List<VocabularyInstanceContainer> GetTypeOf(HtmlNode senseOrMeaningElement)
        {
            return GetInstanceContainers(senseOrMeaningElement, VocabularyInstanceType.TypeOf).ToList();
        }


        public List<VocabularyInstanceContainer> GetInstanceContainers(HtmlNode senseElement, VocabularyInstanceType instanceType)
        {
            var dlElement = GetSpecificInstanceNode(MeaningInstances(senseElement), instanceType);

            if (dlElement == null)
                return new List<VocabularyInstanceContainer>();

            List<VocabularyInstanceContainer> instanceContainersWithTheirValues = new List<VocabularyInstanceContainer>();
            var dds = dlElement.ChildNodes.Where(node => node.Name == "dd");
            foreach (var dd in dds)
            {
                VocabularyInstanceContainer instanceContainer = new VocabularyInstanceContainer();
                instanceContainer.InstanceWords = dd.ChildNodes.Where(x => x.Name == "a").Select(a => a.InnerText.Trim()).ToList();
                instanceContainer.Definition = dd.GetSpecificNode("div", "class", "definition")?.InnerText.Trim() ?? string.Empty;
                instanceContainersWithTheirValues.Add(instanceContainer);
            }

            return instanceContainersWithTheirValues;
        }

        public HtmlNode GetSpecificInstanceNode(IEnumerable<HtmlNode> dlElements, VocabularyInstanceType instanceType)
        {
            foreach (var dlElement in dlElements)
            {
                foreach (var child in dlElement.ChildNodes)
                {
                    if (child.Name != "dt")
                        continue;

                    if (child.Name == "dt")
                    {
                        switch (instanceType)
                        {
                            case VocabularyInstanceType.Synonym:
                                if (child.InnerText.Contains("Synonyms"))
                                    return dlElement;
                                break;

                            case VocabularyInstanceType.Antonym:
                                if (child.InnerText.Contains("Antonyms"))
                                    return dlElement;
                                break;

                            case VocabularyInstanceType.Types:
                                if (child.InnerText.Contains("Types"))
                                    return dlElement;
                                break;

                            case VocabularyInstanceType.TypeOf:
                                if (child.InnerText.Contains("Type of:"))
                                    return dlElement;
                                break;
                        }
                        break;
                    }
                }
            }
            return null;
        }

        public List<VocabularyInstanceContainer> GetSpecificInstanceTypeValues(VocabularyInstanceType instanceType)
        {
            return null;
        }
        public void PopulateInstanceValues(HtmlNode senseElement, VocabularyInstanceType instanceType)
        {

        }

        /// <summary>
        /// Returns all the dl elements (of class value == instances) inside of a div.sense element
        /// </summary>
        /// <param name="meaningSenseElement"></param>
        /// <returns></returns>
        public IEnumerable<HtmlNode> MeaningInstances(HtmlNode meaningSenseElement)
        {
            HtmlNode defContentDiv = meaningSenseElement.GetSpecificNode("div", "class", "defContent");

            return defContentDiv.ChildNodes.Where(child => child.Name == "dl");
        }

        public List<Group<VocabularyMeaning>> GetPrimaryMeaningGroups()
        {
            var returnList = new List<Group<VocabularyMeaning>>();

            if (HasPrimaryMeaningsSection())
            {
                HtmlNode section = GetPrimaryMeaningSection();

                HtmlNode tableNode = section.ChildNodes.FirstOrDefault(node => node.Name == "table");

                if (tableNode != null)
                {
                    var trs = tableNode.ChildNodes.Descendants("tr").ToList();

                    foreach (HtmlNode tr in trs)
                    {
                        Group<VocabularyMeaning> meaningGroup = new Group<VocabularyMeaning>();
                        List<string> posList = tr.ChildNodes.FirstOrDefault(td => td.GetAttributeValue("class", "") == "posList")?.Descendants().Where(node => node.GetAttributeValue("class", "").Contains("pos")).Select(pos => pos.InnerText).ToList();
                        var definitions = tr.Descendants("td").Last().Descendants("div").Select(node => node.InnerText.Trim()).ToList();

                        if (posList?.Count == definitions.Count)
                        {
                            for (int i = 0; i < posList.Count; i++)
                            {
                                meaningGroup.Meanings.Add(new VocabularyMeaning()
                                {
                                    Text = definitions[i],
                                    Type = posList[i]
                                });
                            }
                        }
                        else
                        {
                            throw new AnomalyException();
                        }
                        returnList.Add(meaningGroup);
                    }
                }
            }

            return returnList;
        }

        public List<Meaning> GetFullDefinitions()
        {
            return null;
        }

        /// <summary>
        /// Her bir section descendant'larý içerisinde attr class olan ve deðeri definitionNavigator olan node var mý yok mu diye kontrol ediyor. 
        /// </summary>
        /// <returns></returns>
        public bool HasPrimaryMeaningsSection()
        {
            List<HtmlNode> sections = GetSections();
            return sections.Any(sectionDiv => sectionDiv.Descendants().Any(secDesc => secDesc.Attributes.Any(secDescAttr => secDescAttr.Name == "class" && secDescAttr.Value == "definitionNavigator")));
        }

        public bool HasFullDefinitionsSection()
        {
            // Burada þimdilik bir þey yapmaya gerek yok. 
            // Her kelime de mecburen Full Definiton Section'ý bulunuyor.
            return true;
        }

        /// <summary>
        /// Html içerisindeki 'div.section definiton' elementlerini dönderir.
        /// </summary>
        /// <returns></returns>
        public List<HtmlNode> GetSections()
        {
            var returnList = GetElements("div", "class", "section definition").ToList();

            if (returnList.Count > 2)
                throw new AnomalyException(); // 2 taneden fazla olmamasý lazým.

            return returnList;
        }

        public HtmlNode GetPrimaryMeaningSection()
        {
            var sections = GetSections();

            if (sections.Any())
            {
                if (HasPrimaryMeaningsSection())
                    return sections.First();
            }

            return null;
        }

        public HtmlNode GetFullDefinitionSection()
        {
            var sections = GetSections();

            if (sections.Any())
            {
                if (HasFullDefinitionsSection())
                    return sections.Last();
            }

            return null;
        }

        public List<HtmlNode> GetMeaningGroups()
        {
            HtmlNode fullDefinitionsSection = GetFullDefinitionSection();

            List<HtmlNode> groups = fullDefinitionsSection.GetSpecificElements("div", "class", "group");

            return groups;
        }

        public string GetWordText()
        {
            return GetElement("h1", "class", "dynamictext")?.InnerText ?? "";
        }

        public string GetTypeTextFromSenseDiv(HtmlNode senseDiv)
        {
            // example: adj functioning correctly and ready for action [type_part definition_part]
            // step A and B points to gettin the type_part and definition_part of the definition respectively

            // STEP I-A
            var definitionElement =
                senseDiv.ChildNodes.FirstOrDefault(node => node.GetAttributeValue("class", "") == "definition"); // h3

            if (definitionElement == null)
                throw new AnomalyException();

            string typeText = definitionElement?.ChildNodes.FirstOrDefault(node => node.Name == "a")?.InnerText;

            if (string.IsNullOrWhiteSpace(typeText))
                throw new AnomalyException();

            return typeText;
        }

        public string GetDefinitionTextFromSenseDiv(HtmlNode senseDiv)
        {
            // STEP 1-B
            var definitionElement =
            senseDiv.ChildNodes.FirstOrDefault(node => node.GetAttributeValue("class", "") == "definition"); // h3

            if (definitionElement == null)
                throw new AnomalyException();

            string definitionText = definitionElement.ChildNodes.LastOrDefault()?.InnerText.Trim();

            if (string.IsNullOrWhiteSpace(definitionText))
                throw new AnomalyException();

            return definitionText;
        }

        public List<Illustration> GetIllustrationsForSense(HtmlNode senseDiv)
        {
            var defContentElement = senseDiv.ChildNodes.FirstOrDefault(node => node.GetAttributeValue("class", "") == "defContent"); // div

            var exampleDivs = defContentElement.GetSpecificElements("div", "class", "example");

            var illustrations = exampleDivs.Select(exampleDiv => exampleDiv.InnerText.Trim().CleanQuotationMarks());

            return illustrations.Select(illustration => new Illustration() { Text = illustration }).ToList();
        }

        public bool IsValidWord()
        {
            //HtmlNode header = HtmlDocument.DocumentNode.Descendants().FirstOrDefault(node => node.Name == "head");
            //return header.InnerText.Contains(base.Word);
            var definitionSection = GetFullDefinitionSection();

            return definitionSection != null;
        }

        public VocabularyWord Populate()
        {
            if (!IsValidWord())
                return null;

            VocabularyWord word = new VocabularyWord
            {
                Text = GetWordText(),
                ShortBlurb = GetShortBlurb(),
                LongBlurb = GetLongBlurb(),
                PrimaryMeaningGroups = GetPrimaryMeaningGroups() // PrimaryMeaning = Kýsa tanýmlar,
            };

            List<HtmlNode> groups = GetMeaningGroups();

            // her bir group "tamamýyla" farklý bir anlamý ifade eder
            foreach (HtmlNode group in groups)
            {
                //// a new group has to be created
                Group<VocabularyMeaning> groupToAdd = new Group<VocabularyMeaning>();

                // inside a div.group
                foreach (HtmlNode groupChildNode in group.ChildNodes)
                {
                    var senseDivs = groupChildNode.ChildNodes.Where(node => node.Name == "div").Where(sense => sense.Attributes.Any(attr => attr.Name == "class" && attr.Value.Contains("sense"))).ToList();

                    if (senseDivs.Any())
                    {
                        // the meaning
                        VocabularyMeaning m = new VocabularyMeaning()
                        {
                            Type = GetTypeTextFromSenseDiv(senseDivs.First()),
                            Text = GetDefinitionTextFromSenseDiv(senseDivs.First()),
                            Illustrations = GetIllustrationsForSense(senseDivs.First()),
                            Synonyms = GetSpecificInstanceTypeValues(senseDivs.First(), VocabularyInstanceType.Synonym),
                            Antonyms = GetSpecificInstanceTypeValues(senseDivs.First(), VocabularyInstanceType.Antonym),
                            AntonymContainers = GetAntonyms(senseDivs.First()),
                            SynonymContainers = GetSynonyms(senseDivs.First()),
                            ExampleContainers = GetExamples(senseDivs.First()),
                            TypeContainers = GetTypes(senseDivs.First()),
                            TypeOfContainers = GetTypeOf(senseDivs.First())
                        };

                        // it's senses
                        for (int i = 1; i < senseDivs.Count; i++)
                        {
                            HtmlNode senseElement = senseDivs[i];

                            VocabularyMeaning sense = new VocabularyMeaning()
                            {
                                Type = GetTypeTextFromSenseDiv(senseElement),
                                Text = GetDefinitionTextFromSenseDiv(senseElement),
                                Illustrations = GetIllustrationsForSense(senseElement),
                                Synonyms = GetSpecificInstanceTypeValues(senseDivs.First(), VocabularyInstanceType.Synonym),
                                Antonyms = GetSpecificInstanceTypeValues(senseDivs.First(), VocabularyInstanceType.Antonym),
                                AntonymContainers = GetAntonyms(senseDivs.First()),
                                SynonymContainers = GetSynonyms(senseDivs.First()),
                                ExampleContainers = GetExamples(senseDivs.First()),
                                TypeContainers = GetTypes(senseDivs.First()),
                                TypeOfContainers = GetTypeOf(senseDivs.First())
                            };

                            sense.Illustrations = GetIllustrationsForSense(senseElement); // examples
                            m.SubMeanings.Add(sense);
                        }

                        groupToAdd.Meanings.Add(m);
                    }
                }
                word.FullMeaningGroups.Add(groupToAdd);
            }
            return word;
        }
    }
}
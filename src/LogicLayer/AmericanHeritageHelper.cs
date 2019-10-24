using System;
using System.Collections.Generic;
using System.Linq;
using GDomain;
using HtmlAgilityPack;
using LogicLayer.Extensions;

namespace LogicLayer
{
    public class AmericanHeritageHelper : HelperBase
    {
        public AmericanHeritageHelper(string htmlString) : base(htmlString)
        {
        }

        private HtmlNode GetResultsDiv()
        {
           return GetElement("div", "results");
        }

        /// <summary>
        /// Works on the div.pvseg elements and returns phrasal verbs.
        /// </summary>
        /// <returns></returns>
        public List<Word> GetPhrasalVerbs()
        {
            HtmlNode section = GetResultsDiv();
            List<HtmlNode> nodes = section.GetSpecificElements("div", "class", "pvseg").ToList();

            List<Word> returnList = new List<Word>();
            foreach (var node in nodes)
            {
                var word = PopulateIdiomOrPhrasalVerb(node);
                returnList.Add(word);
            }

            return returnList;
        }

        public List<Word> GetIdioms()
        {
            HtmlNode section = GetResultsDiv();
            List<HtmlNode> nodes = section.GetSpecificElements("div", "class", "idmseg").ToList();

            List<Word> returnList = new List<Word>();
            foreach (var node in nodes)
            {
                var word = PopulateIdiomOrPhrasalVerb(node);
                returnList.Add(word);
            }

            return returnList;
        }

        public Word PopulateIdiomOrPhrasalVerb(HtmlNode idiomSegDiv)
        {
            Word idiom = new Word
            {
                Text = idiomSegDiv.Descendants("i").FirstOrDefault()?.InnerText
            };

            var divs = idiomSegDiv.ChildNodes.Where(idmseg => idmseg.Name == "div").ToList();

            foreach (var div in divs)
            {
                // div.ds-single veya div.ds-list olabilir.
                AmericanHeritageMeaning meaning = ConstructMeaning(div);

                // NOT 1)
                if (string.IsNullOrWhiteSpace(meaning.SenseRegister))
                {
                    bool hasSenseInformation = new Func<HtmlNode, bool>(node => node.Element("i") != null).Invoke(idiomSegDiv);
                    meaning.SenseRegister = hasSenseInformation ? idiomSegDiv.Element("i").InnerText : "";
                }

                idiom.Meanings.Add(meaning);
            }

            return idiom;
        }

        public AmericanHeritageMeaning ConstructMeaning(HtmlNode dsListDiv)
        {
            AmericanHeritageMeaning meaning = new AmericanHeritageMeaning();  
                      
            // Construct the sub meanings
            var sdsList = dsListDiv.GetSpecificElements("div", "class", "sds-list");
            if (sdsList.Any())
            {
                foreach (HtmlNode sds in sdsList)
                {
                    meaning.SubMeanings.Add(ConstructMeaning(sds));
                }
            }

            // combined span.hvrs
            meaning.TextComponents = GetTextComponents(dsListDiv);

            meaning.Text = meaning.GetMergedTextComponents;
            meaning.Nth = DetecthNth(meaning.TextComponents.First().Text);

            //meaning.DetectMeaningText();
            //meaning.DetectContext();
            //meaning.DetectUsageForm();

            // AH'de italic olan (armageddon) context, bold olan (ravage) usage form'u temsil ediyor. (Evrensel doğru olmayabilir)
            var bis = meaning.TextComponents.TakeWhile(comp => comp.Style != "text").ToList();
            
            // ilk item daima sırayı belirtiyor (1. veya a.)
            // Fakat kimi kelimelerde (propeller) sadece tanım var, ordinal olan sayı veya harf yok.
            // O yüzden if...
            if (bis.Any())
                bis.RemoveAt(0);

            foreach (var textComponent in bis)
            {
                if (textComponent.Style == "b")
                {
                    meaning.UsageForm = textComponent.Text;
                }
                else if (textComponent.Style == "i")
                {
                    var split = textComponent.Text.Split(' ');
                    foreach (var s in split)
                    {
                        if (SenseRegisterPolice.IsSenseRegister(s))
                        {
                            meaning.SenseRegister = textComponent.Text;
                            break;
                        }
                        if (SenseRegisterPolice.IsSenseRegionRegister(s))
                        {
                            meaning.SenseRegion = textComponent.Text;
                        }
                        else
                        {
                            meaning.Context = textComponent.Text;
                            // todo: log the context variable
                            break;
                        }
                    }
                }
            }

            // Context olanı GetMergedTextComponents de atlayamadığımdan, oradaki notu oku anlamadıysan
            // context bilgisi hem meanin.Text de hem de meaning.Context'de aynı anda yer alabilir
            // eğer durum buysa, sil:
            if (meaning.Context != "")
            {
                var originalTextsLength = meaning.Text.Length;
                var possiblyModifiedTextsLength = originalTextsLength;

                meaning.Text = meaning.Text.Replace($" {meaning.Context} ", "");

                if (originalTextsLength == possiblyModifiedTextsLength)
                {                   
                    meaning.Text = meaning.Text.Replace($"{meaning.Context} ", "");
                    possiblyModifiedTextsLength = meaning.Text.Length;
                }

                if (originalTextsLength == possiblyModifiedTextsLength)
                {
                    meaning.Text = meaning.Text.Replace($"{meaning.Context}", "");
                }
            }

            if (meaning.Text.Any() && !char.IsUpper(meaning.Text[0]))
            {
                // lower case ise başlıyorsa bir şeyler ters gidiyor demektir
                // aynı şu örnekteki gibi
                // often Go The starting point.
                // often xx Go The starting point. Selam ne haber.
                var q = meaning.Text.Split(' ');
                for (int i = 1; i < 2; i++) //i = 1 den başlıyor. İlk item'in lowercase ise başladığını biliyorum.
                {
                    var currentSplit = q[i];
                    var currentSplitsFirstChar = currentSplit[0];
                    if (char.IsUpper(currentSplitsFirstChar))
                    {
                        if (i != q.Length - 1)
                        {
                            var ch = q[i+1][0];
                            if (char.IsUpper(ch))
                            {
                                string remove = q[0];
                                string remove2 = q[1];
                                var newText = meaning.Text.Replace(remove, "").Replace(remove2, "").Trim();

                                if (!string.IsNullOrWhiteSpace(meaning.UsageForm))
                                {
                                    // bu metodun görevi UsageForm tespit etmek idi.
                                    // Usage Form önceden set edilmişse, neden anlamak isterim.
                                    // todo: log meaning.UsageForm & $"{remove} {remove2}"
                                }

                                meaning.UsageForm = $"{remove} {remove2}";
                                meaning.Text = newText;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // often xx Go The starting point. Selam ne haber.
                        // 1. ve 2. lowercase 3. upper. 1. 2. ve 3., 4. den ayrılmalı ve silinmeli
                        if (i != q.Length - 1)
                        {
                            if (i + 1 != q.Length - 1)
                            {
                                string remove = q[0];
                                string remove2 = q[i];
                                string remove3 = q[i + 1];
                                var newText = meaning.Text.Replace(remove, "").Replace(remove2, "").Replace(remove3, "").Trim();

                                if (!string.IsNullOrWhiteSpace(meaning.UsageForm))
                                {
                                    // bu metodun görevi UsageForm tespit etmek idi.
                                    // Usage Form önceden set edilmişse, neden anlamak isterim.
                                    // todo: log meaning.UsageForm & $"{remove} {remove2} {remove3}"
                                }

                                meaning.UsageForm = $"{remove} {remove2} {remove3}";
                                meaning.Text = newText;
                                break;
                            }
                        }
                    }
                }
                // todo: log the meaning text
            }

            // check for illustrations
            var illustrations = GetIllustrations(dsListDiv);
            foreach (string illustration in illustrations)
            {
                meaning.Illustrations.Add(new Illustration() { Text = illustration, Note = "" });
            }

            return meaning;
        }

        private int DetecthNth(string text)
        {
            text = text.Replace(".", "");

            // int (1. 2. 3.) veya karakter (a. b. c.) olabilir

            int result;
            bool isSucceed = int.TryParse(text, out result);

            if (isSucceed)
                return result;
            else
            {
                switch (text)
                {
                    case "a":
                        return 1;
                    case "b":
                        return 2;                   
                    case "c":
                        return 3;
                    case "d":
                        return 4;                   
                    case "e":
                        return 5;
                    case "f":
                        return 6;
                }
            }

            return 1;
        }

        public void SetContextSense(HtmlNode dsListDiv, AmericanHeritageMeaning meaning)
        {
            var childNodes = dsListDiv.ChildNodes.ToList();
            for (int i = 0; i < childNodes.Count; i++)
            {
                if (childNodes[i].GetAttributeValue("class", "") == "illustration")
                {
                    continue;
                }

                if (i <= 3)
                {
                    if (childNodes[i].Name == "i" && !string.IsNullOrWhiteSpace(childNodes[i].InnerText))
                    {
                        var text = childNodes[i].InnerText;

                        if (text == "Obsolete" || text == "Dated" || text == "Archaic" || text == "Formal" || text == "Informal")
                            meaning.SenseRegister = text;
                        else
                        {
                            // TODO: log the (italic) context text
                            meaning.Context = text;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nthDiscreteGroup">Tamamıyla farklı anlam barındıran kelimelerde, bu anlamlardan hangisinin istediğini belirtir.</param>
        /// <param name="nthTypeGroup">Hangi anlam tip grubunun istendiğini belirtir (v, n, adj)</param>
        /// <param name="nthMeaning">Kaçıncı anlamın istendiğini belirtir</param>
        /// <returns></returns>
        public AmericanHeritageMeaning PopulateNthMeaning(int nthDiscreteGroup = 1, int nthTypeGroup = 1, int nthMeaning = 1)
        {
            HtmlNode resultsDiv = GetResultsDiv();

            if (resultsDiv == null)
                return null;      
                  
            var diffMeaningTables = GetDsListsGroupedV2().ToList();

            var requestedDiffMeaning = diffMeaningTables[nthDiscreteGroup - 1];
            var requestedGroup = requestedDiffMeaning[nthTypeGroup - 1];
            var requestedMeaningDsList = requestedGroup[nthMeaning - 1];

            AmericanHeritageMeaning meaning = ConstructMeaning(requestedMeaningDsList);

            return meaning;
        }

        public Word PopulateNthPhrasalVerb(int nth)
        {
            HtmlNode section = GetResultsDiv();
            List<HtmlNode> nodes = section.GetSpecificElements("div", "class", "pvseg").ToList();

            var requestedNode = nodes[--nth];

            return PopulateIdiomOrPhrasalVerb(requestedNode);
        }

        public Word PopulateNthIdiom(int nth)
        {
            HtmlNode section = GetResultsDiv();
            List<HtmlNode> nodes = section.GetSpecificElements("div", "class", "idmseg").ToList();

            var requestedNode = nodes[--nth];

            return PopulateIdiomOrPhrasalVerb(requestedNode);
        }

        public List<AmericanHeritageWord> Populate()
        {
            HtmlNode resultsDiv = GetResultsDiv();

            if (resultsDiv == null)
                return new List<AmericanHeritageWord>();

            List<AmericanHeritageWord> words = new List<AmericanHeritageWord>();
            List<List<List<HtmlNode>>> discreteMeaningGroups = GetDsListsGroupedV2().ToList();
            foreach (List<List<HtmlNode>> group in discreteMeaningGroups)
            {
                AmericanHeritageWord word = new AmericanHeritageWord
                {
                    Text = GetWord(),
                    //Idioms = GetIdioms(dictionary),
                    //PhrasalVerbs = GetPhrasalVerbs(dictionary)
                };

                foreach (List<HtmlNode> pseg in group)
                {
                    var meaningGroup = new Group<AmericanHeritageMeaning>();
                    meaningGroup.Type = DetectGroupText(pseg.First());
                    foreach (var dsList in pseg)
                    {
                        AmericanHeritageMeaning meaning = ConstructMeaning(dsList);
                        meaningGroup.Meanings.Add(meaning);
                    }
                    word.Groups.Add(meaningGroup);
                }

                words.Add(word);
            }

            return words;
        }

        public string DetectGroupText(HtmlNode node)
        {
            var dsList = HtmlDocument.DocumentNode.SelectSingleNode(node.XPath);

            if (dsList != null)
            {
                HtmlNode sectionDiv = dsList.ParentNode;

                List<string> texts = sectionDiv.ChildNodes.Where(ch => ch.Name != "div").Where(n => !string.IsNullOrWhiteSpace(n.InnerText)).Select(n => n.InnerText).ToList();

                if (!texts.Any())
                {
                    // ANOMALİ
                    // Tip değeri olmayan div.pseg olamaz, olmamalı -ne de olsa her bir anlamın bir tipi olmalı; noun, verb, adj, ... Hangisi? 
                    throw new AnomalyException();
                }

                string aggragated = texts.Aggregate((i, j) => i + " " + j);
                return aggragated;
            }

            return "";
        }

        public List<List<List<HtmlNode>>> GetDsListsGroupedV2()
        {
            HtmlNode resultsDiv = GetResultsDiv();

            // TODO
            var completelyDifferentMeaningTables = resultsDiv.Descendants("table").ToList();

            var returnList2 = new List<List<List<HtmlNode>>>();
            foreach (var definitonTable in completelyDifferentMeaningTables)
            {                
                // her bir tablo tamamıyla farklı anlamlar barındırıyor (go için mesela iki tane var)
                List<List<HtmlNode>> meaningTypeGroups = new List<List<HtmlNode>>();

                var psegList = definitonTable.Descendants().Where(node => node.GetAttributeValue("class", "") == "pseg").ToList();
                foreach (var pseg in psegList)
                {
                    List<HtmlNode> innerList;
                    // her bir pseg bir grubu temsil ediyor (v tr, v intr, adj gibi)

                    // ya birden fazla ds-list vardır ya da tek bir ds-single vardır
                    bool hasAnyDsList = pseg.Descendants("div").Any(div => div.GetAttributeValue("class", "") == "ds-list");

                    if (hasAnyDsList)
                    {
                        // ds-list
                        innerList = pseg.GetSpecificElements("div", "class", "ds-list").ToList();
                    }
                    else
                    {
                        // ds-single
                        innerList = pseg.GetSpecificElements("div", "class", "ds-single").ToList();
                    }

                    if (innerList.Any())
                        meaningTypeGroups.Add(innerList);
                }

                returnList2.Add(meaningTypeGroups);
            }

            return returnList2;
        }

        public bool HasDicreetMeanings()
        {
            var section = GetResultsDiv();

            if (section != null)
            {
                var tables = section.Descendants("table");
                var divsInsideTables = tables.SelectMany(table => table.Descendants("div"));
                var countOfRtsegDivs = divsInsideTables.Count(div => div.GetAttributeValue("class", "") == "rtseg");
                return countOfRtsegDivs > 1;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">div.ds-list element</param>
        /// <returns></returns>
        public List<string> GetIllustrations(HtmlNode node)
        {
            // possible values for illustrationText:
            //   the pleasures of love; a night of love. 
            //   We Love our parents. I love my wife.

            var list = new List<string>();

            var illustrations = node.ChildNodes.Where(t => t.Name == "font").ToList();

            if (illustrations.Count == 2)
            {
                var text = illustrations[0].InnerText;
                var author = illustrations[1].InnerText;
                list.Add($"{text} {author}");
                return list;
            }

            var illustrationElement = node.ChildNodes.FirstOrDefault(t => t.Name == "font");
            var illustrationText = illustrationElement?.InnerText;

            if (illustrationText != null && illustrationText.Contains("..."))
            {
                list.Add(illustrationText);
                return list;
            }

            if (!string.IsNullOrWhiteSpace(illustrationText) && illustrationText.Contains(';') || !string.IsNullOrWhiteSpace(illustrationText) && illustrationText.Contains('.'))
            {
                var splitted = illustrationText.Split(';', '.');
                if (splitted.Length > 1)
                {
                    foreach (string split in splitted)
                    {
                        if (split.Any())
                        {
                            if (char.IsLetter(split.Last()))
                                list.Add(split.Trim() + ".");
                            else
                                list.Add(split.Trim());
                        }

                    }
                }
            }
            if (illustrationText != null && list.Count == 0)
                list.Add(illustrationText);


            return list;
        }

        public string GetWord()
        {
            try
            {
                // checking for title element
                var title = HtmlDocument.DocumentNode.Descendants("title").SingleOrDefault();

                var items = title?.InnerText.Split(':');

                if (items != null && items.Any())
                    return items.Last().Trim();

                return "";
            }
            catch (InvalidOperationException e)
            {
                return e.Message;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Takes a div.ds-list or div.sds-list and populates TextComponent items by reading ChildNodes of the given node.
        /// Skips the nodes that either has a value of 'div' in the Name property or has empty or whitespace value in the Name property.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<TextComponent> GetTextComponents(HtmlNode node)
        {
            var returnList = new List<TextComponent>();

            foreach (var childNode in node.ChildNodes)
            {
                if (childNode.Name == "div")
                    continue;

                if (string.IsNullOrWhiteSpace(childNode.InnerText))
                    continue;

                if (childNode.Name == "font")
                    continue;

                returnList.Add(new TextComponent
                {
                    Style = childNode.Name.Replace("#", ""),
                    Text = childNode.InnerText.Trim()
                });
            }

            return returnList;
        }

        public List<TextComponent> GetUsageNoteTextComponents()
        {
            HtmlNode section = GetResultsDiv();
            HtmlNode usageNoteNode = section.GetSpecificNode("div", "class", "usen");
            List<TextComponent> textComponents = GetTextComponents(usageNoteNode);           
            return textComponents;
        }

        public List<TextComponent> GetWordEtymologyTextComponents()
        {
            HtmlNode section = GetResultsDiv();
            HtmlNode node = section.GetSpecificNode("div", "class", "usen");
            List<TextComponent> textComponents = GetTextComponents(node);
            return textComponents;
        }

        public List<TextComponent> GetWordHistoryTextComponents()
        {
            HtmlNode section = GetResultsDiv();
            HtmlNode node = section.GetSpecificNode("div", "class", "wrdhst");
            List<TextComponent> textComponents = GetTextComponents(node);
            return textComponents;
        }
    }
}

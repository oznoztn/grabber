using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GDomain;
using HtmlAgilityPack;
using LogicLayer.Extensions;

namespace LogicLayer
{
    public static class TfdDictionariesExtensions
    {
        public static string ToHtmlSectionValue(this DictionariesEnum dictionariesEnum)
        {
            switch (dictionariesEnum)
            {
                case DictionariesEnum.AmericanHeritage:
                    return "hm";
                case DictionariesEnum.HarperCollinsEnglishDictionary:
                    return "hc_dict";
                case DictionariesEnum.RandomHouseKernermanWebster:
                    return "rHouse";
                case DictionariesEnum.DictionaryOfUnfamiliarWords:
                    return "shUnfW";
                default:
                    return "";
            }
        }
    }

    public enum DictionariesEnum
    {
        AmericanHeritage,
        OxfordAmerican, 
        Vocabulary,
        HarperCollinsEnglishDictionary,
        RandomHouseKernermanWebster,
        DictionaryOfUnfamiliarWords
    }

    public class TheFreeDictionaryHelper : HelperBase
    {
        public TheFreeDictionaryHelper(string htmlString) : base(htmlString)
        {
            //if (CheckForCompletelyDifferentMeanings(DictionariesEnum.AmericanHeritage))
            //{
            //    // TODO: NOTIFY & LOG
            //}
        }

        public List<HtmlNode> GetPsegList(DictionariesEnum dictionary)
        {
            var section = GetElement("section", "data-src", dictionary.ToHtmlSectionValue());
            return section.GetSpecificElements("div", "class", "pseg", isFirstGenOnly: true).ToList();
        }

        /// <summary>
        /// Works on the div.pvseg elements in the given dictionary's section element and returns phrasal verbs.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public List<Word> GetPhrasalVerbs(DictionariesEnum dictionary)
        {
            HtmlNode section = GetElement("section", "data-src", dictionary.ToHtmlSectionValue());
            List<HtmlNode> nodes = section.GetSpecificElements("div", "class", "pvseg", isFirstGenOnly: true).ToList();

            return PopulateIdiomsOrPhrasalVerbs(nodes);
        }

        public List<Word> GetIdioms(DictionariesEnum dictionary)
        {
            HtmlNode section = GetElement("section", "data-src", dictionary.ToHtmlSectionValue());
            List<HtmlNode> nodes = section.GetSpecificElements("div", "class", "idmseg", isFirstGenOnly: true).ToList();

            return PopulateIdiomsOrPhrasalVerbs(nodes);
        }

        public List<Word> PopulateIdiomsOrPhrasalVerbs(List<HtmlNode> nodes)
        {
            var phrasalVerbsList = new List<Word>();

            foreach (HtmlNode idiomSegDiv in nodes)
            {
                Word idiom = new Word();
                idiom.Text = idiomSegDiv.Descendants("i").FirstOrDefault()?.InnerText;

                bool hasSenseInformation = new Func<HtmlNode, bool>(node => node.Element("i") != null).Invoke(idiomSegDiv);

                var divs = idiomSegDiv.ChildNodes.Where(idmseg => idmseg.Name == "div").ToList();

                foreach (var div in divs)
                {
                    // div.ds-single veya div.ds-list olabilir.
                    TheFreeDictionaryMeaning meaning = ConstructMeaning(div);

                    // NOT 1)
                    if (string.IsNullOrWhiteSpace(meaning.SenseRegister))
                        meaning.SenseRegister = hasSenseInformation ? idiomSegDiv.Element("i").InnerText : "";

                    idiom.Meanings.Add(meaning);
                    phrasalVerbsList.Add(idiom);
                }
            }

            return phrasalVerbsList;
        }
        
        public TheFreeDictionaryMeaning ConstructMeaning(HtmlNode dsListDiv)
        {
            TheFreeDictionaryMeaning meaning = new TheFreeDictionaryMeaning();
            SetContextSense(dsListDiv, meaning);

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
            
            // AH'de italic olan (armageddon) context, bold olan (ravage) usage form'u temsil ediyor. (Evrensel doðru olmayabilir)
            var bis = meaning.TextComponents.TakeWhile(comp => comp.Style != "text").ToList();
            // ilk item daima sýrayý belirtiyor (1. veya a.)
            // Fakat kimi kelimelerde (propeller) sadece taným var, ordinal olan sayý veya harf yok.
            // O yüzden if...
            if(bis.Any())
                bis.RemoveAt(0); 

            foreach (var textComponent in bis)
            {
                if (textComponent.Style == "b")
                {
                    meaning.UsageForm = textComponent.Text;
                }
                else if (textComponent.Style == "i")
                {
                    if(IsContextText(textComponent.Text))
                        meaning.Context = textComponent.Text;
                }
            }
            
            // check for illustrations
            var illustrations = GetIllustrations(dsListDiv);
            foreach (string illustration in illustrations)
            {
                meaning.Illustrations.Add(new Illustration() { Text = illustration, Note = "" });
            }

            return meaning;
        }

        public bool IsContextText(string text)
        {
            if (text == "Obsolete" || text == "Dated" || text == "Archaic" || text == "Formal" || text == "Informal")
                return false;

            // TODO: Log the text
            return true;
        }

        public void SetContextSense(HtmlNode dsListDiv, TheFreeDictionaryMeaning meaning)
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
                            // log the italic text
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

        public Word Populate(DictionariesEnum dictionary)
        {
            HtmlNode psegSection = GetElement("section", "data-src", dictionary.ToHtmlSectionValue());

            if (psegSection == null)
                return new Word();

            TheFreeDictionaryWord word = new TheFreeDictionaryWord()
            {
                Text = GetWord(),
                //Idioms = GetIdioms(dictionary),
                //PhrasalVerbs = GetPhrasalVerbs(dictionary)
            };

            var psegList = GetDsListsGrouped(DictionariesEnum.AmericanHeritage).ToList();

            foreach (List<HtmlNode> pseg in psegList)
            {
                var meaningGroup = new Group<TheFreeDictionaryMeaning>();
                meaningGroup.Type = DetectGroupText(pseg.First());
                foreach (var dsList in pseg)
                {
                    TheFreeDictionaryMeaning meaning = ConstructMeaning(dsList);
                    meaningGroup.Meanings.Add(meaning);
                }
                word.Groups.Add(meaningGroup);
            }

            return word;
        }

        public string DetectGroupText(HtmlNode node)
        {
            var dsList = HtmlDocument.DocumentNode.SelectSingleNode(node.XPath);

            if (dsList != null)
            {
                HtmlNode sectionDiv = dsList.ParentNode;

                List<string> texts =
                    sectionDiv.ChildNodes
                        .Where(ch => ch.Name != "div")
                        .Where(n => !string.IsNullOrWhiteSpace(n.InnerText)).Select(n => n.InnerText).ToList();

                if (!texts.Any())
                {
                    // ANOMALÝ
                    // Tip deðeri olmayan div.pseg olamaz, olmamalý -ne de olsa her bir anlamýn bir tipi olmalý; noun, verb, adj, ... Hangisi? 
                    throw new AnomalyException();
                }

                string aggragated = texts.Aggregate((i, j) => i + " " + j);
                return aggragated;
            }

            return "";
        }

        /// <summary>
        /// Section'a ait div.ds-list'leri dönderir.
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        //public IEnumerable<HtmlNode> GetDsLists(DictionariesEnum dict)
        //{
        //    HtmlNode node = GetElement("section", "data-src", dict.ToHtmlSectionValue());
                        
        //    if (node != null)
        //        return node.GetSpecificElements("div", "class", "ds-list");
        //    else
        //        return new List<HtmlNode>();
        //}

        public List<List<HtmlNode>> GetDsListsGrouped(DictionariesEnum dictionary)
        {
            HtmlNode sectionNode = GetElement("section", "data-src", dictionary.ToHtmlSectionValue());
            

            var returnList = new List<List<HtmlNode>>();

            var psegList = sectionNode?.ChildNodes.Where(node => node.GetAttributeValue("class", "") == "pseg").ToList();
            if (psegList != null)
            {
                bool hasDsListElements = psegList.Any(pseg => pseg.GetSpecificElements("div", "class", "ds-list").Any());

                // Direk Select operatörünü kullanmamamýn nedeni þu:
                // PSEG daima var fakat ds-list herzaman olmayabilir. Ds-list'in olmadýðý durumda her bir pseg için List<HtmlNode> oluþturuluyor fakat ds-list olmadýðý için oluþturulan listenin içi boþ oluyor.
                //returnList = psegList.Select(pseg => pseg.GetSpecificElements("div", "class", "ds-list")).ToList();

                if (hasDsListElements)
                {
                    // ds-list'lerden oluþmuþ (ds-list varsa en az iki tane vardýr, aksi durumda zaten ds-single bulunur)
                    returnList = psegList.Select(pseg => pseg.GetSpecificElements("div", "class", "ds-list")).ToList();
                }
                else
                {
                    // section boþ deðil, ds-list yok, o zaman ds-single (tek taným, sub meaning yok) vardýr.
                    returnList = psegList.Select(pseg => pseg.GetSpecificElements("div", "class", "ds-single")).ToList();
                }
            }

            if (returnList.Any())
            {
                // pseg için oluþturulan liste içinde hiç HtmlNode yoksa, 
                // yani liste boþsa, 
                // bu boþ olanlarý hariç tut.
                var emptiesExcluded = returnList.Where(innerList => innerList.Any()).ToList();
                return emptiesExcluded;
            }

            return returnList;
        }


        public bool CheckForCompletelyDifferentMeanings(DictionariesEnum dictionary)
        {
            var section = GetElement("section", "data-src", dictionary.ToHtmlSectionValue());

            if (section != null)
            {
                HtmlNode h2 = section.Element("h2");

                bool result = h2.ChildNodes.Any(n => n.Name == "sup");

                return result;
            }

            return false;
        }

        public List<string> GetIllustrations(HtmlNode node)
        {
            // possible values for illustrationText:
            //   the pleasures of love; a night of love. 
            //   We Love our parents. I love my wife.

            var list = new List<string>();
            string illustrationText = node.GetSpecificNode("span", "class", "illustration", isFirstGenOnly:true)?.InnerText;

            if (node.GetSpecificElements("span", "class", "illustration", isFirstGenOnly: true).Count > 1)
            {
                string aggragatedString = node.GetSpecificElements("span", "class", "illustration", isFirstGenOnly: true).Select(n => n.InnerText).Aggregate((x, y) => x + " " + y);
                illustrationText = aggragatedString;
            }
            
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
            if(illustrationText != null && list.Count == 0)
                list.Add(illustrationText);


            return list;
        }

        public string GetWord()
        {
            try
            {
                // checking for title element
                var title = HtmlDocument.DocumentNode.Descendants("title").SingleOrDefault();

                var items = title?.InnerText.Split(' ');

                if (items != null && items.Any())
                    return items[0];

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

                if (childNode.GetAttributeValue("class", "") == "illustration")
                    continue;

                returnList.Add(new TextComponent
                {
                    Style = childNode.Name.Replace("#", ""),
                    Text = childNode.InnerText.Trim()
                });
            }

            return returnList;
        }
    }
}

/*
    go to the wall [Italic]Informal
      1. To lose a conflict or be defeated; yield: Despite their efforts, the team went to the wall.
      2. To be forced into bankruptcy; fail.
      3. To make an all-out effort, especially in defending another.
*/
// Kimi zaman, üstteki örnekteki gibi, birden fazla anlamý olan bir idiom'da kapsayýcý bir sense register bulunur.
// bu durumda eðer submeaning'de sense register yoksa ve kapsayýcý bir sense register varsa s
// submeaning'e parent meaning'in sense register'i verilir.


// Alttaki if ifadesi þunu amaçlýyor:
// Yalnýzca Meaning'in sense-register'i olmadýðý durumda kapsayýcý sense-register olup olmadýðýna bak. 
// Eðer, alttaki örnekteki gibi, meaning'in kendi sense register'i varsa bir þey yapma:

/*
    some idiom 
        1. [Italic]Informal First_definiton.
        2. Second definition

    Not: Böyle bir örneðe hiç rastlamadý.
 */
//// NOT 1


using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace LogicLayer
{
    public class HelperBase
    {
        protected readonly HtmlDocument HtmlDocument;
        public HelperBase(string htmlString)
        {
            HtmlDocument = new HtmlDocument();
            HtmlDocument.LoadHtml(htmlString);
        }

        /// <summary>
        /// Verilen element tipinde olan ve id attribute'ý idValue ile eþleþen elementi getirir.
        /// </summary>
        /// <param name="element">Element tipi: div, section, vs.</param>
        /// <param name="idValue">Element id attribute deðeri</param>
        /// <returns></returns>
        public HtmlNode GetElement(string element, string idValue)
        {
            IEnumerable<HtmlNode> requestedElements = GetElements(element);
            HtmlNode reqElement = requestedElements.FirstOrDefault(reqElemAttributes => reqElemAttributes.Attributes.Any(attr => attr.Name == "id" && attr.Value == idValue));
            return reqElement;
        }

        /// <summary>
        /// Verilen deðerlere uygun 'single' bir HtmlNode veya HtmlNode için 'default' deðer dönderir.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public HtmlNode GetElement(string element, string attr, string value)
        {
            IEnumerable<HtmlNode> requestedElements = GetElements(element, attr, value);
            return requestedElements.FirstOrDefault();
        }

        /// <summary>
        /// Ýstenen bütün elementleri getirir.
        /// </summary>
        /// <param name="element">Ýstenen element</param>
        /// <returns></returns>
        public IEnumerable<HtmlNode> GetElements(string element)
        {
            return HtmlDocument.DocumentNode.Descendants(element);
        }

        /// <summary>
        /// Belirtilen attribute'a sahip olan elementleri getirir.
        /// </summary>
        /// <param name="element">Ýstenen element</param>
        /// <param name="attribute">Sahip olmasý gereken attribute</param>
        /// <returns></returns>
        public IEnumerable<HtmlNode> GetElements(string element, string attribute)
        {
            return GetElements(element).Where(givenElement => givenElement.Attributes.Contains(attribute));
        }

        /// <summary>
        /// Belirtilen attribute'a ve attribute deðerine sahip elementleri getirir
        /// </summary>
        /// <param name="element">Ýstenen element</param>
        /// <param name="attribute">Sahip olmasý gereken attribute</param>
        /// <param name="value">Sahip olmasý gereken attribute deðeri</param>
        /// <param name="isExact"></param>
        /// <returns></returns>
        public IEnumerable<HtmlNode> GetElements(string element, string attribute, string value, bool isExact = true)
        {
            if(isExact)
                return GetElements(element, attribute)
                    .Where(node => node.Attributes.Any(attr => attr.Name == attribute && attr.Value == value));

            return GetElements(element, attribute)
                .Where(node => node.Attributes.Any(attr => attr.Name == attribute && attr.Value.Contains(value)));

        }
    }
}
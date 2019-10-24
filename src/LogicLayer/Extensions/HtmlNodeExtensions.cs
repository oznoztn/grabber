using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace LogicLayer.Extensions
{
    public static class HtmlNodeExtensions
    {
        /// <summary>
        /// Parent node i�erisindeki verilen parametrelere uygun HtmlNode'lar�n bulundu�u bir liste d�nderir.
        /// </summary>
        /// <param name="htmlNode"></param>
        /// <param name="element"></param>
        /// <param name="attribute"></param>
        /// <param name="isFirstGenOnly">Returns a list of elements satisfy the specified conditions and belong to the first generation, if it is set to true.</param>
        /// <returns></returns>
        public static List<HtmlNode> GetSpecificElements(this HtmlNode htmlNode, string element, string attribute, string value, bool isFirstGenOnly = false)
        {
            if (isFirstGenOnly)
                return htmlNode.Elements(element)
                    .Where(node => node.Attributes.Any(attr => attr.Name == attribute && attr.Value == value))
                    .ToList();

            return htmlNode.Descendants()
                .Where(node => node.Attributes.Any(attr => attr.Name == attribute && attr.Value == value))
                .ToList();
        }

        /// <summary>
        /// Parent node i�erisinde verilen parametrelere uygun tek bir HtmlNode d�nderir. Birden fazla bulgu var ise ilkini d�nderir.
        /// </summary>
        /// <param name="htmlNode">Parent node</param>
        /// <param name="element"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <param name="isExact"></param>       
        /// <param name="isFirstGenOnly"></param>        
        /// <returns></returns>
        public static HtmlNode GetSpecificNode(this HtmlNode htmlNode, string element, string attribute, string value, bool isExact = true, bool isFirstGenOnly = false)
        {
            IEnumerable<HtmlNode> query;

            query = isFirstGenOnly ? htmlNode.Elements(element) : htmlNode.Descendants(element);

            if (isExact)
            {
                query = query.Where(node => node.Attributes.Any(attr => attr.Name == attribute && attr.Value == value));
            }
            else
            {
                query = query.Where(node => node.Attributes.Any(attr => attr.Name.Contains(attribute) && attr.Value.Contains(value)));
            }

            return query.FirstOrDefault();
        }
    }
}
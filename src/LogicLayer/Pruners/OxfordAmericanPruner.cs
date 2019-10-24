using System.Linq;
using HtmlAgilityPack;

namespace LogicLayer.Pruners
{
    public class OxfordAmericanPruner : Pruner
    {
        public OxfordAmericanPruner(string htmlContent) : base(htmlContent)
        {

        }

        public override string Prune()
        {
            var document = new HtmlDocument();
            document.LoadHtml(HtmlString);

            var descendants = document.DocumentNode.Descendants();
            foreach (var descendant in descendants)
            {
                if (descendant.Name == "div")
                {
                    if (descendant.GetAttributeValue("class", "") == "lex-content")
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(descendant.WriteContentTo());

                        CleanSocials(descendant);
                        CleanBreadCrumbs(descendant);

                        return descendant.WriteContentTo();
                    }
                }
            }

            return null;
        }

        private void CleanSocials(HtmlNode node)
        {
            var socials = node.DescendantsAndSelf().Where(des => des.Name == "div" && des.GetAttributeValue("class", "") == "socials").ToList();
            foreach (var htmlNode in socials)
            {
                htmlNode.Remove();
            }
        }

        private void CleanBreadCrumbs(HtmlNode node)
        {
            HtmlNode breadcrumbs =
                node.Descendants().FirstOrDefault(t => t.Name == "div" && t.GetAttributeValue("class", "") == "breadcrumbs layout");
            breadcrumbs?.Remove();
        }
    }
}
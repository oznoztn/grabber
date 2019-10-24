using System;
using System.Linq;
using HtmlAgilityPack;
using LogicLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicLayerTests
{
    [TestClass]
    public class GrabberHelperTests
    {

        [TestMethod]
        public void GetDivs_GetAllDivs_True()
        {
            string html = "<body>" +
                          "<div>" +
                          "<div></div>" +
                          "<div id='divSomeId'></div>" +
                          "<div class='green-div'></div>" +
                          "</div>" +
                          "</body>";
            HelperBase tfd = new HelperBase(html);
            int count = tfd.GetElements("div").Count();

            Assert.AreEqual(count, 4);
        }

        [TestMethod]
        public void GetElements_ShouldNotCountDivsHaveDifferentAttrThanGivenAttr_True()
        {
            string html = "<div>" +
                          "<div data='someData'></div>" +
                          "<div id='divTx'></div>" +
                          "<div class='divTt'></div>" +
                          "<div class='green-div'></div>" +
                          "</div>";

            HelperBase tfd = CreateTheFreeDictionaryHelper(html);

            int divsWithIdAttributeCount = tfd.GetElements("div", "id").Count();
            int divsClassAttributeCount = tfd.GetElements("div", "class").Count();

            Assert.AreEqual(divsWithIdAttributeCount, 1);
            Assert.AreEqual(divsClassAttributeCount, 2);
        }


        [TestMethod]
        public void GetElement_ShouldReturnTheElementByItsId()
        {
            string html = "<body>" +
                          "<div id='superDiv'></div>" +
                          "<div id='megaDiv'></div>" +
                          "<input id='superInput' type='button' class='btn-green'/>" +
                          "</body>";

            HelperBase tfd = CreateTheFreeDictionaryHelper(html);

            HtmlNode htmlNodex = tfd.GetElement("div", "megaDiv");
            HtmlNode htmlNodey = tfd.GetElement("input", "superInput");

            Assert.AreEqual(htmlNodex.Id, "megaDiv");
            Assert.AreEqual(htmlNodey.Attributes.Contains("class"), true);
        }

        [TestMethod]
        public void GetElement_ShouldReturnNullIfNoMathchingElementWithSpecifiedIdFound()
        {
            string html = "<body>" +
                          "<div id='superDiv'></div>" +
                          "</body>";

            HelperBase tfd = CreateTheFreeDictionaryHelper(html);

            HtmlNode htmlNodex = tfd.GetElement("div", "deneme");

            Assert.AreEqual(htmlNodex, null);
        }

        [TestMethod]
        public void GetElementsMatchGivenAtrributeAndVal_ShouldCountProperly()
        {
            string html = "<div>" +
                          "<section data-src='hm'> </section>" +
                          "<section data-src='hc_dict'> </section>" +
                          "<div class='ds-list'></div>" +
                          "<div class='ds-list'></div>" +
                          "<div class='ds-list'></div>" +
                          "</div>";
            HelperBase helper = CreateTheFreeDictionaryHelper(html);

            var matchesx = helper.GetElements("section", "data-src", "hm");
            var matchesy = helper.GetElements("div", "class", "ds-list");

            Assert.AreEqual(matchesx.Count(), 1);
            Assert.AreEqual(matchesy.Count(), 3);
        }

        [TestMethod]
        public void GetElementsMatchGivenAtrributeAndVal_ShouldReturnZeroIfNothingsFound()
        {
            string html = "<div>" +
                          "<section data-src='hm'> </section>" +
                          "<section data-src='hc_dict'> </section>" +
                          "</div>";
            HelperBase helper = CreateTheFreeDictionaryHelper(html);

            var matchesx = helper.GetElements("div", "class", "ds-list");

            Assert.AreEqual(matchesx.Count(), 0);
        }


        public HtmlDocument CreateHtmlDocument(string htmlString)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlString);
            return htmlDocument;
        }

        public HelperBase CreateTheFreeDictionaryHelper(string html)
        {
            return new HelperBase(html);
        }
    }
}

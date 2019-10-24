using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using LogicLayer.Pruners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicLayerTests
{
    public class OxfordAmericanPrunerFacade : Pruner
    {
        public OxfordAmericanPrunerFacade(string htmlString) : base(htmlString)
        {
        }

        public override string Prune()
        {
            return new OxfordAmericanPruner(HtmlString).Prune();
        }
    }

    public class VocabularyPrunerFacade : Pruner
    {
        public VocabularyPrunerFacade(string htmlString) : base(htmlString)
        {
        }

        public override string Prune()
        {
            // Bunun vocabulary için olduğunu farzet.
            return new VocabularyPruner(HtmlString).Prune();
        }
    }

    public class PrunerGenericFacade
    {
        private readonly Pruner _pruner;

        public PrunerGenericFacade(Pruner pruner)
        {
            _pruner = pruner;
        }

        public string Prune()
        {
            return _pruner.Prune();
        }

        // imitates the client call
        public PrunerGenericFacade()
        {
            PrunerGenericFacade pruner = new PrunerGenericFacade(new OxfordAmericanPruner(""));
        }
    }
}

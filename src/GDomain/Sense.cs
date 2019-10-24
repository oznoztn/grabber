using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GDomain
{
    public class Sense
    {
        public int Nth { get; set; }
        public string Text { get; set; } // for now, only used by VocabularyHelper
        public string Type { get; set; } // for now, only used by VocabularyHelper
        public List<TextComponent> TextComponents { get; set; }
        public string Context { get; set; }
        public List<string> Illustrations { get; set; }
        public string GetMergedTextComponents
        {
            get
            {
                if (!TextComponents.Any())
                    return "";

                StringBuilder sbuilder = new StringBuilder();

                for (int index = 0; index < TextComponents.Count; index++)
                {
                    sbuilder.Append(TextComponents[index].Text);

                    if (index != TextComponents.Count - 1)
                        sbuilder.Append(" ");
                }

                return sbuilder.ToString();
            }
        }
    }
}
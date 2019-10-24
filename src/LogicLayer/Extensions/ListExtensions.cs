using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDomain;

namespace LogicLayer.Extensions
{
    public static class ListExtensions
    {
        public static string Merge(this List<TextComponent> textComponents)
        {
            if (!textComponents.Any())
                return "";

            StringBuilder sbuilder = new StringBuilder();

            for (int index = 0; index < textComponents.Count; index++)
            {
                string text = textComponents[index].Text;
                if (text == "Obsolete" || text == "Dated" || text == "Archaic" || text == "Formal" || text == "Informal")
                    continue;

                sbuilder.Append(textComponents[index].Text);

                if (index != textComponents.Count - 1)
                    sbuilder.Append(" ");
            }

            // sırayla boşluk varsa (örnek: merhaba  ne haber?) fazlalık olan boşluğu kaldırır
            for (int index = 0; index < sbuilder.Length; index++)
            {
                if (sbuilder[index] == ' ' && index + 1 < sbuilder.Length && sbuilder[index + 1] == ' ')
                {
                    sbuilder.Remove(index, 1);
                }

                // SUNSTROKE: Also called insolation , siriasis .
                if (char.IsPunctuation(sbuilder[index]))
                {
                    // noktalama işaretinden bir önceki karakter boş olamaz!
                    if (char.IsWhiteSpace(sbuilder[index - 1]))
                    {
                        sbuilder.Remove(index - 1, 1);
                    }
                }
            }

            // sondan trim
            if (sbuilder.Length != 0 && sbuilder[sbuilder.Length - 1] == ' ')
            {
                sbuilder.Remove(sbuilder.Length - 1, 1);
            }

            // son 
            if (sbuilder.Length != 0 && char.IsPunctuation(sbuilder[sbuilder.Length - 1]) && sbuilder[sbuilder.Length - 1] != '.')
            {
                sbuilder.Remove(sbuilder.Length - 1, 1);
                sbuilder.Append('.');
            }

            return sbuilder.ToString();
        }
    }
}
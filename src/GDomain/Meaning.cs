using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace GDomain
{
    public class Illustration
    {
        public string Text { get; set; }
        public string Note { get; set; }
    }

    public class Meaning
    {
        public Meaning()
        {
            var properties = this.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.GetType().IsValueType)
                {
                    propertyInfo.SetValue(this, Activator.CreateInstance(propertyInfo.GetType()));
                }
                else
                {
                    if (typeof(string) == propertyInfo.PropertyType)
                    {
                        var t = propertyInfo.GetSetMethod(true);
                        if (propertyInfo.GetSetMethod(true) != null)
                        {
                            propertyInfo.SetValue(this, "");
                        }
                    }
                }
            }

            SubMeanings = new List<Meaning>();
            Illustrations = new List<Illustration>();
        }

        public int Nth { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }

        /// <summary>
        /// For example, 'Techonology', 'Computing'
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Formal, Informal, Archaic, Dated, Obsolete, etc.
        /// </summary>
        public string SenseRegister { get; set; }

        /// <summary>
        /// British, North American, Chiefly British.
        /// </summary>
        public string SenseRegion { get; set; }
        
        /// <summary>
        /// For example 'play at'
        /// </summary>
        public string UsageForm { get; set; }

        /// <summary>
        /// For example 'predicative', 'no object', etc.
        /// </summary>
        public string GrammaticalNote { get; set; }

        public List<Meaning> SubMeanings { get; set; }
        public List<Illustration> Illustrations { get; set; }
        public List<string> Synonyms { get; set; }
        public List<string> Antonyms { get; set; }  
    }

    public class VocabularyInstanceContainer
    {
        public string Definition { get; set; }

        /// <summary>
        /// The words.
        /// </summary>
        public List<string> InstanceWords { get; set; }
    }

    public enum VocabularyInstanceType
    {
        Synonym,
        Antonym,
        Types,
        TypeOf,
        Example
    }
    public class VocabularyMeaning : Meaning
    {
        public List<VocabularyInstanceContainer> SynonymContainers { get; set; }
        public List<VocabularyInstanceContainer> AntonymContainers { get; set; }
        public List<VocabularyInstanceContainer> ExampleContainers { get; set; }
        public List<VocabularyInstanceContainer> TypeContainers { get; set; }
        public List<VocabularyInstanceContainer> TypeOfContainers { get; set; }
    }

    public class AmericanHeritageMeaning : TheFreeDictionaryMeaning
    {
        public AmericanHeritageMeaning()
        {
            SubMeanings = new List<Meaning>();
            TextComponents = new List<TextComponent>();

            var properties = this.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.GetType().IsValueType)
                {
                    propertyInfo.SetValue(this, Activator.CreateInstance(propertyInfo.GetType()));
                }
                else
                {
                    if (typeof(string) == propertyInfo.PropertyType)
                    {
                        var t = propertyInfo.GetSetMethod(true);
                        if (propertyInfo.GetSetMethod(true) != null)
                        {
                            propertyInfo.SetValue(this, "");
                        }
                    }
                }
            }
        }
        public List<Group<OxfordAmericanMeaning>> Groups { get; set; }
        public List<TextComponent> TextComponents { get; set; }
        public string GetMergedTextComponents
        {
            get
            {
                if (!TextComponents.Any())
                    return "";

                StringBuilder sbuilder = new StringBuilder();

                for (int index = 0; index < TextComponents.Count; index++)
                {
                    string text = TextComponents[index].Text;
                    
                    // todo: connect to the database and check for if the text is a sense register. 
                    if (text == "Obsolete" || text == "Dated" || text == "Archaic" || text == "Formal" || text == "Informal" || text == "Slang")
                        continue;

                    // Aynı Sense-Register olanı atlar gibi Context olanı da atlamam lazım 
                    // fakat context sınırsız miktarda olabilir. O yüzden yapamam.
                    // İlerde db de contextleri toplarsam kontrol edebilirm

                    // Kaçıncı şey olduğunu belirten stringi dahil etme
                    if (index == 0)
                    {
                        if (TextComponents[index].Style == "b" && char.IsNumber(TextComponents[index].Text, 0))
                            continue;

                        switch (TextComponents[index].Text)
                        {
                            case "a.":
                                continue;
                            case "b.":
                                continue;
                            case "c.":
                                continue;
                            case "d.":
                                continue;
                            case "e.":
                                continue;
                            case "f.":
                                continue;
                        }
                    }
                    sbuilder.Append(TextComponents[index].Text);

                    if (index != TextComponents.Count - 1)
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

    public class TheFreeDictionaryMeaning : Meaning
    {
        public TheFreeDictionaryMeaning()
        {
            SubMeanings = new List<Meaning>();
            TextComponents = new List<TextComponent>();

            var properties = this.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.GetType().IsValueType)
                {
                    propertyInfo.SetValue(this, Activator.CreateInstance(propertyInfo.GetType()));
                }
                else
                {
                    if (typeof(string) == propertyInfo.PropertyType)
                    {
                        var t = propertyInfo.GetSetMethod(true);
                        if (propertyInfo.GetSetMethod(true) != null)
                        {
                            propertyInfo.SetValue(this, "");
                        }
                    }
                }
            }
        }
        public List<Group<TheFreeDictionaryMeaning>> Groups { get; set; }
        public List<TextComponent> TextComponents { get; set; }
        public string GetMergedTextComponents
        {
            get
            {
                if (!TextComponents.Any())
                    return "";

                StringBuilder sbuilder = new StringBuilder();

                for (int index = 0; index < TextComponents.Count; index++)
                {
                    string text = TextComponents[index].Text;
                    if (text == "Obsolete" || text == "Dated" || text == "Archaic" || text == "Formal" || text == "Informal")
                        continue;

                    // Kaçıncı şey olduğunu belirten stringi dahil etme
                    if (index == 0)
                    {
                        if (TextComponents[index].Style == "b" && char.IsNumber(TextComponents[index].Text, 0))
                            continue;

                        switch (TextComponents[index].Text)
                        {
                            case "a.":
                                continue;
                            case "b.":
                                continue;
                            case "c.":
                                continue;
                            case "d.":
                                continue;
                            case "e.":
                                continue;
                            case "f.":
                                continue;
                        }
                    }
                    sbuilder.Append(TextComponents[index].Text);

                    if (index != TextComponents.Count - 1)
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

                //// son 
                //if (sbuilder.Length != 0 && char.IsPunctuation(sbuilder[sbuilder.Length - 1]) && sbuilder[sbuilder.Length - 1] != '.')
                //{
                //    sbuilder.Remove(sbuilder.Length - 1, 1);
                //    sbuilder.Append('.');
                //}

                return sbuilder.ToString();
            }
        }
    }

    public class OxfordAmericanMeaning : Meaning
    {
        public OxfordAmericanMeaning()
        {
            SubMeanings = new List<OxfordAmericanMeaning>();
            IllustrationsAdvanced = new List<Illustration>();
            Groups = new List<Group<OxfordAmericanMeaning>>();
        }
        public List<OxfordAmericanMeaning> SubMeanings { get; set; }
        public List<Group<OxfordAmericanMeaning>> Groups { get; set; }
        public List<Illustration> IllustrationsAdvanced { get; set; }

    }
}

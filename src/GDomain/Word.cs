using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace GDomain
{
    public class Word
    {
        public Word()
        {
            Meanings = new List<Meaning>();
            Groups = new List<Group<Meaning>>();
        }

        public string Text { get; set; }
        public string UsageNote { get; set; }
        public string Origin { get; set; }

        public List<Meaning> Meanings { get; set; }
        public List<Word> Idioms { get; set; }
        public List<Word> PhrasalVerbs { get; set; }
        public List<Group<Meaning>> Groups { get; set; }
    }

    public class VocabularyWord : Word
    {
        public VocabularyWord()
        {
            FullMeaningGroups = new List<Group<VocabularyMeaning>>();
            PrimaryMeaningGroups = new List<Group<VocabularyMeaning>>();
        }

        public string ShortBlurb { get; set; }
        public string LongBlurb { get; set; }
        public List<Group<VocabularyMeaning>> PrimaryMeaningGroups { get; set; }
        
        /// <summary>
        /// Every single group refers to the meanings that are inside the full meanings section in the html, 
        /// not to different word types as in Oxford American Meaning class.
        /// </summary>
        public List<Group<VocabularyMeaning>> FullMeaningGroups { get; set; }
    }

    public class OxfordAmericanWord : Word
    {
        public OxfordAmericanWord()
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
                        if (propertyInfo.GetSetMethod(true) != null)
                        {
                            propertyInfo.SetValue(this, "");
                        }
                    }
                }
            }

            Meanings = new List<OxfordAmericanMeaning>();
            Groups = new List<Group<OxfordAmericanMeaning>>();
        }

        public new List<OxfordAmericanMeaning> Meanings { get; set; }
        public new List<Group<OxfordAmericanMeaning>> Groups { get; set; }
    }

    public class AmericanHeritageWord : Word
    {
        public AmericanHeritageWord()
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
                        if (propertyInfo.GetSetMethod(true) != null)
                        {
                            propertyInfo.SetValue(this, "");
                        }
                    }
                }
            }

            Meanings = new List<AmericanHeritageMeaning>();
            Groups = new List<Group<AmericanHeritageMeaning>>();
        }

        public new List<AmericanHeritageMeaning> Meanings { get; set; }
        public new List<Group<AmericanHeritageMeaning>> Groups { get; set; }
    }

    public class TheFreeDictionaryWord : Word
    {
        public TheFreeDictionaryWord()
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
                        if (propertyInfo.GetSetMethod(true) != null)
                        {
                            propertyInfo.SetValue(this, "");
                        }
                    }
                }
            }

            Meanings = new List<TheFreeDictionaryMeaning>();
            Groups = new List<Group<TheFreeDictionaryMeaning>>();
        }

        public new List<TheFreeDictionaryMeaning> Meanings { get; set; }
        public new List<Group<TheFreeDictionaryMeaning>> Groups { get; set; }
    }

}
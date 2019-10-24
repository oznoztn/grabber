using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GData.Ef6.Entities
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public class Illustration : IEntity
    {
        public int Id { get; set; }
        public int MeaningId { get; set; }
        public string Text { get; set; }

        /// <summary>
        /// E.g. 'ironic', etc.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// E.g. 'as modifier', etc.
        /// </summary>
        public string GrammaticalNote { get; set; }
        public virtual Meaning Meaning { get; set; }
    }

    public enum WordType
    {
        Word,
        Idiom,
        PhrasalVerb,
    }

    public class PartOfSpeech : IEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class WordList : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WordCount { get; set; }
        public ICollection<WordListEntry> WordListEntries { get; set; }
    }

    public class WordListEntry : IEntity
    {
        public int Id { get; set; }
        public int WordId { get; set; }
        public Word Word { get; set; }
    }

    public class Dictionary : IEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int WordCount { get; set; }
        public ICollection<WordDictionaryMapping> WordDictionaryMaps { get; set; }
    }

    public class WordDictionaryMapping : IEntity
    {
        public int Id { get; set; }
        public int DictionaryId { get; set; }
        public int WordId { get; set; }
        public int PartOfSpeechId { get; set; }

        public PartOfSpeech PartOfSpeech { get; set; }
        public Dictionary Dictionary { get; set; }
        public Word Word { get; set; }
    }

    public class Word : IEntity
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string Text { get; set; }

        public ICollection<WordDictionaryMapping> WordDictionaryMaps { get; set; }
        public WordType WordType { get; set; }
    }
    public enum Degree
    {
        Failed = 0,
        Ok = 1,
        Repeat = 3
    }

    //public class PhrasalVerb : Word
    //{
    //    public int ParentId { get; set; }
    //    public Word Parent { get; set; }
    //}

    //public class Idiom : Word
    //{
    //    public int ParentId { get; set; }
    //    public Word Parent { get; set; }
    //}

    // todo: sub meaning olayı halledikecek
    public class Meaning : IEntity
    {
        public int Id { get; set; }
        public int WordDictionaryMappingId { get; set; }

        /// <summary>
        /// 'Techonology', 'Computing', 'Medicine', or the like.
        /// </summary>
        public int? MeaningContexId { get; set; }

        /// <summary>
        /// Formal, Informal, Archaic, Dated, Obsolete, or the like.
        /// </summary>
        public int? SenseRegisterId { get; set; }

        /// <summary>
        /// Indicates whether the meaning is a sub meaning or not.
        /// </summary>
        public bool IsSub { get; set; }

        public string Text { get; set; }

        /// <summary>
        /// British, North American, Chiefly British, or the like.
        /// </summary>
        public string SenseRegion { get; set; }

        /// <summary>
        /// For example 'play at'
        /// </summary>
        public string UsageForm { get; set; }

        /// <summary>
        /// For example 'predicative', 'no object' etc.
        /// </summary>
        public string GrammaticalNote { get; set; }

        public MeaningContext MeaningContext { get; set; }
        public SenseRegister SenseRegister { get; set; }
        public WordDictionaryMapping WordDictionaryMapping { get; set; }
        public ICollection<Illustration> Illustrations { get; set; }
    }

    public class MeaningContext : IEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class SenseRegister : IEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }





}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using GData.Ef6.Entities;

namespace GData.Ef6.MappingConfigurations
{
    public class WordConfiguration : EntityTypeConfiguration<Word>
    {
        public WordConfiguration()
        {

        }
    }

    public class DictionaryConfiguration : EntityTypeConfiguration<Dictionary>
    {
        public DictionaryConfiguration()
        {
            ToTable("Dictionaries");
        }
    }

    public class WordDictionaryMappingConfiguration : EntityTypeConfiguration<WordDictionaryMapping>
    {
        public WordDictionaryMappingConfiguration()
        {
            HasKey(t => new {t.Id, t.DictionaryId, t.WordId, t.PartOfSpeechId});
        }
    }

    public class WordListConfiguration : EntityTypeConfiguration<WordList>
    {
        public WordListConfiguration()
        {

        }
    }

    public class WordListEntryConfiguration : EntityTypeConfiguration<WordListEntry>
    {
        public WordListEntryConfiguration()
        {
            ToTable("WordListEntries");
        }
    }

    public class MeaningConfiguration : EntityTypeConfiguration<Meaning>
    {
        public MeaningConfiguration()
        {
            
            HasOptional(m => m.SenseRegister).WithMany().HasForeignKey(m => m.SenseRegisterId);
            HasOptional(m => m.MeaningContext).WithMany().HasForeignKey(m => m.MeaningContexId);
        }
    }

    public class SenseRegisterConfiguration : EntityTypeConfiguration<SenseRegister>
    {
        public SenseRegisterConfiguration()
        {
            
        }
    }

    public class MeaningContextConfiguration : EntityTypeConfiguration<MeaningContext>
    {
        public MeaningContextConfiguration()
        {
            
        }
    }

    public class IllustrationConfiguration : EntityTypeConfiguration<Illustration>
    {
        public IllustrationConfiguration()
        {

        }
    }
}

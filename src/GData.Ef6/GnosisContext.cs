using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GData.Ef6.Entities;
using GData.Ef6.Entities.InternetCatalog;
using GData.Ef6.MappingConfigurations;

namespace GData.Ef6
{
    public class GnosisContext : DbContext
    {
        public DbSet<Word> Words { get; set; }
        public DbSet<Dictionary> Dictionaries { get; set; }
        public DbSet<WordDictionaryMapping> WordDictionaryMappings { get; set; }

        public DbSet<WordList> WordLists { get; set; }
        public DbSet<WordListEntry> WordListEntries { get; set; }

        public DbSet<Meaning> Meanings { get; set; }
        public DbSet<MeaningContext> MeaningContexts { get; set; }
        public DbSet<SenseRegister> SenseRegisters { get; set; }

        public DbSet<Illustration> Illustrations { get; set; }

        public IDbSet<DictionaryInfo> DictionaryInfos { get; set; }
        public DbSet<DownloadLog> DownloadLogs { get; set; }
        public DbSet<DownloadEntry> DownloadEntries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new WordConfiguration());
            modelBuilder.Configurations.Add(new DictionaryConfiguration());
            modelBuilder.Configurations.Add(new WordDictionaryMappingConfiguration());

            modelBuilder.Configurations.Add(new WordListConfiguration());
            modelBuilder.Configurations.Add(new WordListEntryConfiguration());

            modelBuilder.Configurations.Add(new MeaningConfiguration());
            modelBuilder.Configurations.Add(new MeaningContextConfiguration());
            modelBuilder.Configurations.Add(new SenseRegisterConfiguration());

            modelBuilder.Configurations.Add(new IllustrationConfiguration());

            modelBuilder.Configurations.Add(new DictionaryInfoConfiguration());
            modelBuilder.Configurations.Add(new DownloadEntryConfiguration());
            modelBuilder.Configurations.Add(new DownloadLogConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    } 
}

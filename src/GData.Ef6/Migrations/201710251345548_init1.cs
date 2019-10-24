namespace GData.Ef6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dictionaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        WordCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WordDictionaryMappings",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        DictionaryId = c.Int(nullable: false),
                        WordId = c.Int(nullable: false),
                        PartOfSpeechId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Id, t.DictionaryId, t.WordId, t.PartOfSpeechId })
                .ForeignKey("dbo.Dictionaries", t => t.DictionaryId, cascadeDelete: true)
                .ForeignKey("dbo.PartOfSpeeches", t => t.PartOfSpeechId, cascadeDelete: true)
                .ForeignKey("dbo.Words", t => t.WordId, cascadeDelete: true)
                .Index(t => t.DictionaryId)
                .Index(t => t.WordId)
                .Index(t => t.PartOfSpeechId);
            
            CreateTable(
                "dbo.PartOfSpeeches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Words",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Illustrations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MeaningId = c.Int(nullable: false),
                        Text = c.String(),
                        Note = c.String(),
                        GrammaticalNote = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Meanings", t => t.MeaningId, cascadeDelete: true)
                .Index(t => t.MeaningId);
            
            CreateTable(
                "dbo.Meanings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MeaningContexId = c.Int(),
                        SenseRegisterId = c.Int(),
                        IsSub = c.Boolean(nullable: false),
                        Text = c.String(),
                        SenseRegion = c.String(),
                        UsageForm = c.String(),
                        GrammaticalNote = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MeaningContexts", t => t.MeaningContexId)
                .ForeignKey("dbo.SenseRegisters", t => t.SenseRegisterId)
                .Index(t => t.MeaningContexId)
                .Index(t => t.SenseRegisterId);
            
            CreateTable(
                "dbo.MeaningContexts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SenseRegisters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WordListEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WordId = c.Int(nullable: false),
                        WordList_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Words", t => t.WordId, cascadeDelete: true)
                .ForeignKey("dbo.WordLists", t => t.WordList_Id)
                .Index(t => t.WordId)
                .Index(t => t.WordList_Id);
            
            CreateTable(
                "dbo.WordLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        WordCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WordListEntries", "WordList_Id", "dbo.WordLists");
            DropForeignKey("dbo.WordListEntries", "WordId", "dbo.Words");
            DropForeignKey("dbo.Meanings", "SenseRegisterId", "dbo.SenseRegisters");
            DropForeignKey("dbo.Meanings", "MeaningContexId", "dbo.MeaningContexts");
            DropForeignKey("dbo.Illustrations", "MeaningId", "dbo.Meanings");
            DropForeignKey("dbo.WordDictionaryMappings", "WordId", "dbo.Words");
            DropForeignKey("dbo.WordDictionaryMappings", "PartOfSpeechId", "dbo.PartOfSpeeches");
            DropForeignKey("dbo.WordDictionaryMappings", "DictionaryId", "dbo.Dictionaries");
            DropIndex("dbo.WordListEntries", new[] { "WordList_Id" });
            DropIndex("dbo.WordListEntries", new[] { "WordId" });
            DropIndex("dbo.Meanings", new[] { "SenseRegisterId" });
            DropIndex("dbo.Meanings", new[] { "MeaningContexId" });
            DropIndex("dbo.Illustrations", new[] { "MeaningId" });
            DropIndex("dbo.WordDictionaryMappings", new[] { "PartOfSpeechId" });
            DropIndex("dbo.WordDictionaryMappings", new[] { "WordId" });
            DropIndex("dbo.WordDictionaryMappings", new[] { "DictionaryId" });
            DropTable("dbo.WordLists");
            DropTable("dbo.WordListEntries");
            DropTable("dbo.SenseRegisters");
            DropTable("dbo.MeaningContexts");
            DropTable("dbo.Meanings");
            DropTable("dbo.Illustrations");
            DropTable("dbo.Words");
            DropTable("dbo.PartOfSpeeches");
            DropTable("dbo.WordDictionaryMappings");
            DropTable("dbo.Dictionaries");
        }
    }
}

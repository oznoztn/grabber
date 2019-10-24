namespace GData.Ef6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initttt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Words", "TypeId", c => c.Int(nullable: false));
            AddColumn("dbo.Words", "WordType", c => c.Int(nullable: false));
            AddColumn("dbo.Meanings", "WordDictionaryMappingId", c => c.Int(nullable: false));
            AddColumn("dbo.Meanings", "WordDictionaryMapping_Id", c => c.Int());
            AddColumn("dbo.Meanings", "WordDictionaryMapping_DictionaryId", c => c.Int());
            AddColumn("dbo.Meanings", "WordDictionaryMapping_WordId", c => c.Int());
            AddColumn("dbo.Meanings", "WordDictionaryMapping_PartOfSpeechId", c => c.Int());
            CreateIndex("dbo.Meanings", new[] { "WordDictionaryMapping_Id", "WordDictionaryMapping_DictionaryId", "WordDictionaryMapping_WordId", "WordDictionaryMapping_PartOfSpeechId" });
            AddForeignKey("dbo.Meanings", new[] { "WordDictionaryMapping_Id", "WordDictionaryMapping_DictionaryId", "WordDictionaryMapping_WordId", "WordDictionaryMapping_PartOfSpeechId" }, "dbo.WordDictionaryMappings", new[] { "Id", "DictionaryId", "WordId", "PartOfSpeechId" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Meanings", new[] { "WordDictionaryMapping_Id", "WordDictionaryMapping_DictionaryId", "WordDictionaryMapping_WordId", "WordDictionaryMapping_PartOfSpeechId" }, "dbo.WordDictionaryMappings");
            DropIndex("dbo.Meanings", new[] { "WordDictionaryMapping_Id", "WordDictionaryMapping_DictionaryId", "WordDictionaryMapping_WordId", "WordDictionaryMapping_PartOfSpeechId" });
            DropColumn("dbo.Meanings", "WordDictionaryMapping_PartOfSpeechId");
            DropColumn("dbo.Meanings", "WordDictionaryMapping_WordId");
            DropColumn("dbo.Meanings", "WordDictionaryMapping_DictionaryId");
            DropColumn("dbo.Meanings", "WordDictionaryMapping_Id");
            DropColumn("dbo.Meanings", "WordDictionaryMappingId");
            DropColumn("dbo.Words", "WordType");
            DropColumn("dbo.Words", "TypeId");
        }
    }
}

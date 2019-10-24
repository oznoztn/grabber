namespace GData.Ef6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class t : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DownloadEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Word = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DownloadLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WordToDownloadId = c.Int(nullable: false),
                        DictionaryId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        DownloadStatus = c.Int(nullable: false),
                        DownloadEntry_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dictionaries", t => t.DictionaryId, cascadeDelete: true)
                .ForeignKey("dbo.DownloadEntries", t => t.DownloadEntry_Id)
                .Index(t => t.DictionaryId)
                .Index(t => t.DownloadEntry_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DownloadLogs", "DownloadEntry_Id", "dbo.DownloadEntries");
            DropForeignKey("dbo.DownloadLogs", "DictionaryId", "dbo.Dictionaries");
            DropIndex("dbo.DownloadLogs", new[] { "DownloadEntry_Id" });
            DropIndex("dbo.DownloadLogs", new[] { "DictionaryId" });
            DropTable("dbo.DownloadLogs");
            DropTable("dbo.DownloadEntries");
        }
    }
}

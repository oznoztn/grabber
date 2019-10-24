namespace GData.Ef6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class t3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DownloadLogs", "DictionaryId", "dbo.Dictionaries");
            DropIndex("dbo.DownloadLogs", new[] { "DictionaryId" });
            CreateTable(
                "dbo.DictionaryInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        DownloadUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DownloadLogs", "DictionaryInfoId", c => c.Int(nullable: false));
            CreateIndex("dbo.DownloadLogs", "DictionaryInfoId");
            AddForeignKey("dbo.DownloadLogs", "DictionaryInfoId", "dbo.DictionaryInfoes", "Id", cascadeDelete: true);
            DropColumn("dbo.DownloadLogs", "DictionaryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DownloadLogs", "DictionaryId", c => c.Int(nullable: false));
            DropForeignKey("dbo.DownloadLogs", "DictionaryInfoId", "dbo.DictionaryInfoes");
            DropIndex("dbo.DownloadLogs", new[] { "DictionaryInfoId" });
            DropColumn("dbo.DownloadLogs", "DictionaryInfoId");
            DropTable("dbo.DictionaryInfoes");
            CreateIndex("dbo.DownloadLogs", "DictionaryId");
            AddForeignKey("dbo.DownloadLogs", "DictionaryId", "dbo.Dictionaries", "Id", cascadeDelete: true);
        }
    }
}

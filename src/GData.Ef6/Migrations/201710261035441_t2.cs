namespace GData.Ef6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class t2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DownloadLogs", "DownloadEntry_Id", "dbo.DownloadEntries");
            DropIndex("dbo.DownloadLogs", new[] { "DownloadEntry_Id" });
            RenameColumn(table: "dbo.DownloadLogs", name: "DownloadEntry_Id", newName: "DownloadEntryId");
            AddColumn("dbo.DownloadLogs", "DownloadStatusId", c => c.Int(nullable: false));
            AlterColumn("dbo.DownloadLogs", "DownloadEntryId", c => c.Int(nullable: false));
            CreateIndex("dbo.DownloadLogs", "DownloadEntryId");
            AddForeignKey("dbo.DownloadLogs", "DownloadEntryId", "dbo.DownloadEntries", "Id", cascadeDelete: true);
            DropColumn("dbo.DownloadLogs", "WordToDownloadId");
            DropColumn("dbo.DownloadLogs", "StatusId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DownloadLogs", "StatusId", c => c.Int(nullable: false));
            AddColumn("dbo.DownloadLogs", "WordToDownloadId", c => c.Int(nullable: false));
            DropForeignKey("dbo.DownloadLogs", "DownloadEntryId", "dbo.DownloadEntries");
            DropIndex("dbo.DownloadLogs", new[] { "DownloadEntryId" });
            AlterColumn("dbo.DownloadLogs", "DownloadEntryId", c => c.Int());
            DropColumn("dbo.DownloadLogs", "DownloadStatusId");
            RenameColumn(table: "dbo.DownloadLogs", name: "DownloadEntryId", newName: "DownloadEntry_Id");
            CreateIndex("dbo.DownloadLogs", "DownloadEntry_Id");
            AddForeignKey("dbo.DownloadLogs", "DownloadEntry_Id", "dbo.DownloadEntries", "Id");
        }
    }
}

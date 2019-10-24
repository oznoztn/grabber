namespace GData.Ef6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class t4 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DictionaryInfoes", newName: "DictionaryInfos");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.DictionaryInfos", newName: "DictionaryInfoes");
        }
    }
}

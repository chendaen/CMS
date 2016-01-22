namespace CMS.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CardBalanceViewModels",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CardNo = c.String(nullable: false),
                        Balance = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CardBalanceViewModels");
        }
    }
}

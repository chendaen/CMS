namespace CMS.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public  void zcf()
        {
        }
        public override void Up()
        {
            AddColumn("dbo.RechargeViewModels", "Operate", c => c.String(nullable: false));
            DropColumn("dbo.RechargeViewModels", "Balance");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RechargeViewModels", "Balance", c => c.Double(nullable: false));
            DropColumn("dbo.RechargeViewModels", "Operate");
        }
    }
}

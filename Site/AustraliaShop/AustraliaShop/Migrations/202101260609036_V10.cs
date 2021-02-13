namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "AvailableForDealOfDay", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "SoldInDealOfDay", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "SoldInDealOfDay");
            DropColumn("dbo.Products", "AvailableForDealOfDay");
        }
    }
}

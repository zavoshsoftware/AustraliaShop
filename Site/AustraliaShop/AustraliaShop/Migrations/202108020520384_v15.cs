namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v15 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "CountryId", "dbo.Countries");
            DropIndex("dbo.Orders", new[] { "CountryId" });
            AlterColumn("dbo.Orders", "CountryId", c => c.Int());
            CreateIndex("dbo.Orders", "CountryId");
            AddForeignKey("dbo.Orders", "CountryId", "dbo.Countries", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "CountryId", "dbo.Countries");
            DropIndex("dbo.Orders", new[] { "CountryId" });
            AlterColumn("dbo.Orders", "CountryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Orders", "CountryId");
            AddForeignKey("dbo.Orders", "CountryId", "dbo.Countries", "Id", cascadeDelete: true);
        }
    }
}

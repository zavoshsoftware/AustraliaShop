namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V09 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "DealOfDayExpireDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "DealOfDayExpireDate");
        }
    }
}

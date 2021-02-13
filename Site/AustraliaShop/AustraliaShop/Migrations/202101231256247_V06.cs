namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V06 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductComments", "Rate", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductComments", "Rate");
        }
    }
}

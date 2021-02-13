namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V02 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ProductGroups", "Code");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductGroups", "Code", c => c.Int());
        }
    }
}

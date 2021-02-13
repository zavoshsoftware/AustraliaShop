namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V08 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "State", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "State");
        }
    }
}

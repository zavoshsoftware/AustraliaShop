namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V07 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "FullName", c => c.String(nullable: false, maxLength: 350));
            AlterColumn("dbo.Users", "CellNum", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "CellNum", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Users", "FullName", c => c.String(nullable: false, maxLength: 250));
        }
    }
}

namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Colors",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        HexCode = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletionDate = c.DateTime(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Products", "ColorId", c => c.Guid());
            AddColumn("dbo.Products", "ParentId", c => c.Guid());
            CreateIndex("dbo.Products", "ColorId");
            CreateIndex("dbo.Products", "ParentId");
            AddForeignKey("dbo.Products", "ColorId", "dbo.Colors", "Id");
            AddForeignKey("dbo.Products", "ParentId", "dbo.Products", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "ParentId", "dbo.Products");
            DropForeignKey("dbo.Products", "ColorId", "dbo.Colors");
            DropIndex("dbo.Products", new[] { "ParentId" });
            DropIndex("dbo.Products", new[] { "ColorId" });
            DropColumn("dbo.Products", "ParentId");
            DropColumn("dbo.Products", "ColorId");
            DropTable("dbo.Colors");
        }
    }
}

namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V04 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductSizes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SizeId = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletionDate = c.DateTime(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Sizes", t => t.SizeId, cascadeDelete: true)
                .Index(t => t.SizeId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Sizes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Order = c.Int(nullable: false),
                        Title = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletionDate = c.DateTime(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.OrderDetails", "ProductSizeId", c => c.Guid());
            CreateIndex("dbo.OrderDetails", "ProductSizeId");
            AddForeignKey("dbo.OrderDetails", "ProductSizeId", "dbo.ProductSizes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductSizes", "SizeId", "dbo.Sizes");
            DropForeignKey("dbo.ProductSizes", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderDetails", "ProductSizeId", "dbo.ProductSizes");
            DropIndex("dbo.ProductSizes", new[] { "ProductId" });
            DropIndex("dbo.ProductSizes", new[] { "SizeId" });
            DropIndex("dbo.OrderDetails", new[] { "ProductSizeId" });
            DropColumn("dbo.OrderDetails", "ProductSizeId");
            DropTable("dbo.Sizes");
            DropTable("dbo.ProductSizes");
        }
    }
}

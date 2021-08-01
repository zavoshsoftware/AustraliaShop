namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v14 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletionDate = c.DateTime(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Amount = c.Decimal(nullable: false, storeType: "money"),
                        PaymentTypeId = c.Guid(nullable: false),
                        IsDeposit = c.Boolean(nullable: false),
                        Code = c.String(),
                        FileAttched = c.String(),
                        PaymentDay = c.DateTime(nullable: false),
                        OrderId = c.Guid(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletionDate = c.DateTime(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.PaymentTypes", t => t.PaymentTypeId, cascadeDelete: true)
                .Index(t => t.PaymentTypeId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.PaymentTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Order = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletionDate = c.DateTime(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Orders", "IsPos", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "AdditiveAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "PaymentAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "RemainAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "DecreaseAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "OrderDate", c => c.DateTime());
            AddColumn("dbo.Orders", "CustomerTypeId", c => c.Guid());
            CreateIndex("dbo.Orders", "CustomerTypeId");
            AddForeignKey("dbo.Orders", "CustomerTypeId", "dbo.CustomerTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payments", "PaymentTypeId", "dbo.PaymentTypes");
            DropForeignKey("dbo.Payments", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "CustomerTypeId", "dbo.CustomerTypes");
            DropIndex("dbo.Payments", new[] { "OrderId" });
            DropIndex("dbo.Payments", new[] { "PaymentTypeId" });
            DropIndex("dbo.Orders", new[] { "CustomerTypeId" });
            DropColumn("dbo.Orders", "CustomerTypeId");
            DropColumn("dbo.Orders", "OrderDate");
            DropColumn("dbo.Orders", "DecreaseAmount");
            DropColumn("dbo.Orders", "RemainAmount");
            DropColumn("dbo.Orders", "PaymentAmount");
            DropColumn("dbo.Orders", "AdditiveAmount");
            DropColumn("dbo.Orders", "IsPos");
            DropTable("dbo.PaymentTypes");
            DropTable("dbo.Payments");
            DropTable("dbo.CustomerTypes");
        }
    }
}

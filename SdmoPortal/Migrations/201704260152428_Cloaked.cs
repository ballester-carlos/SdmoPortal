namespace SdmoPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cloaked : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Categories", "ParentCategoryId", "dbo.Categories");
            DropForeignKey("dbo.WorkOrders", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.InventoryItems", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Categories", new[] { "ParentCategoryId" });
            DropIndex("dbo.Categories", "AK_Category_CategoryName");
            DropIndex("dbo.Customers", "AK_Customer_AccountNumber");
            DropIndex("dbo.Customers", "AK_Customer_CompanyName");
            DropIndex("dbo.Labors", "AK_Labor");
            DropIndex("dbo.Parts", "AK_Part");
            DropIndex("dbo.InventoryItems", "AK_InventoryItem_InventoryItemCode");
            DropIndex("dbo.InventoryItems", "AK_InventoryItem_InventoryItemName");
            DropIndex("dbo.LogEntries", "IDX_LogEntries_Entity");
            DropIndex("dbo.ServiceItems", "AK_ServiceItem_ServiceItemCode");
            DropIndex("dbo.ServiceItems", "AK_ServiceItem_ServiceItemName");
            RenameColumn(table: "dbo.Categories", name: "CategoryId", newName: "Id");
            AddColumn("dbo.Categories", "Parent_Id", c => c.Int());
            AddColumn("dbo.Customers", "Cloaked", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Categories", "CategoryName", c => c.String());
            AlterColumn("dbo.WorkOrders", "OrderDateTime", c => c.DateTime(nullable: false));
            //AlterColumn("dbo.WorkOrders", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.AspNetUsers", "FirstName", c => c.String());
            AlterColumn("dbo.AspNetUsers", "LastName", c => c.String());
            AlterColumn("dbo.AspNetUsers", "Address", c => c.String());
            AlterColumn("dbo.AspNetUsers", "City", c => c.String());
            AlterColumn("dbo.AspNetUsers", "State", c => c.String());
            AlterColumn("dbo.AspNetUsers", "ZipCode", c => c.String());
            //AlterColumn("dbo.Labors", "ExtendedPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            //AlterColumn("dbo.Parts", "ExtendedPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.LogEntries", "Logger", c => c.String());
            AlterColumn("dbo.LogEntries", "LogLevel", c => c.String());
            AlterColumn("dbo.LogEntries", "Thread", c => c.String());
            AlterColumn("dbo.LogEntries", "EntityFormalNamePlural", c => c.String());
            AlterColumn("dbo.LogEntries", "UserName", c => c.String());
            AlterColumn("dbo.LogEntries", "Message", c => c.String());
            CreateIndex("dbo.Categories", "Parent_Id");
            CreateIndex("dbo.Labors", "WorkOrderId");
            CreateIndex("dbo.Parts", "WorkOrderId");
            AddForeignKey("dbo.Categories", "Parent_Id", "dbo.Categories", "Id");
            AddForeignKey("dbo.WorkOrders", "CustomerId", "dbo.Customers", "CustomerId", cascadeDelete: true);
            AddForeignKey("dbo.InventoryItems", "CategoryId", "dbo.Categories", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventoryItems", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.WorkOrders", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Categories", "Parent_Id", "dbo.Categories");
            DropIndex("dbo.Parts", new[] { "WorkOrderId" });
            DropIndex("dbo.Labors", new[] { "WorkOrderId" });
            DropIndex("dbo.Categories", new[] { "Parent_Id" });
            AlterColumn("dbo.LogEntries", "Message", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("dbo.LogEntries", "UserName", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("dbo.LogEntries", "EntityFormalNamePlural", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.LogEntries", "Thread", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.LogEntries", "LogLevel", c => c.String(nullable: false, maxLength: 5));
            AlterColumn("dbo.LogEntries", "Logger", c => c.String(nullable: false, maxLength: 30));
            //AlterColumn("dbo.Parts", "ExtendedPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            //AlterColumn("dbo.Labors", "ExtendedPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.AspNetUsers", "ZipCode", c => c.String(maxLength: 10));
            AlterColumn("dbo.AspNetUsers", "State", c => c.String(maxLength: 2));
            AlterColumn("dbo.AspNetUsers", "City", c => c.String(maxLength: 20));
            AlterColumn("dbo.AspNetUsers", "Address", c => c.String(maxLength: 30));
            AlterColumn("dbo.AspNetUsers", "LastName", c => c.String(maxLength: 15));
            AlterColumn("dbo.AspNetUsers", "FirstName", c => c.String(maxLength: 15));
            //AlterColumn("dbo.WorkOrders", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.WorkOrders", "OrderDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Categories", "CategoryName", c => c.String(nullable: false, maxLength: 20));
            DropColumn("dbo.Customers", "Cloaked");
            DropColumn("dbo.Categories", "Parent_Id");
            RenameColumn(table: "dbo.Categories", name: "Id", newName: "CategoryId");
            CreateIndex("dbo.ServiceItems", "ServiceItemName", unique: true, name: "AK_ServiceItem_ServiceItemName");
            CreateIndex("dbo.ServiceItems", "ServiceItemCode", unique: true, name: "AK_ServiceItem_ServiceItemCode");
            CreateIndex("dbo.LogEntries", new[] { "EntityKeyValue", "EntityFormalNamePlural" }, name: "IDX_LogEntries_Entity");
            CreateIndex("dbo.InventoryItems", "InventoryItemName", unique: true, name: "AK_InventoryItem_InventoryItemName");
            CreateIndex("dbo.InventoryItems", "InventoryItemCode", unique: true, name: "AK_InventoryItem_InventoryItemCode");
            CreateIndex("dbo.Parts", new[] { "WorkOrderId", "InventoryItemCode" }, unique: true, name: "AK_Part");
            CreateIndex("dbo.Labors", new[] { "WorkOrderId", "ServiceItemCode" }, unique: true, name: "AK_Labor");
            CreateIndex("dbo.Customers", "CompanyName", unique: true, name: "AK_Customer_CompanyName");
            CreateIndex("dbo.Customers", "AccountNumber", unique: true, name: "AK_Customer_AccountNumber");
            CreateIndex("dbo.Categories", "CategoryName", unique: true, name: "AK_Category_CategoryName");
            CreateIndex("dbo.Categories", "ParentCategoryId");
            AddForeignKey("dbo.InventoryItems", "CategoryId", "dbo.Categories", "CategoryId");
            AddForeignKey("dbo.WorkOrders", "CustomerId", "dbo.Customers", "CustomerId");
            AddForeignKey("dbo.Categories", "ParentCategoryId", "dbo.Categories", "CategoryId");
        }
    }
}

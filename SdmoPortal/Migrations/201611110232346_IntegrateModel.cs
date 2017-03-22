namespace SdmoPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IntegrateModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkOrders", "CurrentWorkerId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.WorkOrders", "CurrentWorkerId");
            AddForeignKey("dbo.WorkOrders", "CurrentWorkerId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkOrders", "CurrentWorkerId", "dbo.AspNetUsers");
            DropIndex("dbo.WorkOrders", new[] { "CurrentWorkerId" });
            DropColumn("dbo.WorkOrders", "CurrentWorkerId");
        }
    }
}

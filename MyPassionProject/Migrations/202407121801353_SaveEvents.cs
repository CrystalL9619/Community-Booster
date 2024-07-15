namespace MyPassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaveEvents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SavedEvents",
                c => new
                    {
                        Event_EventId = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Event_EventId, t.ApplicationUser_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Events", t => t.Event_EventId, cascadeDelete: true)
                .Index(t => t.Event_EventId)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SavedEvents", "Event_EventId", "dbo.Events");
            DropForeignKey("dbo.SavedEvents", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.SavedEvents", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.SavedEvents", new[] { "Event_EventId" });
            DropTable("dbo.SavedEvents");
        }
    }
}

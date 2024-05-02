namespace MyPassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventAppUsers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventAppUsers",
                c => new
                    {
                        Event_EventId = c.Int(nullable: false),
                        AppUser_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Event_EventId, t.AppUser_UserId })
                .ForeignKey("dbo.Events", t => t.Event_EventId, cascadeDelete: true)
                .ForeignKey("dbo.AppUsers", t => t.AppUser_UserId, cascadeDelete: true)
                .Index(t => t.Event_EventId)
                .Index(t => t.AppUser_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventAppUsers", "AppUser_UserId", "dbo.AppUsers");
            DropForeignKey("dbo.EventAppUsers", "Event_EventId", "dbo.Events");
            DropIndex("dbo.EventAppUsers", new[] { "AppUser_UserId" });
            DropIndex("dbo.EventAppUsers", new[] { "Event_EventId" });
            DropTable("dbo.EventAppUsers");
        }
    }
}

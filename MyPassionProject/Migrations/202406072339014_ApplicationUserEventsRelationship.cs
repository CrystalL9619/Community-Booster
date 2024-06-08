namespace MyPassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationUserEventsRelationship : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ApplicationUserEvents", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserEvents", new[] { "Event_EventId" });
            RenameColumn(table: "dbo.ApplicationUserEvents", name: "ApplicationUser_Id", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.ApplicationUserEvents", name: "Event_EventId", newName: "ApplicationUser_Id");
            RenameColumn(table: "dbo.ApplicationUserEvents", name: "__mig_tmp__0", newName: "Event_EventId");
            DropPrimaryKey("dbo.ApplicationUserEvents");
            AlterColumn("dbo.ApplicationUserEvents", "ApplicationUser_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ApplicationUserEvents", "Event_EventId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.ApplicationUserEvents", new[] { "Event_EventId", "ApplicationUser_Id" });
            CreateIndex("dbo.ApplicationUserEvents", "Event_EventId");
            CreateIndex("dbo.ApplicationUserEvents", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ApplicationUserEvents", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserEvents", new[] { "Event_EventId" });
            DropPrimaryKey("dbo.ApplicationUserEvents");
            AlterColumn("dbo.ApplicationUserEvents", "Event_EventId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ApplicationUserEvents", "ApplicationUser_Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.ApplicationUserEvents", new[] { "ApplicationUser_Id", "Event_EventId" });
            RenameColumn(table: "dbo.ApplicationUserEvents", name: "Event_EventId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.ApplicationUserEvents", name: "ApplicationUser_Id", newName: "Event_EventId");
            RenameColumn(table: "dbo.ApplicationUserEvents", name: "__mig_tmp__0", newName: "ApplicationUser_Id");
            CreateIndex("dbo.ApplicationUserEvents", "Event_EventId");
            CreateIndex("dbo.ApplicationUserEvents", "ApplicationUser_Id");
        }
    }
}

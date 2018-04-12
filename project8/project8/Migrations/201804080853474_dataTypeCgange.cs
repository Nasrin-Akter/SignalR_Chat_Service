namespace project8.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dataTypeCgange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RequestInfoes", "ReqDateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RequestInfoes", "ReqDateTime", c => c.String());
        }
    }
}

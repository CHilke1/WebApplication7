namespace WebApplication7.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _new : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Donor", "Website_ID", c => c.Int(nullable: false));
            AddColumn("dbo.Donor", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Donor", "Amount");
            DropColumn("dbo.Donor", "Website_ID");
        }
    }
}

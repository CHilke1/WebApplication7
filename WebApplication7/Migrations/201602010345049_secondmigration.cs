namespace WebApplication7.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class secondmigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Website", "Owner", c => c.String());
            AddColumn("dbo.Website", "Image", c => c.String());
            AddColumn("dbo.Donor", "CreditCard", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Donor", "CreditCard");
            DropColumn("dbo.Website", "Image");
            DropColumn("dbo.Website", "Owner");
        }
    }
}

namespace WebApplication7.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comment",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CommentText = c.String(),
                        CommentName = c.String(),
                        CommentEmail = c.String(),
                        Site_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Website", t => t.Site_ID)
                .Index(t => t.Site_ID);
            
            CreateTable(
                "dbo.Website",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SiteName = c.String(),
                        Type = c.String(),
                        URL = c.String(),
                        iTunes = c.String(),
                        RSS = c.String(),
                        Rating = c.Int(nullable: false),
                        OwnerStatement = c.String(),
                        Description = c.String(),
                        proprietor_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Proprietor", t => t.proprietor_ID)
                .Index(t => t.proprietor_ID);
            
            CreateTable(
                "dbo.Proprietor",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PayPalInfo = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Donation",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        Donor_ID = c.Int(),
                        Site_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Donor", t => t.Donor_ID)
                .ForeignKey("dbo.Website", t => t.Site_ID)
                .Index(t => t.Donor_ID)
                .Index(t => t.Site_ID);
            
            CreateTable(
                "dbo.Donor",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Phone = c.String(),
                        ZipCode = c.String(),
                        PayPalInfo = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Topic",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TopicText = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Donation", "Site_ID", "dbo.Website");
            DropForeignKey("dbo.Donation", "Donor_ID", "dbo.Donor");
            DropForeignKey("dbo.Comment", "Site_ID", "dbo.Website");
            DropForeignKey("dbo.Website", "proprietor_ID", "dbo.Proprietor");
            DropIndex("dbo.Donation", new[] { "Site_ID" });
            DropIndex("dbo.Donation", new[] { "Donor_ID" });
            DropIndex("dbo.Website", new[] { "proprietor_ID" });
            DropIndex("dbo.Comment", new[] { "Site_ID" });
            DropTable("dbo.Topic");
            DropTable("dbo.Donor");
            DropTable("dbo.Donation");
            DropTable("dbo.Proprietor");
            DropTable("dbo.Website");
            DropTable("dbo.Comment");
        }
    }
}

namespace WebApplication7.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebApplication7.Models;

    //public class Configuration : DbMigrationsConfiguration<WebApplication7.Models.Model1>
    public class WebApplication7Intializer : System.Data.Entity.DropCreateDatabaseAlways<Model1>
    {
        /*public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }*/

        protected override void Seed(WebApplication7.Models.Model1 context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            context.Topics.AddOrUpdate(x => x.ID,
                new Models.Topic() { ID = 1, TopicText = "Miscellaneous" },
                new Models.Topic() { ID = 2, TopicText = "Pop Culture" },
                new Models.Topic() { ID = 3, TopicText = "Sports" },
                new Models.Topic() { ID = 4, TopicText = "Government" },
                new Models.Topic() { ID = 5, TopicText = "Politics" },
                new Models.Topic() { ID = 6, TopicText = "Entertainment" },
                new Models.Topic() { ID = 7, TopicText = "History" }
            );

            context.Website.AddOrUpdate(x => x.ID,
                new Models.Website()
                {
                    ID = 1,
                    SiteName = "Tank Riot",
                    Type = "Podcast",
                    URL = "http://www.tankriot.com/",
                    RSS = "http://www.tankriot.com/rss.xml",
                    iTunes = "https://itunes.apple.com/us/podcast/tank-riot/id84359875?mt=2",
                    Rating=1,
                    Topic = "Pop Culture",
                    proprietor = new Proprietor { Name = "Sputnik", Email = "feedback@tankriot.com", PayPalInfo = "USA" }
                },
                new Models.Website()
                {
                    ID = 2,
                    SiteName = "The C-Realm Podcast",
                    Type = "Podcast",
                    URL = "http://c-realm.com/",
                    RSS = "http://c-realm.com/feed/podcast/",
                    iTunes = "https://itunes.apple.com/us/podcast/c-realm-podcast/id497263927?mt=2",
                    Rating = 1,
                    Topic = "Miscellaneous",
                    proprietor = new Proprietor { Name = "KMO", Email = "kmo@crealm.com", PayPalInfo = "USA" },
                },
                new Models.Website()
                {
                    ID = 3,
                    SiteName = "Tangentially Speaking",
                    Type = "Podcast",
                    URL = "http://chrisryanphd.com/tangentially-speaking/",
                    RSS = "http://feeds.feedburner.com/TangentiallySpeaking-ChristopherRyanPhd",
                    iTunes = "https://itunes.apple.com/us/podcast/tangentially-speaking-dr./id566908883?mt=2",
                    Rating = 1,
                    Topic = "Interview",
                    proprietor = new Proprietor { Name = "Christopher Ryan", Email = "chris.ryan@email.com", PayPalInfo = "Sweden" }
                },
                new Models.Website()
                {
                    ID = 4,
                    SiteName = "The Hipcrime Vocab",
                    Type = "Blog",
                    URL = "http://hipcrime.blogspot.com/",
                    RSS = "http://hipcrime.blogspot.com/feeds/posts/default?alt=rss",
                    iTunes = "NA",
                    Rating = 1,
                    Topic = "History, Economics",
                    proprietor = new Proprietor { Name = "Chad Hilke", Email = "chad.hilke@email.com", PayPalInfo = "Sweden" },
                },
                new Models.Website()
                {
                    ID = 5,
                    SiteName = "Ran Prieur",
                    Type = "Blog",
                    URL = "http://ranprieur.com/",
                    RSS = "http://ranprieur.com/feed.php",
                    iTunes = "NA",
                    Rating = 1,
                    Topic = "Miscellaneous",
                    proprietor = new Proprietor { Name = "Ran Prieur", Email = "ranprieur@email.com", PayPalInfo = "Sweden" }
                }
                );
        }
    }
}

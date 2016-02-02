using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication7.Models
{
    public class Website
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string SiteName { get; set; }
        public string Type { get; set; }
        public string URL { get; set; }
        public string iTunes { get; set; }
        public string RSS { get; set; }
        public int Rating { get; set; }
        public string OwnerStatement { get; set; }
        public string Description { get; set; }
        public virtual Proprietor proprietor { get; set; }
        public string Topic { get; set; }
        public string Owner { get; set; }
        public string Image { get; set; }
    }
}

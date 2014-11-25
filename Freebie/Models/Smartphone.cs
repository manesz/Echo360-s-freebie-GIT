using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Freebie.Models
{
    public class Smartphone
    {
        [Key]
        public string Smartphone_Cd { get; set; }
        public string Smartphone_Desc_Th { get; set; }
        public string Smartphone_Desc_En { get; set; }
        public string Created_By { get; set; }
        public string Updated_By { get; set; }
        public System.DateTime Created_Dttm { get; set; }
        public System.DateTime Updated_Dttm { get; set; }
    }
}
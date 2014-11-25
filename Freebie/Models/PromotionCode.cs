using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Freebie.Models
{
    [Table("Promotion_Code")]
    public class PromotionCode
    {
        [Key]
        public string Promotion_Cd { get; set; }
        public string Promotion_Desc { get; set; }
        public string Promotion_Code_Value { get; set; }
        public System.DateTime? Start_Dttm { get; set; }
        public System.DateTime? End_Dttm { get; set; }

        public string Status { get; set; }
        public string Online_Message { get; set; }
        public string No_of_Target { get; set; }
        public string Special_Quota_Cd { get; set; }

        public string Created_By { get; set; }
        public string Updated_By { get; set; }
        public System.DateTime Created_Dttm { get; set; }
        public System.DateTime Updated_Dttm { get; set; }

        [ForeignKey("Special_Quota_Cd")]
        public virtual Quota Quota { get; set; }
    }
}